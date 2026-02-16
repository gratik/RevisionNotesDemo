using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevisionNotes.Identity.AuthServer.Contracts;
using RevisionNotes.Identity.AuthServer.Infrastructure;
using RevisionNotes.Identity.AuthServer.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtIssuerOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<JwtTokenFactory>();
builder.Services.AddSingleton<IRefreshTokenStore, InMemoryRefreshTokenStore>();

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

builder.Services.AddAuthorization();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks()
    .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<IdentityStoreHealthCheck>("token-store", tags: ["ready"]);
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

app.MapPost("/connect/token", (
    PasswordGrantRequest request,
    IOptions<JwtIssuerOptions> options,
    JwtTokenFactory tokenFactory,
    IRefreshTokenStore refreshTokenStore,
    IConfiguration config) =>
{
    if (!string.Equals(request.GrantType, "password", StringComparison.Ordinal))
    {
        return Results.BadRequest(new { error = "unsupported_grant_type" });
    }

    var expectedUser = config["DemoCredentials:Username"] ?? "identity-admin";
    var expectedPassword = config["DemoCredentials:Password"] ?? "ChangeMe!123";

    if (!string.Equals(request.Username, expectedUser, StringComparison.Ordinal) ||
        !string.Equals(request.Password, expectedPassword, StringComparison.Ordinal))
    {
        return Results.Unauthorized();
    }

    var accessToken = tokenFactory.CreateToken(options.Value, request.Username);
    var refreshToken = refreshTokenStore.Create(request.Username);

    return Results.Ok(new TokenResponse(
        AccessToken: accessToken,
        TokenType: "Bearer",
        ExpiresInSeconds: options.Value.ExpiresMinutes * 60,
        RefreshToken: refreshToken.Token));
});

app.MapPost("/connect/refresh", (
    RefreshGrantRequest request,
    IOptions<JwtIssuerOptions> options,
    JwtTokenFactory tokenFactory,
    IRefreshTokenStore refreshTokenStore) =>
{
    if (!string.Equals(request.GrantType, "refresh_token", StringComparison.Ordinal))
    {
        return Results.BadRequest(new { error = "unsupported_grant_type" });
    }

    var principal = refreshTokenStore.TryConsume(request.RefreshToken);
    if (principal is null)
    {
        return Results.Unauthorized();
    }

    var accessToken = tokenFactory.CreateToken(options.Value, principal);
    var nextRefresh = refreshTokenStore.Create(principal);
    return Results.Ok(new TokenResponse(accessToken, "Bearer", options.Value.ExpiresMinutes * 60, nextRefresh.Token));
});

app.MapGet("/api/profile", (HttpContext httpContext) =>
{
    var userName = httpContext.User.Identity?.Name ?? "unknown";
    return Results.Ok(new { sub = userName, issuedAt = DateTimeOffset.UtcNow });
})
.RequireAuthorization();

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.Run();
