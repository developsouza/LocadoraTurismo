using RentalTourismSystem.Configuration;
using RentalTourismSystem.Extensions;
using RentalTourismSystem.HealthChecks;
using RentalTourismSystem.Middleware;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);


// ===== CONFIGURAÇÃO DO SERILOG =====
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "RentalTourismSystem")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console(
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/rental-tourism-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        fileSizeLimitBytes: 10485760,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

Log.Information("=== Iniciando Rental Tourism System ===");
Log.Information("Ambiente: {Environment}", builder.Environment.EnvironmentName);



// ===== CONFIGURAÇÃO DE SERVIÇOS =====

// Configurações fortemente tipadas
builder.Services.AddApplicationConfiguration(builder.Configuration);

// Localização e cultura brasileira
builder.Services.AddBrazilianLocalization();

// Banco de dados
builder.Services.AddDatabaseConfiguration(builder.Configuration, builder.Environment);

// Identity e autenticação
builder.Services.AddIdentityConfiguration(builder.Configuration);

// API, Swagger e versionamento
builder.Services.AddApiConfiguration();

// Segurança (CORS, Rate Limiting, Antiforgery)
builder.Services.AddSecurityConfiguration(builder.Configuration, builder.Environment);

// Cache e sessão
builder.Services.AddCacheConfiguration();

// Health Checks
builder.Services.AddHealthCheckConfiguration();




// ===== REGISTRAR SERVIÇOS DA APLICAÇÃO =====
builder.Services.AddApplicationServices();

// ===== OTIMIZAÇÕES DE PERFORMANCE =====
builder.Services.AddPerformanceOptimizations();

Log.Information("Serviços configurados com sucesso");




var app = builder.Build();

// ===== INICIALIZAÇÃO DO BANCO DE DADOS =====
try
{
    await app.Services.InitializeDatabaseAsync();
    Log.Information("Banco de dados inicializado com sucesso");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro crítico ao inicializar banco de dados");
    throw;
}



// ===== CONFIGURAÇÃO DO PIPELINE DE MIDDLEWARE =====

// Localização (precisa ser uma das primeiras)
app.UseRequestLocalization();

// Rate Limiting
app.UseRateLimiter();

// Compressão de resposta - DESABILITADA EM DESENVOLVIMENTO para evitar conflitos com IIS Express
if (!app.Environment.IsDevelopment())
{
    app.UseResponseCompression();
}

// Tratamento de erros
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerConfiguration();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Headers de segurança
app.UseSecurityHeaders();

// Arquivos estáticos
app.UseHttpsRedirection();
app.UseStaticFiles();

// Roteamento
app.UseRouting();

// CORS
var corsPolicy = app.Environment.IsDevelopment() ? "AllowDevelopment" : "AllowSpecificOrigins";
app.UseCors(corsPolicy);

// Sessão
app.UseSession();

// Autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

// Cultura brasileira
app.UseBrazilianCulture();

// Logging de API
app.UseApiLogging();

// Serilog Request Logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information;
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            diagnosticContext.Set("UserName", httpContext.User.Identity.Name);
        }
    };
});

// Health Checks
app.UseHealthCheckConfiguration();



// ===== CONFIGURAÇÃO DE ROTAS =====
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller}/{action=Index}/{id?}")
    .RequireRateLimiting("ApiPolicy");

app.MapControllerRoute(
    name: "api_v1",
    pattern: "api/v1/{controller}/{action}/{id?}")
    .RequireRateLimiting("ApiPolicy");



// ===== INICIAR APLICAÇÃO =====
try
{
    Log.Information("=== SISTEMA INICIADO COM SUCESSO ===");
    Log.Information("Ambiente: {Environment}", app.Environment.EnvironmentName);
    Log.Information("Autenticação: ASP.NET Core Identity configurado");
    Log.Information("Swagger: {Status}", app.Environment.IsDevelopment() ? "Disponível em /api/docs" : "Desabilitado (Produção)");
    Log.Information("Health Checks: /health e /health/ready");
    Log.Information("Rate Limiting: Ativo");
    Log.Information("Usuário Admin padrão: admin@litoralsul.com.br / Admin@123456");
    Log.Information("=== SISTEMA PRONTO ===");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro fatal ao iniciar a aplicação");
    throw;
}
finally
{
    Log.Information("Encerrando aplicação...");
    Log.CloseAndFlush();
}
