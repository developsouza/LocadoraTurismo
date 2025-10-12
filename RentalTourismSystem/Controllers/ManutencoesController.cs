using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models;

namespace RentalTourismSystem.Controllers
{
    [Authorize]
    public class ManutencoesController : Controller
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<ManutencoesController> _logger;

        public ManutencoesController(RentalTourismContext context, ILogger<ManutencoesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Manutencoes
        public async Task<IActionResult> Index(int? veiculoId, int? statusId, int? tipoId, bool? preventiva, bool? urgente, DateTime? dataInicio, DateTime? dataFim)
        {
            try
            {
                _logger.LogInformation("Lista de manuten��es acessada por usu�rio {User}", User.Identity?.Name);

                var query = _context.ManutencoesVeiculos
                    .Include(m => m.Veiculo)
                    .Include(m => m.TipoManutencao)
                    .Include(m => m.StatusManutencao)
                    .Include(m => m.Funcionario)
                    .AsQueryable();

                // Filtros
                if (veiculoId.HasValue)
                {
                    query = query.Where(m => m.VeiculoId == veiculoId);
                }

                if (statusId.HasValue)
                {
                    query = query.Where(m => m.StatusManutencaoId == statusId);
                }

                if (tipoId.HasValue)
                {
                    query = query.Where(m => m.TipoManutencaoId == tipoId);
                }

                if (preventiva.HasValue)
                {
                    query = query.Where(m => m.Preventiva == preventiva.Value);
                }

                if (urgente.HasValue)
                {
                    query = query.Where(m => m.Urgente == urgente.Value);
                }

                if (dataInicio.HasValue)
                {
                    query = query.Where(m => m.DataAgendada >= dataInicio.Value);
                }

                if (dataFim.HasValue)
                {
                    query = query.Where(m => m.DataAgendada <= dataFim.Value);
                }

                // Carregar ViewBags para filtros
                await CarregarViewBagsFiltros(veiculoId, statusId, tipoId);
                ViewBag.Preventiva = preventiva;
                ViewBag.Urgente = urgente;
                ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
                ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");

                var manutencoes = await query
                    .OrderByDescending(m => m.DataAgendada)
                    .ToListAsync();

                // Estat�sticas
                ViewBag.TotalManutencoes = manutencoes.Count;
                ViewBag.ManutencoesAgendadas = manutencoes.Count(m => m.StatusManutencao?.Status == "Agendada");
                ViewBag.ManutencoesEmAndamento = manutencoes.Count(m => m.StatusManutencao?.Status == "Em Andamento");
                ViewBag.CustoTotal = manutencoes.Sum(m => m.CustoTotal);
                ViewBag.MediaCusto = manutencoes.Any() ? manutencoes.Average(m => m.CustoTotal) : 0;

                return View(manutencoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de manuten��es para usu�rio {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar lista de manuten��es. Tente novamente.";
                return View(new List<ManutencaoVeiculo>());
            }
        }

        // GET: Manutencoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de acesso a detalhes de manuten��o com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var manutencao = await _context.ManutencoesVeiculos
                    .Include(m => m.Veiculo)
                        .ThenInclude(v => v.StatusCarro)
                    .Include(m => m.Veiculo)
                        .ThenInclude(v => v.Agencia)
                    .Include(m => m.TipoManutencao)
                    .Include(m => m.StatusManutencao)
                    .Include(m => m.Funcionario)
                    .Include(m => m.Itens)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (manutencao == null)
                {
                    _logger.LogWarning("Manuten��o com ID {ManutencaoId} n�o encontrada. Acessado por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                _logger.LogInformation("Detalhes da manuten��o {ManutencaoId} acessados por {User}", id, User.Identity?.Name);
                return View(manutencao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes da manuten��o {ManutencaoId} para usu�rio {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados da manuten��o.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Manutencoes/Create
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(int? veiculoId)
        {
            try
            {
                await CarregarViewBags(veiculoId: veiculoId);

                var manutencao = new ManutencaoVeiculo();
                if (veiculoId.HasValue)
                {
                    var veiculo = await _context.Veiculos.FindAsync(veiculoId.Value);
                    if (veiculo != null)
                    {
                        manutencao.VeiculoId = veiculoId.Value;
                        manutencao.QuilometragemAtual = veiculo.Quilometragem;
                    }
                }

                _logger.LogInformation("Formul�rio de cria��o de manuten��o acessado por {User}", User.Identity?.Name);
                return View(manutencao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formul�rio de cria��o de manuten��o por {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar formul�rio.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Manutencoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([Bind("VeiculoId,TipoManutencaoId,StatusManutencaoId,DataAgendada,DataInicio,DataConclusao,QuilometragemAtual,ProximaQuilometragem,Descricao,Observacoes,Oficina,Custo,CustoPecas,CustoMaoObra,GarantiaDias,Preventiva,Urgente,FuncionarioId,NotaFiscal")] ManutencaoVeiculo manutencao)
        {
            try
            {
                // Valida��es customizadas
                if (manutencao.DataInicio.HasValue && manutencao.DataInicio.Value < manutencao.DataAgendada)
                {
                    ModelState.AddModelError("DataInicio", "A data de in�cio n�o pode ser anterior � data agendada.");
                }

                if (manutencao.DataConclusao.HasValue && manutencao.DataInicio.HasValue && manutencao.DataConclusao.Value < manutencao.DataInicio.Value)
                {
                    ModelState.AddModelError("DataConclusao", "A data de conclus�o n�o pode ser anterior � data de in�cio.");
                }

                if (manutencao.ProximaQuilometragem.HasValue && manutencao.ProximaQuilometragem.Value <= manutencao.QuilometragemAtual)
                {
                    ModelState.AddModelError("ProximaQuilometragem", "A pr�xima quilometragem deve ser maior que a quilometragem atual.");
                }

                if (ModelState.IsValid)
                {
                    manutencao.DataCadastro = DateTime.Now;

                    // Atualizar status do ve�culo se a manuten��o estiver em andamento
                    if (manutencao.StatusManutencaoId == 2) // Em Andamento
                    {
                        var veiculo = await _context.Veiculos.FindAsync(manutencao.VeiculoId);
                        if (veiculo != null && veiculo.StatusCarroId != 3) // Se n�o estiver em manuten��o
                        {
                            veiculo.StatusCarroId = 3; // Manuten��o
                        }
                    }

                    _context.Add(manutencao);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Nova manuten��o criada para ve�culo {VeiculoId} por {User}", manutencao.VeiculoId, User.Identity?.Name);

                    TempData["Sucesso"] = "Manuten��o cadastrada com sucesso!";
                    return RedirectToAction(nameof(Details), new { id = manutencao.Id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar manuten��o por {User}", User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro ao salvar manuten��o. Tente novamente.");
            }

            await CarregarViewBags(manutencao.VeiculoId, manutencao.TipoManutencaoId, manutencao.StatusManutencaoId, manutencao.FuncionarioId);
            return View(manutencao);
        }

        // GET: Manutencoes/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de edi��o de manuten��o com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var manutencao = await _context.ManutencoesVeiculos
                    .Include(m => m.Itens)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (manutencao == null)
                {
                    _logger.LogWarning("Tentativa de edi��o de manuten��o inexistente {ManutencaoId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                await CarregarViewBags(manutencao.VeiculoId, manutencao.TipoManutencaoId, manutencao.StatusManutencaoId, manutencao.FuncionarioId);
                _logger.LogInformation("Formul�rio de edi��o da manuten��o {ManutencaoId} acessado por {User}", id, User.Identity?.Name);
                return View(manutencao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formul�rio de edi��o da manuten��o {ManutencaoId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados da manuten��o para edi��o.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Manutencoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VeiculoId,TipoManutencaoId,StatusManutencaoId,DataAgendada,DataInicio,DataConclusao,QuilometragemAtual,ProximaQuilometragem,Descricao,Observacoes,Oficina,Custo,CustoPecas,CustoMaoObra,GarantiaDias,Preventiva,Urgente,FuncionarioId,NotaFiscal")] ManutencaoVeiculo manutencao)
        {
            if (id != manutencao.Id)
            {
                _logger.LogWarning("Tentativa de edi��o com ID inconsistente {Id} != {ManutencaoId} por {User}", id, manutencao.Id, User.Identity?.Name);
                return NotFound();
            }

            try
            {
                // Valida��es customizadas
                if (manutencao.DataInicio.HasValue && manutencao.DataInicio.Value < manutencao.DataAgendada)
                {
                    ModelState.AddModelError("DataInicio", "A data de in�cio n�o pode ser anterior � data agendada.");
                }

                if (manutencao.DataConclusao.HasValue && manutencao.DataInicio.HasValue && manutencao.DataConclusao.Value < manutencao.DataInicio.Value)
                {
                    ModelState.AddModelError("DataConclusao", "A data de conclus�o n�o pode ser anterior � data de in�cio.");
                }

                if (manutencao.ProximaQuilometragem.HasValue && manutencao.ProximaQuilometragem.Value <= manutencao.QuilometragemAtual)
                {
                    ModelState.AddModelError("ProximaQuilometragem", "A pr�xima quilometragem deve ser maior que a quilometragem atual.");
                }

                if (ModelState.IsValid)
                {
                    // Preservar a data de cadastro original
                    var manutencaoOriginal = await _context.ManutencoesVeiculos.AsNoTracking().FirstOrDefaultAsync(m => m.Id == manutencao.Id);
                    if (manutencaoOriginal != null)
                    {
                        manutencao.DataCadastro = manutencaoOriginal.DataCadastro;
                    }

                    // Atualizar status do ve�culo conforme status da manuten��o
                    var veiculo = await _context.Veiculos.FindAsync(manutencao.VeiculoId);
                    if (veiculo != null)
                    {
                        if (manutencao.StatusManutencaoId == 2) // Em Andamento
                        {
                            if (veiculo.StatusCarroId != 3)
                            {
                                veiculo.StatusCarroId = 3; // Manuten��o
                            }
                        }
                        else if (manutencao.StatusManutencaoId == 3) // Conclu�da
                        {
                            if (veiculo.StatusCarroId == 3) // Se estava em manuten��o
                            {
                                // Verificar se h� outras manuten��es em andamento
                                var outraManutencaoEmAndamento = await _context.ManutencoesVeiculos
                                    .AnyAsync(m => m.VeiculoId == manutencao.VeiculoId && m.Id != manutencao.Id && m.StatusManutencaoId == 2);

                                if (!outraManutencaoEmAndamento)
                                {
                                    veiculo.StatusCarroId = 1; // Dispon�vel
                                }
                            }

                            // Atualizar quilometragem do ve�culo se a manuten��o foi conclu�da
                            if (manutencao.QuilometragemAtual > veiculo.Quilometragem)
                            {
                                veiculo.Quilometragem = manutencao.QuilometragemAtual;
                            }
                        }
                    }

                    _context.Update(manutencao);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Manuten��o {ManutencaoId} atualizada por {User}", manutencao.Id, User.Identity?.Name);

                    TempData["Sucesso"] = "Manuten��o atualizada com sucesso!";
                    return RedirectToAction(nameof(Details), new { id = manutencao.Id });
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ManutencaoExists(manutencao.Id))
                {
                    _logger.LogWarning("Manuten��o {ManutencaoId} n�o existe mais durante edi��o por {User}", manutencao.Id, User.Identity?.Name);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Erro de concorr�ncia ao editar manuten��o {ManutencaoId} por {User}", manutencao.Id, User.Identity?.Name);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao editar manuten��o {ManutencaoId} por {User}", manutencao.Id, User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro ao salvar manuten��o. Tente novamente.");
            }

            await CarregarViewBags(manutencao.VeiculoId, manutencao.TipoManutencaoId, manutencao.StatusManutencaoId, manutencao.FuncionarioId);
            return View(manutencao);
        }

        // GET: Manutencoes/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de exclus�o de manuten��o com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var manutencao = await _context.ManutencoesVeiculos
                    .Include(m => m.Veiculo)
                    .Include(m => m.TipoManutencao)
                    .Include(m => m.StatusManutencao)
                    .Include(m => m.Funcionario)
                    .Include(m => m.Itens)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (manutencao == null)
                {
                    _logger.LogWarning("Tentativa de exclus�o de manuten��o inexistente {ManutencaoId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                _logger.LogInformation("Formul�rio de confirma��o de exclus�o da manuten��o {ManutencaoId} acessado por {User}", id, User.Identity?.Name);
                return View(manutencao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formul�rio de exclus�o da manuten��o {ManutencaoId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados da manuten��o para exclus�o.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Manutencoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var manutencao = await _context.ManutencoesVeiculos
                    .Include(m => m.Itens)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (manutencao != null)
                {
                    _context.ManutencoesVeiculos.Remove(manutencao);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Manuten��o {ManutencaoId} exclu�da por {User}", id, User.Identity?.Name);
                    TempData["Sucesso"] = "Manuten��o exclu�da com sucesso!";
                }
                else
                {
                    _logger.LogWarning("Tentativa de exclus�o de manuten��o inexistente {ManutencaoId} por {User}", id, User.Identity?.Name);
                    TempData["Erro"] = "Manuten��o n�o encontrada.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir manuten��o {ManutencaoId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao excluir manuten��o. Tente novamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Manutencoes/HistoricoVeiculo/5
        public async Task<IActionResult> HistoricoVeiculo(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de acesso ao hist�rico com ve�culo ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var veiculo = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .Include(v => v.Agencia)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (veiculo == null)
                {
                    _logger.LogWarning("Ve�culo {VeiculoId} n�o encontrado por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                var manutencoes = await _context.ManutencoesVeiculos
                    .Include(m => m.TipoManutencao)
                    .Include(m => m.StatusManutencao)
                    .Include(m => m.Funcionario)
                    .Include(m => m.Itens)
                    .Where(m => m.VeiculoId == id)
                    .OrderByDescending(m => m.DataAgendada)
                    .ToListAsync();

                ViewBag.Veiculo = veiculo;
                ViewBag.TotalManutencoes = manutencoes.Count;
                ViewBag.CustoTotal = manutencoes.Sum(m => m.CustoTotal);
                ViewBag.MediaCusto = manutencoes.Any() ? manutencoes.Average(m => m.CustoTotal) : 0;
                ViewBag.UltimaManutencao = manutencoes.FirstOrDefault()?.DataAgendada;

                _logger.LogInformation("Hist�rico de manuten��es do ve�culo {VeiculoId} acessado por {User}", id, User.Identity?.Name);
                return View(manutencoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar hist�rico de manuten��es do ve�culo {VeiculoId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar hist�rico de manuten��es.";
                return RedirectToAction("Index", "Veiculos");
            }
        }

        // GET: Manutencoes/Relatorio
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Relatorio(DateTime? dataInicio, DateTime? dataFim, int? veiculoId, int? tipoId)
        {
            try
            {
                dataInicio ??= DateTime.Now.AddMonths(-1);
                dataFim ??= DateTime.Now;

                var query = _context.ManutencoesVeiculos
                    .Include(m => m.Veiculo)
                    .Include(m => m.TipoManutencao)
                    .Include(m => m.StatusManutencao)
                    .Include(m => m.Itens)
                    .Where(m => m.DataAgendada >= dataInicio && m.DataAgendada <= dataFim);

                if (veiculoId.HasValue)
                {
                    query = query.Where(m => m.VeiculoId == veiculoId);
                }

                if (tipoId.HasValue)
                {
                    query = query.Where(m => m.TipoManutencaoId == tipoId);
                }

                var manutencoes = await query.ToListAsync();

                // Estat�sticas gerais
                ViewBag.DataInicio = dataInicio.Value.ToString("yyyy-MM-dd");
                ViewBag.DataFim = dataFim.Value.ToString("yyyy-MM-dd");
                ViewBag.TotalManutencoes = manutencoes.Count;
                ViewBag.CustoTotal = manutencoes.Sum(m => m.CustoTotal);
                ViewBag.MediaCusto = manutencoes.Any() ? manutencoes.Average(m => m.CustoTotal) : 0;
                ViewBag.TotalPecas = manutencoes.Sum(m => m.CustoPecas ?? 0);
                ViewBag.TotalMaoObra = manutencoes.Sum(m => m.CustoMaoObra ?? 0);
                ViewBag.ManutencoesPreventivas = manutencoes.Count(m => m.Preventiva);
                ViewBag.ManutencoesUrgentes = manutencoes.Count(m => m.Urgente);

                // Manuten��es por tipo
                ViewBag.ManutencoesPorTipo = manutencoes
                    .GroupBy(m => m.TipoManutencao?.Nome ?? "N�o especificado")
                    .Select(g => new { Tipo = g.Key, Quantidade = g.Count(), Custo = g.Sum(m => m.CustoTotal) })
                    .OrderByDescending(x => x.Quantidade)
                    .ToList();

                // Manuten��es por ve�culo
                ViewBag.ManutencoesPorVeiculo = manutencoes
                    .GroupBy(m => new { m.VeiculoId, Descricao = $"{m.Veiculo?.Marca} {m.Veiculo?.Modelo} ({m.Veiculo?.Placa})" })
                    .Select(g => new { g.Key.VeiculoId, g.Key.Descricao, Quantidade = g.Count(), Custo = g.Sum(m => m.CustoTotal) })
                    .OrderByDescending(x => x.Custo)
                    .ToList();

                await CarregarViewBagsFiltros(veiculoId, null, tipoId);

                _logger.LogInformation("Relat�rio de manuten��es acessado por {User}", User.Identity?.Name);
                return View(manutencoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relat�rio de manuten��es por {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao gerar relat�rio.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ========== M�TODOS AUXILIARES ==========

        private bool ManutencaoExists(int id)
        {
            return _context.ManutencoesVeiculos.Any(e => e.Id == id);
        }

        private async Task CarregarViewBags(int? veiculoId = null, int? tipoId = null, int? statusId = null, int? funcionarioId = null)
        {
            ViewBag.VeiculoId = new SelectList(
                await _context.Veiculos
                    .OrderBy(v => v.Marca)
                    .ThenBy(v => v.Modelo)
                    .Select(v => new { v.Id, Descricao = $"{v.Marca} {v.Modelo} ({v.Placa})" })
                    .ToListAsync(),
                "Id",
                "Descricao",
                veiculoId);

            ViewBag.TipoManutencaoId = new SelectList(
                await _context.TiposManutencao.Where(t => t.Ativo).OrderBy(t => t.Nome).ToListAsync(),
                "Id",
                "Nome",
                tipoId);

            ViewBag.StatusManutencaoId = new SelectList(
                await _context.StatusManutencoes.ToListAsync(),
                "Id",
                "Status",
                statusId);

            ViewBag.FuncionarioId = new SelectList(
                await _context.Funcionarios
                    .Where(f => f.Ativo)
                    .OrderBy(f => f.Nome)
                    .ToListAsync(),
                "Id",
                "Nome",
                funcionarioId);
        }

        private async Task CarregarViewBagsFiltros(int? veiculoId = null, int? statusId = null, int? tipoId = null)
        {
            ViewBag.VeiculoIdFiltro = new SelectList(
                await _context.Veiculos
                    .OrderBy(v => v.Marca)
                    .ThenBy(v => v.Modelo)
                    .Select(v => new { v.Id, Descricao = $"{v.Marca} {v.Modelo} ({v.Placa})" })
                    .ToListAsync(),
                "Id",
                "Descricao",
                veiculoId);

            ViewBag.StatusIdFiltro = new SelectList(
                await _context.StatusManutencoes.ToListAsync(),
                "Id",
                "Status",
                statusId);

            ViewBag.TipoIdFiltro = new SelectList(
                await _context.TiposManutencao.Where(t => t.Ativo).OrderBy(t => t.Nome).ToListAsync(),
                "Id",
                "Nome",
                tipoId);
        }
    }
}
