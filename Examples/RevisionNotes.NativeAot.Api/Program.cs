using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RevisionNotes.NativeAot.Api.Infrastructure;
using RevisionNotes.NativeAot.Api.Models;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.Configure<ApiSecurityOptions>(builder.Configuration.GetSection("ApiSecurity"));
builder.Services.AddSingleton<IProductStore, InMemoryProductStore>();
builder.Services.AddTransient<ApiKeyAuthMiddleware>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks()
    .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<StoreHealthCheck>("store", tags: ["ready"]);
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseMiddleware<ApiKeyAuthMiddleware>();
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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => Results.Ok(new { name = "RevisionNotes.NativeAot.Api", mode = "AOT-friendly" }));

app.MapGet("/api/products", (IProductStore store) => Results.Ok(store.GetAll()));

app.MapPost("/api/products", (CreateProductRequest request, IProductStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Name) || request.Price <= 0)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["name"] = ["Name is required."],
            ["price"] = ["Price must be greater than zero."]
        });
    }

    var created = store.Add(request.Name, request.Price);
    return Results.Created($"/api/products/{created.Id}", created);
});

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.Run();
