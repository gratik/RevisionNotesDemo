using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevisionNotes.Resilience.ChaosDemo.Contracts;
using RevisionNotes.Resilience.ChaosDemo.Infrastructure;
using RevisionNotes.Resilience.ChaosDemo.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtIssuerOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<ChaosSettings>(builder.Configuration.GetSection("Chaos"));
builder.Services.AddSingleton<JwtTokenFactory>();
builder.Services.AddSingleton<ChaosState>();
builder.Services.AddSingleton<UnstableDependencyService>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ResilientValueService>();

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
    options.AddPolicy("resilience.user", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks()
    .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<ChaosHealthCheck>("chaos-state", tags: ["ready"]);
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
    var expectedUser = config["DemoCredentials:Username"] ?? "resilience-admin";
    var expectedPassword = config["DemoCredentials:Password"] ?? "ChangeMe!123";

    if (!string.Equals(request.Username, expectedUser, StringComparison.Ordinal) ||
        !string.Equals(request.Password, expectedPassword, StringComparison.Ordinal))
    {
        return Results.Unauthorized();
    }

    var token = tokenFactory.CreateToken(options.Value, request.Username);
    return Results.Ok(new { access_token = token, token_type = "Bearer" });
});

app.MapGet("/api/resilience/value", async (ResilientValueService service, CancellationToken cancellationToken) =>
{
    var response = await service.GetValueAsync(cancellationToken);
    return Results.Ok(response);
})
.RequireAuthorization("resilience.user");

app.MapPost("/api/chaos/config", (ChaosConfigRequest request, ChaosState state) =>
{
    state.Update(request.Enabled, request.FailureRatePercent, request.MaxDelayMs);
    return Results.Ok(new { state.Enabled, state.FailureRatePercent, state.MaxDelayMs });
})
.RequireAuthorization("resilience.user");

app.MapGet("/api/chaos/config", (ChaosState state) => Results.Ok(new
{
    state.Enabled,
    state.FailureRatePercent,
    state.MaxDelayMs,
    state.CircuitOpenUntilUtc
}))
.RequireAuthorization("resilience.user");

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.Run();
