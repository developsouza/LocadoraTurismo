using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RentalTourismSystem.Controllers;
using RentalTourismSystem.Middleware;

namespace RentalTourismSystem.Tests;

public class SecurityPolicyTests
{
    [Fact]
    public void LoginPost_DeveTerRateLimitEspecifico()
    {
        var action = typeof(AccountController).GetMethods()
            .Single(m => m.Name == nameof(AccountController.Login) &&
                         m.GetParameters().FirstOrDefault()?.ParameterType.Name == "LoginViewModel");

        var attribute = action.GetCustomAttribute<EnableRateLimitingAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("LoginPolicy", attribute.PolicyName);
    }

    [Fact]
    public async Task SecurityHeaders_DeveAplicarCspSemUnsafeEval()
    {
        var context = new DefaultHttpContext();
        var middleware = new SecurityHeadersMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        var csp = context.Response.Headers.ContentSecurityPolicy.ToString();
        Assert.Contains("object-src 'none'", csp);
        Assert.Contains("form-action 'self'", csp);
        Assert.Contains("https://fonts.googleapis.com", csp);
        Assert.Contains("https://fonts.gstatic.com", csp);
        Assert.Contains("connect-src 'self' https://viacep.com.br", csp);
        Assert.DoesNotContain("unsafe-eval", csp);
        Assert.Equal("nosniff", context.Response.Headers.XContentTypeOptions.ToString());
        Assert.Contains("camera=()", context.Response.Headers["Permissions-Policy"].ToString());
    }

    [Theory]
    [InlineData(nameof(NotificacoesController.MarcarComoLida))]
    [InlineData(nameof(NotificacoesController.MarcarTodasComoLidas))]
    [InlineData(nameof(NotificacoesController.GerarNotificacoesAutomaticas))]
    public void NotificacoesMutaveis_DevemValidarAntiforgery(string actionName)
    {
        var action = typeof(NotificacoesController).GetMethod(actionName);

        Assert.NotNull(action);
        Assert.NotNull(action!.GetCustomAttribute<ValidateAntiForgeryTokenAttribute>());
    }

    [Theory]
    [InlineData("=HYPERLINK(\"https://malicioso\")")]
    [InlineData("  +CMD")]
    [InlineData("@SUM(1,1)")]
    [InlineData("-2+3")]
    public void ExportacaoCsv_DeveNeutralizarFormulas(string value)
    {
        var method = typeof(RelatoriosController).GetMethod(
            "EscapeCsvField", BindingFlags.NonPublic | BindingFlags.Static)!;

        var escaped = Assert.IsType<string>(method.Invoke(null, [value]));

        Assert.StartsWith("\"'", escaped);
        Assert.StartsWith("\"", escaped);
        Assert.EndsWith("\"", escaped);
    }
}
