namespace GoodHamburger.Api.Middleware;

/// <summary>
/// Adiciona cabeçalhos de segurança recomendados pelo  Secure Headers Project.
/// Mitiga XSS (A03), Clickjacking, MIME sniffing e information disclosure.
/// </summary>
public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;

            // Endpoints de API não precisam carregar recursos ativos do browser.
            headers["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none'; base-uri 'none'; form-action 'none'";
            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["Referrer-Policy"] = "no-referrer";
            headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
            headers["X-Permitted-Cross-Domain-Policies"] = "none";
            headers["Cache-Control"] = "no-store";
            headers.Remove("X-Powered-By");

            return Task.CompletedTask;
        });

        await next(context);
    }
}
