using Microsoft.Extensions.Diagnostics.HealthChecks;
using RevisionNotes.BackgroundJobs.Infrastructure;

namespace RevisionNotes.BackgroundJobs.Jobs;

public sealed record BackgroundJob(string JobId, string JobType, string Payload, int Attempt, DateTimeOffset CreatedAtUtc);

public sealed class DemoJobProducerService(
    IBackgroundJobQueue jobQueue,
    ILogger<DemoJobProducerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var random = new Random();

        while (!stoppingToken.IsCancellationRequested)
        {
            var id = Guid.NewGuid().ToString("N");
            var amount = random.Next(50, 600);
            var job = new BackgroundJob(
                JobId: id,
                JobType: "GenerateInvoicePdf",
                Payload: $"{{\"invoiceId\":\"{id}\",\"amount\":{amount}}}",
                Attempt: 0,
                CreatedAtUtc: DateTimeOffset.UtcNow);

            await jobQueue.EnqueueAsync(job, stoppingToken);
            logger.LogInformation("Enqueued job {JobId} ({JobType})", job.JobId, job.JobType);

            if (random.Next(0, 8) == 0)
            {
                await jobQueue.EnqueueAsync(job, stoppingToken);
                logger.LogInformation("Enqueued duplicate job {JobId}", job.JobId);
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}

public sealed class JobProcessorService(
    IBackgroundJobQueue jobQueue,
    IProcessedJobStore processedStore,
    JobProcessingState state,
    ILogger<JobProcessorService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var job = await jobQueue.DequeueAsync(stoppingToken);

            if (!processedStore.TryStart(job.JobId))
            {
                logger.LogWarning("Skipped duplicate/in-flight job {JobId}", job.JobId);
                continue;
            }

            try
            {
                await ProcessAsync(job, stoppingToken);
                processedStore.MarkCompleted(job.JobId);
                state.RecordSuccess();
                logger.LogInformation("Completed job {JobId} on attempt {Attempt}", job.JobId, job.Attempt + 1);
            }
            catch (Exception ex)
            {
                state.RecordFailure();
                logger.LogError(ex, "Failed job {JobId} attempt {Attempt}", job.JobId, job.Attempt + 1);

                if (job.Attempt < 2)
                {
                    var retry = job with { Attempt = job.Attempt + 1 };
                    await Task.Delay(TimeSpan.FromSeconds(1 + job.Attempt), stoppingToken);
                    await jobQueue.EnqueueAsync(retry, stoppingToken);
                    logger.LogWarning("Requeued job {JobId} for attempt {Attempt}", retry.JobId, retry.Attempt + 1);
                }
            }
        }
    }

    private static Task ProcessAsync(BackgroundJob job, CancellationToken cancellationToken)
    {
        if (job.Attempt == 0 && job.JobId.EndsWith("f", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Simulated transient failure.");
        }

        return Task.Delay(TimeSpan.FromMilliseconds(120), cancellationToken);
    }
}

public sealed class BackgroundJobsHealthCheck(
    IBackgroundJobQueue queue,
    JobProcessingState state) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (queue.Depth > 150)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"Backlog too high: {queue.Depth}"));
        }

        return Task.FromResult(HealthCheckResult.Healthy($"QueueDepth={queue.Depth}; Processed={state.ProcessedCount}; Failures={state.FailureCount}"));
    }
}

public sealed class HealthReporterService(
    HealthCheckService healthCheckService,
    ILogger<HealthReporterService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var report = await healthCheckService.CheckHealthAsync(_ => true, stoppingToken);
            logger.LogInformation("Health status: {Status}", report.Status);
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }
}
