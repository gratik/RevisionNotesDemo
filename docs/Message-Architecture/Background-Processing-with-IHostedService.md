# Background Processing with IHostedService

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Distributed systems basics, queues/topics, and eventual consistency concepts.
- Related examples: docs/Message-Architecture/README.md
> Subject: [Message-Architecture](../README.md)

## Background Processing with IHostedService

### Basic Background Worker

```csharp
public class OrderProcessingWorker : BackgroundService
{
    private readonly ILogger<OrderProcessingWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public OrderProcessingWorker(
        ILogger<OrderProcessingWorker> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Order Processing Worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // ✅ Create scope for scoped services
                using var scope = _serviceProvider.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

                await orderService.ProcessPendingOrdersAsync();

                // ✅ Wait before next iteration
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Order Processing Worker");

                // ✅ Wait before retry
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        _logger.LogInformation("Order Processing Worker stopped");
    }
}

// Register in Program.cs
builder.Services.AddHostedService<OrderProcessingWorker>();
```

### Queued Background Tasks

```csharp
public interface IBackgroundTaskQueue
{
    ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem);
    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

    public BackgroundTaskQueue(int capacity)
    {
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem)
    {
        await _queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}

public class QueuedHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger<QueuedHostedService> _logger;

    public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger)
    {
        _taskQueue = taskQueue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await _taskQueue.DequeueAsync(stoppingToken);

            try
            {
                await workItem(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing background task");
            }
        }
    }
}

// Usage in controller
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IBackgroundTaskQueue _taskQueue;

    public OrdersController(IBackgroundTaskQueue taskQueue)
    {
        _taskQueue = taskQueue;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        // ✅ Queue long-running task
        await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
        {
            await Task.Delay(5000, token);  // Simulate long operation
            Console.WriteLine($"Order processed: {request.OrderId}");
        });

        return Accepted();  // Return immediately
    }
}

// Register in Program.cs
builder.Services.AddSingleton<IBackgroundTaskQueue>(new BackgroundTaskQueue(100));
builder.Services.AddHostedService<QueuedHostedService>();
```

---


## Interview Answer Block
30-second answer:
- Background Processing with IHostedService is about asynchronous messaging and event-driven coordination. It matters because it improves decoupling and throughput in distributed systems.
- Use it when reliable integration between independently evolving services.

2-minute answer:
- Start with the problem Background Processing with IHostedService solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: scalability and decoupling vs operational complexity.
- Close with one failure mode and mitigation: underestimating retries, ordering, and idempotency concerns.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Background Processing with IHostedService but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Background Processing with IHostedService, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Background Processing with IHostedService and map it to one concrete implementation in this module.
- 3 minutes: compare Background Processing with IHostedService with an alternative, then walk through one failure mode and mitigation.