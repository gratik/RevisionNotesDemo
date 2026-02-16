using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevisionNotes.Testing.Pyramid.Contracts;
using RevisionNotes.Testing.Pyramid.Domain;
using RevisionNotes.Testing.Pyramid.Infrastructure;
using RevisionNotes.Testing.Pyramid.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtIssuerOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<JwtTokenFactory>();
builder.Services.AddSingleton<IOrderReadStore, InMemoryOrderReadStore>();
builder.Services.AddScoped<OrderScoringService>();

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
    options.AddPolicy("contracts.read", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks()
    .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<StoreHealthCheck>("store", tags: ["ready"]);
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

app.MapPost("/auth/token", (LoginRequest request, IOptions<JwtIssuerOptions> options, JwtTokenFactory tokenFactory, IConfiguration configuration) =>
{
    var expectedUser = configuration["DemoCredentials:Username"] ?? "tester";
    var expectedPassword = configuration["DemoCredentials:Password"] ?? "ChangeMe!123";

    if (!string.Equals(request.Username, expectedUser, StringComparison.Ordinal) ||
        !string.Equals(request.Password, expectedPassword, StringComparison.Ordinal))
    {
        return Results.Unauthorized();
    }

    var token = tokenFactory.CreateToken(options.Value, request.Username);
    return Results.Ok(new { access_token = token, token_type = "Bearer" });
});

// Stable contract endpoint intended for contract tests.
app.MapGet("/contract/v1/orders/{id:int}", async (int id, IOrderReadStore store, CancellationToken cancellationToken) =>
{
    var order = await store.GetByIdAsync(id, cancellationToken);
    return order is null
        ? Results.NotFound(new { code = "order_not_found", message = "Order was not found." })
        : Results.Ok(new OrderContractResponse(order.Id, order.CustomerId, order.TotalAmount, order.Status));
})
.RequireAuthorization("contracts.read")
.WithName("GetOrderContractV1");

app.MapPost("/unit/score-order", (OrderScoreRequest request, OrderScoringService service) =>
{
    var score = service.CalculateRiskScore(request.TotalAmount, request.IsPriorityCustomer, request.HasFraudSignals);
    return Results.Ok(new { riskScore = score });
});

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.Run();
