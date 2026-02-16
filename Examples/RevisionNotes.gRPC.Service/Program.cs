using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevisionNotes.gRPC.Service.Infrastructure;
using RevisionNotes.gRPC.Service.Security;
using RevisionNotes.gRPC.Service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtIssuerOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<JwtTokenFactory>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IInventoryRepository, InMemoryInventoryRepository>();

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
    options.AddPolicy("grpc.read", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<GrpcExceptionInterceptor>();
});
builder.Services.AddSingleton<GrpcExceptionInterceptor>();

builder.Services.AddHealthChecks()
    .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck("ready", () => HealthCheckResult.Healthy("gRPC service ready"), tags: ["ready"]);

var app = builder.Build();

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

    app.Logger.LogInformation("HTTP {Method} {Path} -> {StatusCode} in {ElapsedMs}ms", context.Request.Method, context.Request.Path, context.Response.StatusCode, sw.ElapsedMilliseconds);
});

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/auth/token", (LoginRequest request, IOptions<JwtIssuerOptions> options, JwtTokenFactory tokenFactory, IConfiguration config) =>
{
    var expectedUser = config["DemoCredentials:Username"] ?? "demo";
    var expectedPassword = config["DemoCredentials:Password"] ?? "ChangeMe!123";

    if (!string.Equals(request.Username, expectedUser, StringComparison.Ordinal) ||
        !string.Equals(request.Password, expectedPassword, StringComparison.Ordinal))
    {
        return Results.Unauthorized();
    }

    var token = tokenFactory.CreateToken(options.Value, request.Username);
    return Results.Ok(new { access_token = token, token_type = "Bearer" });
});

app.MapGrpcService<InventoryGrpcService>();

app.MapGet("/", () => "Use a gRPC client. Get token from /auth/token.");
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.Run();
