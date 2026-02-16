using Microsoft.Extensions.Diagnostics.HealthChecks;
using RevisionNotes.EventDriven.Worker.Domain;

namespace RevisionNotes.EventDriven.Worker.Infrastructure;

public sealed class DemoEventProducerService(
    IEventQueue eventQueue,
    ILogger<DemoEventProducerService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var random = new Random();

        while (!stoppingToken.IsCancellationRequested)
        {
            var aggregateId = random.Next(1000, 1006).ToString();
            var eventId = Guid.NewGuid().ToString("N");
            var created = DateTimeOffset.UtcNow;

            var envelope = new EventEnvelope(
                EventId: eventId,
                EventType: "OrderStatusChanged",
                AggregateId: aggregateId,
                CreatedAtUtc: created,
                Attempt: 0,
                Payload: $"{{\"aggregateId\":\"{aggregateId}\",\"status\":\"Updated\"}}");

            await eventQueue.PublishAsync(envelope, stoppingToken);
            logger.LogInformation("Published event {EventId} for aggregate {AggregateId}. QueueDepth={Depth}", envelope.EventId, envelope.AggregateId, eventQueue.Depth);

            // Intentional duplicate occasionally to prove idempotency handling.
            if (random.Next(0, 6) == 0)
            {
                await eventQueue.PublishAsync(envelope, stoppingToken);
                logger.LogInformation("Published duplicate event {EventId}", envelope.EventId);
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}

public sealed class EventProcessorService(
    IEventQueue eventQueue,
    IIdempotencyStore idempotencyStore,
    ProcessingState state,
    ILogger<EventProcessorService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var envelope = await eventQueue.DequeueAsync(stoppingToken);

            if (!idempotencyStore.TryStart(envelope.EventId))
            {
                logger.LogWarning("Skipped duplicate/in-flight event {EventId}", envelope.EventId);
                continue;
            }

            try
            {
                await HandleAsync(envelope, stoppingToken);
                idempotencyStore.MarkCompleted(envelope.EventId);
                state.RecordSuccess();

                logger.LogInformation(
                    "Processed event {EventId} ({EventType}) for aggregate {AggregateId} on attempt {Attempt}",
                    envelope.EventId,
                    envelope.EventType,
                    envelope.AggregateId,
                    envelope.Attempt + 1);
            }
            catch (Exception ex)
            {
                state.RecordFailure();
                logger.LogError(ex, "Failed processing event {EventId} (attempt {Attempt})", envelope.EventId, envelope.Attempt + 1);

                if (envelope.Attempt < 2)
                {
                    var retry = envelope with { Attempt = envelope.Attempt + 1 };
                    await Task.Delay(TimeSpan.FromSeconds(1 + envelope.Attempt), stoppingToken);
                    await eventQueue.PublishAsync(retry, stoppingToken);
                    logger.LogWarning("Requeued event {EventId} for retry {RetryAttempt}", envelope.EventId, retry.Attempt + 1);
                }
            }
        }
    }

    private static Task HandleAsync(EventEnvelope envelope, CancellationToken cancellationToken)
    {
        // Simulated transient failure pattern for retry demonstration.
        if (envelope.AggregateId.EndsWith("5") && envelope.Attempt == 0)
        {
            throw new InvalidOperationException("Simulated transient processing failure.");
        }

        return Task.Delay(TimeSpan.FromMilliseconds(120), cancellationToken);
    }
}

public sealed class WorkerPipelineHealthCheck(
    IEventQueue queue,
    ProcessingState state) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (queue.Depth > 200)
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

            foreach (var entry in report.Entries)
            {
                logger.LogInformation("Health {Name}: {Status} - {Description}", entry.Key, entry.Value.Status, entry.Value.Description ?? "n/a");
            }

            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }
}
