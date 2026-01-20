using System.Globalization;

namespace RentalTourismSystem.Middleware;

/// <summary>
/// Middleware para garantir cultura brasileira em todas as requisições
/// </summary>
public class CultureMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly CultureInfo BrazilCulture;

    static CultureMiddleware()
    {
        BrazilCulture = new CultureInfo("pt-BR")
        {
            NumberFormat =
            {
                CurrencySymbol = "R$",
                CurrencyDecimalDigits = 2,
                CurrencyDecimalSeparator = ",",
                CurrencyGroupSeparator = ".",
                NumberDecimalSeparator = ",",
                NumberGroupSeparator = "."
            },
            DateTimeFormat =
            {
                ShortDatePattern = "dd/MM/yyyy",
                LongDatePattern = "dddd, d 'de' MMMM 'de' yyyy",
                ShortTimePattern = "HH:mm",
                LongTimePattern = "HH:mm:ss"
            }
        };
    }

    public CultureMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Define a cultura para a thread atual
        CultureInfo.CurrentCulture = BrazilCulture;
        CultureInfo.CurrentUICulture = BrazilCulture;

        Thread.CurrentThread.CurrentCulture = BrazilCulture;
        Thread.CurrentThread.CurrentUICulture = BrazilCulture;

        await _next(context);
    }
}

/// <summary>
/// Extension method para adicionar o middleware
/// </summary>
public static class CultureMiddlewareExtensions
{
    public static IApplicationBuilder UseBrazilianCulture(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CultureMiddleware>();
    }
}
