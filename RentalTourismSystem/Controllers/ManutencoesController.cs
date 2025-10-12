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
                _logger.LogInformation("Lista de manutenções acessada por usuário {User}", User.Identity?.Name);

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

                // Estatísticas
                ViewBag.TotalManutencoes = manutencoes.Count;
                ViewBag.ManutencoesAgendadas = manutencoes.Count(m => m.StatusManutencao?.Status == "Agendada");
                ViewBag.ManutencoesEmAndamento = manutencoes.Count(m => m.StatusManutencao?.Status == "Em Andamento");
                ViewBag.CustoTotal = manutencoes.Sum(m => m.CustoTotal);
                ViewBag.MediaCusto = manutencoes.Any() ? manutencoes.Average(m => m.CustoTotal) : 0;

                return View(manutencoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de manutenções para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar lista de manutenções. Tente novamente.";
                return View(new List<ManutencaoVeiculo>());
            }
        }

        // GET: Manutencoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de acesso a detalhes de manutenção com ID nulo por {User}", User.Identity?.Name);
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
                    _logger.LogWarning("Manutenção com ID {ManutencaoId} não encontrada. Acessado por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                _logger.LogInformation("Detalhes da manutenção {ManutencaoId} acessados por {User}", id, User.Identity?.Name);
                return View(manutencao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar detalhes da manutenção {ManutencaoId} para usuário {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados da manutenção.";
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

                _logger.LogInformation("Formulário de criação de manutenção acessado por {User}", User.Identity?.Name);
                return View(manutencao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de criação de manutenção por {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar formulário.";
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
                // Validações customizadas
                if (manutencao.DataInicio.HasValue && manutencao.DataInicio.Value < manutencao.DataAgendada)
                {
                    ModelState.AddModelError("DataInicio", "A data de início não pode ser anterior à data agendada.");
                }

                if (manutencao.DataConclusao.HasValue && manutencao.DataInicio.HasValue && manutencao.DataConclusao.Value < manutencao.DataInicio.Value)
                {
                    ModelState.AddModelError("DataConclusao", "A data de conclusão não pode ser anterior à data de início.");
                }

                if (manutencao.ProximaQuilometragem.HasValue && manutencao.ProximaQuilometragem.Value <= manutencao.QuilometragemAtual)
                {
                    ModelState.AddModelError("ProximaQuilometragem", "A próxima quilometragem deve ser maior que a quilometragem atual.");
                }

                if (ModelState.IsValid)
                {
                    manutencao.DataCadastro = DateTime.Now;

                    // Atualizar status do veículo se a manutenção estiver em andamento
                    if (manutencao.StatusManutencaoId == 2) // Em Andamento
                    {
                        var veiculo = await _context.Veiculos.FindAsync(manutencao.VeiculoId);
                        if (veiculo != null && veiculo.StatusCarroId != 3) // Se não estiver em manutenção
                        {
                            veiculo.StatusCarroId = 3; // Manutenção
                        }
                    }

                    _context.Add(manutencao);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Nova manutenção criada para veículo {VeiculoId} por {User}", manutencao.VeiculoId, User.Identity?.Name);

                    TempData["Sucesso"] = "Manutenção cadastrada com sucesso!";
                    return RedirectToAction(nameof(Details), new { id = manutencao.Id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar manutenção por {User}", User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro ao salvar manutenção. Tente novamente.");
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
                _logger.LogWarning("Tentativa de edição de manutenção com ID nulo por {User}", User.Identity?.Name);
                return NotFound();
            }

            try
            {
                var manutencao = await _context.ManutencoesVeiculos
                    .Include(m => m.Itens)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (manutencao == null)
                {
                    _logger.LogWarning("Tentativa de edição de manutenção inexistente {ManutencaoId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                await CarregarViewBags(manutencao.VeiculoId, manutencao.TipoManutencaoId, manutencao.StatusManutencaoId, manutencao.FuncionarioId);
                _logger.LogInformation("Formulário de edição da manutenção {ManutencaoId} acessado por {User}", id, User.Identity?.Name);
                return View(manutencao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de edição da manutenção {ManutencaoId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados da manutenção para edição.";
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
                _logger.LogWarning("Tentativa de edição com ID inconsistente {Id} != {ManutencaoId} por {User}", id, manutencao.Id, User.Identity?.Name);
                return NotFound();
            }

            try
            {
                // Validações customizadas
                if (manutencao.DataInicio.HasValue && manutencao.DataInicio.Value < manutencao.DataAgendada)
                {
                    ModelState.AddModelError("DataInicio", "A data de início não pode ser anterior à data agendada.");
                }

                if (manutencao.DataConclusao.HasValue && manutencao.DataInicio.HasValue && manutencao.DataConclusao.Value < manutencao.DataInicio.Value)
                {
                    ModelState.AddModelError("DataConclusao", "A data de conclusão não pode ser anterior à data de início.");
                }

                if (manutencao.ProximaQuilometragem.HasValue && manutencao.ProximaQuilometragem.Value <= manutencao.QuilometragemAtual)
                {
                    ModelState.AddModelError("ProximaQuilometragem", "A próxima quilometragem deve ser maior que a quilometragem atual.");
                }

                if (ModelState.IsValid)
                {
                    // Preservar a data de cadastro original
                    var manutencaoOriginal = await _context.ManutencoesVeiculos.AsNoTracking().FirstOrDefaultAsync(m => m.Id == manutencao.Id);
                    if (manutencaoOriginal != null)
                    {
                        manutencao.DataCadastro = manutencaoOriginal.DataCadastro;
                    }

                    // Atualizar status do veículo conforme status da manutenção
                    var veiculo = await _context.Veiculos.FindAsync(manutencao.VeiculoId);
                    if (veiculo != null)
                    {
                        if (manutencao.StatusManutencaoId == 2) // Em Andamento
                        {
                            if (veiculo.StatusCarroId != 3)
                            {
                                veiculo.StatusCarroId = 3; // Manutenção
                            }
                        }
                        else if (manutencao.StatusManutencaoId == 3) // Concluída
                        {
                            if (veiculo.StatusCarroId == 3) // Se estava em manutenção
                            {
                                // Verificar se há outras manutenções em andamento
                                var outraManutencaoEmAndamento = await _context.ManutencoesVeiculos
                                    .AnyAsync(m => m.VeiculoId == manutencao.VeiculoId && m.Id != manutencao.Id && m.StatusManutencaoId == 2);

                                if (!outraManutencaoEmAndamento)
                                {
                                    veiculo.StatusCarroId = 1; // Disponível
                                }
                            }

                            // Atualizar quilometragem do veículo se a manutenção foi concluída
                            if (manutencao.QuilometragemAtual > veiculo.Quilometragem)
                            {
                                veiculo.Quilometragem = manutencao.QuilometragemAtual;
                            }
                        }
                    }

                    _context.Update(manutencao);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Manutenção {ManutencaoId} atualizada por {User}", manutencao.Id, User.Identity?.Name);

                    TempData["Sucesso"] = "Manutenção atualizada com sucesso!";
                    return RedirectToAction(nameof(Details), new { id = manutencao.Id });
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ManutencaoExists(manutencao.Id))
                {
                    _logger.LogWarning("Manutenção {ManutencaoId} não existe mais durante edição por {User}", manutencao.Id, User.Identity?.Name);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Erro de concorrência ao editar manutenção {ManutencaoId} por {User}", manutencao.Id, User.Identity?.Name);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao editar manutenção {ManutencaoId} por {User}", manutencao.Id, User.Identity?.Name);
                ModelState.AddModelError(string.Empty, "Erro ao salvar manutenção. Tente novamente.");
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
                _logger.LogWarning("Tentativa de exclusão de manutenção com ID nulo por {User}", User.Identity?.Name);
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
                    _logger.LogWarning("Tentativa de exclusão de manutenção inexistente {ManutencaoId} por {User}", id, User.Identity?.Name);
                    return NotFound();
                }

                _logger.LogInformation("Formulário de confirmação de exclusão da manutenção {ManutencaoId} acessado por {User}", id, User.Identity?.Name);
                return View(manutencao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar formulário de exclusão da manutenção {ManutencaoId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados da manutenção para exclusão.";
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

                    _logger.LogInformation("Manutenção {ManutencaoId} excluída por {User}", id, User.Identity?.Name);
                    TempData["Sucesso"] = "Manutenção excluída com sucesso!";
                }
                else
                {
                    _logger.LogWarning("Tentativa de exclusão de manutenção inexistente {ManutencaoId} por {User}", id, User.Identity?.Name);
                    TempData["Erro"] = "Manutenção não encontrada.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir manutenção {ManutencaoId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao excluir manutenção. Tente novamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Manutencoes/HistoricoVeiculo/5
        public async Task<IActionResult> HistoricoVeiculo(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Tentativa de acesso ao histórico com veículo ID nulo por {User}", User.Identity?.Name);
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
                    _logger.LogWarning("Veículo {VeiculoId} não encontrado por {User}", id, User.Identity?.Name);
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

                _logger.LogInformation("Histórico de manutenções do veículo {VeiculoId} acessado por {User}", id, User.Identity?.Name);
                return View(manutencoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar histórico de manutenções do veículo {VeiculoId} por {User}", id, User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar histórico de manutenções.";
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

                // Estatísticas gerais
                ViewBag.DataInicio = dataInicio.Value.ToString("yyyy-MM-dd");
                ViewBag.DataFim = dataFim.Value.ToString("yyyy-MM-dd");
                ViewBag.TotalManutencoes = manutencoes.Count;
                ViewBag.CustoTotal = manutencoes.Sum(m => m.CustoTotal);
                ViewBag.MediaCusto = manutencoes.Any() ? manutencoes.Average(m => m.CustoTotal) : 0;
                ViewBag.TotalPecas = manutencoes.Sum(m => m.CustoPecas ?? 0);
                ViewBag.TotalMaoObra = manutencoes.Sum(m => m.CustoMaoObra ?? 0);
                ViewBag.ManutencoesPreventivas = manutencoes.Count(m => m.Preventiva);
                ViewBag.ManutencoesUrgentes = manutencoes.Count(m => m.Urgente);

                // Manutenções por tipo
                ViewBag.ManutencoesPorTipo = manutencoes
                    .GroupBy(m => m.TipoManutencao?.Nome ?? "Não especificado")
                    .Select(g => new { Tipo = g.Key, Quantidade = g.Count(), Custo = g.Sum(m => m.CustoTotal) })
                    .OrderByDescending(x => x.Quantidade)
                    .ToList();

                // Manutenções por veículo
                ViewBag.ManutencoesPorVeiculo = manutencoes
                    .GroupBy(m => new { m.VeiculoId, Descricao = $"{m.Veiculo?.Marca} {m.Veiculo?.Modelo} ({m.Veiculo?.Placa})" })
                    .Select(g => new { g.Key.VeiculoId, g.Key.Descricao, Quantidade = g.Count(), Custo = g.Sum(m => m.CustoTotal) })
                    .OrderByDescending(x => x.Custo)
                    .ToList();

                await CarregarViewBagsFiltros(veiculoId, null, tipoId);

                _logger.LogInformation("Relatório de manutenções acessado por {User}", User.Identity?.Name);
                return View(manutencoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de manutenções por {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao gerar relatório.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ========== MÉTODOS AUXILIARES ==========

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
