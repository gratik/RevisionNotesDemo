using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevisionNotes.MultiTenant.SaaS.Contracts;
using RevisionNotes.MultiTenant.SaaS.Infrastructure;
using RevisionNotes.MultiTenant.SaaS.Security;
using RevisionNotes.MultiTenant.SaaS.Tenants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtIssuerOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<TenantCatalogOptions>(builder.Configuration.GetSection("Tenants"));
builder.Services.AddSingleton<JwtTokenFactory>();
builder.Services.AddSingleton<ITenantProjectStore, InMemoryTenantProjectStore>();
builder.Services.AddTransient<TenantResolutionMiddleware>();

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
    options.AddPolicy("tenant.user", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks()
    .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<TenantConfigurationHealthCheck>("tenant-config", tags: ["ready"]);
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseMiddleware<TenantResolutionMiddleware>();

app.Use(async (context, next) =>
{
    var sw = Stopwatch.StartNew();
    await next();
    sw.Stop();
    app.Logger.LogInformation(
        "HTTP {Method} {Path} tenant={Tenant} -> {StatusCode} in {ElapsedMs}ms",
        context.Request.Method,
        context.Request.Path,
        context.Items[TenantResolutionMiddleware.TenantItemKey] ?? "n/a",
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
    var expectedUser = config["DemoCredentials:Username"] ?? "tenant-admin";
    var expectedPassword = config["DemoCredentials:Password"] ?? "ChangeMe!123";

    if (!string.Equals(request.Username, expectedUser, StringComparison.Ordinal) ||
        !string.Equals(request.Password, expectedPassword, StringComparison.Ordinal))
    {
        return Results.Unauthorized();
    }

    var token = tokenFactory.CreateToken(options.Value, request.Username);
    return Results.Ok(new { access_token = token, token_type = "Bearer" });
});

app.MapGet("/api/tenant/info", (HttpContext context) =>
{
    var tenantId = context.GetTenantId();
    return Results.Ok(new { tenantId });
})
.RequireAuthorization("tenant.user");

app.MapGet("/api/tenant/projects", async (HttpContext context, ITenantProjectStore store, CancellationToken cancellationToken) =>
{
    var tenantId = context.GetTenantId();
    return Results.Ok(await store.GetProjectsAsync(tenantId, cancellationToken));
})
.RequireAuthorization("tenant.user");

app.MapPost("/api/tenant/projects", async (HttpContext context, CreateTenantProjectRequest request, ITenantProjectStore store, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]> { ["name"] = ["Project name is required."] });
    }

    var tenantId = context.GetTenantId();
    var item = await store.AddProjectAsync(tenantId, request.Name, cancellationToken);
    return Results.Created($"/api/tenant/projects/{item.Id}", item);
})
.RequireAuthorization("tenant.user");

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.Run();
