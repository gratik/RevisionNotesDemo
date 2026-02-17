# Background Workers in .NET

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Message-Architecture](../README.md)

## Background Workers in .NET

Background workers run outside request/response paths using `IHostedService` or
`BackgroundService`, and are commonly used for queue processing and recurring jobs.

### Best fit

- Async processing triggered by queues/events
- Scheduled cleanup, reconciliation, and batch tasks
- Startup warmup and cache preloading tasks

### Key design points

- Respect cancellation for graceful shutdown
- Create scopes for scoped dependencies per iteration
- Apply retry/backoff for transient dependencies
- Emit metrics for throughput, failures, and queue delay

### Minimal worker sketch

```csharp
public class OrderWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IOrderProcessor>();
            await service.ProcessNextAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}
```

---

## Interview Answer Block
30-second answer:
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

