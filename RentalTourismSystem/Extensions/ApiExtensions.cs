using Asp.Versioning;
using Microsoft.OpenApi.Models;
using RentalTourismSystem.ModelBinders;

namespace RentalTourismSystem.Extensions;

/// <summary>
/// Extensões para configuração de API, Swagger e versionamento
/// </summary>
public static class ApiExtensions
{
    /// <summary>
    /// Adiciona configuração de API com versionamento e documentação
    /// </summary>
    public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
    {
        // Configuração de Controllers
        services.AddControllersWithViews(options =>
        {
            options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
            options.ReturnHttpNotAcceptable = true;
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = false;
        })
        .AddDataAnnotationsLocalization()
        .AddViewLocalization()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .SelectMany(x => x.Value?.Errors ?? [])
                    .Select(x => x.ErrorMessage)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();

                var errorResponse = new
                {
                    Message = "Dados inválidos",
                    Errors = errors,
                    Timestamp = DateTime.UtcNow
                };

                return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(errorResponse);
            };
        });

        // API Versioning
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Version"),
                new QueryStringApiVersionReader("version")
            );
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        // Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Rental Tourism System API",
                Version = "v1",
                Description = "API para Sistema de Locação e Turismo Integrado - Litoral Sul",
                Contact = new OpenApiContact
                {
                    Name = "Suporte Técnico",
                    Email = "suporte@litoralsul.com.br"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Adicionar suporte para autenticação JWT (futuro)
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            // Incluir comentários XML se existirem
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // Ordenar por nome
            c.OrderActionsBy(apiDesc => apiDesc.RelativePath);
        });

        // Configuração de JSON
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = null;
            options.SerializerOptions.WriteIndented = true;
            options.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

        return services;
    }

    /// <summary>
    /// Configura o pipeline de Swagger
    /// </summary>
    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rental Tourism System API v1");
            c.RoutePrefix = "api/docs";
            c.DisplayRequestDuration();
            c.EnableTryItOutByDefault();
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
            c.DefaultModelsExpandDepth(-1); // Ocultar schemas por padrão
        });

        return app;
    }
}
