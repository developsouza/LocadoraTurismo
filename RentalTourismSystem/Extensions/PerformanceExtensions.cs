using RentalTourismSystem.Services;

namespace RentalTourismSystem.Extensions;

/// <summary>
/// Extensões para otimizações de performance
/// </summary>
public static class PerformanceExtensions
{
    /// <summary>
    /// Adiciona configurações de performance e otimizações
    /// </summary>
    public static IServiceCollection AddPerformanceOptimizations(this IServiceCollection services)
    {
        // Output Caching (ASP.NET Core 7+)
        services.AddOutputCache(options =>
        {
            // Cache padrão de 60 segundos
            options.AddBasePolicy(builder => builder
                .Expire(TimeSpan.FromSeconds(60))
                .Tag("default"));

            // Cache para páginas públicas
            options.AddPolicy("PublicPages", builder => builder
                .Expire(TimeSpan.FromMinutes(5))
                .SetVaryByQuery("page", "pageSize")
                .Tag("public"));

            // Cache para API
            options.AddPolicy("ApiCache", builder => builder
                .Expire(TimeSpan.FromMinutes(2))
                .SetVaryByQuery("*")
                .Tag("api"));

            // Cache para relatórios (mais longo)
            options.AddPolicy("Reports", builder => builder
                .Expire(TimeSpan.FromMinutes(30))
                .Tag("reports"));
        });

        // Response Caching
        services.AddResponseCaching(options =>
        {
            options.MaximumBodySize = 1024 * 1024; // 1 MB
            options.UseCaseSensitivePaths = false;
        });

        // HTTP Client Factory para performance
        services.AddHttpClient();

        // Configurações de threading otimizadas
        ThreadPool.SetMinThreads(50, 50);

        return services;
    }

    /// <summary>
    /// Registra todos os serviços da aplicação
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Serviços principais
        services.AddScoped<IRelatorioService, RelatorioService>();
        services.AddScoped<ILocacaoService, LocacaoService>();
        services.AddScoped<IVeiculoService, VeiculoService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IDocumentacaoService, DocumentacaoService>();

        // Background services
        services.AddHostedService<NotificationBackgroundService>();

        return services;
    }
}
