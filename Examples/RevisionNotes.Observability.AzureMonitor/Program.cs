using System.Diagnostics;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.ApplicationInsights.Extensibility;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RevisionNotes.Observability.AzureMonitor.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddHttpClient("downstream", client =>
{
    var baseUrl = builder.Configuration["Downstream:BaseUrl"];
    if (!string.IsNullOrWhiteSpace(baseUrl))
    {
        client.BaseAddress = new Uri(baseUrl);
    }
});

builder.Services.AddSingleton<ServiceBusCorrelationPublisher>();
builder.Services.AddSingleton<ServiceBusCorrelationConsumerDemo>();

builder.Services.AddApplicationInsightsTelemetry();

var connectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
var useConsoleExporter = builder.Configuration.GetValue("Observability:UseConsoleExporter", true);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(
        serviceName: "RevisionNotes.Observability.AzureMonitor",
        serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString()))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
                options.EnrichWithHttpRequest = (activity, request) =>
                {
                    if (request.Headers.TryGetValue(CorrelationIdMiddleware.HeaderName, out var correlationId))
                    {
                        activity.SetTag("app.correlation_id", correlationId.ToString());
                    }
                };
            })
            .AddHttpClientInstrumentation();

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            tracing.AddAzureMonitorTraceExporter(options =>
            {
                options.ConnectionString = connectionString;
            });
        }

        if (useConsoleExporter)
        {
            tracing.AddConsoleExporter();
        }
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            metrics.AddAzureMonitorMetricExporter(options =>
            {
                options.ConnectionString = connectionString;
            });
        }

        if (useConsoleExporter)
        {
            metrics.AddConsoleExporter();
        }
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<CorrelationIdMiddleware>();

app.MapControllers();

app.MapGet("/observability/ping", () => Results.Ok(new
{
    status = "ok",
    traceId = Activity.Current?.TraceId.ToString(),
    spanId = Activity.Current?.SpanId.ToString()
}));

app.Run();
