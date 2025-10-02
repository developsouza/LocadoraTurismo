using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentalTourismSystem.Services;

namespace RentalTourismSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacoesController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificacoesController> _logger;

        public NotificacoesController(INotificationService notificationService, ILogger<NotificacoesController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Obter notificações ativas (não lidas)
        /// </summary>
        [HttpGet("ativas")]
        public async Task<IActionResult> ObterNotificacoesAtivas([FromQuery] int limite = 10)
        {
            try
            {
                var notificacoes = await _notificationService.ObterNotificacoesAtivasAsync(limite);
                return Ok(notificacoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter notificações ativas");
                return StatusCode(500, new { erro = "Erro ao carregar notificações" });
            }
        }

        /// <summary>
        /// Contar notificações não lidas
        /// </summary>
        [HttpGet("nao-lidas/count")]
        public async Task<IActionResult> ContarNotificacoesNaoLidas()
        {
            try
            {
                var count = await _notificationService.ContarNotificacoesNaoLidasAsync();
                return Ok(new { total = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao contar notificações não lidas");
                return StatusCode(500, new { erro = "Erro ao contar notificações" });
            }
        }

        /// <summary>
        /// Marcar notificação como lida
        /// </summary>
        [HttpPut("{id}/marcar-lida")]
        public async Task<IActionResult> MarcarComoLida(int id)
        {
            try
            {
                var sucesso = await _notificationService.MarcarComoLidaAsync(id);
                if (!sucesso) return NotFound(new { erro = "Notificação não encontrada" });

                return Ok(new { sucesso = true, mensagem = "Notificação marcada como lida" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao marcar notificação {Id} como lida", id);
                return StatusCode(500, new { erro = "Erro ao processar solicitação" });
            }
        }

        /// <summary>
        /// Marcar todas as notificações como lidas
        /// </summary>
        [HttpPut("marcar-todas-lidas")]
        public async Task<IActionResult> MarcarTodasComoLidas()
        {
            try
            {
                var sucesso = await _notificationService.MarcarTodasComoLidasAsync();
                return Ok(new { sucesso, mensagem = "Todas as notificações foram marcadas como lidas" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao marcar todas as notificações como lidas");
                return StatusCode(500, new { erro = "Erro ao processar solicitação" });
            }
        }

        /// <summary>
        /// Gerar notificações automáticas manualmente (para testes/debug)
        /// </summary>
        [HttpPost("gerar-automaticas")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GerarNotificacoesAutomaticas()
        {
            try
            {
                await _notificationService.GerarNotificacoesAutomaticasAsync();
                return Ok(new { sucesso = true, mensagem = "Notificações automáticas geradas" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar notificações automáticas");
                return StatusCode(500, new { erro = "Erro ao gerar notificações" });
            }
        }
    }
}