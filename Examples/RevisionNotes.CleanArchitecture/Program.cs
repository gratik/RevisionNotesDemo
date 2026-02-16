using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevisionNotes.CleanArchitecture.Application.Orders;
using RevisionNotes.CleanArchitecture.Contracts;
using RevisionNotes.CleanArchitecture.Infrastructure;
using RevisionNotes.CleanArchitecture.Infrastructure.Orders;
using RevisionNotes.CleanArchitecture.Security;

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
    options.AddPolicy("orders.readwrite", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddScoped<OrderService>();

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
    var expectedUser = config["DemoCredentials:Username"] ?? "architect";
    var expectedPassword = config["DemoCredentials:Password"] ?? "ChangeMe!123";

    if (!string.Equals(request.Username, expectedUser, StringComparison.Ordinal) ||
        !string.Equals(request.Password, expectedPassword, StringComparison.Ordinal))
    {
        return Results.Unauthorized();
    }

    var token = tokenFactory.CreateToken(options.Value, request.Username);
    return Results.Ok(new { access_token = token, token_type = "Bearer" });
});

var orders = app.MapGroup("/api/orders")
    .RequireAuthorization("orders.readwrite")
    .WithTags("Orders");

orders.MapGet("/", async (OrderService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetOrdersAsync(cancellationToken)));

orders.MapGet("/{id:guid}", async (Guid id, OrderService service, CancellationToken cancellationToken) =>
{
    var item = await service.GetOrderAsync(id, cancellationToken);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

orders.MapPost("/", async (CreateOrderRequest request, OrderService service, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.CustomerId) || request.TotalAmount <= 0)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["customerId"] = ["CustomerId is required."],
            ["totalAmount"] = ["TotalAmount must be greater than zero."]
        });
    }

    var created = await service.CreateOrderAsync(new CreateOrderCommand(request.CustomerId, request.TotalAmount), cancellationToken);
    return Results.Created($"/api/orders/{created.Id}", created);
});

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.Run();
