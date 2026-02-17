# Background Workers in .NET

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
