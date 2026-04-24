using System.Net;
using System.Text.Json;
using GoodHamburger.Api.DTOs;
using GoodHamburger.Core.Exceptions;

namespace GoodHamburger.Api.Middleware;

/// <summary>
/// Middleware de tratamento global de erros — A05 Security Misconfiguration.
/// Garante que stack traces e detalhes internos NUNCA vazem para o cliente em produção.
/// Retorna Problem Details (RFC 7807).
/// </summary>
public sealed class ExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlerMiddleware> logger,
    IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        var (status, title, detail) = exception switch
        {
            NotFoundException nfe  => (HttpStatusCode.NotFound,           "Recurso não encontrado",    nfe.Message),
            DomainException   de   => (HttpStatusCode.UnprocessableEntity,"Regra de negócio violada",  de.Message),
            _                      => (HttpStatusCode.InternalServerError, "Erro interno do servidor",
                                       env.IsProduction()
                                           ? "Ocorreu um erro inesperado. Por favor, tente novamente."
                                           : exception.Message)
        };

        // Nunca logar dados sensíveis — OWASP A09 Security Logging
        if (status == HttpStatusCode.InternalServerError)
            logger.LogError(exception, "Unhandled exception [TraceId: {TraceId}]", traceId);
        else
            logger.LogWarning("Business/domain error [{Status}] — {Detail} [TraceId: {TraceId}]",
                (int)status, detail, traceId);

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode  = (int)status;

        var problem = new ProblemResponse(
            Type:    $"https://httpstatuses.io/{(int)status}",
            Title:   title,
            Status:  (int)status,
            Detail:  detail,
            TraceId: traceId
        );

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
    }
}
