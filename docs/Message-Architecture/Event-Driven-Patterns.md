# Event-Driven Patterns

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


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

