using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.Models;

namespace RentalTourismSystem.Services
{
    public interface INotificationService
    {
        Task<List<NotificacaoResumoDto>> ObterNotificacoesAtivasAsync(int limite = 10);
        Task<int> ContarNotificacoesNaoLidasAsync();
        Task<Notificacao> CriarNotificacaoAsync(string titulo, string mensagem, string tipo, string? categoria = null, int prioridade = 1);
        Task<bool> MarcarComoLidaAsync(int notificacaoId);
        Task<bool> MarcarTodasComoLidasAsync();
        Task LimparNotificacoesAntigasAsync(int diasRetencao = 30);
        Task GerarNotificacoesAutomaticasAsync();
    }

    public class NotificationService : INotificationService
    {
        private readonly RentalTourismContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(RentalTourismContext context, ILogger<NotificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<NotificacaoResumoDto>> ObterNotificacoesAtivasAsync(int limite = 10)
        {
            var notificacoes = await _context.Notificacoes
                .Where(n => !n.Lida)
                .OrderByDescending(n => n.Prioridade)
                .ThenByDescending(n => n.DataCriacao)
                .Take(limite)
                .Select(n => new NotificacaoResumoDto
                {
                    Id = n.Id,
                    Titulo = n.Titulo,
                    Mensagem = n.Mensagem,
                    Tipo = n.Tipo,
                    Categoria = n.Categoria,
                    LinkAcao = n.LinkAcao,
                    TextoLinkAcao = n.TextoLinkAcao,
                    Lida = n.Lida,
                    DataCriacao = n.DataCriacao,
                    Prioridade = n.Prioridade
                })
                .ToListAsync();

            // Calcular tempo decorrido
            foreach (var notif in notificacoes)
            {
                notif.TempoDecorrido = CalcularTempoDecorrido(notif.DataCriacao);
            }

            return notificacoes;
        }

        public async Task<int> ContarNotificacoesNaoLidasAsync()
        {
            return await _context.Notificacoes.CountAsync(n => !n.Lida);
        }

        public async Task<Notificacao> CriarNotificacaoAsync(string titulo, string mensagem, string tipo, string? categoria = null, int prioridade = 1)
        {
            var notificacao = new Notificacao
            {
                Titulo = titulo,
                Mensagem = mensagem,
                Tipo = tipo,
                Categoria = categoria,
                Prioridade = prioridade,
                DataCriacao = DateTime.Now
            };

            _context.Notificacoes.Add(notificacao);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Notificação criada: {Titulo}", titulo);
            return notificacao;
        }

        public async Task<bool> MarcarComoLidaAsync(int notificacaoId)
        {
            var notificacao = await _context.Notificacoes.FindAsync(notificacaoId);
            if (notificacao == null) return false;

            notificacao.Lida = true;
            notificacao.DataLeitura = DateTime.Now;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarcarTodasComoLidasAsync()
        {
            var notificacoesNaoLidas = await _context.Notificacoes
                .Where(n => !n.Lida)
                .ToListAsync();

            if (!notificacoesNaoLidas.Any()) return false;

            foreach (var notif in notificacoesNaoLidas)
            {
                notif.Lida = true;
                notif.DataLeitura = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Todas as {Count} notificações foram marcadas como lidas", notificacoesNaoLidas.Count);

            return true;
        }

        public async Task LimparNotificacoesAntigasAsync(int diasRetencao = 30)
        {
            var dataLimite = DateTime.Now.AddDays(-diasRetencao);
            var notificacoesAntigas = await _context.Notificacoes
                .Where(n => n.Lida && n.DataCriacao < dataLimite)
                .ToListAsync();

            if (notificacoesAntigas.Any())
            {
                _context.Notificacoes.RemoveRange(notificacoesAntigas);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Limpeza: {Count} notificações antigas removidas", notificacoesAntigas.Count);
            }
        }

        public async Task GerarNotificacoesAutomaticasAsync()
        {
            var hoje = DateTime.Now.Date;
            var dataLimite30Dias = hoje.AddDays(30);

            // 1. CNHs Vencidas
            var cnhsVencidas = await _context.Clientes
                .Where(c => c.ValidadeCNH.HasValue && c.ValidadeCNH.Value.Date < hoje)
                .ToListAsync();

            foreach (var cliente in cnhsVencidas)
            {
                // Verificar se já existe notificação recente para este cliente
                var notifExistente = await _context.Notificacoes
                    .AnyAsync(n => n.ClienteId == cliente.Id &&
                                   n.Categoria == "CNH" &&
                                   n.DataCriacao >= hoje.AddDays(-7));

                if (!notifExistente)
                {
                    await _context.Notificacoes.AddAsync(new Notificacao
                    {
                        Titulo = "CNH Vencida",
                        Mensagem = $"A CNH do cliente {cliente.Nome} está vencida desde {cliente.ValidadeCNH:dd/MM/yyyy}. Bloqueie novas locações.",
                        Tipo = "danger",
                        Categoria = "CNH",
                        ClienteId = cliente.Id,
                        LinkAcao = $"/Clientes/Details/{cliente.Id}",
                        TextoLinkAcao = "Ver Cliente",
                        Prioridade = 3
                    });
                }
            }

            // 2. CNHs Vencendo em 30 dias
            var cnhsVencendo = await _context.Clientes
                .Where(c => c.ValidadeCNH.HasValue &&
                           c.ValidadeCNH.Value.Date >= hoje &&
                           c.ValidadeCNH.Value.Date <= dataLimite30Dias)
                .ToListAsync();

            foreach (var cliente in cnhsVencendo)
            {
                var diasRestantes = (cliente.ValidadeCNH!.Value.Date - hoje).Days;

                var notifExistente = await _context.Notificacoes
                    .AnyAsync(n => n.ClienteId == cliente.Id &&
                                   n.Categoria == "CNH" &&
                                   n.DataCriacao >= hoje.AddDays(-7));

                if (!notifExistente)
                {
                    await _context.Notificacoes.AddAsync(new Notificacao
                    {
                        Titulo = "CNH Próxima do Vencimento",
                        Mensagem = $"A CNH do cliente {cliente.Nome} vence em {diasRestantes} dias ({cliente.ValidadeCNH:dd/MM/yyyy}). Notifique o cliente.",
                        Tipo = "warning",
                        Categoria = "CNH",
                        ClienteId = cliente.Id,
                        LinkAcao = $"/Clientes/Details/{cliente.Id}",
                        TextoLinkAcao = "Ver Cliente",
                        Prioridade = 2
                    });
                }
            }

            // 3. Locações Atrasadas
            var locacoesAtrasadas = await _context.Locacoes
                .Include(l => l.Cliente)
                .Include(l => l.Veiculo)
                .Where(l => l.DataDevolucaoReal == null && l.DataDevolucao < hoje)
                .ToListAsync();

            foreach (var locacao in locacoesAtrasadas)
            {
                var diasAtraso = (hoje - locacao.DataDevolucao).Days;

                var notifExistente = await _context.Notificacoes
                    .AnyAsync(n => n.LocacaoId == locacao.Id &&
                                   n.Categoria == "Locacao" &&
                                   n.DataCriacao >= hoje.AddDays(-3));

                if (!notifExistente)
                {
                    await _context.Notificacoes.AddAsync(new Notificacao
                    {
                        Titulo = "Locação Atrasada",
                        Mensagem = $"Locação #{locacao.Id} está atrasada há {diasAtraso} dias. Cliente: {locacao.Cliente?.Nome}. Veículo: {locacao.Veiculo?.Marca} {locacao.Veiculo?.Modelo}",
                        Tipo = "danger",
                        Categoria = "Locacao",
                        ClienteId = locacao.ClienteId,
                        VeiculoId = locacao.VeiculoId,
                        LocacaoId = locacao.Id,
                        LinkAcao = $"/Locacoes/Details/{locacao.Id}",
                        TextoLinkAcao = "Ver Locação",
                        Prioridade = 3
                    });
                }
            }

            // 4. Veículos em Manutenção
            var veiculosManutencao = await _context.Veiculos
                .Include(v => v.StatusCarro)
                .Where(v => v.StatusCarro != null && v.StatusCarro.Status == "Manutenção")
                .ToListAsync();

            if (veiculosManutencao.Any())
            {
                var notifExistente = await _context.Notificacoes
                    .AnyAsync(n => n.Categoria == "Veiculo" &&
                                   n.Titulo.Contains("Manutenção") &&
                                   n.DataCriacao >= hoje.AddDays(-7));

                if (!notifExistente)
                {
                    await _context.Notificacoes.AddAsync(new Notificacao
                    {
                        Titulo = "Veículos em Manutenção",
                        Mensagem = $"{veiculosManutencao.Count} veículo(s) em manutenção. Verifique o status dos reparos.",
                        Tipo = "info",
                        Categoria = "Veiculo",
                        LinkAcao = "/Veiculos/Index?statusId=3",
                        TextoLinkAcao = "Ver Veículos",
                        Prioridade = 1
                    });
                }
            }

            // 5. Devoluções Programadas para Hoje
            var devolucoesHoje = await _context.Locacoes
                .Include(l => l.Cliente)
                .Include(l => l.Veiculo)
                .Where(l => l.DataDevolucaoReal == null && l.DataDevolucao.Date == hoje)
                .ToListAsync();

            if (devolucoesHoje.Any())
            {
                var notifExistente = await _context.Notificacoes
                    .AnyAsync(n => n.Categoria == "Locacao" &&
                                   n.Titulo.Contains("Devoluções Hoje") &&
                                   n.DataCriacao >= hoje);

                if (!notifExistente)
                {
                    await _context.Notificacoes.AddAsync(new Notificacao
                    {
                        Titulo = "Devoluções Programadas para Hoje",
                        Mensagem = $"{devolucoesHoje.Count} veículo(s) com devolução programada para hoje. Prepare a inspeção.",
                        Tipo = "info",
                        Categoria = "Locacao",
                        LinkAcao = "/Locacoes/Index?status=ativa",
                        TextoLinkAcao = "Ver Locações",
                        Prioridade = 2
                    });
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Notificações automáticas geradas com sucesso");
        }

        private string CalcularTempoDecorrido(DateTime data)
        {
            var diferenca = DateTime.Now - data;

            if (diferenca.TotalMinutes < 1) return "Agora";
            if (diferenca.TotalMinutes < 60) return $"{(int)diferenca.TotalMinutes} min atrás";
            if (diferenca.TotalHours < 24) return $"{(int)diferenca.TotalHours}h atrás";
            if (diferenca.TotalDays < 7) return $"{(int)diferenca.TotalDays}d atrás";
            if (diferenca.TotalDays < 30) return $"{(int)(diferenca.TotalDays / 7)} semanas atrás";

            return data.ToString("dd/MM/yyyy");
        }
    }
}