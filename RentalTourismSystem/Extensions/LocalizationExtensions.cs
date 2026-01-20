using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace RentalTourismSystem.Extensions;

/// <summary>
/// Extensões para configuração de localização e cultura brasileira
/// </summary>
public static class LocalizationExtensions
{
    /// <summary>
    /// Adiciona configuração de localização para pt-BR
    /// </summary>
    public static IServiceCollection AddBrazilianLocalization(this IServiceCollection services)
    {
        var supportedCultures = new[]
        {
            new CultureInfo("pt-BR"),
            new CultureInfo("pt"),
            new CultureInfo("en-US")
        };

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("pt-BR", "pt-BR");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;

            options.RequestCultureProviders.Clear();
            options.RequestCultureProviders.Add(new QueryStringRequestCultureProvider());
            options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
            options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());

            options.FallBackToParentCultures = true;
            options.FallBackToParentUICultures = true;
        });

        // Configurar cultura padrão do sistema
        ConfigureBrazilianCulture();

        return services;
    }

    /// <summary>
    /// Configura a cultura brasileira como padrão
    /// </summary>
    private static void ConfigureBrazilianCulture()
    {
        var brazilCulture = new CultureInfo("pt-BR")
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

        CultureInfo.DefaultThreadCurrentCulture = brazilCulture;
        CultureInfo.DefaultThreadCurrentUICulture = brazilCulture;
    }
}
