using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RentalTourismSystem.Services
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<NotificationBackgroundService> _logger;

        public NotificationBackgroundService(IServiceProvider services, ILogger<NotificationBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Background Service iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                        // Gerar notificações automáticas a cada hora
                        await notificationService.GerarNotificacoesAutomaticasAsync();

                        // Limpar notificações antigas uma vez por dia (se for entre 2h e 3h da manhã)
                        var now = DateTime.Now;
                        if (now.Hour == 2)
                        {
                            await notificationService.LimparNotificacoesAntigasAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no Notification Background Service");
                }

                // Aguardar 1 hora antes da próxima execução
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }

            _logger.LogInformation("Notification Background Service parado");
        }
    }
}