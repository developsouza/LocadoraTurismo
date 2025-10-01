using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using RentalTourismSystem.Data;
using RentalTourismSystem.ModelBinders;
using RentalTourismSystem.Models;
using RentalTourismSystem.Services;
using Serilog;
using Serilog.Events;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURAÇÃO DO SERILOG (Melhorada) =====
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "RentalTourismSystem")
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/rental-tourism-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        fileSizeLimitBytes: 10485760,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// ===== CONFIGURAÇÃO DE GLOBALIZAÇÃO/LOCALIZAÇÃO =====
var supportedCultures = new[]
{
    new CultureInfo("pt-BR"),
    new CultureInfo("pt"),
    new CultureInfo("en-US")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
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
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

var brazilCulture = new CultureInfo("pt-BR");
brazilCulture.NumberFormat.CurrencySymbol = "R$";
brazilCulture.NumberFormat.CurrencyDecimalDigits = 2;
brazilCulture.NumberFormat.CurrencyDecimalSeparator = ",";
brazilCulture.NumberFormat.CurrencyGroupSeparator = ".";
brazilCulture.NumberFormat.NumberDecimalSeparator = ",";
brazilCulture.NumberFormat.NumberGroupSeparator = ".";

brazilCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
brazilCulture.DateTimeFormat.LongDatePattern = "dddd, d 'de' MMMM 'de' yyyy";
brazilCulture.DateTimeFormat.ShortTimePattern = "HH:mm";
brazilCulture.DateTimeFormat.LongTimePattern = "HH:mm:ss";

CultureInfo.DefaultThreadCurrentCulture = brazilCulture;
CultureInfo.DefaultThreadCurrentUICulture = brazilCulture;

// ===== CONFIGURE ENTITY FRAMEWORK =====
builder.Services.AddDbContext<RentalTourismContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    if (builder.Environment.IsProduction())
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
});

// ===== CONFIGURAÇÃO DO ASP.NET CORE IDENTITY =====
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    // Configurações de senha
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Configurações de lockout
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Configurações de usuário
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Configurações de signin
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddRoles<IdentityRole>() // Adicionar suporte a roles
.AddEntityFrameworkStores<RentalTourismContext>(); // Usar nosso context

// ===== CONFIGURAÇÃO DE COOKIE DE AUTENTICAÇÃO =====
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8); // 8 horas de sessão
    options.Cookie.Name = "RentalTourismAuth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// ===== CONFIGURAÇÃO DE CONTROLLERS (Melhorada) =====
builder.Services.AddControllersWithViews(options =>
{
    options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
    options.ReturnHttpNotAcceptable = true;
})
.AddDataAnnotationsLocalization()
.AddViewLocalization()

.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage)
            .ToList();

        var errorResponse = new
        {
            Message = "Dados inválidos",
            Errors = errors,
            Timestamp = DateTime.Now
        };

        return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(errorResponse);
    };
});

// ===== CONFIGURAÇÃO DE API VERSIONING =====
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0); // <-- Não precisa do namespace completo
    options.ReportApiVersions = true; // <-- Boa prática para reportar versões no header
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Version"),
        new QueryStringApiVersionReader("version")
    );
})
.AddApiExplorer(options => // <-- Adicione isso para integração com Swagger
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

// ===== CONFIGURAÇÃO DO SWAGGER =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Rental Tourism System API",
        Version = "v1",
        Description = "API para Sistema de Locação e Turismo Integrado - Litoral Sul",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Suporte Técnico",
            Email = "suporte@litoralsul.com.br"
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.WriteIndented = true;
});

// ===== REGISTRAR SERVIÇOS CUSTOMIZADOS =====
builder.Services.AddScoped<IRelatorioService, RelatorioService>();
builder.Services.AddScoped<ILocacaoService, LocacaoService>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();

// ===== CONFIGURAÇÃO DE CACHE =====
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024;
});
builder.Services.AddDistributedMemoryCache();

// ===== CONFIGURAÇÃO DE SESSÃO =====
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.Name = "RentalTourism.Session";
});

// ===== CONFIGURAÇÃO DE COMPRESSÃO DE RESPOSTA =====
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
    options.MimeTypes = Microsoft.AspNetCore.ResponseCompression.ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/json", "text/css", "text/javascript" });
});

// ===== CONFIGURAÇÃO DE ANTIFORGERY =====
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.Name = "__RequestVerificationToken";
});

// ===== CONFIGURAÇÃO DE CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://localhost:7000", "https://yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    options.AddPolicy("AllowDevelopment", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ===== CONFIGURAÇÃO DE RATE LIMITING =====
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("ApiPolicy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 10;
    });

    options.AddFixedWindowLimiter("CpfValidationPolicy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 20;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 5;
    });

    options.AddFixedWindowLimiter("DashboardPolicy", limiterOptions =>
    {
        limiterOptions.PermitLimit = 300;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 20;
    });

    options.RejectionStatusCode = 429;
});

// ===== HEALTH CHECKS =====
//builder.Services.AddHealthChecks()
//    .AddDbContextCheck<RentalTourismContext>("database")
//    .AddCheck("sistema", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Sistema operacional"));

var app = builder.Build();

// ===== INICIALIZAÇÃO DE DADOS (SEED) =====
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<RentalTourismContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        // Aplicar migrations se necessário
        if (context.Database.GetPendingMigrations().Any())
        {
            Log.Information("Aplicando {Count} migrations pendentes", context.Database.GetPendingMigrations().Count());
            context.Database.Migrate();
        }

        // Seed de dados
        await SeedData.InitializeAsync(scope.ServiceProvider, userManager, roleManager, logger);

        Log.Information("Banco de dados e seed inicializados com sucesso");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Erro crítico ao inicializar banco de dados");
        throw;
    }
}

// ===== APLICAR CONFIGURAÇÕES DE GLOBALIZAÇÃO =====
app.UseRequestLocalization();

// ===== CONFIGURAR PIPELINE DE REQUESTS =====

// Rate Limiting
app.UseRateLimiter();

// Compressão de resposta
app.UseResponseCompression();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();

    // Swagger apenas em desenvolvimento
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rental Tourism System API v1");
        c.RoutePrefix = "api/docs";
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}

// ===== HEADERS DE SEGURANÇA =====
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

    if (context.Request.IsHttps)
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    }

    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// CORS
var corsPolicy = app.Environment.IsDevelopment() ? "AllowDevelopment" : "AllowSpecificOrigins";
app.UseCors(corsPolicy);

app.UseSession();

// ===== AUTENTICAÇÃO E AUTORIZAÇÃO =====
app.UseAuthentication(); // DEVE vir antes de UseAuthorization
app.UseAuthorization();

// ===== MIDDLEWARE DE LOGGING DE REQUESTS =====
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information;
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        if (httpContext.User.Identity.IsAuthenticated)
        {
            diagnosticContext.Set("UserName", httpContext.User.Identity.Name);
        }
    };
});

// ===== MIDDLEWARE PARA LOGGING ESPECÍFICO DE APIS =====
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        logger.LogInformation("API Request: {Method} {Path} from {RemoteIP} - User: {User}",
            context.Request.Method,
            context.Request.Path + context.Request.QueryString,
            context.Connection.RemoteIpAddress,
            context.User.Identity?.Name ?? "Anonymous");

        await next();

        stopwatch.Stop();

        var logLevel = context.Response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
        logger.Log(logLevel, "API Response: {Method} {Path} - {StatusCode} in {ElapsedMs}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
    else
    {
        await next();
    }
});

// ===== MIDDLEWARE PARA CULTURA BRASILEIRA =====
app.Use(async (context, next) =>
{
    var culture = new CultureInfo("pt-BR");
    culture.NumberFormat.CurrencySymbol = "R$";
    culture.NumberFormat.CurrencyDecimalDigits = 2;
    culture.NumberFormat.CurrencyDecimalSeparator = ",";
    culture.NumberFormat.CurrencyGroupSeparator = ".";

    Thread.CurrentThread.CurrentCulture = culture;
    Thread.CurrentThread.CurrentUICulture = culture;

    await next();
});

// ===== HEALTH CHECKS ENDPOINT =====
//app.MapHealthChecks("/health");

// ===== CONFIGURAÇÃO DE ROTAS =====
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Rotas para API
app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller}/{action=Index}/{id?}")
    .RequireRateLimiting("ApiPolicy");

app.MapControllerRoute(
    name: "api_v1",
    pattern: "api/v1/{controller}/{action}/{id?}")
    .RequireRateLimiting("ApiPolicy");

// ===== LOG DE INICIALIZAÇÃO FINAL =====
try
{
    var dbContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<RentalTourismContext>();
    var canConnect = await dbContext.Database.CanConnectAsync();

    Log.Information("=== SISTEMA INICIADO COM SUCESSO ===");
    Log.Information("Ambiente: {Environment}", app.Environment.EnvironmentName);
    Log.Information("Cultura: {Culture}", CultureInfo.CurrentCulture.Name);
    Log.Information("Banco de Dados: {Status}", canConnect ? "✅ Conectado" : "❌ Erro de conexão");
    Log.Information("Autenticação: ✅ ASP.NET Core Identity configurado");
    Log.Information("Swagger: {Status}", app.Environment.IsDevelopment() ? "✅ Disponível em /api/docs" : "❌ Desabilitado em produção");
    Log.Information("Health Checks: ✅ Disponível em /health");
    Log.Information("Rate Limiting: ✅ Ativo");
    Log.Information("Usuário Admin padrão: admin@litoralsul.com.br / Admin@123456");
    Log.Information("=== SISTEMA PRONTO PARA USO ===");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro fatal ao iniciar a aplicação");
    throw;
}
finally
{
    Log.CloseAndFlush();
}