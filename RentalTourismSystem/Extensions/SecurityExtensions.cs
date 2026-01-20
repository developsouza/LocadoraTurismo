using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace RentalTourismSystem.Extensions;

/// <summary>
/// Extensões para configuração de segurança
/// </summary>
public static class SecurityExtensions
{
    /// <summary>
    /// Adiciona configuração de segurança (CORS, Rate Limiting, Antiforgery)
    /// </summary>
    public static IServiceCollection AddSecurityConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", policy =>
            {
                var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                    ?? ["https://localhost:7000"];

                policy.WithOrigins(allowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials()
                      .SetIsOriginAllowedToAllowWildcardSubdomains();
            });

            options.AddPolicy("AllowDevelopment", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // Rate Limiting
        services.AddRateLimiter(options =>
        {
            // Política global para API
            options.AddFixedWindowLimiter("ApiPolicy", limiterOptions =>
            {
                limiterOptions.PermitLimit = 100;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 10;
            });

            // Política para validação de CPF
            options.AddFixedWindowLimiter("CpfValidationPolicy", limiterOptions =>
            {
                limiterOptions.PermitLimit = 20;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 5;
            });

            // Política para dashboard
            options.AddFixedWindowLimiter("DashboardPolicy", limiterOptions =>
            {
                limiterOptions.PermitLimit = 300;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 20;
            });

            // Política de concorrência para operações críticas
            options.AddConcurrencyLimiter("CriticalOperations", limiterOptions =>
            {
                limiterOptions.PermitLimit = 10;
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 5;
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        error = "Too many requests",
                        message = "Você excedeu o limite de requisições. Tente novamente em alguns instantes.",
                        retryAfter = retryAfter.TotalSeconds
                    }, cancellationToken);
                }
                else
                {
                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        error = "Too many requests",
                        message = "Você excedeu o limite de requisições. Tente novamente em alguns instantes."
                    }, cancellationToken);
                }
            };
        });

        // Antiforgery
        services.AddAntiforgery(options =>
        {
            options.HeaderName = "X-CSRF-TOKEN";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Cookie.Name = "__RequestVerificationToken";
            options.Cookie.SameSite = SameSiteMode.Strict;
        });

        // Compressão de resposta
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
            options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
            options.MimeTypes = Microsoft.AspNetCore.ResponseCompression.ResponseCompressionDefaults.MimeTypes.Concat(
                new[] { "application/json", "text/css", "text/javascript", "application/javascript" });
        });

        // Configuração de Brotli (melhor compressão)
        services.Configure<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProviderOptions>(options =>
        {
            options.Level = System.IO.Compression.CompressionLevel.Fastest;
        });

        return services;
    }

    /// <summary>
    /// Adiciona configuração de cache e sessão
    /// </summary>
    public static IServiceCollection AddCacheConfiguration(this IServiceCollection services)
    {
        // Memory Cache
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 1024;
            options.CompactionPercentage = 0.25;
            options.ExpirationScanFrequency = TimeSpan.FromMinutes(5);
        });

        // Distributed Cache
        services.AddDistributedMemoryCache();

        // Session
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.Cookie.Name = "RentalTourism.Session";
            options.Cookie.SameSite = SameSiteMode.Lax;
        });

        return services;
    }
}
