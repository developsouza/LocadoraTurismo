using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Services;
using System.Diagnostics;
using RentalTourismSystem.Extensions;

namespace RentalTourismSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly INotificationService _notificationService;


        public HomeController(RentalTourismContext context, ILogger<HomeController> logger,
            INotificationService notificationService)
        {
            _context = context;
            _logger = logger;
            _notificationService = notificationService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Dashboard acessado por usuário {User}", User.Identity?.Name);

                var hoje = DateTime.Now;
                var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
                var fimMes = inicioMes.AddMonths(1).AddDays(-1);

                // ========== CLIENTES ==========
                ViewBag.TotalClientes = await _context.Clientes.CountAsync();

                var clientesMesAnterior = await _context.Clientes
                    .CountAsync(c => c.DataCadastro < inicioMes);
                var clientesMesAtual = await _context.Clientes.CountAsync();
                ViewBag.CrescimentoClientes = clientesMesAnterior > 0
                    ? (int)Math.Round(((clientesMesAtual - clientesMesAnterior) / (double)clientesMesAnterior) * 100)
                    : 0;

                // ========== VEÍCULOS ==========
                ViewBag.TotalVeiculos = await _context.Veiculos.CountAsync();

                ViewBag.VeiculosDisponiveis = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .CountAsync(v => v.StatusCarro != null && v.StatusCarro.Status == "Disponível");

                ViewBag.VeiculosAlugados = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .CountAsync(v => v.StatusCarro != null && v.StatusCarro.Status == "Alugado");

                ViewBag.VeiculosManutencao = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .CountAsync(v => v.StatusCarro != null && v.StatusCarro.Status == "Manutenção");

                var totalVeiculos = (int)ViewBag.TotalVeiculos;
                ViewBag.TaxaDisponibilidade = totalVeiculos > 0
                    ? (int)Math.Round(((int)ViewBag.VeiculosDisponiveis / (double)totalVeiculos) * 100)
                    : 0;

                // ========== LOCAÇÕES ==========
                ViewBag.TotalLocacoes = await _context.Locacoes.CountAsync();

                ViewBag.LocacoesAtivas = await _context.Locacoes
                    .CountAsync(l => l.DataDevolucaoReal == null);

                ViewBag.LocacoesAtrasadas = await _context.Locacoes
                    .CountAsync(l => l.DataDevolucaoReal == null && l.DataDevolucao < hoje);

                ViewBag.LocacoesMes = await _context.Locacoes
                    .CountAsync(l => l.DataRetirada >= inicioMes && l.DataRetirada <= fimMes);

                var locacoesMesAnterior = await _context.Locacoes
                    .CountAsync(l => l.DataRetirada >= inicioMes.AddMonths(-1) &&
                                    l.DataRetirada < inicioMes);
                ViewBag.CrescimentoLocacoes = locacoesMesAnterior > 0
                    ? (int)Math.Round((((int)ViewBag.LocacoesMes - locacoesMesAnterior) / (double)locacoesMesAnterior) * 100)
                    : 0;

                // ========== RECEITA DE LOCAÇÕES - MÊS ==========
                var receitaLocacoesMes = await _context.Locacoes
                    .Where(l => l.DataRetirada >= inicioMes && l.DataRetirada <= fimMes)
                    .SumAsync(l => (decimal?)l.ValorTotal) ?? 0;

                // ========== TURISMO - RESERVAS ==========
                ViewBag.TotalReservas = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .CountAsync(r => r.StatusReservaViagem != null &&
                                   r.StatusReservaViagem.Status != "Cancelada");

                ViewBag.ReservasMes = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .CountAsync(r => r.DataReserva >= inicioMes && r.DataReserva <= fimMes &&
                                   r.StatusReservaViagem != null &&
                                   r.StatusReservaViagem.Status == "Confirmada");

                // ========== RECEITA DE TURISMO - MÊS (COM SERVIÇOS ADICIONAIS) ==========
                // CORREÇÃO PRINCIPAL: Buscar reservas confirmadas com serviços adicionais
                var reservasConfirmadasMes = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .Include(r => r.ServicosAdicionais) // INCLUIR SERVIÇOS
                    .Where(r => r.DataReserva >= inicioMes && r.DataReserva <= fimMes &&
                               r.StatusReservaViagem != null &&
                               r.StatusReservaViagem.Status == "Confirmada")
                    .ToListAsync();

                // Calcular receita REAL incluindo serviços adicionais
                var receitaReservasMes = reservasConfirmadasMes.Sum(r => r.ObterValorTotalComServicos());

                ViewBag.ReceitaTotalMes = receitaLocacoesMes + receitaReservasMes;

                // ========== OCUPAÇÃO ==========
                var totalVeiculosCalc = totalVeiculos > 0 ? totalVeiculos : 1;
                ViewBag.TaxaOcupacao = (int)Math.Round(((int)ViewBag.VeiculosAlugados / (double)totalVeiculosCalc) * 100);

                // ========== PACOTES ==========
                ViewBag.PacotesAtivos = await _context.PacotesViagens.CountAsync();

                if (ViewBag.ReservasMes > 0)
                {
                    var totalPessoasMes = await _context.ReservasViagens
                        .Where(r => r.DataReserva >= inicioMes && r.DataReserva <= fimMes &&
                                   r.StatusReservaViagem != null &&
                                   r.StatusReservaViagem.Status == "Confirmada")
                        .SumAsync(r => (int?)r.Quantidade) ?? 0;

                    ViewBag.MediaPessoasPorReserva = (int)Math.Round(totalPessoasMes / (double)ViewBag.ReservasMes);
                }
                else
                {
                    ViewBag.MediaPessoasPorReserva = 0;
                }

                // ========== TICKET MÉDIO ==========
                var totalTransacoes = (int)ViewBag.LocacoesMes + (int)ViewBag.ReservasMes;
                ViewBag.TicketMedio = totalTransacoes > 0
                    ? ViewBag.ReceitaTotalMes / totalTransacoes
                    : 0;

                // ========== ALERTAS - CNH ==========
                var dataLimite = hoje.AddDays(30);
                ViewBag.CNHsVencendo = await _context.Clientes
                    .CountAsync(c => c.ValidadeCNH != null &&
                                   c.ValidadeCNH > hoje &&
                                   c.ValidadeCNH <= dataLimite);

                ViewBag.CNHsVencidas = await _context.Clientes
                    .CountAsync(c => c.ValidadeCNH != null && c.ValidadeCNH < hoje);

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dashboard para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar o dashboard. Tente novamente.";
                return View();
            }
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // ========== MÉTODO AUXILIAR PARA VERIFICAR PERMISSÕES ==========
        private bool UserCanAccessFinancialData()
        {
            return User.IsInRole("Admin") || User.IsInRole("Manager");
        }

        private bool UserIsAdminOrManager()
        {
            return User.IsInRole("Admin") || User.IsInRole("Manager");
        }

        // ========== API PARA DADOS DO DASHBOARD (AJAX) ==========
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = new
                {
                    TotalClientes = await _context.Clientes.CountAsync(),
                    TotalVeiculos = await _context.Veiculos.CountAsync(),
                    VeiculosDisponiveis = await _context.Veiculos
                        .Include(v => v.StatusCarro)
                        .CountAsync(v => v.StatusCarro.Status == "Disponível"),
                    LocacoesAtivas = await _context.Locacoes
                        .CountAsync(l => l.DataDevolucaoReal == null),
                    ReservasAtivas = await _context.ReservasViagens
                        .Include(r => r.StatusReservaViagem)
                        .CountAsync(r => r.StatusReservaViagem.Status == "Confirmada" ||
                                        r.StatusReservaViagem.Status == "Pendente")
                };

                return Json(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas do dashboard via API");
                return Json(new { error = "Erro ao carregar estatísticas" });
            }
        }

        // ========== API PARA DADOS DO GRÁFICO DE PERFORMANCE ==========
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPerformanceChartData(int dias = 7)
        {
            try
            {
                _logger.LogInformation("Solicitação de dados do gráfico para {Dias} dias", dias);
                
                var dataFim = DateTime.Now.Date;
                var dataInicio = dataFim.AddDays(-dias + 1);

                _logger.LogInformation("Período: {DataInicio} até {DataFim}", dataInicio, dataFim);

                var dados = new List<object>();

                for (var data = dataInicio; data <= dataFim; data = data.AddDays(1))
                {
                    var dataProxima = data.AddDays(1);

                    // Locações do dia
                    var locacoesDia = await _context.Locacoes
                        .Where(l => l.DataRetirada >= data && l.DataRetirada < dataProxima)
                        .CountAsync();

                    var receitaLocacoesDia = await _context.Locacoes
                        .Where(l => l.DataRetirada >= data && l.DataRetirada < dataProxima)
                        .SumAsync(l => (decimal?)l.ValorTotal) ?? 0;

                    // Reservas do dia (simplificado - sem dependência da extensão)
                    var reservasDia = await _context.ReservasViagens
                        .Include(r => r.StatusReservaViagem)
                        .Where(r => r.DataReserva >= data && r.DataReserva < dataProxima &&
                                   r.StatusReservaViagem != null &&
                                   r.StatusReservaViagem.Status == "Confirmada")
                        .CountAsync();

                    var receitaReservasDia = await _context.ReservasViagens
                        .Where(r => r.DataReserva >= data && r.DataReserva < dataProxima &&
                                   r.StatusReservaViagem != null &&
                                   r.StatusReservaViagem.Status == "Confirmada")
                        .SumAsync(r => (decimal?)r.ValorTotal) ?? 0;

                    // Soma dos serviços adicionais
                    var servicosAdicionaisDia = await _context.ReservasViagens
                        .Where(r => r.DataReserva >= data && r.DataReserva < dataProxima &&
                                   r.StatusReservaViagem != null &&
                                   r.StatusReservaViagem.Status == "Confirmada")
                        .SelectMany(r => r.ServicosAdicionais)
                        .SumAsync(s => (decimal?)s.Preco) ?? 0;

                    var receitaTotalReservas = receitaReservasDia + servicosAdicionaisDia;

                    dados.Add(new
                    {
                        data = data.ToString("dd/MM"),
                        dataCompleta = data.ToString("dd/MM/yyyy"),
                        locacoes = locacoesDia,
                        reservas = reservasDia,
                        receitaLocacoes = receitaLocacoesDia,
                        receitaReservas = receitaTotalReservas,
                        receitaTotal = receitaLocacoesDia + receitaTotalReservas,
                        totalOperacoes = locacoesDia + reservasDia
                    });
                }

                _logger.LogInformation("Retornando {Count} dias de dados do gráfico", dados.Count);
                
                // Log de debug para ver os dados
                _logger.LogDebug("Dados do gráfico: {@Dados}", dados);
                
                return Json(new { success = true, dados = dados });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados do gráfico de performance");
                return Json(new { success = false, error = ex.Message, details = ex.ToString() });
            }
        }

        // ========== API PARA DADOS MENSAIS (30 e 90 dias) ==========
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPerformanceChartDataMonthly(int dias = 30)
        {
            try
            {
                _logger.LogInformation("Solicitação de dados mensais para {Dias} dias", dias);
                
                var dataFim = DateTime.Now.Date;
                var dataInicio = dataFim.AddDays(-dias + 1);

                _logger.LogInformation("Período mensal: {DataInicio} até {DataFim}", dataInicio, dataFim);

                var dados = new List<object>();
                
                if (dias <= 30)
                {
                    // Agrupar por dia
                    for (var data = dataInicio; data <= dataFim; data = data.AddDays(1))
                    {
                        var dataProxima = data.AddDays(1);

                        var locacoesDia = await _context.Locacoes
                            .Where(l => l.DataRetirada >= data && l.DataRetirada < dataProxima)
                            .CountAsync();

                        var receitaLocacoesDia = await _context.Locacoes
                            .Where(l => l.DataRetirada >= data && l.DataRetirada < dataProxima)
                            .SumAsync(l => (decimal?)l.ValorTotal) ?? 0;

                        var reservasDia = await _context.ReservasViagens
                            .Include(r => r.StatusReservaViagem)
                            .Where(r => r.DataReserva >= data && r.DataReserva < dataProxima &&
                                       r.StatusReservaViagem != null &&
                                       r.StatusReservaViagem.Status == "Confirmada")
                            .CountAsync();

                        var receitaReservasDia = await _context.ReservasViagens
                            .Where(r => r.DataReserva >= data && r.DataReserva < dataProxima &&
                                       r.StatusReservaViagem != null &&
                                       r.StatusReservaViagem.Status == "Confirmada")
                            .SumAsync(r => (decimal?)r.ValorTotal) ?? 0;

                        var servicosAdicionaisDia = await _context.ReservasViagens
                            .Where(r => r.DataReserva >= data && r.DataReserva < dataProxima &&
                                       r.StatusReservaViagem != null &&
                                       r.StatusReservaViagem.Status == "Confirmada")
                            .SelectMany(r => r.ServicosAdicionais)
                            .SumAsync(s => (decimal?)s.Preco) ?? 0;

                        var receitaTotalReservas = receitaReservasDia + servicosAdicionaisDia;

                        dados.Add(new
                        {
                            data = data.ToString("dd/MM"),
                            dataCompleta = data.ToString("dd/MM/yyyy"),
                            locacoes = locacoesDia,
                            reservas = reservasDia,
                            receitaLocacoes = receitaLocacoesDia,
                            receitaReservas = receitaTotalReservas,
                            receitaTotal = receitaLocacoesDia + receitaTotalReservas,
                            totalOperacoes = locacoesDia + reservasDia
                        });
                    }
                }
                else
                {
                    // Agrupar por semana para 90 dias
                    var semanas = (int)Math.Ceiling(dias / 7.0);
                    
                    for (int i = 0; i < semanas; i++)
                    {
                        var inicioSemana = dataInicio.AddDays(i * 7);
                        var fimSemana = inicioSemana.AddDays(7);
                        
                        if (fimSemana > dataFim)
                            fimSemana = dataFim.AddDays(1);

                        var locacoesSemana = await _context.Locacoes
                            .Where(l => l.DataRetirada >= inicioSemana && l.DataRetirada < fimSemana)
                            .CountAsync();

                        var receitaLocacoesSemana = await _context.Locacoes
                            .Where(l => l.DataRetirada >= inicioSemana && l.DataRetirada < fimSemana)
                            .SumAsync(l => (decimal?)l.ValorTotal) ?? 0;

                        var reservasSemana = await _context.ReservasViagens
                            .Include(r => r.StatusReservaViagem)
                            .Where(r => r.DataReserva >= inicioSemana && r.DataReserva < fimSemana &&
                                       r.StatusReservaViagem != null &&
                                       r.StatusReservaViagem.Status == "Confirmada")
                            .CountAsync();

                        var receitaReservasSemana = await _context.ReservasViagens
                            .Where(r => r.DataReserva >= inicioSemana && r.DataReserva < fimSemana &&
                                       r.StatusReservaViagem != null &&
                                       r.StatusReservaViagem.Status == "Confirmada")
                            .SumAsync(r => (decimal?)r.ValorTotal) ?? 0;

                        var servicosAdicionaisSemana = await _context.ReservasViagens
                            .Where(r => r.DataReserva >= inicioSemana && r.DataReserva < fimSemana &&
                                       r.StatusReservaViagem != null &&
                                       r.StatusReservaViagem.Status == "Confirmada")
                            .SelectMany(r => r.ServicosAdicionais)
                            .SumAsync(s => (decimal?)s.Preco) ?? 0;

                        var receitaTotalReservas = receitaReservasSemana + servicosAdicionaisSemana;

                        dados.Add(new
                        {
                            data = $"Sem {i + 1}",
                            dataCompleta = $"{inicioSemana:dd/MM} - {fimSemana.AddDays(-1):dd/MM}",
                            locacoes = locacoesSemana,
                            reservas = reservasSemana,
                            receitaLocacoes = receitaLocacoesSemana,
                            receitaReservas = receitaTotalReservas,
                            receitaTotal = receitaLocacoesSemana + receitaTotalReservas,
                            totalOperacoes = locacoesSemana + reservasSemana
                        });
                    }
                }

                _logger.LogInformation("Retornando {Count} pontos de dados mensais", dados.Count);
                
                // Log de debug
                _logger.LogDebug("Dados mensais do gráfico: {@Dados}", dados);
                
                return Json(new { success = true, dados = dados });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados mensais do gráfico");
                return Json(new { success = false, error = ex.Message, details = ex.ToString() });
            }
        }
    }

    // Classe auxiliar para dados do erro
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}