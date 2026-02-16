using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevisionNotes.EventSourcing.Cqrs.Commands;
using RevisionNotes.EventSourcing.Cqrs.Contracts;
using RevisionNotes.EventSourcing.Cqrs.Events;
using RevisionNotes.EventSourcing.Cqrs.Infrastructure;
using RevisionNotes.EventSourcing.Cqrs.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtIssuerOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<JwtTokenFactory>();
builder.Services.AddSingleton<IEventStore, InMemoryEventStore>();
builder.Services.AddSingleton<AccountProjectionStore>();
builder.Services.AddScoped<AccountCommandService>();
builder.Services.AddScoped<AccountQueryService>();

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
    options.AddPolicy("cqrs.user", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks()
    .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<EventStoreHealthCheck>("event-store", tags: ["ready"]);
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
    var expectedUser = config["DemoCredentials:Username"] ?? "cqrs-admin";
    var expectedPassword = config["DemoCredentials:Password"] ?? "ChangeMe!123";

    if (!string.Equals(request.Username, expectedUser, StringComparison.Ordinal) ||
        !string.Equals(request.Password, expectedPassword, StringComparison.Ordinal))
    {
        return Results.Unauthorized();
    }

    var token = tokenFactory.CreateToken(options.Value, request.Username);
    return Results.Ok(new { access_token = token, token_type = "Bearer" });
});

var commands = app.MapGroup("/api/commands/accounts").RequireAuthorization("cqrs.user");
commands.MapPost("/", async (OpenAccountRequest request, AccountCommandService service, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.OwnerName))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]> { ["ownerName"] = ["OwnerName is required."] });
    }

    var id = await service.OpenAccountAsync(request.OwnerName, request.InitialDeposit, cancellationToken);
    return Results.Accepted($"/api/queries/accounts/{id}", new { accountId = id });
});

commands.MapPost("/{accountId:guid}/deposit", async (Guid accountId, DepositFundsRequest request, AccountCommandService service, CancellationToken cancellationToken) =>
{
    await service.DepositAsync(accountId, request.Amount, cancellationToken);
    return Results.Accepted();
});

var queries = app.MapGroup("/api/queries/accounts").RequireAuthorization("cqrs.user");
queries.MapGet("/{accountId:guid}", (Guid accountId, AccountQueryService service) =>
{
    var account = service.GetAccount(accountId);
    return account is null ? Results.NotFound() : Results.Ok(account);
});

queries.MapGet("/{accountId:guid}/events", (Guid accountId, IEventStore eventStore) =>
    Results.Ok(eventStore.GetEvents(accountId)));

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.Run();
