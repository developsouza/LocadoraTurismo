using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;

namespace RentalTourismSystem.Controllers
{
    [Authorize(Roles = "Admin,Manager")] // Apenas gestores podem ver relatórios
    public class RelatoriosController : Controller
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<RelatoriosController> _logger;

        public RelatoriosController(RentalTourismContext context, ILogger<RelatoriosController> logger)
        {
            _context = context;
            _logger = logger;
        }

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
                    .Where(v => v.StatusCarro.Status == "Disponível")
                    .CountAsync();

                // ========== LOCAÇÕES - ESTATÍSTICAS DO MÊS ==========
                ViewBag.LocacoesMes = await _context.Locacoes
                    .Where(l => l.DataRetirada >= inicioMes && l.DataRetirada <= fimMes)
                    .CountAsync();

                ViewBag.ReceitaLocacoesMes = await _context.Locacoes
                    .Where(l => l.DataRetirada >= inicioMes && l.DataRetirada <= fimMes)
                    .SumAsync(l => l.ValorTotal);

                ViewBag.LocacoesAtivas = await _context.Locacoes
                    .Where(l => l.DataDevolucaoReal == null)
                    .CountAsync();

                // ========== TURISMO - ESTATÍSTICAS DO MÊS ==========
                ViewBag.ReservasMes = await _context.ReservasViagens
                    .Where(r => r.DataReserva >= inicioMes && r.DataReserva <= fimMes)
                    .CountAsync();

                ViewBag.ReceitaReservasMes = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .Where(r => r.DataReserva >= inicioMes && r.DataReserva <= fimMes &&
                               r.StatusReservaViagem.Status == "Confirmada")
                    .SumAsync(r => r.ValorTotal);

                ViewBag.ReservasAtivas = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .Where(r => r.StatusReservaViagem.Status == "Confirmada" || r.StatusReservaViagem.Status == "Pendente")
                    .CountAsync();

                // ========== RECEITAS CONSOLIDADAS ==========
                ViewBag.ReceitaTotalMes = ViewBag.ReceitaLocacoesMes + ViewBag.ReceitaReservasMes;

                ViewBag.ReceitaLocacoesAno = await _context.Locacoes
                    .Where(l => l.DataRetirada >= inicioAno)
                    .SumAsync(l => l.ValorTotal);

                ViewBag.ReceitaReservasAno = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .Where(r => r.DataReserva >= inicioAno && r.StatusReservaViagem.Status == "Confirmada")
                    .SumAsync(r => r.ValorTotal);

                ViewBag.ReceitaTotalAno = ViewBag.ReceitaLocacoesAno + ViewBag.ReceitaReservasAno;

                // ========== TOP 5 CLIENTES POR VALOR GASTO ==========
                ViewBag.TopClientes = await _context.Clientes
                    .Select(c => new
                    {
                        c.Id,
                        c.Nome,
                        c.Email,
                        TotalGastoLocacoes = c.Locacoes.Sum(l => l.ValorTotal),
                        TotalGastoReservas = c.ReservasViagens.Sum(r => r.ValorTotal),
                        TotalGasto = c.Locacoes.Sum(l => l.ValorTotal) + c.ReservasViagens.Sum(r => r.ValorTotal),
                        TotalLocacoes = c.Locacoes.Count(),
                        TotalReservas = c.ReservasViagens.Count(),
                        TotalTransacoes = c.Locacoes.Count() + c.ReservasViagens.Count()
                    })
                    .Where(c => c.TotalGasto > 0)
                    .OrderByDescending(c => c.TotalGasto)
                    .Take(5)
                    .ToListAsync();

                // ========== VEÍCULOS MAIS ALUGADOS ==========
                ViewBag.TopVeiculos = await _context.Veiculos
                    .Include(v => v.Locacoes)
                    .Select(v => new
                    {
                        v.Id,
                        Veiculo = $"{v.Marca} {v.Modelo} - {v.Placa}",
                        v.Placa,
                        TotalLocacoes = v.Locacoes.Count(),
                        ReceitaTotal = v.Locacoes.Sum(l => l.ValorTotal),
                        MediaDias = v.Locacoes.Any(l => l.DataDevolucaoReal != null) ?
                            v.Locacoes.Where(l => l.DataDevolucaoReal != null)
                                     .Average(l => (l.DataDevolucaoReal.Value - l.DataRetirada).TotalDays) : 0
                    })
                    .Where(v => v.TotalLocacoes > 0)
                    .OrderByDescending(v => v.TotalLocacoes)
                    .ThenByDescending(v => v.ReceitaTotal)
                    .Take(5)
                    .ToListAsync();

                // ========== PACOTES MAIS VENDIDOS ==========
                ViewBag.TopPacotes = await _context.PacotesViagens
                    .Include(p => p.ReservasViagens)
                        .ThenInclude(r => r.StatusReservaViagem)
                    .Select(p => new
                    {
                        p.Id,
                        p.Nome,
                        p.Destino,
                        p.Preco,
                        TotalReservas = p.ReservasViagens.Count(),
                        TotalPessoas = p.ReservasViagens.Sum(r => r.Quantidade),
                        ReceitaTotal = p.ReservasViagens
                            .Where(r => r.StatusReservaViagem.Status == "Confirmada")
                            .Sum(r => r.ValorTotal)
                    })
                    .Where(p => p.TotalReservas > 0)
                    .OrderByDescending(p => p.TotalReservas)
                    .ThenByDescending(p => p.ReceitaTotal)
                    .Take(5)
                    .ToListAsync();

                // ========== FUNCIONÁRIOS TOP VENDEDORES ==========
                ViewBag.TopFuncionarios = await _context.Funcionarios
                    .Include(f => f.Locacoes)
                    .Include(f => f.Agencia)
                    .Select(f => new
                    {
                        f.Id,
                        f.Nome,
                        f.Cargo,
                        Agencia = f.Agencia.Nome,
                        VendasMes = f.Locacoes.Where(l => l.DataRetirada >= inicioMes && l.DataRetirada <= fimMes).Count(),
                        ReceitaMes = f.Locacoes.Where(l => l.DataRetirada >= inicioMes && l.DataRetirada <= fimMes).Sum(l => l.ValorTotal),
                        TotalVendas = f.Locacoes.Count(),
                        ReceitaTotal = f.Locacoes.Sum(l => l.ValorTotal)
                    })
                    .Where(f => f.TotalVendas > 0)
                    .OrderByDescending(f => f.ReceitaMes)
                    .ThenByDescending(f => f.VendasMes)
                    .Take(5)
                    .ToListAsync();

                // ========== CNHs VENCENDO (ALERTA) ==========
                var dataLimite = hoje.AddDays(30);
                ViewBag.CNHsVencendo = await _context.Clientes
                    .Where(c => c.ValidadeCNH.HasValue &&
                               c.ValidadeCNH.Value.Date <= dataLimite.Date &&
                               c.ValidadeCNH.Value.Date >= hoje.Date)
                    .CountAsync();

                ViewBag.CNHsVencidas = await _context.Clientes
                    .Where(c => c.ValidadeCNH.HasValue && c.ValidadeCNH.Value.Date < hoje.Date)
                    .CountAsync();

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dashboard de relatórios para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar relatórios. Tente novamente.";
                return View();
            }
        }

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
                ViewBag.ReceitaTotal = listaLocacoes.Sum(l => l.ValorTotal);
                ViewBag.TicketMedio = listaLocacoes.Any() ? listaLocacoes.Average(l => l.ValorTotal) : 0;
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

                var veiculosEstatisticas = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .Include(v => v.Agencia)
                    .Include(v => v.Locacoes.Where(l => l.DataRetirada >= dataInicio && l.DataRetirada <= dataFim))
                        .ThenInclude(l => l.Cliente)
                    .Select(v => new
                    {
                        Veiculo = v,
                        TotalLocacoes = v.Locacoes.Count(),
                        ReceitaTotal = v.Locacoes.Sum(l => l.ValorTotal),
                        DiasAlugado = v.Locacoes.Where(l => l.DataDevolucaoReal != null)
                                               .Sum(l => (l.DataDevolucaoReal.Value - l.DataRetirada).TotalDays),
                        MediaDiasPorLocacao = v.Locacoes.Where(l => l.DataDevolucaoReal != null).Any() ?
                            v.Locacoes.Where(l => l.DataDevolucaoReal != null)
                                     .Average(l => (l.DataDevolucaoReal.Value - l.DataRetirada).TotalDays) : 0,
                        UltimaLocacao = v.Locacoes.Any() ?
                            v.Locacoes.Max(l => l.DataRetirada) : (DateTime?)null,
                        StatusAtual = v.StatusCarro.Status,
                        Agencia = v.Agencia.Nome
                    })
                    .OrderByDescending(v => v.TotalLocacoes)
                    .ThenByDescending(v => v.ReceitaTotal)
                    .ToListAsync();

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
                        TotalVendas = f.Locacoes.Count(), // Total histórico
                        ReceitaTotal = f.Locacoes.Sum(l => l.ValorTotal) // Total histórico
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

        // ========== APIS PARA GRÁFICOS E DASHBOARDS ==========

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
                        .SumAsync(l => l.ValorTotal);

                    var receitaReservas = await _context.ReservasViagens
                        .Include(r => r.StatusReservaViagem)
                        .Where(r => r.DataReserva >= inicioMes && r.DataReserva <= fimMes &&
                                   r.StatusReservaViagem.Status == "Confirmada")
                        .SumAsync(r => r.ValorTotal);

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

        // ========== EXPORTAÇÃO DE RELATÓRIOS ==========

        [HttpGet]
        public async Task<IActionResult> ExportarCSV(string relatorio, DateTime? dataInicio, DateTime? dataFim)
        {
            try
            {
                _logger.LogInformation("Exportação de relatório CSV '{Relatorio}' solicitada por {User}", relatorio, User.Identity?.Name);

                dataInicio ??= DateTime.Now.AddMonths(-1);
                dataFim ??= DateTime.Now;

                var csv = new System.Text.StringBuilder();

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
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());

                return File(bytes, "text/csv", nomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao exportar relatório CSV por {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao exportar relatório. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ========== MÉTODOS AUXILIARES PARA EXPORTAÇÃO ==========

        private async Task<System.Text.StringBuilder> GerarCSVLocacoes(DateTime dataInicio, DateTime dataFim)
        {
            var locacoes = await _context.Locacoes
                .Include(l => l.Cliente)
                .Include(l => l.Veiculo)
                .Include(l => l.Funcionario)
                .Include(l => l.Agencia)
                .Where(l => l.DataRetirada >= dataInicio && l.DataRetirada <= dataFim)
                .OrderByDescending(l => l.DataRetirada)
                .ToListAsync();

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Data Retirada;Cliente;CPF;Veículo;Placa;Funcionário;Agência;Data Devolução Prevista;Data Devolução Real;Valor Total;Status");

            foreach (var locacao in locacoes)
            {
                csv.AppendLine($"{locacao.DataRetirada:dd/MM/yyyy};" +
                              $"{locacao.Cliente.Nome};" +
                              $"{locacao.Cliente.Cpf};" +
                              $"{locacao.Veiculo.Marca} {locacao.Veiculo.Modelo};" +
                              $"{locacao.Veiculo.Placa};" +
                              $"{locacao.Funcionario.Nome};" +
                              $"{locacao.Agencia.Nome};" +
                              $"{locacao.DataDevolucao:dd/MM/yyyy};" +
                              $"{locacao.DataDevolucaoReal?.ToString("dd/MM/yyyy") ?? ""};" +
                              $"R$ {locacao.ValorTotal:N2};" +
                              $"{(locacao.DataDevolucaoReal.HasValue ? "Finalizada" : "Ativa")}");
            }

            return csv;
        }

        private async Task<System.Text.StringBuilder> GerarCSVReservas(DateTime dataInicio, DateTime dataFim)
        {
            var reservas = await _context.ReservasViagens
                .Include(r => r.Cliente)
                .Include(r => r.PacoteViagem)
                .Include(r => r.StatusReservaViagem)
                .Where(r => r.DataReserva >= dataInicio && r.DataReserva <= dataFim)
                .OrderByDescending(r => r.DataReserva)
                .ToListAsync();

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Data Reserva;Cliente;CPF;Pacote;Destino;Data Viagem;Quantidade;Valor Total;Status;Observações");

            foreach (var reserva in reservas)
            {
                csv.AppendLine($"{reserva.DataReserva:dd/MM/yyyy};" +
                              $"{reserva.Cliente.Nome};" +
                              $"{reserva.Cliente.Cpf};" +
                              $"{reserva.PacoteViagem.Nome};" +
                              $"{reserva.PacoteViagem.Destino};" +
                              $"{reserva.DataViagem:dd/MM/yyyy};" +
                              $"{reserva.Quantidade};" +
                              $"R$ {reserva.ValorTotal:N2};" +
                              $"{reserva.StatusReservaViagem.Status};" +
                              $"\"{reserva.Observacoes?.Replace("\"", "\"\"") ?? ""}\"");
            }

            return csv;
        }

        private async Task<System.Text.StringBuilder> GerarCSVClientes()
        {
            var clientes = await _context.Clientes
                .Include(c => c.Locacoes)
                .Include(c => c.ReservasViagens)
                .OrderBy(c => c.Nome)
                .ToListAsync();

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Nome;CPF;Email;Telefone;Total Locações;Total Reservas;Valor Total Gasto;Última Transação;Status CNH");

            foreach (var cliente in clientes)
            {
                var totalGasto = cliente.Locacoes.Sum(l => l.ValorTotal) +
                                cliente.ReservasViagens.Sum(r => r.ValorTotal);

                var ultimaTransacao = new[] {
                    cliente.Locacoes.Any() ? cliente.Locacoes.Max(l => l.DataRetirada) : DateTime.MinValue,
                    cliente.ReservasViagens.Any() ? cliente.ReservasViagens.Max(r => r.DataReserva) : DateTime.MinValue
                }.Max();

                var statusCNH = !cliente.ValidadeCNH.HasValue ? "Não informada" :
                               cliente.ValidadeCNH.Value.Date < DateTime.Now.Date ? "Vencida" :
                               cliente.ValidadeCNH.Value.Date <= DateTime.Now.AddDays(30).Date ? "Vencendo" : "Válida";

                csv.AppendLine($"{cliente.Nome};" +
                              $"{cliente.Cpf};" +
                              $"{cliente.Email ?? ""};" +
                              $"{cliente.Telefone ?? ""};" +
                              $"{cliente.Locacoes.Count};" +
                              $"{cliente.ReservasViagens.Count};" +
                              $"R$ {totalGasto:N2};" +
                              $"{(ultimaTransacao != DateTime.MinValue ? ultimaTransacao.ToString("dd/MM/yyyy") : "")};" +
                              $"{statusCNH}");
            }

            return csv;
        }
    }
}