using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models;

namespace RentalTourismSystem.Controllers
{
    [Authorize] // Todo o controller requer autenticação
    public class PacotesViagensController : Controller
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<PacotesViagensController> _logger;

        public PacotesViagensController(RentalTourismContext context, ILogger<PacotesViagensController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: PacotesViagens - Todos os funcionários podem ver
        public async Task<IActionResult> Index(string? busca, decimal? precoMin, decimal? precoMax, string? orderBy)
        {
            try
            {
                _logger.LogInformation("Lista de pacotes de viagens acessada por usuário {User}", User.Identity?.Name);

                var pacotes = _context.PacotesViagens.AsQueryable();

                // Filtros
                if (!string.IsNullOrEmpty(busca))
                {
                    pacotes = pacotes.Where(p => p.Nome.Contains(busca) ||
                                               p.Destino.Contains(busca) ||
                                               p.Descricao.Contains(busca));
                    _logger.LogInformation("Busca por pacotes '{Busca}' realizada por {User}", busca, User.Identity?.Name);
                }

                if (precoMin.HasValue)
                {
                    pacotes = pacotes.Where(p => p.Preco >= precoMin.Value);
                }

                if (precoMax.HasValue)
                {
                    pacotes = pacotes.Where(p => p.Preco <= precoMax.Value);
                }

                // Ordenação
                pacotes = orderBy switch
                {
                    "preco_asc" => pacotes.OrderBy(p => p.Preco),
                    "preco_desc" => pacotes.OrderByDescending(p => p.Preco),
                    "duracao_asc" => pacotes.OrderBy(p => p.Duracao),
                    "duracao_desc" => pacotes.OrderByDescending(p => p.Duracao),
                    "destino" => pacotes.OrderBy(p => p.Destino),
                    _ => pacotes.OrderBy(p => p.Nome)
                };

                ViewBag.Busca = busca;
                ViewBag.PrecoMin = precoMin;
                ViewBag.PrecoMax = precoMax;
                ViewBag.OrderBy = orderBy;

                var listaPacotes = await pacotes.ToListAsync();
                return View(listaPacotes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de pacotes de viagens para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar lista de pacotes. Tente novamente.";
                return View(new List<PacoteViagem>());
            }
        }

        // GET: PacotesViagens/Details/5 - Todos podem ver detalhes
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de acesso a detalhes de pacote com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var pacote = await _context.PacotesViagens
                    .Include(p => p.ReservasViagens.OrderByDescending(r => r.DataReserva).Take(10))
                        .ThenInclude(r => r.Cliente)
                    .Include(p => p.ReservasViagens)
                        .ThenInclude(r => r.StatusReservaViagem)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (pacote == null)
                {
                    _logger.LogWarning("Pacote com ID {PacoteId} não encontrado. Acessado por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                // Calcular estatísticas do pacote
                var hoje = DateTime.Now;
                var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);

                ViewBag.ReservasMes = pacote.ReservasViagens
                    .Where(r => r.DataReserva >= inicioMes)
                    .Count();

                ViewBag.ReceitaMes = pacote.ReservasViagens
                    .Where(r => r.DataReserva >= inicioMes && r.StatusReservaViagem.Status == "Confirmada")
                    .Sum(r => r.ValorTotal);

                ViewBag.TotalReservas = pacote.ReservasViagens.Count();

                ViewBag.ReceitaTotal = pacote.ReservasViagens
                    .Where(r => r.StatusReservaViagem.Status == "Confirmada")
                    .Sum(r => r.ValorTotal);

                ViewBag.ReservasAtivas = pacote.ReservasViagens
                    .Where(r => r.StatusReservaViagem.Status == "Confirmada" || r.StatusReservaViagem.Status == "Pendente")
                    .Count();

                _logger.LogInformation("Detalhes do pacote {PacoteId} ('{PacoteNome}') acessados por {User}",
                    id, pacote.Nome, User.Identity?.Name);
                return View(pacote);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes do pacote {PacoteId} para usuário {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do pacote.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PacotesViagens/Create - Apenas Admin e Manager podem criar
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            _logger.LogInformation("Formulário de criação de pacote de viagem acessado por {User}", User.Identity?.Name);
            return View();
        }

        // POST: PacotesViagens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([Bind("Nome,Descricao,Destino,Duracao,Preco")] PacoteViagem pacote)
        {
            try
            {
                // Validar nome único
                await ValidarPacoteUnico(pacote);

                if (ModelState.IsValid)
                {
                    _context.Add(pacote);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Novo pacote de viagem '{PacoteNome}' para '{PacoteDestino}' criado por {User}",
                        pacote.Nome, pacote.Destino, User.Identity?.Name);

                    TempData["Sucesso"] = "Pacote de viagem criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro de banco ao criar pacote de viagem por {User}", User.Identity?.Name);
                TratarErrosBanco(ex, pacote);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar pacote de viagem por {User}", User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro interno do sistema. Tente novamente.");
            }

            return View(pacote);
        }

        // GET: PacotesViagens/Edit/5 - Apenas Admin e Manager podem editar
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de edição de pacote com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var pacote = await _context.PacotesViagens.FindAsync(id);
                if (pacote == null)
                {
                    _logger.LogWarning("Tentativa de edição de pacote inexistente {PacoteId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                _logger.LogInformation("Formulário de edição do pacote {PacoteId} ('{PacoteNome}') acessado por {User}",
                    id, pacote.Nome, User.Identity?.Name);
                return View(pacote);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição do pacote {PacoteId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do pacote para edição.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PacotesViagens/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,Destino,Duracao,Preco")] PacoteViagem pacote)
        {
            if (id != pacote.Id)
            {
                _logger.LogWarning("Tentativa de edição com ID inconsistente {Id} != {PacoteId} por {User}",
                    id, pacote.Id, User.Identity?.Name);
                return NotFound();
            }

            try
            {
                // Validar nome único (excluindo o próprio pacote)
                await ValidarPacoteUnico(pacote, pacote.Id);

                if (ModelState.IsValid)
                {
                    _context.Update(pacote);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Pacote {PacoteId} ('{PacoteNome}') atualizado por {User}",
                        pacote.Id, pacote.Nome, User.Identity?.Name);

                    TempData["Sucesso"] = "Pacote de viagem atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PacoteExists(pacote.Id))
                {
                    _logger.LogWarning("Pacote {PacoteId} não existe mais durante edição por {User}", pacote.Id, User.Identity?.Name);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Erro de concorrência ao editar pacote {PacoteId} por {User}", pacote.Id, User.Identity?.Name);
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Erro de banco ao editar pacote {PacoteId} por {User}", pacote.Id, User.Identity?.Name);
                TratarErrosBanco(ex, pacote);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao editar pacote {PacoteId} por {User}", pacote.Id, User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro interno do sistema. Tente novamente.");
            }

            return View(pacote);
        }

        // GET: PacotesViagens/Delete/5 - Apenas Admin pode excluir
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de exclusão de pacote com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var pacote = await _context.PacotesViagens
                    .Include(p => p.ReservasViagens)
                        .ThenInclude(r => r.Cliente)
                    .Include(p => p.ReservasViagens)
                        .ThenInclude(r => r.StatusReservaViagem)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (pacote == null)
                {
                    _logger.LogWarning("Tentativa de exclusão de pacote inexistente {PacoteId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                // Verificar se há impedimentos para exclusão
                var impedimentos = new List<string>();

                var reservasAtivas = pacote.ReservasViagens
                    .Where(r => r.StatusReservaViagem.Status == "Confirmada" || r.StatusReservaViagem.Status == "Pendente")
                    .Count();

                if (reservasAtivas > 0)
                {
                    impedimentos.Add($"{reservasAtivas} reserva(s) ativa(s)");
                }

                var totalReservas = pacote.ReservasViagens.Count;
                if (totalReservas > 0)
                {
                    impedimentos.Add($"{totalReservas} reserva(s) no histórico");
                }

                if (impedimentos.Any())
                {
                    ViewBag.Impedimentos = impedimentos;
                    _logger.LogInformation("Exclusão do pacote {PacoteId} bloqueada devido a impedimentos: {Impedimentos}",
                        id, string.Join(", ", impedimentos));
                }

                _logger.LogInformation("Formulário de confirmação de exclusão do pacote {PacoteId} ('{PacoteNome}') acessado por {User}",
                    id, pacote.Nome, User.Identity?.Name);
                return View(pacote);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de exclusão do pacote {PacoteId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do pacote para exclusão.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PacotesViagens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var pacote = await _context.PacotesViagens
                    .Include(p => p.ReservasViagens)
                        .ThenInclude(r => r.StatusReservaViagem)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pacote != null)
                {
                    // Verificar novamente os impedimentos (segurança extra)
                    var reservasAtivas = pacote.ReservasViagens
                        .Where(r => r.StatusReservaViagem.Status == "Confirmada" || r.StatusReservaViagem.Status == "Pendente")
                        .Count();

                    if (reservasAtivas > 0)
                    {
                        TempData["Erro"] = $"Não é possível excluir o pacote. Existem {reservasAtivas} reserva(s) ativa(s) vinculada(s) a ele.";
                        _logger.LogWarning("Exclusão do pacote {PacoteId} negada devido a reservas ativas por {User}", id, User.Identity?.Name);
                        return RedirectToAction(nameof(Delete), new { id = id });
                    }

                    string nomePacote = pacote.Nome;
                    string destinoPacote = pacote.Destino;

                    _context.PacotesViagens.Remove(pacote);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Pacote {PacoteId} ('{PacoteNome}' - {PacoteDestino}) excluído por {User}",
                        id, nomePacote, destinoPacote, User.Identity?.Name);

                    TempData["Sucesso"] = "Pacote de viagem excluído com sucesso!";
                }
                else
                {
                    _logger.LogWarning("Tentativa de exclusão de pacote inexistente {PacoteId} por {User}", id, User.Identity?.Name);
                    TempData["Erro"] = "Pacote não encontrado.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir pacote {PacoteId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao excluir pacote. Tente novamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ========== AÇÕES ESPECIAIS ==========

        // POST: PacotesViagens/AlterarPreco/5 - Admin e Manager podem alterar preço
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AlterarPreco(int id, decimal novoPreco, string? motivo)
        {
            try
            {
                var pacote = await _context.PacotesViagens.FindAsync(id);

                if (pacote == null)
                {
                    return Json(new { success = false, message = "Pacote não encontrado" });
                }

                if (novoPreco <= 0)
                {
                    return Json(new { success = false, message = "Preço deve ser maior que zero" });
                }

                var precoAnterior = pacote.Preco;
                pacote.Preco = novoPreco;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Preço do pacote {PacoteId} ('{PacoteNome}') alterado de R$ {PrecoAnterior:N2} para R$ {NovoPreco:N2} por {User}. Motivo: {Motivo}",
                    id, pacote.Nome, precoAnterior, novoPreco, User.Identity?.Name, motivo ?? "Não informado");

                return Json(new
                {
                    success = true,
                    message = $"Preço alterado de R$ {precoAnterior:N2} para R$ {novoPreco:N2} com sucesso!",
                    novoPreco = novoPreco
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar preço do pacote {PacoteId} por {User}", id, User.Identity?.Name);
                return Json(new { success = false, message = "Erro interno do sistema" });
            }
        }

        // ========== RELATÓRIO DO PACOTE (Apenas Admin e Manager) ==========

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Relatorio(int id, DateTime? dataInicio, DateTime? dataFim)
        {
            try
            {
                var pacote = await _context.PacotesViagens
                    .Include(p => p.ReservasViagens.OrderByDescending(r => r.DataReserva))
                        .ThenInclude(r => r.Cliente)
                    .Include(p => p.ReservasViagens)
                        .ThenInclude(r => r.StatusReservaViagem)
                    .Include(p => p.ReservasViagens)
                        .ThenInclude(r => r.ServicosAdicionais)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pacote == null)
                {
                    _logger.LogWarning("Tentativa de gerar relatório de pacote inexistente {PacoteId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                // Definir período padrão (últimos 3 meses)
                dataInicio ??= DateTime.Now.AddMonths(-3);
                dataFim ??= DateTime.Now;

                var reservasPeriodo = pacote.ReservasViagens
                    .Where(r => r.DataReserva >= dataInicio && r.DataReserva <= dataFim)
                    .ToList();

                ViewBag.DataInicio = dataInicio;
                ViewBag.DataFim = dataFim;
                ViewBag.ReservasPeriodo = reservasPeriodo;
                ViewBag.ReceitaPeriodo = reservasPeriodo.Where(r => r.StatusReservaViagem.Status == "Confirmada").Sum(r => r.ValorTotal);
                ViewBag.TotalPessoas = reservasPeriodo.Sum(r => r.Quantidade);

                // Análise por status
                ViewBag.ReservasConfirmadas = reservasPeriodo.Where(r => r.StatusReservaViagem.Status == "Confirmada").Count();
                ViewBag.ReservasPendentes = reservasPeriodo.Where(r => r.StatusReservaViagem.Status == "Pendente").Count();
                ViewBag.ReservasCanceladas = reservasPeriodo.Where(r => r.StatusReservaViagem.Status == "Cancelada").Count();

                // Top clientes do período
                ViewBag.TopClientes = reservasPeriodo
                    .Where(r => r.StatusReservaViagem.Status == "Confirmada")
                    .GroupBy(r => new { r.ClienteId, r.Cliente.Nome })
                    .Select(g => new
                    {
                        Nome = g.Key.Nome,
                        TotalReservas = g.Count(),
                        ValorTotal = g.Sum(r => r.ValorTotal),
                        TotalPessoas = g.Sum(r => r.Quantidade)
                    })
                    .OrderByDescending(c => c.ValorTotal)
                    .Take(5)
                    .ToList();

                _logger.LogInformation("Relatório do pacote {PacoteId} ('{PacoteNome}') gerado por {User} - Período: {DataInicio} a {DataFim}",
                    id, pacote.Nome, User.Identity?.Name, dataInicio?.ToString("dd/MM/yyyy"), dataFim?.ToString("dd/MM/yyyy"));

                return View(pacote);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório do pacote {PacoteId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao gerar relatório do pacote.";
                return RedirectToAction(nameof(Details), new { id = id });
            }
        }

        // ========== MÉTODOS AUXILIARES ==========

        private bool PacoteExists(int id)
        {
            return _context.PacotesViagens.Any(e => e.Id == id);
        }

        private async Task ValidarPacoteUnico(PacoteViagem pacote, int? idExcluir = null)
        {
            // Validar nome único para o mesmo destino
            var nomeDestinoExistente = await _context.PacotesViagens
                .AnyAsync(p => p.Nome == pacote.Nome && p.Destino == pacote.Destino && p.Id != idExcluir);

            if (nomeDestinoExistente)
            {
                ModelState.AddModelError("Nome", $"Já existe um pacote com o nome '{pacote.Nome}' para o destino '{pacote.Destino}'.");
            }
        }

        private void TratarErrosBanco(DbUpdateException ex, PacoteViagem pacote)
        {
            if (ex.InnerException != null && ex.InnerException.Message.Contains("Nome"))
            {
                ModelState.AddModelError("Nome", "Já existe um pacote com este nome para este destino.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Erro ao salvar no banco de dados. Verifique os dados e tente novamente.");
            }
        }

        // ========== APIs PARA CONSUMO INTERNO (AJAX) ==========

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPacoteData(int id)
        {
            try
            {
                var pacote = await _context.PacotesViagens
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pacote == null)
                {
                    return NotFound(new { message = "Pacote não encontrado" });
                }

                var resultado = new
                {
                    id = pacote.Id,
                    nome = pacote.Nome,
                    descricao = pacote.Descricao,
                    destino = pacote.Destino,
                    duracao = pacote.Duracao,
                    preco = pacote.Preco
                };

                return Json(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados do pacote {PacoteId} via API por {User}", id, User.Identity?.Name);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }

        // Calcular preço total do pacote
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CalcularPrecoTotal(int pacoteId, int quantidade)
        {
            try
            {
                var pacote = await _context.PacotesViagens.FindAsync(pacoteId);

                if (pacote == null)
                {
                    return NotFound();
                }

                var precoTotal = pacote.Preco * quantidade;

                return Json(new
                {
                    precoUnitario = pacote.Preco,
                    quantidade = quantidade,
                    precoTotal = precoTotal
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular preço do pacote {PacoteId} para {Quantidade} pessoas por {User}",
                    pacoteId, quantidade, User.Identity?.Name);
                return Json(new { precoTotal = 0 });
            }
        }

        // Busca rápida de pacotes para autocomplete
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> BuscarPacotes(string? termo = null)
        {
            try
            {
                var query = _context.PacotesViagens.AsQueryable();

                if (!string.IsNullOrWhiteSpace(termo))
                {
                    query = query.Where(p => p.Nome.Contains(termo) ||
                                           p.Destino.Contains(termo) ||
                                           p.Descricao.Contains(termo));
                }

                var pacotes = await query
                    .Select(p => new
                    {
                        id = p.Id,
                        nome = p.Nome,
                        destino = p.Destino,
                        duracao = p.Duracao,
                        preco = p.Preco,
                        descricao = $"{p.Nome} - {p.Destino} ({p.Duracao} dias) - R$ {p.Preco:N2}"
                    })
                    .Take(20)
                    .ToListAsync();

                return Json(pacotes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na busca de pacotes por {User}", User.Identity?.Name);
                return Json(new List<object>());
            }
        }

        // Obter pacotes por faixa de preço
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPacotesPorPreco(decimal precoMin = 0, decimal precoMax = 99999)
        {
            try
            {
                var pacotes = await _context.PacotesViagens
                    .Where(p => p.Preco >= precoMin && p.Preco <= precoMax)
                    .Select(p => new
                    {
                        id = p.Id,
                        nome = p.Nome,
                        destino = p.Destino,
                        preco = p.Preco,
                        duracao = p.Duracao
                    })
                    .OrderBy(p => p.preco)
                    .ToListAsync();

                return Json(pacotes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pacotes por faixa de preço por {User}", User.Identity?.Name);
                return Json(new List<object>());
            }
        }

        // Estatísticas do pacote
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetEstatisticasPacote(int id, DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            try
            {
                dataInicio ??= DateTime.Now.AddMonths(-1);
                dataFim ??= DateTime.Now;

                var pacote = await _context.PacotesViagens
                    .Include(p => p.ReservasViagens)
                        .ThenInclude(r => r.StatusReservaViagem)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (pacote == null)
                {
                    return NotFound();
                }

                var reservasPeriodo = pacote.ReservasViagens
                    .Where(r => r.DataReserva >= dataInicio && r.DataReserva <= dataFim)
                    .ToList();

                var estatisticas = new
                {
                    reservas = reservasPeriodo.Count,
                    receita = reservasPeriodo.Where(r => r.StatusReservaViagem.Status == "Confirmada").Sum(r => r.ValorTotal),
                    pessoas = reservasPeriodo.Sum(r => r.Quantidade),
                    reservasConfirmadas = reservasPeriodo.Count(r => r.StatusReservaViagem.Status == "Confirmada"),
                    reservasPendentes = reservasPeriodo.Count(r => r.StatusReservaViagem.Status == "Pendente"),
                    reservasCanceladas = reservasPeriodo.Count(r => r.StatusReservaViagem.Status == "Cancelada"),
                    totalReservas = pacote.ReservasViagens.Count,
                    receitaTotal = pacote.ReservasViagens.Where(r => r.StatusReservaViagem.Status == "Confirmada").Sum(r => r.ValorTotal)
                };

                return Json(estatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas do pacote {PacoteId} por {User}", id, User.Identity?.Name);
                return Json(new { error = "Erro ao carregar estatísticas" });
            }
        }
    }
}