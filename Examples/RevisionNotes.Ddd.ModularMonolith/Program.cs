using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RevisionNotes.Ddd.ModularMonolith.Contracts;
using RevisionNotes.Ddd.ModularMonolith.Infrastructure;
using RevisionNotes.Ddd.ModularMonolith.Modules.Billing;
using RevisionNotes.Ddd.ModularMonolith.Modules.Catalog;
using RevisionNotes.Ddd.ModularMonolith.Modules.Shared;
using RevisionNotes.Ddd.ModularMonolith.Security;

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
    options.AddPolicy("modules.readwrite", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddSingleton<IModularEventBus, InMemoryModularEventBus>();
builder.Services.AddSingleton<ICatalogRepository, InMemoryCatalogRepository>();
builder.Services.AddSingleton<IInvoiceRepository, InMemoryInvoiceRepository>();
builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<BillingService>();
builder.Services.AddScoped<CatalogToBillingProjection>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks()
    .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<ModularMonolithHealthCheck>("modules", tags: ["ready"]);
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var bus = scope.ServiceProvider.GetRequiredService<IModularEventBus>();
    var projection = scope.ServiceProvider.GetRequiredService<CatalogToBillingProjection>();
    projection.Register(bus);
}

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
    var expectedUser = config["DemoCredentials:Username"] ?? "modular-admin";
    var expectedPassword = config["DemoCredentials:Password"] ?? "ChangeMe!123";

    if (!string.Equals(request.Username, expectedUser, StringComparison.Ordinal) ||
        !string.Equals(request.Password, expectedPassword, StringComparison.Ordinal))
    {
        return Results.Unauthorized();
    }

    var token = tokenFactory.CreateToken(options.Value, request.Username);
    return Results.Ok(new { access_token = token, token_type = "Bearer" });
});

var catalog = app.MapGroup("/api/catalog")
    .RequireAuthorization("modules.readwrite")
    .WithTags("Catalog");

catalog.MapGet("/", async (CatalogService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetItemsAsync(cancellationToken)));

catalog.MapPost("/", async (CreateCatalogItemRequest request, CatalogService service, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) || request.Price <= 0)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["name"] = ["Name is required."],
            ["price"] = ["Price must be greater than zero."]
        });
    }

    var item = await service.CreateItemAsync(request.Name, request.Price, cancellationToken);
    return Results.Created($"/api/catalog/{item.Id}", item);
});

var billing = app.MapGroup("/api/billing")
    .RequireAuthorization("modules.readwrite")
    .WithTags("Billing");

billing.MapGet("/invoices", async (BillingService service, CancellationToken cancellationToken) =>
    Results.Ok(await service.GetInvoicesAsync(cancellationToken)));

billing.MapPost("/invoices", async (CreateInvoiceRequest request, BillingService service, CancellationToken cancellationToken) =>
{
    if (request.Amount <= 0 || string.IsNullOrWhiteSpace(request.Reference))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["amount"] = ["Amount must be greater than zero."],
            ["reference"] = ["Reference is required."]
        });
    }

    var invoice = await service.CreateInvoiceAsync(request.Reference, request.Amount, cancellationToken);
    return Results.Created($"/api/billing/invoices/{invoice.Id}", invoice);
});

app.MapGet("/api/events", (IModularEventBus bus) => Results.Ok(bus.GetPublishedEvents()))
    .RequireAuthorization("modules.readwrite")
    .WithTags("Events");

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.Run();
