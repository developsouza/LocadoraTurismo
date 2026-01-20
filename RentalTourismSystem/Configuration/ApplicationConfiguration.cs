namespace RentalTourismSystem.Configuration;

/// <summary>
/// Configurações da aplicação fortemente tipadas
/// </summary>
public class ApplicationConfiguration
{
    public const string SectionName = "ApplicationSettings";

    /// <summary>
    /// Configurações de logging
    /// </summary>
    public LoggingSettings Logging { get; set; } = new();

    /// <summary>
    /// Configurações de segurança
    /// </summary>
    public SecuritySettings Security { get; set; } = new();

    /// <summary>
    /// Configurações de upload de arquivos
    /// </summary>
    public FileUploadSettings FileUpload { get; set; } = new();

    /// <summary>
    /// Configurações de email
    /// </summary>
    public EmailSettings Email { get; set; } = new();

    /// <summary>
    /// Configurações de performance
    /// </summary>
    public PerformanceSettings Performance { get; set; } = new();
}

public class LoggingSettings
{
    public bool EnableApiLogging { get; set; } = true;
    public bool EnablePerformanceLogging { get; set; } = true;
    public int SlowRequestThresholdMs { get; set; } = 3000;
}

public class SecuritySettings
{
    public int MaxLoginAttempts { get; set; } = 5;
    public int LockoutMinutes { get; set; } = 15;
    public bool RequireHttps { get; set; } = true;
    public string[] AllowedOrigins { get; set; } = ["https://localhost:7000"];
}

public class FileUploadSettings
{
    public long MaxFileSizeBytes { get; set; } = 10485760; // 10 MB
    public string[] AllowedExtensions { get; set; } = [".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx"];
    public string UploadPath { get; set; } = "wwwroot/uploads";
}

public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
}

public class PerformanceSettings
{
    public int CacheDurationMinutes { get; set; } = 30;
    public int DatabaseCommandTimeoutSeconds { get; set; } = 30;
    public int MaxDegreeOfParallelism { get; set; } = 4;
    public bool EnableResponseCaching { get; set; } = true;
    public bool EnableOutputCaching { get; set; } = false;
}

/// <summary>
/// Extension methods para registrar configurações
/// </summary>
public static class ConfigurationExtensions
{
    public static IServiceCollection AddApplicationConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind e registra as configurações
        var appConfig = new ApplicationConfiguration();
        configuration.GetSection(ApplicationConfiguration.SectionName).Bind(appConfig);

        services.AddSingleton(appConfig);
        services.Configure<ApplicationConfiguration>(
            configuration.GetSection(ApplicationConfiguration.SectionName));

        return services;
    }
}
