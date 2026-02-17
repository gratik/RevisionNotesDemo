# Event-Driven Patterns

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Distributed systems basics, queues/topics, and eventual consistency concepts.
- Related examples: docs/Message-Architecture/README.md
> Subject: [Message-Architecture](../README.md)

## Event-Driven Patterns

### Domain Events

```csharp
// Event definition
public record OrderCreatedEvent(int OrderId, string CustomerId, decimal TotalAmount);

// Event bus
public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent eventData) where TEvent : class;
}

public class InMemoryEventBus : IEventBus
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryEventBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync<TEvent>(TEvent eventData) where TEvent : class
    {
        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>();

        foreach (var handler in handlers)
        {
            await handler.HandleAsync(eventData);
        }
    }
}

// Event handler interface
public interface IEventHandler<TEvent> where TEvent : class
{
    Task HandleAsync(TEvent eventData);
}

// Event handlers
public class SendEmailOnOrderCreated : IEventHandler<OrderCreatedEvent>
{
    private readonly IEmailService _emailService;

    public SendEmailOnOrderCreated(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task HandleAsync(OrderCreatedEvent eventData)
    {
        await _emailService.SendOrderConfirmationAsync(eventData.CustomerId, eventData.OrderId);
    }
}

public class UpdateInventoryOnOrderCreated : IEventHandler<OrderCreatedEvent>
{
    private readonly IInventoryService _inventoryService;

    public UpdateInventoryOnOrderCreated(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task HandleAsync(OrderCreatedEvent eventData)
    {
        await _inventoryService.ReserveStockAsync(eventData.OrderId);
    }
}

// Usage
public class OrderService
{
    private readonly IEventBus _eventBus;

    public OrderService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task CreateOrderAsync(CreateOrderRequest request)
    {
        // Create order...
        var order = new Order { Id = 123, CustomerId = "user1", TotalAmount = 99.99m };

        // ✅ Publish event
        await _eventBus.PublishAsync(new OrderCreatedEvent(order.Id, order.CustomerId, order.TotalAmount));
    }
}

// Register in Program.cs
builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();
builder.Services.AddScoped<IEventHandler<OrderCreatedEvent>, SendEmailOnOrderCreated>();
builder.Services.AddScoped<IEventHandler<OrderCreatedEvent>, UpdateInventoryOnOrderCreated>();
```

---


## Interview Answer Block
30-second answer:
- Event-Driven Patterns is about asynchronous messaging and event-driven coordination. It matters because it improves decoupling and throughput in distributed systems.
- Use it when reliable integration between independently evolving services.

2-minute answer:
- Start with the problem Event-Driven Patterns solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: scalability and decoupling vs operational complexity.
- Close with one failure mode and mitigation: underestimating retries, ordering, and idempotency concerns.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Event-Driven Patterns but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Event-Driven Patterns, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Event-Driven Patterns and map it to one concrete implementation in this module.
- 3 minutes: compare Event-Driven Patterns with an alternative, then walk through one failure mode and mitigation.