using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;

namespace RentalTourismSystem.Extensions;

/// <summary>
/// Extensões para configuração de banco de dados
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Adiciona e configura o Entity Framework Core com SQL Server
    /// </summary>
    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddDbContext<RentalTourismContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            options.UseSqlServer(connectionString, sqlServerOptions =>
            {
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);

                sqlServerOptions.CommandTimeout(30);
                sqlServerOptions.MigrationsAssembly(typeof(RentalTourismContext).Assembly.FullName);
            });

            // Configurações de ambiente
            if (environment.IsProduction())
            {
                options.EnableSensitiveDataLogging(false);
                options.EnableServiceProviderCaching();
                options.ConfigureWarnings(warnings => warnings.Ignore());
            }
            else
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }

            // Performance otimizations
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
        });

        return services;
    }

    /// <summary>
    /// Aplica migrations e executa seed de dados
    /// </summary>
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<RentalTourismContext>();
            var logger = services.GetRequiredService<ILogger<RentalTourismContext>>();

            // Verificar conexão
            var canConnect = await context.Database.CanConnectAsync();
            if (!canConnect)
            {
                logger.LogError("Não foi possível conectar ao banco de dados");
                throw new InvalidOperationException("Database connection failed");
            }

            // Aplicar migrations pendentes
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Aplicando {Count} migrations pendentes", pendingMigrations.Count());
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations aplicadas com sucesso");
            }

            // Seed de dados
            var userManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<Models.ApplicationUser>>();
            var roleManager = services.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();

            await Data.SeedData.InitializeAsync(services, userManager, roleManager, logger);

            logger.LogInformation("Banco de dados inicializado com sucesso");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<RentalTourismContext>>();
            logger.LogError(ex, "Erro ao inicializar banco de dados");
            throw;
        }
    }
}
