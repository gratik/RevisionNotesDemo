using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevisionNotes.DataAccess.AdvancedEfCore.Contracts;
using RevisionNotes.DataAccess.AdvancedEfCore.Data;
using RevisionNotes.DataAccess.AdvancedEfCore.Infrastructure;
using RevisionNotes.DataAccess.AdvancedEfCore.Security;

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
    options.AddPolicy("data.readwrite", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("advanced-efcore"));
builder.Services.AddScoped<ProductRepository>();

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

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapPost("/auth/token", (LoginRequest request, IOptions<JwtIssuerOptions> options, JwtTokenFactory tokenFactory, IConfiguration config) =>
{
    var expectedUser = config["DemoCredentials:Username"] ?? "data-admin";
    var expectedPassword = config["DemoCredentials:Password"] ?? "ChangeMe!123";

    if (!string.Equals(request.Username, expectedUser, StringComparison.Ordinal) ||
        !string.Equals(request.Password, expectedPassword, StringComparison.Ordinal))
    {
        return Results.Unauthorized();
    }

    var token = tokenFactory.CreateToken(options.Value, request.Username);
    return Results.Ok(new { access_token = token, token_type = "Bearer" });
});

var products = app.MapGroup("/api/products")
    .RequireAuthorization("data.readwrite")
    .WithTags("Products");

products.MapGet("/", async (decimal? minPrice, ProductRepository repository, CancellationToken cancellationToken) =>
{
    var result = await repository.GetProductsAsync(minPrice ?? 0, cancellationToken);
    return Results.Ok(result);
});

products.MapGet("/{id:int}", async (int id, ProductRepository repository, CancellationToken cancellationToken) =>
{
    var result = await repository.GetProductByIdAsync(id, cancellationToken);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

products.MapPost("/", async (CreateProductRequest request, ProductRepository repository, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) || request.Price <= 0)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["name"] = ["Name is required."],
            ["price"] = ["Price must be greater than zero."]
        });
    }

    var created = await repository.CreateAsync(request, cancellationToken);
    return Results.Created($"/api/products/{created.Id}", created);
});

products.MapPut("/{id:int}", async (int id, UpdateProductRequest request, ProductRepository repository, CancellationToken cancellationToken) =>
{
    var updated = await repository.UpdateAsync(id, request, cancellationToken);
    return updated switch
    {
        UpdateResult.NotFound => Results.NotFound(),
        UpdateResult.Conflict => Results.Conflict(new { code = "concurrency_conflict", message = "Version mismatch." }),
        UpdateResult.Success => Results.NoContent(),
        _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
    };
});

products.MapDelete("/{id:int}", async (int id, ProductRepository repository, CancellationToken cancellationToken) =>
{
    var deleted = await repository.SoftDeleteAsync(id, cancellationToken);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db);
}

app.Run();
