using Serilog;
using GoodHamburger.Api.Auth;
using GoodHamburger.Api.Middleware;
using GoodHamburger.Api.Services;
using GoodHamburger.Core.Interfaces;
using GoodHamburger.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

// ── Bootstrap Serilog antes de qualquer coisa ────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

    // ── Logging ──────────────────────────────────────────────────────────────
    builder.Host.UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .WriteTo.Console(outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"));

    // ── Services ─────────────────────────────────────────────────────────────
    builder.Services.AddProblemDetails();
    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            // Customizar resposta de validação padrão para Problem Details
            options.InvalidModelStateResponseFactory = ctx =>
            {
                var errors = ctx.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        k => k.Key,
                        v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return new Microsoft.AspNetCore.Mvc.UnprocessableEntityObjectResult(new
                {
                    type    = "https://httpstatuses.io/422",
                    title   = "Erro de validação",
                    status  = 422,
                    errors
                });
            };
        });

    builder.Services
        .AddAuthentication(ApiKeyAuthenticationDefaults.SchemeName)
        .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
            ApiKeyAuthenticationDefaults.SchemeName,
            options =>
            {
                builder.Configuration.GetSection("Security:ApiKey").Bind(options);
                options.HeaderName = ApiKeyAuthenticationDefaults.HeaderName;
            });

    builder.Services.AddAuthorization();

    // ── Repositórios (InMemory — fácil de trocar por EF Core) ────────────────
    builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
    builder.Services.AddSingleton<IMenuRepository,  InMemoryMenuRepository>();

    // ── Application Services ─────────────────────────────────────────────────
    builder.Services.AddScoped<IOrderService, OrderService>();
    builder.Services.AddScoped<IMenuService,  MenuService>();

    // ── CORS — OWASP A05: apenas origens conhecidas ──────────────────────────
    builder.Services.AddCors(options =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        options.AddPolicy("BlazorFrontend", policy =>
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader());
    });

    // ── Swagger / OpenAPI ─────────────────────────────────────────────────────
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title       = "Good Hamburger API",
            Version     = "v1",
            Description = "Sistema de pedidos da lanchonete Good Hamburger. " +
                          "Construído com Clean Architecture + OWASP Top 10.",
            Contact = new OpenApiContact
            {
                Name  = "Good Hamburger Dev",
                Email = "dev@goodhamburger.com"
            }
        });

        c.AddSecurityDefinition(ApiKeyAuthenticationDefaults.SchemeName, new OpenApiSecurityScheme
        {
            Name = ApiKeyAuthenticationDefaults.HeaderName,
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Description = "API key necessária para endpoints protegidos."
        });
        c.OperationFilter<ApiKeySecurityRequirementsOperationFilter>();

        // Incluir comentários XML nos endpoints
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
            c.IncludeXmlComments(xmlPath);
    });

    // ── Rate Limiting — OWASP A04: prevenção de abuso ────────────────────────
    builder.Services.AddRateLimiter(options =>
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            var partitionKey = httpContext.User.Identity?.IsAuthenticated == true
                ? httpContext.User.Identity.Name ?? "authenticated-client"
                : httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

            return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 10
            });
        });
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.OnRejected = async (context, ct) =>
        {
            context.HttpContext.Response.ContentType = "application/problem+json";
            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                type = "https://owasp.org/API-Security/editions/2023/en/0xa4-unrestricted-resource-consumption/",
                title = "Limite de requisições excedido",
                status = StatusCodes.Status429TooManyRequests,
                detail = "Aguarde antes de realizar novas chamadas para esta API.",
                traceId = context.HttpContext.TraceIdentifier
            }, cancellationToken: ct);
        };
    });

    // ── Build ─────────────────────────────────────────────────────────────────
    var app = builder.Build();

    // ── Security Headers (OWASP) ──────────────────────────────────────────────
    app.UseMiddleware<SecurityHeadersMiddleware>();

    // ── Exception Handler (sempre antes de controllers) ───────────────────────
    app.UseMiddleware<ExceptionHandlerMiddleware>();

    // ── HTTPS Redirect (apenas em produção) ───────────────────────────────────
    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
        app.UseHttpsRedirection();
    }

    // ── CORS ──────────────────────────────────────────────────────────────────
    app.UseCors("BlazorFrontend");

    app.UseAuthentication();
    app.UseAuthorization();

    // ── Rate Limiter ──────────────────────────────────────────────────────────
    app.UseRateLimiter();

    // ── Swagger (apenas em desenvolvimento) ───────────────────────────────────
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Good Hamburger API v1");
            c.RoutePrefix = string.Empty; // Swagger na raiz
        });
    }

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação encerrada inesperadamente.");
}
finally
{
    Log.CloseAndFlush();
}

// Necessário para testes de integração (WebApplicationFactory)
public partial class Program { }
