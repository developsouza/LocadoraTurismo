using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using System.Text;

namespace RentalTourismSystem.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class RelatoriosController : Controller
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<RelatoriosController> _logger;

        public RelatoriosController(RentalTourismContext context, ILogger<RelatoriosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region DASHBOARD PRINCIPAL

        // GET: Relatórios - Dashboard de Relatórios
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Dashboard de relatórios acessado por usuário {User}", User.Identity?.Name);

                var hoje = DateTime.Now;
                var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
                var fimMes = inicioMes.AddMonths(1).AddDays(-1);
                var inicioAno = new DateTime(hoje.Year, 1, 1);

                // ========== ESTATÍSTICAS GERAIS ==========
                ViewBag.TotalClientes = await _context.Clientes.CountAsync();
                ViewBag.TotalVeiculos = await _context.Veiculos.CountAsync();
                ViewBag.TotalPacotes = await _context.PacotesViagens.CountAsync();
                ViewBag.TotalAgencias = await _context.Agencias.CountAsync();
                ViewBag.TotalFuncionarios = await _context.Funcionarios.CountAsync();

                ViewBag.VeiculosDisponiveis = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .CountAsync(v => v.StatusCarro != null && v.StatusCarro.Status == "Disponível");

                // ========== LOCAÇÕES - ESTATÍSTICAS DO MÊS ==========
                ViewBag.LocacoesMes = await _context.Locacoes
                    .CountAsync(l => l.DataRetirada >= inicioMes && l.DataRetirada <= fimMes);

                ViewBag.ReceitaLocacoesMes = await _context.Locacoes
                    .Where(l => l.DataRetirada >= inicioMes && l.DataRetirada <= fimMes)
                    .SumAsync(l => (decimal?)l.ValorTotal) ?? 0;

                ViewBag.LocacoesAtivas = await _context.Locacoes
                    .CountAsync(l => l.DataDevolucaoReal == null);

                // ========== TURISMO - ESTATÍSTICAS DO MÊS ==========
                ViewBag.ReservasMes = await _context.ReservasViagens
                    .CountAsync(r => r.DataReserva >= inicioMes && r.DataReserva <= fimMes);

                ViewBag.ReceitaReservasMes = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .Where(r => r.DataReserva >= inicioMes && r.DataReserva <= fimMes &&
                               r.StatusReservaViagem != null && r.StatusReservaViagem.Status == "Confirmada")
                    .SumAsync(r => (decimal?)r.ValorTotal) ?? 0;

                ViewBag.ReservasAtivas = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .CountAsync(r => r.StatusReservaViagem != null &&
                                    (r.StatusReservaViagem.Status == "Confirmada" ||
                                     r.StatusReservaViagem.Status == "Pendente"));

                // ========== RECEITAS CONSOLIDADAS ==========
                ViewBag.ReceitaTotalMes = (decimal)ViewBag.ReceitaLocacoesMes + (decimal)ViewBag.ReceitaReservasMes;

                ViewBag.ReceitaLocacoesAno = await _context.Locacoes
                    .Where(l => l.DataRetirada >= inicioAno)
                    .SumAsync(l => (decimal?)l.ValorTotal) ?? 0;

                ViewBag.ReceitaReservasAno = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .Where(r => r.DataReserva >= inicioAno &&
                               r.StatusReservaViagem != null &&
                               r.StatusReservaViagem.Status == "Confirmada")
                    .SumAsync(r => (decimal?)r.ValorTotal) ?? 0;

                ViewBag.ReceitaTotalAno = (decimal)ViewBag.ReceitaLocacoesAno + (decimal)ViewBag.ReceitaReservasAno;

                // ========== TOP 5 CLIENTES POR VALOR GASTO ==========
                // CORREÇÃO: Buscar clientes com relacionamentos e processar em memória
                var todosClientes = await _context.Clientes
                    .Include(c => c.Locacoes)
                    .Include(c => c.ReservasViagens)
                        .ThenInclude(r => r.StatusReservaViagem)
                    .ToListAsync();

                ViewBag.TopClientes = todosClientes
                    .Select(c => new
                    {
                        c.Id,
                        c.Nome,
                        c.Email,
                        TotalGastoLocacoes = c.Locacoes.Sum(l => (decimal?)l.ValorTotal) ?? 0,
                        TotalGastoReservas = c.ReservasViagens
                            .Where(r => r.StatusReservaViagem != null && r.StatusReservaViagem.Status == "Confirmada")
                            .Sum(r => (decimal?)r.ValorTotal) ?? 0,
                        TotalGasto = (c.Locacoes.Sum(l => (decimal?)l.ValorTotal) ?? 0) +
                                    (c.ReservasViagens
                                        .Where(r => r.StatusReservaViagem != null && r.StatusReservaViagem.Status == "Confirmada")
                                        .Sum(r => (decimal?)r.ValorTotal) ?? 0),
                        TotalLocacoes = c.Locacoes.Count,
                        TotalReservas = c.ReservasViagens.Count,
                        TotalTransacoes = c.Locacoes.Count + c.ReservasViagens.Count
                    })
                    .Where(c => c.TotalGasto > 0)
                    .OrderByDescending(c => c.TotalGasto)
                    .Take(5)
                    .ToList();

                // ========== VEÍCULOS MAIS ALUGADOS ==========
                var todosVeiculos = await _context.Veiculos
                    .Include(v => v.Locacoes)
                    .ToListAsync();

                ViewBag.TopVeiculos = todosVeiculos
                    .Select(v => new
                    {
                        v.Id,
                        Veiculo = $"{v.Marca} {v.Modelo} - {v.Placa}",
                        v.Placa,
                        TotalLocacoes = v.Locacoes.Count,
                        ReceitaTotal = v.Locacoes.Sum(l => (decimal?)l.ValorTotal) ?? 0,
                        MediaDias = v.Locacoes.Any(l => l.DataDevolucaoReal != null) ?
                            v.Locacoes.Where(l => l.DataDevolucaoReal != null)
                                     .Average(l => (double?)(l.DataDevolucaoReal!.Value - l.DataRetirada).TotalDays) ?? 0 : 0
                    })
                    .Where(v => v.TotalLocacoes > 0)
                    .OrderByDescending(v => v.TotalLocacoes)
                    .ThenByDescending(v => v.ReceitaTotal)
                    .Take(5)
                    .ToList();

                // ========== PACOTES MAIS VENDIDOS ==========
                var todosPacotes = await _context.PacotesViagens
                    .Include(p => p.ReservasViagens)
                        .ThenInclude(r => r.StatusReservaViagem)
                    .ToListAsync();

                ViewBag.TopPacotes = todosPacotes
                    .Select(p => new
                    {
                        p.Id,
                        p.Nome,
                        p.Destino,
                        p.Preco,
                        TotalReservas = p.ReservasViagens.Count,
                        TotalPessoas = p.ReservasViagens.Sum(r => (int?)r.Quantidade) ?? 0,
                        ReceitaTotal = p.ReservasViagens
                            .Where(r => r.StatusReservaViagem != null && r.StatusReservaViagem.Status == "Confirmada")
                            .Sum(r => (decimal?)r.ValorTotal) ?? 0
                    })
                    .Where(p => p.TotalReservas > 0)
                    .OrderByDescending(p => p.TotalReservas)
                    .ThenByDescending(p => p.ReceitaTotal)
                    .Take(5)
                    .ToList();

                // ========== FUNCIONÁRIOS TOP VENDEDORES ==========
                var todosFuncionarios = await _context.Funcionarios
                    .Include(f => f.Locacoes)
                    .Include(f => f.Agencia)
                    .ToListAsync();

                ViewBag.TopFuncionarios = todosFuncionarios
                    .Select(f => new
                    {
                        f.Id,
                        f.Nome,
                        f.Cargo,
                        Agencia = f.Agencia?.Nome ?? "Sem Agência",
                        VendasMes = f.Locacoes.Count(l => l.DataRetirada >= inicioMes && l.DataRetirada <= fimMes),
                        ReceitaMes = f.Locacoes
                            .Where(l => l.DataRetirada >= inicioMes && l.DataRetirada <= fimMes)
                            .Sum(l => (decimal?)l.ValorTotal) ?? 0,
                        TotalVendas = f.Locacoes.Count,
                        ReceitaTotal = f.Locacoes.Sum(l => (decimal?)l.ValorTotal) ?? 0
                    })
                    .Where(f => f.TotalVendas > 0)
                    .OrderByDescending(f => f.ReceitaMes)
                    .ThenByDescending(f => f.VendasMes)
                    .Take(5)
                    .ToList();

                // ========== CNHs VENCENDO (ALERTA) ==========
                var dataLimite = hoje.AddDays(30);
                ViewBag.CNHsVencendo = await _context.Clientes
                    .CountAsync(c => c.ValidadeCNH.HasValue &&
                                   c.ValidadeCNH.Value.Date <= dataLimite.Date &&
                                   c.ValidadeCNH.Value.Date >= hoje.Date);

                ViewBag.CNHsVencidas = await _context.Clientes
                    .CountAsync(c => c.ValidadeCNH.HasValue && c.ValidadeCNH.Value.Date < hoje.Date);

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dashboard de relatórios para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar relatórios. Tente novamente.";
                return View();
            }
        }

        #endregion

        #region RELATÓRIOS DE LOCAÇÃO

        // GET: Relatórios/LocacoesPorPeriodo
        public async Task<IActionResult> LocacoesPorPeriodo(DateTime? dataInicio, DateTime? dataFim, int? agenciaId, int? funcionarioId)
        {
            try
            {
                // Definir período padrão (último mês)
                dataInicio ??= DateTime.Now.AddMonths(-1);
                dataFim ??= DateTime.Now;

                _logger.LogInformation("Relatório de locações por período acessado por {User} - Período: {DataInicio} a {DataFim}",
                    User.Identity?.Name, dataInicio?.ToString("dd/MM/yyyy"), dataFim?.ToString("dd/MM/yyyy"));

                var locacoes = _context.Locacoes
                    .Include(l => l.Cliente)
                    .Include(l => l.Veiculo)
                    .Include(l => l.Funcionario)
                    .Include(l => l.Agencia)
                    .Where(l => l.DataRetirada >= dataInicio && l.DataRetirada <= dataFim)
                    .AsQueryable();

                // Filtros opcionais
                if (agenciaId.HasValue)
                {
                    locacoes = locacoes.Where(l => l.AgenciaId == agenciaId);
                }

                if (funcionarioId.HasValue)
                {
                    locacoes = locacoes.Where(l => l.FuncionarioId == funcionarioId);
                }

                var listaLocacoes = await locacoes
                    .OrderByDescending(l => l.DataRetirada)
                    .ToListAsync();

                // Calcular estatísticas
                ViewBag.TotalLocacoes = listaLocacoes.Count;
                ViewBag.TotalReceita = listaLocacoes.Sum(l => l.ValorTotal);
                ViewBag.TicketMedio = listaLocacoes.Any() ?
                    listaLocacoes.Average(l => l.ValorTotal) : 0;
                ViewBag.LocacoesFinalizadas = listaLocacoes.Where(l => l.DataDevolucaoReal.HasValue).Count();
                ViewBag.LocacoesAtivas = listaLocacoes.Where(l => !l.DataDevolucaoReal.HasValue).Count();

                // Agrupamento por mês para gráfico
                ViewBag.LocacoesPorMes = listaLocacoes
                    .GroupBy(l => new { l.DataRetirada.Year, l.DataRetirada.Month })
                    .Select(g => new
                    {
                        Periodo = $"{g.Key.Month:D2}/{g.Key.Year}",
                        Quantidade = g.Count(),
                        Receita = g.Sum(l => l.ValorTotal)
                    })
                    .OrderBy(g => g.Periodo)
                    .ToList();

                // ViewBags para filtros
                ViewBag.DataInicio = dataInicio;
                ViewBag.DataFim = dataFim;
                ViewBag.AgenciaId = agenciaId;
                ViewBag.FuncionarioId = funcionarioId;

                ViewBag.Agencias = new SelectList(await _context.Agencias.ToListAsync(), "Id", "Nome", agenciaId);
                ViewBag.Funcionarios = new SelectList(await _context.Funcionarios.ToListAsync(), "Id", "Nome", funcionarioId);

                return View(listaLocacoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de locações por período para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao gerar relatório. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Relatórios/VeiculosMaisAlugados
        public async Task<IActionResult> VeiculosMaisAlugados(DateTime? dataInicio, DateTime? dataFim)
        {
            try
            {
                // Definir período padrão (último ano)
                dataInicio ??= DateTime.Now.AddYears(-1);
                dataFim ??= DateTime.Now;

                _logger.LogInformation("Relatório de veículos mais alugados acessado por {User} - Período: {DataInicio} a {DataFim}",
                    User.Identity?.Name, dataInicio?.ToString("dd/MM/yyyy"), dataFim?.ToString("dd/MM/yyyy"));

                // CORREÇÃO: Não usar Include com filtro + Select
                // Em vez disso, buscar todos os dados e fazer a projeção no lado do cliente
                var veiculosComLocacoes = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .Include(v => v.Agencia)
                    .Include(v => v.Locacoes)
                    .ToListAsync();

                // Filtrar e processar no lado do cliente
                var veiculosEstatisticas = veiculosComLocacoes
                    .Select(v => new
                    {
                        Veiculo = v,
                        TotalLocacoes = v.Locacoes
                            .Where(l => l.DataRetirada >= dataInicio && l.DataRetirada <= dataFim)
                            .Count(),
                        ReceitaTotal = v.Locacoes
                            .Where(l => l.DataRetirada >= dataInicio && l.DataRetirada <= dataFim)
                            .Sum(l => (decimal?)l.ValorTotal) ?? 0,
                        DiasAlugado = v.Locacoes
                            .Where(l => l.DataDevolucaoReal != null &&
                                       l.DataRetirada >= dataInicio &&
                                       l.DataRetirada <= dataFim)
                            .Sum(l => (l.DataDevolucaoReal!.Value - l.DataRetirada).TotalDays),
                        MediaDiasAlugado = v.Locacoes
                            .Where(l => l.DataDevolucaoReal != null &&
                                       l.DataRetirada >= dataInicio &&
                                       l.DataRetirada <= dataFim)
                            .Any() ?
                            v.Locacoes
                                .Where(l => l.DataDevolucaoReal != null &&
                                           l.DataRetirada >= dataInicio &&
                                           l.DataRetirada <= dataFim)
                                .Average(l => (l.DataDevolucaoReal!.Value - l.DataRetirada).TotalDays) : 0,
                        UltimaLocacao = v.Locacoes
                            .Where(l => l.DataRetirada >= dataInicio && l.DataRetirada <= dataFim)
                            .Any() ?
                            v.Locacoes
                                .Where(l => l.DataRetirada >= dataInicio && l.DataRetirada <= dataFim)
                                .Max(l => (DateTime?)l.DataRetirada) : null,
                        StatusAtual = v.StatusCarro?.Status ?? "Indefinido",
                        Agencia = v.Agencia?.Nome ?? "Sem Agência"
                    })
                    .OrderByDescending(v => v.TotalLocacoes)
                    .ThenByDescending(v => v.ReceitaTotal)
                    .ToList();

                ViewBag.DataInicio = dataInicio;
                ViewBag.DataFim = dataFim;

                return View(veiculosEstatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de veículos mais alugados para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao gerar relatório. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region RELATÓRIOS DE TURISMO

        // GET: Relatórios/ReservasPorPeriodo
        public async Task<IActionResult> ReservasPorPeriodo(DateTime? dataInicio, DateTime? dataFim, int? statusId)
        {
            try
            {
                // Definir período padrão (último mês)
                dataInicio ??= DateTime.Now.AddMonths(-1);
                dataFim ??= DateTime.Now;

                _logger.LogInformation("Relatório de reservas por período acessado por {User} - Período: {DataInicio} a {DataFim}",
                    User.Identity?.Name, dataInicio?.ToString("dd/MM/yyyy"), dataFim?.ToString("dd/MM/yyyy"));

                var reservas = _context.ReservasViagens
                    .Include(r => r.Cliente)
                    .Include(r => r.PacoteViagem)
                    .Include(r => r.StatusReservaViagem)
                    .Where(r => r.DataReserva >= dataInicio && r.DataReserva <= dataFim)
                    .AsQueryable();

                // Filtro por status
                if (statusId.HasValue)
                {
                    reservas = reservas.Where(r => r.StatusReservaViagemId == statusId);
                }

                var listaReservas = await reservas
                    .OrderByDescending(r => r.DataReserva)
                    .ToListAsync();

                // Calcular estatísticas
                ViewBag.TotalReservas = listaReservas.Count;
                ViewBag.ReceitaTotal = listaReservas.Where(r => r.StatusReservaViagem.Status == "Confirmada").Sum(r => r.ValorTotal);
                ViewBag.TotalPessoas = listaReservas.Sum(r => r.Quantidade);
                ViewBag.TicketMedio = listaReservas.Where(r => r.StatusReservaViagem.Status == "Confirmada").Any() ?
                    listaReservas.Where(r => r.StatusReservaViagem.Status == "Confirmada").Average(r => r.ValorTotal) : 0;

                // Estatísticas por status
                ViewBag.ReservasConfirmadas = listaReservas.Where(r => r.StatusReservaViagem.Status == "Confirmada").Count();
                ViewBag.ReservasPendentes = listaReservas.Where(r => r.StatusReservaViagem.Status == "Pendente").Count();
                ViewBag.ReservasCanceladas = listaReservas.Where(r => r.StatusReservaViagem.Status == "Cancelada").Count();

                // Agrupamento por mês para gráfico
                ViewBag.ReservasPorMes = listaReservas
                    .GroupBy(r => new { r.DataReserva.Year, r.DataReserva.Month })
                    .Select(g => new
                    {
                        Periodo = $"{g.Key.Month:D2}/{g.Key.Year}",
                        Quantidade = g.Count(),
                        Receita = g.Where(r => r.StatusReservaViagem.Status == "Confirmada").Sum(r => r.ValorTotal)
                    })
                    .OrderBy(g => g.Periodo)
                    .ToList();

                // ViewBags para filtros
                ViewBag.DataInicio = dataInicio;
                ViewBag.DataFim = dataFim;
                ViewBag.StatusId = statusId;
                ViewBag.StatusReservas = new SelectList(await _context.StatusReservaViagens.ToListAsync(), "Id", "Status", statusId);

                return View(listaReservas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de reservas por período para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao gerar relatório. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Relatórios/PacotesMaisVendidos
        public async Task<IActionResult> PacotesMaisVendidos(DateTime? dataInicio, DateTime? dataFim)
        {
            try
            {
                // Definir período padrão (último ano)
                dataInicio ??= DateTime.Now.AddYears(-1);
                dataFim ??= DateTime.Now;

                _logger.LogInformation("Relatório de pacotes mais vendidos acessado por {User} - Período: {DataInicio} a {DataFim}",
                    User.Identity?.Name, dataInicio?.ToString("dd/MM/yyyy"), dataFim?.ToString("dd/MM/yyyy"));

                var pacotesEstatisticas = await _context.PacotesViagens
                    .Include(p => p.ReservasViagens.Where(r => r.DataReserva >= dataInicio && r.DataReserva <= dataFim))
                        .ThenInclude(r => r.StatusReservaViagem)
                    .Include(p => p.ReservasViagens)
                        .ThenInclude(r => r.Cliente)
                    .Select(p => new
                    {
                        Pacote = p,
                        TotalReservas = p.ReservasViagens.Count(),
                        ReservasConfirmadas = p.ReservasViagens.Where(r => r.StatusReservaViagem.Status == "Confirmada").Count(),
                        TotalPessoas = p.ReservasViagens.Sum(r => r.Quantidade),
                        ReceitaTotal = p.ReservasViagens
                            .Where(r => r.StatusReservaViagem.Status == "Confirmada")
                            .Sum(r => r.ValorTotal),
                        TicketMedio = p.ReservasViagens.Where(r => r.StatusReservaViagem.Status == "Confirmada").Any() ?
                            p.ReservasViagens.Where(r => r.StatusReservaViagem.Status == "Confirmada").Average(r => r.ValorTotal) : 0,
                        UltimaReserva = p.ReservasViagens.Any() ?
                            p.ReservasViagens.Max(r => r.DataReserva) : (DateTime?)null,
                        TaxaConfirmacao = p.ReservasViagens.Any() ?
                            (double)p.ReservasViagens.Where(r => r.StatusReservaViagem.Status == "Confirmada").Count() / p.ReservasViagens.Count() * 100 : 0
                    })
                    .OrderByDescending(p => p.TotalReservas)
                    .ThenByDescending(p => p.ReceitaTotal)
                    .ToListAsync();

                ViewBag.DataInicio = dataInicio;
                ViewBag.DataFim = dataFim;

                return View(pacotesEstatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de pacotes mais vendidos para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao gerar relatório. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region RELATÓRIOS GERENCIAIS

        // GET: Relatórios/ClientesDetalhado
        public async Task<IActionResult> ClientesDetalhado(string? ordenacao)
        {
            try
            {
                _logger.LogInformation("Relatório detalhado de clientes acessado por {User}", User.Identity?.Name);

                var clientesEstatisticas = await _context.Clientes
                    .Include(c => c.Locacoes)
                    .Include(c => c.ReservasViagens)
                        .ThenInclude(r => r.StatusReservaViagem)
                    .Select(c => new
                    {
                        Cliente = c,
                        TotalLocacoes = c.Locacoes.Count(),
                        TotalReservas = c.ReservasViagens.Count(),
                        GastoLocacoes = c.Locacoes.Sum(l => l.ValorTotal),
                        GastoReservas = c.ReservasViagens.Where(r => r.StatusReservaViagem.Status == "Confirmada").Sum(r => r.ValorTotal),
                        GastoTotal = c.Locacoes.Sum(l => l.ValorTotal) + c.ReservasViagens.Where(r => r.StatusReservaViagem.Status == "Confirmada").Sum(r => r.ValorTotal),
                        TotalTransacoes = c.Locacoes.Count() + c.ReservasViagens.Count(),
                        UltimaTransacao = c.Locacoes.Any() || c.ReservasViagens.Any() ?
                            new[] { c.Locacoes.Any() ? c.Locacoes.Max(l => l.DataRetirada) : DateTime.MinValue,
                                   c.ReservasViagens.Any() ? c.ReservasViagens.Max(r => r.DataReserva) : DateTime.MinValue }.Max() : DateTime.MinValue,
                        CNHStatus = !c.ValidadeCNH.HasValue ? "Não informada" :
                                   c.ValidadeCNH.Value.Date < DateTime.Now.Date ? "Vencida" :
                                   c.ValidadeCNH.Value.Date <= DateTime.Now.AddDays(30).Date ? "Vencendo" : "Válida"
                    })
                    .ToListAsync();

                // Aplicar ordenação
                clientesEstatisticas = ordenacao switch
                {
                    "gasto_desc" => clientesEstatisticas.OrderByDescending(c => c.GastoTotal).ToList(),
                    "transacoes_desc" => clientesEstatisticas.OrderByDescending(c => c.TotalTransacoes).ToList(),
                    "ultima_transacao_desc" => clientesEstatisticas.OrderByDescending(c => c.UltimaTransacao).ToList(),
                    "nome_asc" => clientesEstatisticas.OrderBy(c => c.Cliente.Nome).ToList(),
                    _ => clientesEstatisticas.OrderByDescending(c => c.GastoTotal).ToList()
                };

                ViewBag.Ordenacao = ordenacao;
                ViewBag.TotalClientes = clientesEstatisticas.Count;
                ViewBag.ClientesAtivos = clientesEstatisticas.Where(c => c.TotalTransacoes > 0).Count();
                ViewBag.ReceitaTotal = clientesEstatisticas.Sum(c => c.GastoTotal);

                return View(clientesEstatisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório detalhado de clientes para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao gerar relatório. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Relatórios/PerformanceFuncionarios
        public async Task<IActionResult> PerformanceFuncionarios(DateTime? dataInicio, DateTime? dataFim, int? agenciaId)
        {
            try
            {
                // Definir período padrão (último trimestre)
                dataInicio ??= DateTime.Now.AddMonths(-3);
                dataFim ??= DateTime.Now;

                _logger.LogInformation("Relatório de performance de funcionários acessado por {User} - Período: {DataInicio} a {DataFim}",
                    User.Identity?.Name, dataInicio?.ToString("dd/MM/yyyy"), dataFim?.ToString("dd/MM/yyyy"));

                var funcionarios = _context.Funcionarios
                    .Include(f => f.Agencia)
                    .Include(f => f.Locacoes.Where(l => l.DataRetirada >= dataInicio && l.DataRetirada <= dataFim))
                        .ThenInclude(l => l.Cliente)
                    .AsQueryable();

                if (agenciaId.HasValue)
                {
                    funcionarios = funcionarios.Where(f => f.AgenciaId == agenciaId);
                }

                var performanceList = await funcionarios
                    .Select(f => new
                    {
                        Funcionario = f,
                        VendasPeriodo = f.Locacoes.Count(),
                        ReceitaPeriodo = f.Locacoes.Sum(l => l.ValorTotal),
                        TicketMedio = f.Locacoes.Any() ? f.Locacoes.Average(l => l.ValorTotal) : 0,
                        ClientesUnicos = f.Locacoes.Select(l => l.ClienteId).Distinct().Count(),
                        TotalVendas = f.Locacoes.Count(),
                        ReceitaTotal = f.Locacoes.Sum(l => l.ValorTotal)
                    })
                    .OrderByDescending(p => p.ReceitaPeriodo)
                    .ThenByDescending(p => p.VendasPeriodo)
                    .ToListAsync();

                // Estatísticas gerais
                ViewBag.TotalFuncionarios = performanceList.Count;
                ViewBag.TotalVendas = performanceList.Sum(p => p.VendasPeriodo);
                ViewBag.ReceitaTotal = performanceList.Sum(p => p.ReceitaPeriodo);
                ViewBag.TicketMedioGeral = performanceList.Where(p => p.VendasPeriodo > 0).Any() ?
                    performanceList.Where(p => p.VendasPeriodo > 0).Average(p => p.TicketMedio) : 0;

                ViewBag.DataInicio = dataInicio;
                ViewBag.DataFim = dataFim;
                ViewBag.AgenciaId = agenciaId;
                ViewBag.Agencias = new SelectList(await _context.Agencias.ToListAsync(), "Id", "Nome", agenciaId);

                return View(performanceList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de performance de funcionários para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao gerar relatório. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Relatórios/ReceitaMensal
        public async Task<IActionResult> ReceitaMensal(int? ano)
        {
            try
            {
                ano ??= DateTime.Now.Year;

                _logger.LogInformation("Relatório de receita mensal acessado por {User} - Ano: {Ano}",
                    User.Identity?.Name, ano);

                var inicioAno = new DateTime(ano.Value, 1, 1);
                var fimAno = inicioAno.AddYears(1).AddDays(-1);

                // Receitas de locações por mês
                var receitasLocacoes = await _context.Locacoes
                    .Where(l => l.DataRetirada >= inicioAno && l.DataRetirada <= fimAno)
                    .GroupBy(l => l.DataRetirada.Month)
                    .Select(g => new
                    {
                        Mes = g.Key,
                        Quantidade = g.Count(),
                        Receita = g.Sum(l => l.ValorTotal)
                    })
                    .ToListAsync();

                // Receitas de reservas por mês
                var receitasReservas = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .Where(r => r.DataReserva >= inicioAno && r.DataReserva <= fimAno &&
                               r.StatusReservaViagem.Status == "Confirmada")
                    .GroupBy(r => r.DataReserva.Month)
                    .Select(g => new
                    {
                        Mes = g.Key,
                        Quantidade = g.Count(),
                        Receita = g.Sum(r => r.ValorTotal)
                    })
                    .ToListAsync();

                // Consolidar dados por mês
                var dadosMensais = new List<object>();
                var mesesNome = new[] { "", "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho",
                    "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };

                for (int mes = 1; mes <= 12; mes++)
                {
                    var receitaLocacao = receitasLocacoes.FirstOrDefault(r => r.Mes == mes);
                    var receitaReserva = receitasReservas.FirstOrDefault(r => r.Mes == mes);

                    dadosMensais.Add(new
                    {
                        Mes = mes,
                        NomeMes = mesesNome[mes],
                        LocacoesQuantidade = receitaLocacao?.Quantidade ?? 0,
                        LocacoesReceita = receitaLocacao?.Receita ?? 0,
                        ReservasQuantidade = receitaReserva?.Quantidade ?? 0,
                        ReservasReceita = receitaReserva?.Receita ?? 0,
                        ReceitaTotal = (receitaLocacao?.Receita ?? 0) + (receitaReserva?.Receita ?? 0)
                    });
                }

                ViewBag.Ano = ano;
                ViewBag.DadosMensais = dadosMensais;
                ViewBag.ReceitaTotalAno = dadosMensais.Sum(d => ((dynamic)d).ReceitaTotal);

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de receita mensal para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao gerar relatório. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion

        #region APIS PARA GRÁFICOS

        [HttpGet]
        public async Task<IActionResult> GetDadosGraficoReceita(int ano)
        {
            try
            {
                var inicioAno = new DateTime(ano, 1, 1);
                var fimAno = inicioAno.AddYears(1).AddDays(-1);

                var dadosMensais = new List<object>();

                for (int mes = 1; mes <= 12; mes++)
                {
                    var inicioMes = new DateTime(ano, mes, 1);
                    var fimMes = inicioMes.AddMonths(1).AddDays(-1);

                    var receitaLocacoes = await _context.Locacoes
                        .Where(l => l.DataRetirada >= inicioMes && l.DataRetirada <= fimMes)
                        .SumAsync(l => (decimal?)l.ValorTotal) ?? 0;

                    var receitaReservas = await _context.ReservasViagens
                        .Include(r => r.StatusReservaViagem)
                        .Where(r => r.DataReserva >= inicioMes && r.DataReserva <= fimMes &&
                                   r.StatusReservaViagem.Status == "Confirmada")
                        .SumAsync(r => (decimal?)r.ValorTotal) ?? 0;

                    dadosMensais.Add(new
                    {
                        mes = mes,
                        locacoes = receitaLocacoes,
                        reservas = receitaReservas,
                        total = receitaLocacoes + receitaReservas
                    });
                }

                return Json(dadosMensais);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados do gráfico de receita por {User}", User.Identity?.Name);
                return Json(new { error = "Erro ao carregar dados" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDadosGraficoVendas(int meses = 6)
        {
            try
            {
                var dataLimite = DateTime.Now.AddMonths(-meses);

                var vendas = new List<object>();

                for (int i = meses - 1; i >= 0; i--)
                {
                    var inicioMes = DateTime.Now.AddMonths(-i).Date.AddDays(1 - DateTime.Now.AddMonths(-i).Day);
                    var fimMes = inicioMes.AddMonths(1).AddDays(-1);

                    var locacoes = await _context.Locacoes
                        .Where(l => l.DataRetirada >= inicioMes && l.DataRetirada <= fimMes)
                        .CountAsync();

                    var reservas = await _context.ReservasViagens
                        .Where(r => r.DataReserva >= inicioMes && r.DataReserva <= fimMes)
                        .CountAsync();

                    vendas.Add(new
                    {
                        periodo = inicioMes.ToString("MM/yyyy"),
                        locacoes = locacoes,
                        reservas = reservas,
                        total = locacoes + reservas
                    });
                }

                return Json(vendas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados do gráfico de vendas por {User}", User.Identity?.Name);
                return Json(new { error = "Erro ao carregar dados" });
            }
        }

        #endregion

        #region EXPORTAÇÃO CSV

        [HttpGet]
        public async Task<IActionResult> ExportarCSV(string relatorio, DateTime? dataInicio, DateTime? dataFim)
        {
            try
            {
                _logger.LogInformation("Exportação de relatório CSV '{Relatorio}' solicitada por {User}", relatorio, User.Identity?.Name);

                dataInicio ??= DateTime.Now.AddMonths(-1);
                dataFim ??= DateTime.Now;

                var csv = new StringBuilder();

                switch (relatorio?.ToLower())
                {
                    case "locacoes":
                        csv = await GerarCSVLocacoes(dataInicio.Value, dataFim.Value);
                        break;
                    case "reservas":
                        csv = await GerarCSVReservas(dataInicio.Value, dataFim.Value);
                        break;
                    case "clientes":
                        csv = await GerarCSVClientes();
                        break;
                    default:
                        return BadRequest("Tipo de relatório não suportado.");
                }

                var nomeArquivo = $"{relatorio}_{dataInicio:yyyy-MM-dd}_a_{dataFim:yyyy-MM-dd}.csv";
                var bytes = Encoding.UTF8.GetBytes(csv.ToString());

                return File(bytes, "text/csv", nomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao exportar relatório CSV por {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao exportar relatório. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<StringBuilder> GerarCSVLocacoes(DateTime dataInicio, DateTime dataFim)
        {
            var csv = new StringBuilder();
            csv.AppendLine("ID,Cliente,CPF,Veículo,Placa,Data Retirada,Data Devolução Prevista,Data Devolução Real,Valor Total,Funcionário,Agência");

            var locacoes = await _context.Locacoes
                .Include(l => l.Cliente)
                .Include(l => l.Veiculo)
                .Include(l => l.Funcionario)
                .Include(l => l.Agencia)
                .Where(l => l.DataRetirada >= dataInicio && l.DataRetirada <= dataFim)
                .OrderByDescending(l => l.DataRetirada)
                .ToListAsync();

            foreach (var loc in locacoes)
            {
                csv.AppendLine($"{loc.Id},{loc.Cliente.Nome},{loc.Cliente.Cpf},{loc.Veiculo.Marca} {loc.Veiculo.Modelo}," +
                              $"{loc.Veiculo.Placa},{loc.DataRetirada:dd/MM/yyyy},{loc.DataDevolucao:dd/MM/yyyy}," +
                              $"{loc.DataDevolucaoReal?.ToString("dd/MM/yyyy") ?? "Em aberto"},{loc.ValorTotal:C}," +
                              $"{loc.Funcionario.Nome},{loc.Agencia.Nome}");
            }

            return csv;
        }

        private async Task<StringBuilder> GerarCSVReservas(DateTime dataInicio, DateTime dataFim)
        {
            var csv = new StringBuilder();
            csv.AppendLine("ID,Cliente,CPF,Pacote,Destino,Data Reserva,Data Viagem,Quantidade,Valor Total,Status");

            var reservas = await _context.ReservasViagens
                .Include(r => r.Cliente)
                .Include(r => r.PacoteViagem)
                .Include(r => r.StatusReservaViagem)
                .Where(r => r.DataReserva >= dataInicio && r.DataReserva <= dataFim)
                .OrderByDescending(r => r.DataReserva)
                .ToListAsync();

            foreach (var res in reservas)
            {
                csv.AppendLine($"{res.Id},{res.Cliente.Nome},{res.Cliente.Cpf},{res.PacoteViagem.Nome}," +
                              $"{res.PacoteViagem.Destino},{res.DataReserva:dd/MM/yyyy},{res.DataViagem:dd/MM/yyyy}," +
                              $"{res.Quantidade},{res.ValorTotal:C},{res.StatusReservaViagem.Status}");
            }

            return csv;
        }

        private async Task<StringBuilder> GerarCSVClientes()
        {
            var csv = new StringBuilder();
            csv.AppendLine("ID,Nome,CPF,Email,Telefone,Data Nascimento,Total Locações,Total Reservas,Gasto Total");

            var clientes = await _context.Clientes
                .Include(c => c.Locacoes)
                .Include(c => c.ReservasViagens)
                    .ThenInclude(r => r.StatusReservaViagem)
                .ToListAsync();

            foreach (var cli in clientes)
            {
                var gastoTotal = cli.Locacoes.Sum(l => l.ValorTotal) +
                                cli.ReservasViagens.Where(r => r.StatusReservaViagem.Status == "Confirmada").Sum(r => r.ValorTotal);

                csv.AppendLine($"{cli.Id},{cli.Nome},{cli.Cpf},{cli.Email},{cli.Telefone}," +
                              $"{cli.DataNascimento:dd/MM/yyyy},{cli.Locacoes.Count},{cli.ReservasViagens.Count},{gastoTotal:C}");
            }

            return csv;
        }

        #endregion
    }
}