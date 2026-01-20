using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RentalTourismSystem.Data;

namespace RentalTourismSystem.HealthChecks;

/// <summary>
/// Health check customizado para verificar a saúde do banco de dados
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly RentalTourismContext _context;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(RentalTourismContext context, ILogger<DatabaseHealthCheck> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Tenta conectar ao banco
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);

            if (!canConnect)
            {
                _logger.LogWarning("Health Check: Não foi possível conectar ao banco de dados");
                return HealthCheckResult.Unhealthy(
                    "Não foi possível conectar ao banco de dados",
                    exception: null,
                    data: new Dictionary<string, object>
                    {
                        { "database", "unreachable" }
                    });
            }

            // Verifica se há migrations pendentes
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
            var hasPendingMigrations = pendingMigrations.Any();

            var data = new Dictionary<string, object>
            {
                { "database", "connected" },
                { "pendingMigrations", hasPendingMigrations },
                { "provider", _context.Database.ProviderName ?? "unknown" }
            };

            if (hasPendingMigrations)
            {
                _logger.LogWarning("Health Check: Existem {Count} migrations pendentes", pendingMigrations.Count());
                return HealthCheckResult.Degraded(
                    $"Banco de dados conectado, mas existem {pendingMigrations.Count()} migrations pendentes",
                    data: data);
            }

            return HealthCheckResult.Healthy(
                "Banco de dados conectado e atualizado",
                data: data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health Check: Erro ao verificar saúde do banco de dados");
            return HealthCheckResult.Unhealthy(
                "Erro ao verificar saúde do banco de dados",
                exception: ex,
                data: new Dictionary<string, object>
                {
                    { "error", ex.Message }
                });
        }
    }
}

/// <summary>
/// Health check para verificar a saúde geral do sistema
/// </summary>
public class SystemHealthCheck : IHealthCheck
{
    private readonly ILogger<SystemHealthCheck> _logger;

    public SystemHealthCheck(ILogger<SystemHealthCheck> logger)
    {
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Verifica memória disponível
            var totalMemory = GC.GetTotalMemory(false);
            var memoryInMB = totalMemory / 1024 / 1024;

            var data = new Dictionary<string, object>
            {
                { "memoryUsageMB", memoryInMB },
                { "processorCount", Environment.ProcessorCount },
                { "osVersion", Environment.OSVersion.VersionString },
                { "is64BitProcess", Environment.Is64BitProcess }
            };

            // Se estiver usando muita memória, retorna degraded
            if (memoryInMB > 1024) // Mais de 1GB
            {
                _logger.LogWarning("Health Check: Uso elevado de memória: {Memory}MB", memoryInMB);
                return Task.FromResult(HealthCheckResult.Degraded(
                    $"Sistema operacional com uso elevado de memória: {memoryInMB}MB",
                    data: data));
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                "Sistema operacional",
                data: data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health Check: Erro ao verificar saúde do sistema");
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Erro ao verificar saúde do sistema",
                exception: ex));
        }
    }
}

/// <summary>
/// Extension methods para Health Checks
/// </summary>
public static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthCheckConfiguration(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "db", "sql" })
            .AddCheck<SystemHealthCheck>("system", tags: new[] { "system" });

        return services;
    }

    public static IApplicationBuilder UseHealthCheckConfiguration(this IApplicationBuilder app)
    {
        // Endpoint básico de health
        app.UseHealthChecks("/health");

        // Endpoint detalhado (JSON)
        app.UseHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    timestamp = DateTime.UtcNow,
                    duration = report.TotalDuration,
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        duration = e.Value.Duration,
                        data = e.Value.Data
                    })
                });
                await context.Response.WriteAsync(result);
            }
        });

        return app;
    }
}
