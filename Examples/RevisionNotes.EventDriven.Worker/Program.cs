using Microsoft.Extensions.Diagnostics.HealthChecks;
using RevisionNotes.EventDriven.Worker.Domain;
using RevisionNotes.EventDriven.Worker.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IEventQueue, InMemoryEventQueue>();
builder.Services.AddSingleton<IIdempotencyStore, InMemoryIdempotencyStore>();
builder.Services.AddSingleton<ProcessingState>();

builder.Services.AddHostedService<DemoEventProducerService>();
builder.Services.AddHostedService<EventProcessorService>();
builder.Services.AddHostedService<HealthReporterService>();

builder.Services.AddHealthChecks()
    .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<WorkerPipelineHealthCheck>("pipeline", tags: ["ready"]);

var app = builder.Build();
app.Run();