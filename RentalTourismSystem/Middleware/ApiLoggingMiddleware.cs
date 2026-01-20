using System.Diagnostics;

namespace RentalTourismSystem.Middleware;

/// <summary>
/// Middleware para logging detalhado de requisições de API
/// </summary>
public class ApiLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiLoggingMiddleware> _logger;

    public ApiLoggingMiddleware(RequestDelegate next, ILogger<ApiLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Verifica se é uma requisição de API
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();

        // Adiciona o RequestId ao contexto
        context.Items["RequestId"] = requestId;

        // Log da requisição
        _logger.LogInformation(
            "API Request [{RequestId}]: {Method} {Path} from {RemoteIP} - User: {User}",
            requestId,
            context.Request.Method,
            context.Request.Path + context.Request.QueryString,
            context.Connection.RemoteIpAddress,
            context.User.Identity?.Name ?? "Anonymous");

        // Captura a resposta
        var originalBodyStream = context.Response.Body;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "API Error [{RequestId}]: {Method} {Path} - Exception: {Message}",
                requestId,
                context.Request.Method,
                context.Request.Path,
                ex.Message);
            throw;
        }
        finally
        {
            stopwatch.Stop();

            // Log da resposta
            var logLevel = context.Response.StatusCode >= 500 ? LogLevel.Error :
                          context.Response.StatusCode >= 400 ? LogLevel.Warning :
                          LogLevel.Information;

            _logger.Log(logLevel,
                "API Response [{RequestId}]: {Method} {Path} - {StatusCode} in {ElapsedMs}ms",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}

/// <summary>
/// Extension method para adicionar o middleware
/// </summary>
public static class ApiLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseApiLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiLoggingMiddleware>();
    }
}
