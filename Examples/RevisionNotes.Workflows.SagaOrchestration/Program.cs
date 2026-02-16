using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevisionNotes.Workflows.SagaOrchestration.Contracts;
using RevisionNotes.Workflows.SagaOrchestration.Infrastructure;
using RevisionNotes.Workflows.SagaOrchestration.Saga;
using RevisionNotes.Workflows.SagaOrchestration.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtIssuerOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<JwtTokenFactory>();
builder.Services.AddSingleton<SagaStateStore>();
builder.Services.AddScoped<InventoryStepClient>();
builder.Services.AddScoped<PaymentStepClient>();
builder.Services.AddScoped<OrderSagaOrchestrator>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt").Get<JwtIssuerOptions>() ?? new JwtIssuerOptions();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = JwtTokenFactory.CreateSigningKey(jwt.SigningKey),
            ClockSkew = TimeSpan.FromSeconds(20)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("saga.user", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks()
    .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<SagaHealthCheck>("saga", tags: ["ready"]);
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    var sw = Stopwatch.StartNew();
    await next();
    sw.Stop();
    app.Logger.LogInformation(
        "HTTP {Method} {Path} -> {StatusCode} in {ElapsedMs}ms",
        context.Request.Method,
        context.Request.Path,
        context.Response.StatusCode,
        sw.ElapsedMilliseconds);
});

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/auth/token", (LoginRequest request, IOptions<JwtIssuerOptions> options, JwtTokenFactory tokenFactory, IConfiguration config) =>
{
    var expectedUser = config["DemoCredentials:Username"] ?? "saga-admin";
    var expectedPassword = config["DemoCredentials:Password"] ?? "ChangeMe!123";

    if (!string.Equals(request.Username, expectedUser, StringComparison.Ordinal) ||
        !string.Equals(request.Password, expectedPassword, StringComparison.Ordinal))
    {
        return Results.Unauthorized();
    }

    var token = tokenFactory.CreateToken(options.Value, request.Username);
    return Results.Ok(new { access_token = token, token_type = "Bearer" });
});

app.MapPost("/api/sagas/orders", async (StartOrderSagaRequest request, OrderSagaOrchestrator orchestrator, CancellationToken cancellationToken) =>
{
    if (request.Amount <= 0 || request.Quantity <= 0)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["amount"] = ["Amount must be greater than zero."],
            ["quantity"] = ["Quantity must be greater than zero."]
        });
    }

    var sagaId = await orchestrator.StartAsync(request, cancellationToken);
    return Results.Accepted($"/api/sagas/orders/{sagaId}", new { sagaId });
})
.RequireAuthorization("saga.user");

app.MapGet("/api/sagas/orders/{sagaId:guid}", (Guid sagaId, SagaStateStore store) =>
{
    var state = store.Get(sagaId);
    return state is null ? Results.NotFound() : Results.Ok(state);
})
.RequireAuthorization("saga.user");

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.Run();
