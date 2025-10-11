using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models;

namespace RentalTourismSystem.Controllers
{
    [Authorize]
    public class ReservasViagensController : Controller
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<ReservasViagensController> _logger;

        public ReservasViagensController(RentalTourismContext context, ILogger<ReservasViagensController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ========== ACTIONS PRINCIPAIS (CRUD) ==========

        // GET: ReservasViagens
        public async Task<IActionResult> Index(int? statusId, DateTime? dataInicio, DateTime? dataFim, string? busca)
        {
            try
            {
                _logger.LogInformation("Lista de reservas acessada por {User}", User.Identity?.Name);

                var reservas = _context.ReservasViagens
                    .Include(r => r.Cliente)
                    .Include(r => r.PacoteViagem)
                    .Include(r => r.StatusReservaViagem)
                    .AsQueryable();

                // Filtros
                if (statusId.HasValue)
                {
                    reservas = reservas.Where(r => r.StatusReservaViagemId == statusId);
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
                                                 r.Cliente.CPF.Contains(busca) ||
                                                 r.PacoteViagem.Nome.Contains(busca) ||
                                                 r.PacoteViagem.Destino.Contains(busca));
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
                _logger.LogError(ex, "Erro ao carregar reservas por {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar lista de reservas.";
                ViewBag.StatusId = new SelectList(new List<StatusReservaViagem>(), "Id", "Status");
                return View(new List<ReservaViagem>());
            }
        }

        // GET: ReservasViagens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
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
                    return NotFound();
                }

                return View(reserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes da reserva {ReservaId}", id);
                TempData["Erro"] = "Erro ao carregar dados da reserva.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: ReservasViagens/Create
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Create()
        {
            try
            {
                await CarregarViewBags();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de criação");
                TempData["Erro"] = "Erro ao carregar formulário.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ReservasViagens/Create - ✅ COM EXECUTION STRATEGY
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Create([Bind("DataReserva,DataViagem,Quantidade,Observacoes,ClienteId,PacoteViagemId")] ReservaViagem reserva)
        {
            try
            {
                ModelState.Remove("Cliente");
                ModelState.Remove("PacoteViagem");
                ModelState.Remove("StatusReservaViagem");
                ModelState.Remove("ValorTotal");

                if (ModelState.IsValid)
                {
                    // ✅ EXECUTION STRATEGY
                    var strategy = _context.Database.CreateExecutionStrategy();

                    await strategy.ExecuteAsync(async () =>
                    {
                        using var transaction = await _context.Database.BeginTransactionAsync();
                        try
                        {
                            // Buscar status Pendente
                            var statusPendente = await _context.StatusReservaViagens
                                .FirstOrDefaultAsync(s => s.Status == "Pendente");

                            if (statusPendente == null)
                            {
                                throw new InvalidOperationException("Status 'Pendente' não encontrado.");
                            }

                            reserva.StatusReservaViagemId = statusPendente.Id;

                            // Calcular valor total
                            var pacote = await _context.PacotesViagens.FindAsync(reserva.PacoteViagemId);
                            if (pacote == null)
                            {
                                throw new InvalidOperationException("Pacote não encontrado.");
                            }

                            reserva.ValorTotal = pacote.Preco * reserva.Quantidade;

                            // Validar data
                            if (reserva.DataViagem.Date < DateTime.Now.Date)
                            {
                                throw new InvalidOperationException("Data da viagem não pode ser no passado.");
                            }

                            _context.Add(reserva);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();

                            _logger.LogInformation("Reserva {ReservaId} criada por {User}", reserva.Id, User.Identity?.Name);
                        }
                        catch (InvalidOperationException)
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogError(ex, "Erro na transação ao criar reserva");
                            throw new InvalidOperationException("Erro ao processar reserva.", ex);
                        }
                    });

                    TempData["Sucesso"] = "Reserva criada com sucesso!";
                    return RedirectToAction(nameof(Details), new { id = reserva.Id });
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar reserva");
                ModelState.AddModelError(string.Empty, "Erro interno do sistema.");
            }

            await CarregarViewBags(reserva);
            return View(reserva);
        }

        // GET: ReservasViagens/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var reserva = await _context.ReservasViagens
                    .Include(r => r.Cliente)
                    .Include(r => r.PacoteViagem)
                    .Include(r => r.StatusReservaViagem)
                    .Include(r => r.ServicosAdicionais)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reserva == null)
                {
                    return NotFound();
                }

                if (reserva.StatusReservaViagem.Status == "Cancelada")
                {
                    TempData["Erro"] = "Não é possível editar uma reserva cancelada.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                if (reserva.DataViagem.Date < DateTime.Now.Date)
                {
                    TempData["Erro"] = "Não é possível editar reserva com data passada.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                await CarregarViewBags(reserva);
                return View(reserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar edição da reserva {ReservaId}", id);
                TempData["Erro"] = "Erro ao carregar dados.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ReservasViagens/Edit/5 - ✅ COM EXECUTION STRATEGY
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DataReserva,DataViagem,Quantidade,ValorTotal,Observacoes,ClienteId,PacoteViagemId,StatusReservaViagemId")] ReservaViagem reserva)
        {
            if (id != reserva.Id)
            {
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
                    // ✅ EXECUTION STRATEGY
                    var strategy = _context.Database.CreateExecutionStrategy();

                    await strategy.ExecuteAsync(async () =>
                    {
                        using var transaction = await _context.Database.BeginTransactionAsync();
                        try
                        {
                            // Recalcular valor total
                            var pacote = await _context.PacotesViagens.FindAsync(reserva.PacoteViagemId);
                            if (pacote != null)
                            {
                                reserva.ValorTotal = pacote.Preco * reserva.Quantidade;
                            }

                            _context.Update(reserva);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();

                            _logger.LogInformation("Reserva {ReservaId} atualizada por {User}", reserva.Id, User.Identity?.Name);
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    });

                    TempData["Sucesso"] = "Reserva atualizada com sucesso!";
                    return RedirectToAction(nameof(Details), new { id = reserva.Id });
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ReservaExists(reserva.Id))
                {
                    return NotFound();
                }
                _logger.LogError(ex, "Erro de concorrência ao editar reserva");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao editar reserva {ReservaId}", reserva.Id);
                ModelState.AddModelError(string.Empty, "Erro interno do sistema.");
            }

            await CarregarViewBags(reserva);
            return View(reserva);
        }

        // POST: Confirmar Reserva - ✅ COM EXECUTION STRATEGY
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
                    return NotFound();
                }

                if (reserva.StatusReservaViagem.Status == "Confirmada")
                {
                    TempData["Info"] = "Reserva já está confirmada.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                if (reserva.StatusReservaViagem.Status == "Cancelada")
                {
                    TempData["Erro"] = "Não é possível confirmar reserva cancelada.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                // ✅ EXECUTION STRATEGY
                var strategy = _context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        var statusConfirmada = await _context.StatusReservaViagens
                            .FirstOrDefaultAsync(s => s.Status == "Confirmada");

                        if (statusConfirmada == null)
                        {
                            throw new InvalidOperationException("Status 'Confirmada' não encontrado.");
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
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao confirmar reserva {ReservaId}", id);
                TempData["Erro"] = "Erro ao confirmar reserva.";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        // POST: Cancelar Reserva - ✅ COM EXECUTION STRATEGY
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CancelarReserva(int id, string motivo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(motivo))
                {
                    TempData["Erro"] = "Motivo do cancelamento é obrigatório.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                var reserva = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (reserva == null)
                {
                    return NotFound();
                }

                if (reserva.StatusReservaViagem.Status == "Cancelada")
                {
                    TempData["Info"] = "Reserva já está cancelada.";
                    return RedirectToAction(nameof(Details), new { id = id });
                }

                // ✅ EXECUTION STRATEGY
                var strategy = _context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        var statusCancelada = await _context.StatusReservaViagens
                            .FirstOrDefaultAsync(s => s.Status == "Cancelada");

                        if (statusCancelada == null)
                        {
                            throw new InvalidOperationException("Status 'Cancelada' não encontrado.");
                        }

                        reserva.StatusReservaViagemId = statusCancelada.Id;

                        reserva.Observacoes = string.IsNullOrEmpty(reserva.Observacoes)
                            ? $"Cancelamento: {motivo}"
                            : $"{reserva.Observacoes}\n\nCancelamento: {motivo}";

                        _context.Update(reserva);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("Reserva {ReservaId} cancelada por {User}. Motivo: {Motivo}", id, User.Identity?.Name, motivo);
                        TempData["Sucesso"] = "Reserva cancelada com sucesso!";
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar reserva {ReservaId}", id);
                TempData["Erro"] = "Erro ao cancelar reserva.";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        // GET: ReservasViagens/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
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
                    return NotFound();
                }

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

                return View(reserva);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar exclusão da reserva {ReservaId}", id);
                TempData["Erro"] = "Erro ao carregar dados.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: ReservasViagens/Delete/5 - ✅ COM EXECUTION STRATEGY
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
                    if (reserva.StatusReservaViagem.Status == "Confirmada")
                    {
                        TempData["Erro"] = "Não é possível excluir reserva confirmada.";
                        return RedirectToAction(nameof(Delete), new { id = id });
                    }

                    // ✅ EXECUTION STRATEGY
                    var strategy = _context.Database.CreateExecutionStrategy();

                    await strategy.ExecuteAsync(async () =>
                    {
                        using var transaction = await _context.Database.BeginTransactionAsync();
                        try
                        {
                            // Remover serviços adicionais
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
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir reserva {ReservaId}", id);
                TempData["Erro"] = "Erro ao excluir reserva.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ========== GESTÃO DE SERVIÇOS ADICIONAIS - ✅ COM EXECUTION STRATEGY ==========

        // POST: Adicionar Serviço - ✅ COM EXECUTION STRATEGY
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
                    TempData["Erro"] = "Nome do serviço é obrigatório.";
                    return RedirectToAction(nameof(Details), new { id = reservaId });
                }

                if (string.IsNullOrWhiteSpace(descricao))
                {
                    TempData["Erro"] = "Descrição do serviço é obrigatória.";
                    return RedirectToAction(nameof(Details), new { id = reservaId });
                }

                if (preco <= 0)
                {
                    TempData["Erro"] = "Preço deve ser maior que zero.";
                    return RedirectToAction(nameof(Details), new { id = reservaId });
                }

                var reserva = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .Include(r => r.ServicosAdicionais)
                    .FirstOrDefaultAsync(r => r.Id == reservaId);

                if (reserva == null)
                {
                    TempData["Erro"] = "Reserva não encontrada.";
                    return RedirectToAction(nameof(Index));
                }

                if (reserva.StatusReservaViagem.Status == "Cancelada")
                {
                    TempData["Erro"] = "Não é possível adicionar serviços a reserva cancelada.";
                    return RedirectToAction(nameof(Details), new { id = reservaId });
                }

                // ✅ EXECUTION STRATEGY
                var strategy = _context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        var servico = new ServicoAdicional
                        {
                            ReservaViagemId = reservaId,
                            Nome = nome,
                            Descricao = descricao,
                            Preco = preco
                        };

                        _context.ServicosAdicionais.Add(servico);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        var totalGeral = reserva.ValorTotal + reserva.ServicosAdicionais.Sum(s => s.Preco) + preco;

                        _logger.LogInformation("Serviço '{Nome}' adicionado à reserva {ReservaId} por {User}", nome, reservaId, User.Identity?.Name);
                        TempData["Sucesso"] = $"Serviço '{nome}' adicionado! Total geral: {totalGeral:C}";
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                });

                return RedirectToAction(nameof(Details), new { id = reservaId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar serviço");
                TempData["Erro"] = "Erro ao adicionar serviço.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Remover Serviço - ✅ COM EXECUTION STRATEGY
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
                    TempData["Erro"] = "Serviço não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var reservaId = servico.ReservaViagemId;

                if (servico.ReservaViagem.StatusReservaViagem.Status == "Cancelada")
                {
                    TempData["Erro"] = "Não é possível remover serviços de reserva cancelada.";
                    return RedirectToAction(nameof(Details), new { id = reservaId });
                }

                // ✅ EXECUTION STRATEGY
                var strategy = _context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        _context.ServicosAdicionais.Remove(servico);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        var servicosRestantes = servico.ReservaViagem.ServicosAdicionais.Where(s => s.Id != servicoId).Sum(s => s.Preco);
                        var totalGeral = servico.ReservaViagem.ValorTotal + servicosRestantes;

                        _logger.LogInformation("Serviço '{Nome}' removido da reserva {ReservaId}", servico.Nome, reservaId);
                        TempData["Sucesso"] = $"Serviço '{servico.Nome}' removido! Total geral: {totalGeral:C}";
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                });

                return RedirectToAction(nameof(Details), new { id = reservaId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover serviço");
                TempData["Erro"] = "Erro ao remover serviço.";
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

        // ========== APIs AJAX ==========

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

                return Json(new
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
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados da reserva {ReservaId}", id);
                return StatusCode(500, new { message = "Erro interno" });
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
                _logger.LogError(ex, "Erro ao calcular valor");
                return Json(new { valorTotal = 0 });
            }
        }

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
                        descricao = $"{r.Cliente.Nome} - {r.PacoteViagem.Nome} ({r.DataViagem:dd/MM/yyyy})"
                    })
                    .OrderByDescending(r => r.dataViagem)
                    .Take(20)
                    .ToListAsync();

                return Json(reservas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na busca de reservas");
                return Json(new List<object>());
            }
        }

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
                _logger.LogError(ex, "Erro ao obter estatísticas");
                return Json(new { error = "Erro ao carregar estatísticas" });
            }
        }
    }
}