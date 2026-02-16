using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RevisionNotes.Observability.Showcase.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddSingleton<RequestTelemetry>();

builder.Services.AddHealthChecks()
    .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<ReadinessHealthCheck>("readiness", tags: ["ready"]);

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
    var telemetry = context.RequestServices.GetRequiredService<RequestTelemetry>();
    using var activity = telemetry.ActivitySource.StartActivity("http.request", ActivityKind.Server);

    var sw = Stopwatch.StartNew();
    await next();
    sw.Stop();

    telemetry.RequestCounter.Add(1,
        new KeyValuePair<string, object?>("method", context.Request.Method),
        new KeyValuePair<string, object?>("path", context.Request.Path.Value ?? string.Empty),
        new KeyValuePair<string, object?>("status", context.Response.StatusCode));

    telemetry.RequestDurationMs.Record(sw.Elapsed.TotalMilliseconds,
        new KeyValuePair<string, object?>("path", context.Request.Path.Value ?? string.Empty));

    app.Logger.LogInformation("HTTP {Method} {Path} -> {StatusCode} in {ElapsedMs}ms", context.Request.Method, context.Request.Path, context.Response.StatusCode, sw.ElapsedMilliseconds);
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.MapGet("/observability/metrics/snapshot", (RequestTelemetry telemetry) => Results.Ok(new
{
    TotalRequests = telemetry.TotalRequests,
    ErrorRequests = telemetry.ErrorRequests,
    LastUpdatedUtc = telemetry.LastUpdatedUtc
}));

app.Run();