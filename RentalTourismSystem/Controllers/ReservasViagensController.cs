using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models;

namespace RentalTourismSystem.Controllers
{
    [Authorize] // Todo o controller requer autenticação
    public class ReservasViagensController : Controller
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<ReservasViagensController> _logger;

        public ReservasViagensController(RentalTourismContext context, ILogger<ReservasViagensController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: ReservasViagens - Todos os funcionários podem ver
        public async Task<IActionResult> Index(int? statusId, DateTime? dataInicio, DateTime? dataFim, string? busca)
        {
            try
            {
                _logger.LogInformation("Lista de reservas de viagens acessada por usuário {User}", User.Identity?.Name);

                var reservas = _context.ReservasViagens
                    .Include(r => r.Cliente)
                    .Include(r => r.PacoteViagem)
                    .Include(r => r.StatusReservaViagem)
                    .AsQueryable();

                // Filtros
                if (statusId.HasValue)
                {
                    reservas = reservas.Where(r => r.StatusReservaViagemId == statusId);
                    _logger.LogInformation("Filtro por status {StatusId} aplicado por {User}", statusId, User.Identity?.Name);
                }

                if (dataInicio.HasValue)
                {
                    reservas = reservas.Where(r => r.DataReserva >= dataInicio.Value.Date);
                }

                if (dataFim.HasValue)
                {
                    reservas = reservas.Where(r => r.DataReserva <= dataFim.Value.Date.AddDays(1).AddTicks(-1));
                }

                if (!string.IsNullOrEmpty(busca))
                {
                    reservas = reservas.Where(r => r.Cliente.Nome.Contains(busca) ||
                                                 r.Cliente.Cpf.Contains(busca) ||
                                                 r.PacoteViagem.Nome.Contains(busca) ||
                                                 r.PacoteViagem.Destino.Contains(busca));
                    _logger.LogInformation("Busca por reservas '{Busca}' realizada por {User}", busca, User.Identity?.Name);
                }

                ViewBag.StatusId = new SelectList(await _context.StatusReservaViagens.ToListAsync(), "Id", "Status", statusId);
                ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
                ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");
                ViewBag.Busca = busca;

                var listaReservas = await reservas
                    .OrderByDescending(r => r.DataReserva)
                    .ToListAsync();

                return View(listaReservas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de reservas para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar lista de reservas. Tente novamente.";
                ViewBag.StatusId = new SelectList(new List<StatusReservaViagem>(), "Id", "Status");
                return View(new List<ReservaViagem>());
            }
        }

        // GET: ReservasViagens/Details/5 - Todos podem ver detalhes
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de acesso a detalhes de reserva com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var reserva = await _context.ReservasViagens
                    .Include(r => r.Cliente)
                    .Include(r => r.PacoteViagem)
                    .Include(r => r.StatusReservaViagem)
                    .Include(r => r.ServicosAdicionais)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (reserva == null)
                {
                    _logger.LogWarning("Reserva com ID {ReservaId} não encontrada. Acessado por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                _logger.LogInformation("Detalhes da reserva {ReservaId} acessados por {User}", id, User.Identity?.Name);
                return View(reserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes da reserva {ReservaId} para usuário {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados da reserva.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ReservasViagens/Create - Todos os funcionários podem criar
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Create()
        {
            try
            {
                await CarregarViewBags();
                _logger.LogInformation("Formulário de criação de reserva acessado por {User}", User.Identity?.Name);
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de criação de reserva por {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar formulário.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ReservasViagens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Create([Bind("DataReserva,DataViagem,Quantidade,Observacoes,ClienteId,PacoteViagemId")] ReservaViagem reserva)
        {
            try
            {
                // Remove propriedades de navegação do ModelState
                ModelState.Remove("Cliente");
                ModelState.Remove("PacoteViagem");
                ModelState.Remove("StatusReservaViagem");
                ModelState.Remove("ValorTotal");

                if (ModelState.IsValid)
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        // Buscar o status "Pendente" para novas reservas
                        var statusPendente = await _context.StatusReservaViagens
                            .FirstOrDefaultAsync(s => s.Status == "Pendente");

                        if (statusPendente == null)
                        {
                            ModelState.AddModelError("", "Status 'Pendente' não encontrado no sistema.");
                            await CarregarViewBags(reserva);
                            return View(reserva);
                        }

                        reserva.StatusReservaViagemId = statusPendente.Id;

                        // Calcular valor total baseado no pacote e quantidade
                        var pacote = await _context.PacotesViagens.FindAsync(reserva.PacoteViagemId);
                        if (pacote == null)
                        {
                            ModelState.AddModelError("PacoteViagemId", "Pacote não encontrado.");
                            await CarregarViewBags(reserva);
                            return View(reserva);
                        }

                        reserva.ValorTotal = pacote.Preco * reserva.Quantidade;

                        // Verificar se a data de viagem não é no passado
                        if (reserva.DataViagem.Date < DateTime.Now.Date)
                        {
                            ModelState.AddModelError("DataViagem", "A data da viagem não pode ser no passado.");
                            await CarregarViewBags(reserva);
                            return View(reserva);
                        }

                        _context.Add(reserva);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("Nova reserva {ReservaId} criada por {User}. Cliente: {ClienteId}, Pacote: {PacoteId}, Data: {DataViagem}, Quantidade: {Quantidade}",
                            reserva.Id, User.Identity?.Name, reserva.ClienteId, reserva.PacoteViagemId,
                            reserva.DataViagem.ToString("dd/MM/yyyy"), reserva.Quantidade);

                        TempData["Sucesso"] = "Reserva de viagem criada com sucesso!";
                        return RedirectToAction(nameof(Details), new { id = reserva.Id });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Erro na transação ao criar reserva por {User}", User.Identity?.Name);
                        ModelState.AddModelError(string.Empty, "Erro ao processar reserva. Tente novamente.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar reserva por {User}", User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro interno do sistema. Tente novamente.");
            }

            await CarregarViewBags(reserva);
            return View(reserva);
        }

        // GET: ReservasViagens/Edit/5 - Apenas Admin e Manager podem editar
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de edição de reserva com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var reserva = await _context.ReservasViagens
                    .Include(r => r.Cliente)
                    .Include(r => r.PacoteViagem)
                    .Include(r => r.StatusReservaViagem)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reserva == null)
                {
                    _logger.LogWarning("Tentativa de edição de reserva inexistente {ReservaId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                // Verificar se a reserva pode ser editada
                if (reserva.StatusReservaViagem.Status == "Cancelada")
                {
                    TempData["Erro"] = "Não é possível editar uma reserva cancelada.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                if (reserva.DataViagem.Date < DateTime.Now.Date)
                {
                    TempData["Erro"] = "Não é possível editar uma reserva cuja data de viagem já passou.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                await CarregarViewBags(reserva);
                _logger.LogInformation("Formulário de edição da reserva {ReservaId} acessado por {User}", id, User.Identity?.Name);
                return View(reserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição da reserva {ReservaId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados da reserva para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ReservasViagens/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataReserva,DataViagem,Quantidade,ValorTotal,Observacoes,ClienteId,PacoteViagemId,StatusReservaViagemId")] ReservaViagem reserva)
        {
            if (id != reserva.Id)
            {
                _logger.LogWarning("Tentativa de edição com ID inconsistente {Id} != {ReservaId} por {User}",
                    id, reserva.Id, User.Identity?.Name);
                return NotFound();
            }

            try
            {
                ModelState.Remove("Cliente");
                ModelState.Remove("PacoteViagem");
                ModelState.Remove("StatusReservaViagem");
                ModelState.Remove("ValorTotal");

                if (ModelState.IsValid)
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        // Recalcular valor total se mudou a quantidade ou o pacote
                        var pacote = await _context.PacotesViagens.FindAsync(reserva.PacoteViagemId);
                        if (pacote != null)
                        {
                            reserva.ValorTotal = pacote.Preco * reserva.Quantidade;
                        }

                        _context.Update(reserva);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("Reserva {ReservaId} atualizada por {User}", reserva.Id, User.Identity?.Name);

                        TempData["Sucesso"] = "Reserva atualizada com sucesso!";
                        return RedirectToAction(nameof(Details), new { id = reserva.Id });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Erro na transação ao editar reserva {ReservaId} por {User}", reserva.Id, User.Identity?.Name);
                        throw;
                    }
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ReservaExists(reserva.Id))
                {
                    _logger.LogWarning("Reserva {ReservaId} não existe mais durante edição por {User}", reserva.Id, User.Identity?.Name);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Erro de concorrência ao editar reserva {ReservaId} por {User}", reserva.Id, User.Identity?.Name);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao editar reserva {ReservaId} por {User}", reserva.Id, User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro interno do sistema. Tente novamente.");
            }

            await CarregarViewBags(reserva);
            return View(reserva);
        }

        // POST: Confirmar Reserva - Todos os funcionários podem confirmar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> ConfirmarReserva(int id, string? observacoes)
        {
            try
            {
                var reserva = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reserva == null)
                {
                    _logger.LogWarning("Tentativa de confirmar reserva inexistente {ReservaId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                if (reserva.StatusReservaViagem.Status == "Confirmada")
                {
                    TempData["Info"] = "Esta reserva já está confirmada.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                if (reserva.StatusReservaViagem.Status == "Cancelada")
                {
                    TempData["Erro"] = "Não é possível confirmar uma reserva cancelada.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var statusConfirmada = await _context.StatusReservaViagens
                        .FirstOrDefaultAsync(s => s.Status == "Confirmada");

                    if (statusConfirmada == null)
                    {
                        TempData["Erro"] = "Status 'Confirmada' não encontrado no sistema.";
                        return RedirectToAction(nameof(Details), new { id = id });
                    }

                    reserva.StatusReservaViagemId = statusConfirmada.Id;

                    if (!string.IsNullOrEmpty(observacoes))
                    {
                        reserva.Observacoes = string.IsNullOrEmpty(reserva.Observacoes)
                            ? $"Confirmação: {observacoes}"
                            : $"{reserva.Observacoes}\n\nConfirmação: {observacoes}";
                    }

                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Reserva {ReservaId} confirmada por {User}", id, User.Identity?.Name);
                    TempData["Sucesso"] = "Reserva confirmada com sucesso!";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Erro ao confirmar reserva {ReservaId} por {User}", id, User.Identity?.Name);
                    TempData["Erro"] = "Erro ao confirmar reserva. Tente novamente.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao confirmar reserva {ReservaId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro interno do sistema. Tente novamente.";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        // POST: Cancelar Reserva - Apenas Admin e Manager podem cancelar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CancelarReserva(int id, string motivo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(motivo))
                {
                    TempData["Erro"] = "É obrigatório informar o motivo do cancelamento.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                var reserva = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reserva == null)
                {
                    _logger.LogWarning("Tentativa de cancelar reserva inexistente {ReservaId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                if (reserva.StatusReservaViagem.Status == "Cancelada")
                {
                    TempData["Info"] = "Esta reserva já está cancelada.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var statusCancelada = await _context.StatusReservaViagens
                        .FirstOrDefaultAsync(s => s.Status == "Cancelada");

                    if (statusCancelada == null)
                    {
                        TempData["Erro"] = "Status 'Cancelada' não encontrado no sistema.";
                        return RedirectToAction(nameof(Details), new { id = id });
                    }

                    reserva.StatusReservaViagemId = statusCancelada.Id;

                    reserva.Observacoes = string.IsNullOrEmpty(reserva.Observacoes)
                        ? $"Cancelamento: {motivo}"
                        : $"{reserva.Observacoes}\n\nCancelamento: {motivo}";

                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation("Reserva {ReservaId} cancelada por {User}. Motivo: {Motivo}",
                        id, User.Identity?.Name, motivo);
                    TempData["Sucesso"] = "Reserva cancelada com sucesso!";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Erro ao cancelar reserva {ReservaId} por {User}", id, User.Identity?.Name);
                    TempData["Erro"] = "Erro ao cancelar reserva. Tente novamente.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao cancelar reserva {ReservaId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro interno do sistema. Tente novamente.";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        // GET: ReservasViagens/Delete/5 - Apenas Admin pode excluir
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de exclusão de reserva com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var reserva = await _context.ReservasViagens
                    .Include(r => r.Cliente)
                    .Include(r => r.PacoteViagem)
                    .Include(r => r.StatusReservaViagem)
                    .Include(r => r.ServicosAdicionais)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (reserva == null)
                {
                    _logger.LogWarning("Tentativa de exclusão de reserva inexistente {ReservaId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                // Verificar se a reserva pode ser excluída
                var impedimentos = new List<string>();

                if (reserva.StatusReservaViagem.Status == "Confirmada")
                {
                    impedimentos.Add("A reserva está confirmada");
                }

                if (reserva.ServicosAdicionais.Any())
                {
                    impedimentos.Add($"{reserva.ServicosAdicionais.Count} serviço(s) adicional(is) vinculado(s)");
                }

                if (impedimentos.Any())
                {
                    ViewBag.Impedimentos = impedimentos;
                }

                _logger.LogInformation("Formulário de confirmação de exclusão da reserva {ReservaId} acessado por {User}", id, User.Identity?.Name);
                return View(reserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de exclusão da reserva {ReservaId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados da reserva para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ReservasViagens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var reserva = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .Include(r => r.ServicosAdicionais)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reserva != null)
                {
                    // Verificar novamente se pode ser excluída
                    if (reserva.StatusReservaViagem.Status == "Confirmada")
                    {
                        TempData["Erro"] = "Não é possível excluir uma reserva confirmada. Cancele a reserva primeiro.";
                        return RedirectToAction(nameof(Delete), new { id = id });
                    }

                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        // Remover serviços adicionais primeiro
                        if (reserva.ServicosAdicionais.Any())
                        {
                            _context.ServicosAdicionais.RemoveRange(reserva.ServicosAdicionais);
                        }

                        _context.ReservasViagens.Remove(reserva);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("Reserva {ReservaId} excluída por {User}", id, User.Identity?.Name);
                        TempData["Sucesso"] = "Reserva excluída com sucesso!";
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Erro na transação ao excluir reserva {ReservaId} por {User}", id, User.Identity?.Name);
                        TempData["Erro"] = "Erro ao excluir reserva. Tente novamente.";
                    }
                }
                else
                {
                    _logger.LogWarning("Tentativa de exclusão de reserva inexistente {ReservaId} por {User}", id, User.Identity?.Name);
                    TempData["Erro"] = "Reserva não encontrada.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao excluir reserva {ReservaId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro interno do sistema. Tente novamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ========== GESTÃO DE SERVIÇOS ADICIONAIS - CÁLCULO CORRETO ==========

        // POST: Adicionar Serviço Adicional - CORRIGIDO
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> AdicionarServico(int reservaId, string nome, string descricao, decimal preco)
        {
            try
            {
                // Validações
                if (string.IsNullOrWhiteSpace(nome))
                {
                    TempData["Erro"] = "O nome do serviço é obrigatório.";
                    return RedirectToAction(nameof(Details), new { id = reservaId });
                }

                if (string.IsNullOrWhiteSpace(descricao))
                {
                    TempData["Erro"] = "A descrição do serviço é obrigatória.";
                    return RedirectToAction(nameof(Details), new { id = reservaId });
                }

                if (preco <= 0)
                {
                    TempData["Erro"] = "O preço deve ser maior que zero.";
                    return RedirectToAction(nameof(Details), new { id = reservaId });
                }

                var reserva = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .Include(r => r.ServicosAdicionais)
                    .FirstOrDefaultAsync(r => r.Id == reservaId);

                if (reserva == null)
                {
                    _logger.LogWarning("Tentativa de adicionar serviço a reserva inexistente {ReservaId} por {User}",
                        reservaId, User.Identity?.Name);
                    TempData["Erro"] = "Reserva não encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                if (reserva.StatusReservaViagem.Status == "Cancelada")
                {
                    TempData["Erro"] = "Não é possível adicionar serviços a uma reserva cancelada.";
                    return RedirectToAction(nameof(Details), new { id = reservaId });
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Criar serviço
                    var servico = new ServicoAdicional
                    {
                        ReservaViagemId = reservaId,
                        Nome = nome,
                        Descricao = descricao,
                        Preco = preco
                    };

                    _context.ServicosAdicionais.Add(servico);

                    // ✅ NÃO atualiza o ValorTotal - ele continua sendo apenas o valor do pacote
                    // Os serviços são somados apenas na exibição

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Calcular total geral para mostrar na mensagem
                    var totalGeral = reserva.ValorTotal + reserva.ServicosAdicionais.Sum(s => s.Preco) + preco;

                    _logger.LogInformation("Serviço adicional '{Nome}' (R$ {Valor:N2}) adicionado à reserva {ReservaId} por {User}",
                        nome, preco, reservaId, User.Identity?.Name);

                    TempData["Sucesso"] = $"Serviço '{nome}' adicionado com sucesso! Valor total geral: {totalGeral:C}";
                    return RedirectToAction(nameof(Details), new { id = reservaId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Erro ao adicionar serviço adicional à reserva {ReservaId} por {User}",
                        reservaId, User.Identity?.Name);
                    TempData["Erro"] = "Erro ao adicionar serviço. Tente novamente.";
                    return RedirectToAction(nameof(Details), new { id = reservaId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao adicionar serviço adicional por {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro interno do sistema. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Remover Serviço Adicional - CORRIGIDO
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RemoverServico(int servicoId)
        {
            try
            {
                var servico = await _context.ServicosAdicionais
                    .Include(s => s.ReservaViagem)
                        .ThenInclude(r => r.StatusReservaViagem)
                    .Include(s => s.ReservaViagem)
                        .ThenInclude(r => r.ServicosAdicionais)
                    .FirstOrDefaultAsync(s => s.Id == servicoId);

                if (servico == null)
                {
                    _logger.LogWarning("Tentativa de remover serviço inexistente {ServicoId} por {User}",
                        servicoId, User.Identity?.Name);
                    TempData["Erro"] = "Serviço não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var reservaId = servico.ReservaViagemId;

                if (servico.ReservaViagem.StatusReservaViagem.Status == "Cancelada")
                {
                    TempData["Erro"] = "Não é possível remover serviços de uma reserva cancelada.";
                    return RedirectToAction(nameof(Details), new { id = reservaId });
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Os serviços são somados apenas na exibição

                    _context.ServicosAdicionais.Remove(servico);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Calcular total geral para mostrar na mensagem
                    var servicosRestantes = servico.ReservaViagem.ServicosAdicionais.Where(s => s.Id != servicoId).Sum(s => s.Preco);
                    var totalGeral = servico.ReservaViagem.ValorTotal + servicosRestantes;

                    _logger.LogInformation("Serviço adicional '{Nome}' removido da reserva {ReservaId} por {User}",
                        servico.Nome, reservaId, User.Identity?.Name);

                    TempData["Sucesso"] = $"Serviço '{servico.Nome}' removido com sucesso! Valor total geral: {totalGeral:C}";
                    return RedirectToAction(nameof(Details), new { id = reservaId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Erro ao remover serviço adicional {ServicoId} por {User}", servicoId, User.Identity?.Name);
                    TempData["Erro"] = "Erro ao remover serviço. Tente novamente.";
                    return RedirectToAction(nameof(Details), new { id = reservaId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao remover serviço adicional por {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro interno do sistema. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ========== MÉTODOS AUXILIARES ==========

        private bool ReservaExists(int id)
        {
            return _context.ReservasViagens.Any(e => e.Id == id);
        }

        private async Task CarregarViewBags(ReservaViagem? reserva = null)
        {
            ViewBag.ClienteId = new SelectList(
                await _context.Clientes.OrderBy(c => c.Nome).ToListAsync(),
                "Id", "Nome", reserva?.ClienteId);

            // CORRIGIDO: Buscar primeiro, formatar depois
            var pacotes = await _context.PacotesViagens
                .Where(p => p.Ativo)
                .OrderBy(p => p.Nome)
                .ToListAsync();

            var pacotesFormatados = pacotes.Select(p => new
            {
                Id = p.Id,
                Descricao = $"{p.Nome} - {p.Destino} ({p.Duracao} {p.UnidadeTempo}) - R$ {p.Preco:N2}"
            }).ToList();

            ViewBag.PacoteViagemId = new SelectList(
                pacotesFormatados,
                "Id",
                "Descricao",
                reserva?.PacoteViagemId);

            ViewBag.StatusReservaViagemId = new SelectList(
                await _context.StatusReservaViagens.ToListAsync(),
                "Id", "Status", reserva?.StatusReservaViagemId);
        }

        // ========== APIs PARA CONSUMO INTERNO (AJAX) ==========

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetReservaData(int id)
        {
            try
            {
                var reserva = await _context.ReservasViagens
                    .Include(r => r.Cliente)
                    .Include(r => r.PacoteViagem)
                    .Include(r => r.StatusReservaViagem)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reserva == null)
                {
                    return NotFound(new { message = "Reserva não encontrada" });
                }

                var resultado = new
                {
                    id = reserva.Id,
                    cliente = reserva.Cliente.Nome,
                    clienteId = reserva.ClienteId,
                    pacote = reserva.PacoteViagem.Nome,
                    destino = reserva.PacoteViagem.Destino,
                    dataReserva = reserva.DataReserva.ToString("dd/MM/yyyy"),
                    dataViagem = reserva.DataViagem.ToString("dd/MM/yyyy"),
                    quantidade = reserva.Quantidade,
                    valorTotal = reserva.ValorTotal,
                    status = reserva.StatusReservaViagem.Status,
                    observacoes = reserva.Observacoes
                };

                return Json(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados da reserva {ReservaId} via API por {User}", id, User.Identity?.Name);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CalcularValorReserva(int pacoteId, int quantidade)
        {
            try
            {
                var pacote = await _context.PacotesViagens.FindAsync(pacoteId);

                if (pacote == null)
                {
                    return NotFound();
                }

                var valorTotal = pacote.Preco * quantidade;

                return Json(new
                {
                    precoUnitario = pacote.Preco,
                    quantidade = quantidade,
                    valorTotal = valorTotal
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular valor da reserva por {User}", User.Identity?.Name);
                return Json(new { valorTotal = 0 });
            }
        }

        // Busca rápida de reservas
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BuscarReservas(string? termo = null, int? statusId = null)
        {
            try
            {
                var query = _context.ReservasViagens
                    .Include(r => r.Cliente)
                    .Include(r => r.PacoteViagem)
                    .Include(r => r.StatusReservaViagem)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(termo))
                {
                    query = query.Where(r => r.Cliente.Nome.Contains(termo) ||
                                           r.PacoteViagem.Nome.Contains(termo) ||
                                           r.PacoteViagem.Destino.Contains(termo));
                }

                if (statusId.HasValue)
                {
                    query = query.Where(r => r.StatusReservaViagemId == statusId);
                }

                var reservas = await query
                    .Select(r => new
                    {
                        id = r.Id,
                        cliente = r.Cliente.Nome,
                        pacote = r.PacoteViagem.Nome,
                        destino = r.PacoteViagem.Destino,
                        dataViagem = r.DataViagem,
                        valorTotal = r.ValorTotal,
                        status = r.StatusReservaViagem.Status,
                        descricao = $"{r.Cliente.Nome} - {r.PacoteViagem.Nome} ({r.DataViagem:dd/MM/yyyy}) - {r.StatusReservaViagem.Status}"
                    })
                    .OrderByDescending(r => r.dataViagem)
                    .Take(20)
                    .ToListAsync();

                return Json(reservas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na busca de reservas por {User}", User.Identity?.Name);
                return Json(new List<object>());
            }
        }

        // Estatísticas de reservas
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetEstatisticasReservas(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            try
            {
                dataInicio ??= DateTime.Now.AddMonths(-1);
                dataFim ??= DateTime.Now;

                var reservasPeriodo = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .Where(r => r.DataReserva >= dataInicio && r.DataReserva <= dataFim)
                    .ToListAsync();

                var estatisticas = new
                {
                    totalReservas = reservasPeriodo.Count,
                    reservasConfirmadas = reservasPeriodo.Count(r => r.StatusReservaViagem.Status == "Confirmada"),
                    reservasPendentes = reservasPeriodo.Count(r => r.StatusReservaViagem.Status == "Pendente"),
                    reservasCanceladas = reservasPeriodo.Count(r => r.StatusReservaViagem.Status == "Cancelada"),
                    receitaTotal = reservasPeriodo.Where(r => r.StatusReservaViagem.Status == "Confirmada").Sum(r => r.ValorTotal),
                    totalPessoas = reservasPeriodo.Sum(r => r.Quantidade),
                    ticketMedio = reservasPeriodo.Where(r => r.StatusReservaViagem.Status == "Confirmada").Any() ?
                        reservasPeriodo.Where(r => r.StatusReservaViagem.Status == "Confirmada").Average(r => r.ValorTotal) : 0
                };

                return Json(estatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas de reservas por {User}", User.Identity?.Name);
                return Json(new { error = "Erro ao carregar estatísticas" });
            }
        }
    }
}