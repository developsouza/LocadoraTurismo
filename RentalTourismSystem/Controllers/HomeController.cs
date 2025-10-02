using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Services;
using System.Diagnostics;

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
                var mesPassado = inicioMes.AddMonths(-1);

                // ========== GERAR NOTIFICAÇÕES AUTOMÁTICAS ==========
                // Gera notificações baseadas em CNHs, locações atrasadas, etc.
                await _notificationService.GerarNotificacoesAutomaticasAsync();

                // ========== DADOS BÁSICOS ==========
                ViewBag.TotalClientes = await _context.Clientes.CountAsync();
                ViewBag.TotalVeiculos = await _context.Veiculos.CountAsync();

                // Veículos disponíveis
                ViewBag.VeiculosDisponiveis = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .Where(v => v.StatusCarro.Status == "Disponível")
                    .CountAsync();

                // Veículos alugados
                ViewBag.VeiculosAlugados = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .Where(v => v.StatusCarro.Status == "Alugado")
                    .CountAsync();

                // Veículos em manutenção
                ViewBag.VeiculosManutencao = await _context.Veiculos
                    .Include(v => v.StatusCarro)
                    .Where(v => v.StatusCarro.Status == "Manutenção")
                    .CountAsync();

                // Locações ativas (sem data de devolução real)
                ViewBag.LocacoesAtivas = await _context.Locacoes
                    .Where(l => l.DataDevolucaoReal == null)
                    .CountAsync();

                // Total de locações (histórico)
                ViewBag.TotalLocacoes = await _context.Locacoes.CountAsync();

                // ========== DADOS DE TURISMO ==========
                ViewBag.TotalReservas = await _context.ReservasViagens.CountAsync();

                // Reservas ativas/confirmadas
                ViewBag.ReservasAtivas = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .Where(r => r.StatusReservaViagem.Status == "Confirmada" ||
                               r.StatusReservaViagem.Status == "Pendente")
                    .CountAsync();

                // Pacotes ativos
                ViewBag.PacotesAtivos = await _context.PacotesViagens.CountAsync();

                // ========== DADOS FINANCEIROS DO MÊS ==========
                // Receita de locações do mês atual
                ViewBag.ReceitaLocacoesMes = await _context.Locacoes
                    .Where(l => l.DataRetirada >= inicioMes && l.DataRetirada < inicioMes.AddMonths(1))
                    .SumAsync(l => l.ValorTotal);

                // Receita de reservas confirmadas do mês
                ViewBag.ReceitaReservasMes = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .Where(r => r.DataReserva >= inicioMes && r.DataReserva < inicioMes.AddMonths(1) &&
                               r.StatusReservaViagem.Status == "Confirmada")
                    .SumAsync(r => r.ValorTotal);

                // Receita total do mês
                ViewBag.ReceitaTotalMes = ViewBag.ReceitaLocacoesMes + ViewBag.ReceitaReservasMes;

                // ========== DADOS DE LOCAÇÕES/MÊS ==========
                var locacoesMesAtual = await _context.Locacoes
                    .Where(l => l.DataRetirada >= inicioMes && l.DataRetirada < inicioMes.AddMonths(1))
                    .CountAsync();

                var locacoesMesPassado = await _context.Locacoes
                    .Where(l => l.DataRetirada >= mesPassado && l.DataRetirada < inicioMes)
                    .CountAsync();

                ViewBag.LocacoesMes = locacoesMesAtual;

                if (locacoesMesPassado > 0)
                {
                    // Usa decimal para garantir a precisão no cálculo percentual
                    ViewBag.CrescimentoLocacoes = Math.Round(((decimal)locacoesMesAtual - locacoesMesPassado) / locacoesMesPassado * 100, 2);
                }
                else
                {
                    // Se não houve locações no mês passado, qualquer locação atual é um novo crescimento.
                    // Define como 0 para que a view mostre a mensagem padrão "Contratos no mês".
                    // Um valor positivo (ex: 100) poderia ser usado se a regra de negócio preferir.
                    ViewBag.CrescimentoLocacoes = 0;
                }

                // ========== COMPARAÇÃO COM MÊS ANTERIOR ==========
                var receitaLocacoesMesPassado = await _context.Locacoes
                    .Where(l => l.DataRetirada >= mesPassado && l.DataRetirada < inicioMes)
                    .SumAsync(l => l.ValorTotal);

                var receitaReservasMesPassado = await _context.ReservasViagens
                    .Include(r => r.StatusReservaViagem)
                    .Where(r => r.DataReserva >= mesPassado && r.DataReserva < inicioMes &&
                               r.StatusReservaViagem.Status == "Confirmada")
                    .SumAsync(r => r.ValorTotal);

                var receitaTotalMesPassado = receitaLocacoesMesPassado + receitaReservasMesPassado;

                // Calcular percentual de crescimento
                ViewBag.CrescimentoReceita = receitaTotalMesPassado > 0
                    ? ((ViewBag.ReceitaTotalMes - receitaTotalMesPassado) / receitaTotalMesPassado) * 100
                    : 0;

                // ========== ALERTAS E AVISOS ==========
                // CNHs vencendo em 30 dias
                var dataLimite = hoje.AddDays(30);
                ViewBag.CNHsVencendo = await _context.Clientes
                    .Where(c => c.ValidadeCNH.HasValue &&
                               c.ValidadeCNH.Value.Date <= dataLimite.Date &&
                               c.ValidadeCNH.Value.Date >= hoje.Date)
                    .CountAsync();

                // CNHs já vencidas
                ViewBag.CNHsVencidas = await _context.Clientes
                    .Where(c => c.ValidadeCNH.HasValue && c.ValidadeCNH.Value.Date < hoje.Date)
                    .CountAsync();

                // Locações atrasadas (necessário para a View)
                ViewBag.LocacoesAtrasadas = await _context.Locacoes
                    .Where(l => l.DataDevolucaoReal == null && l.DataDevolucao < hoje)
                    .CountAsync();

                // Taxa de disponibilidade (útil para a View)
                ViewBag.TaxaDisponibilidade = ViewBag.TotalVeiculos > 0
                    ? Math.Round(((decimal)ViewBag.VeiculosDisponiveis / ViewBag.TotalVeiculos) * 100, 1)
                    : 0;

                // ========== SISTEMA DE NOTIFICAÇÕES ==========
                // Contar notificações não lidas para exibir no badge
                ViewBag.TotalNotificacoes = await _notificationService.ContarNotificacoesNaoLidasAsync();

                // ========== TOP 5 CLIENTES (apenas para admins e gerentes) ==========
                if (User.IsInRole("Admin") || User.IsInRole("Manager"))
                {
                    ViewBag.TopClientes = await _context.Clientes
                        .Select(c => new
                        {
                            Nome = c.Nome,
                            TotalGasto = c.Locacoes.Sum(l => l.ValorTotal) + c.ReservasViagens.Sum(r => r.ValorTotal),
                            TotalTransacoes = c.Locacoes.Count() + c.ReservasViagens.Count()
                        })
                        .Where(c => c.TotalGasto > 0)
                        .OrderByDescending(c => c.TotalGasto)
                        .Take(5)
                        .ToListAsync();

                    // ========== VEÍCULOS MAIS ALUGADOS ==========
                    ViewBag.VeiculosPopulares = await _context.Veiculos
                        .Select(v => new
                        {
                            Veiculo = v.Marca + " " + v.Modelo + " - " + v.Placa,
                            TotalLocacoes = v.Locacoes.Count(),
                            ReceitaTotal = v.Locacoes.Sum(l => l.ValorTotal)
                        })
                        .Where(v => v.TotalLocacoes > 0)
                        .OrderByDescending(v => v.TotalLocacoes)
                        .Take(5)
                        .ToListAsync();

                    // ========== PACOTES MAIS VENDIDOS ==========
                    ViewBag.PacotesPopulares = await _context.PacotesViagens
                        .Select(p => new
                        {
                            Nome = p.Nome,
                            TotalReservas = p.ReservasViagens.Count(),
                            ReceitaTotal = p.ReservasViagens
                                .Where(r => r.StatusReservaViagem.Status == "Confirmada")
                                .Sum(r => r.ValorTotal)
                        })
                        .Where(p => p.TotalReservas > 0)
                        .OrderByDescending(p => p.TotalReservas)
                        .Take(5)
                        .ToListAsync();
                }

                // ========== DADOS DO USUÁRIO ==========
                ViewBag.UserName = User.Identity?.Name;
                ViewBag.IsAdmin = User.IsInRole("Admin");
                ViewBag.IsManager = User.IsInRole("Manager");
                ViewBag.IsEmployee = User.IsInRole("Employee");

                _logger.LogInformation("Dashboard carregado com sucesso para usuário {User}", User.Identity?.Name);
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dashboard para usuário {User}", User.Identity?.Name);
                TempData["Erro"] = "Erro ao carregar dados do dashboard. Tente novamente.";

                // Retornar view com dados básicos em caso de erro
                ViewBag.TotalClientes = 0;
                ViewBag.TotalVeiculos = 0;
                ViewBag.VeiculosDisponiveis = 0;
                ViewBag.VeiculosAlugados = 0;
                ViewBag.VeiculosManutencao = 0;
                ViewBag.LocacoesAtivas = 0;
                ViewBag.LocacoesAtrasadas = 0;
                ViewBag.CNHsVencendo = 0;
                ViewBag.CNHsVencidas = 0;
                ViewBag.TotalNotificacoes = 0;
                ViewBag.ReceitaTotalMes = 0;
                ViewBag.TaxaDisponibilidade = 0;
                ViewBag.UserName = User.Identity?.Name;
                ViewBag.IsAdmin = User.IsInRole("Admin");
                ViewBag.IsManager = User.IsInRole("Manager");
                ViewBag.IsEmployee = User.IsInRole("Employee");

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
    }

    // Classe auxiliar para dados do erro
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}