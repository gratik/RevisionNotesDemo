using Microsoft.Extensions.Diagnostics.HealthChecks;
using RevisionNotes.BackgroundJobs.Infrastructure;
using RevisionNotes.BackgroundJobs.Jobs;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IBackgroundJobQueue, InMemoryBackgroundJobQueue>();
builder.Services.AddSingleton<IProcessedJobStore, InMemoryProcessedJobStore>();
builder.Services.AddSingleton<JobProcessingState>();
builder.Services.AddHostedService<DemoJobProducerService>();
builder.Services.AddHostedService<JobProcessorService>();
builder.Services.AddHostedService<HealthReporterService>();

builder.Services.AddHealthChecks()
    .AddCheck("live", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck<BackgroundJobsHealthCheck>("jobs", tags: ["ready"]);

var host = builder.Build();
host.Run();
