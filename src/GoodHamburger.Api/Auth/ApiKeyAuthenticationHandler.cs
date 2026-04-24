using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using GoodHamburger.Api.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace GoodHamburger.Api.Auth;

public sealed class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var providedKey))
            return Task.FromResult(AuthenticateResult.NoResult());

        if (string.IsNullOrWhiteSpace(Options.ApiKey))
            return Task.FromResult(AuthenticateResult.Fail("API key não configurada no servidor."));

        if (!string.Equals(providedKey.ToString(), Options.ApiKey, StringComparison.Ordinal))
            return Task.FromResult(AuthenticateResult.Fail("API key inválida."));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "api-client"),
            new Claim(ClaimTypes.Name, "GoodHamburger API Client")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = "application/problem+json";

        var problem = new ProblemResponse(
            "https://owasp.org/API-Security/editions/2023/en/0xa2-broken-authentication/",
            "Autenticação obrigatória",
            StatusCodes.Status401Unauthorized,
            $"Informe a API key no header '{Options.HeaderName}'.",
            Context.TraceIdentifier);

        await Response.WriteAsync(JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        Response.ContentType = "application/problem+json";

        var problem = new ProblemResponse(
            "https://owasp.org/API-Security/editions/2023/en/0xa5-broken-function-level-authorization/",
            "Acesso negado",
            StatusCodes.Status403Forbidden,
            "A credencial fornecida não possui permissão para este recurso.",
            Context.TraceIdentifier);

        await Response.WriteAsync(JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
