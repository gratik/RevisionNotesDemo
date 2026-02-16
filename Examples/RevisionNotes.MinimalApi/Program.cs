using System.Diagnostics;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevisionNotes.MinimalApi.Features.Todos;
using RevisionNotes.MinimalApi.Infrastructure;
using RevisionNotes.MinimalApi.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtIssuerOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<JwtTokenFactory>();

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
    options.AddPolicy("api.readwrite", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("minimal-api-db"));
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ICachedTodoQueryService, CachedTodoQueryService>();

builder.Services.AddMemoryCache();
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("todos-list", policy => policy.Expire(TimeSpan.FromSeconds(30)).Tag("todos"));
});

builder.Services.AddResponseCompression();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("write-policy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks()
    .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<DatabaseHealthCheck>("database", tags: ["ready"]);
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

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Content-Security-Policy"] = "default-src 'none'; frame-ancestors 'none';";
    await next();
});

app.UseResponseCompression();
app.UseRateLimiter();
app.UseOutputCache();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/auth/token", ([FromBody] LoginRequest request, IOptions<JwtIssuerOptions> jwtOptions, JwtTokenFactory tokenFactory, IConfiguration configuration) =>
{
    var expectedUser = configuration["DemoCredentials:Username"] ?? "demo";
    var expectedPassword = configuration["DemoCredentials:Password"] ?? "ChangeMe!123";

    if (!string.Equals(request.Username, expectedUser, StringComparison.Ordinal) ||
        !string.Equals(request.Password, expectedPassword, StringComparison.Ordinal))
    {
        return Results.Unauthorized();
    }

    var token = tokenFactory.CreateToken(jwtOptions.Value, request.Username);
    return Results.Ok(new { access_token = token, token_type = "Bearer" });
});

var todos = app.MapGroup("/api/todos")
    .RequireAuthorization("api.readwrite")
    .WithTags("Todos");

todos.MapGet("/", async (ICachedTodoQueryService queryService, CancellationToken cancellationToken) =>
    Results.Ok(await queryService.GetAllAsync(cancellationToken)))
    .CacheOutput("todos-list");

todos.MapGet("/{id:int}", async (int id, ITodoRepository repository, CancellationToken cancellationToken) =>
{
    var item = await repository.GetByIdAsync(id, cancellationToken);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

todos.MapPost("/", async (
    CreateTodoRequest request,
    ITodoRepository repository,
    ICachedTodoQueryService cache,
    IOutputCacheStore outputCacheStore,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]> { ["title"] = ["Title is required."] });
    }

    var todo = await repository.CreateAsync(request, cancellationToken);
    cache.Invalidate();
    await outputCacheStore.EvictByTagAsync("todos", cancellationToken);

    return Results.Created($"/api/todos/{todo.Id}", todo);
})
.RequireRateLimiting("write-policy");

todos.MapPut("/{id:int}", async (
    int id,
    UpdateTodoRequest request,
    ITodoRepository repository,
    ICachedTodoQueryService cache,
    IOutputCacheStore outputCacheStore,
    CancellationToken cancellationToken) =>
{
    var updated = await repository.UpdateAsync(id, request, cancellationToken);
    if (updated is null)
    {
        return Results.NotFound();
    }

    cache.Invalidate();
    await outputCacheStore.EvictByTagAsync("todos", cancellationToken);

    return Results.Ok(updated);
})
.RequireRateLimiting("write-policy");

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db);
    app.Logger.LogInformation("Seeded minimal API demo data");
}

app.Run();