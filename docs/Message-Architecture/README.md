# Message-Based Architecture

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Asynchronous messaging basics
- Related examples: Learning/Microservices/EventDrivenArchitecture.cs, Learning/Microservices/ServiceCommunicationPatterns.cs


**Last Updated**: 2026-02-15

Comprehensive guide to building message-driven and event-driven systems with message queues,
pub/sub patterns, background processing, and distributed communication patterns.

## Module Metadata

- **Prerequisites**: Web API and MVC, Resilience
- **When to Study**: When moving from synchronous request/response to event-driven workflows.
- **Related Files**: `../Learning/Microservices/*.cs`, `../Learning/Architecture/*.cs`
- **Estimated Time**: 90-120 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](../Learning-Path.md) | [Track Start](../Design-Patterns.md)
- **Next Step**: [Deployment-DevOps.md](../Deployment-DevOps.md)
<!-- STUDY-NAV-END -->


---

## Why Message-Based Architecture?

**Traditional Request-Response Problems**:

- Tight coupling between services
- Synchronous = slow (wait for response)
- Failures cascade (service1 down → service2 fails)
- Hard to scale independently
- No retry mechanism

**Message-Based Solutions**:

- ✅ Loose coupling (services don't know about each other)
- ✅ Asynchronous processing (don't wait for response)
- ✅ Fault tolerance (retry failed messages)
- ✅ Independent scaling (scale consumers separately)
- ✅ Load leveling (process at your own pace)

---

## Core Concepts

### Message Queue vs Event Bus

| Feature       | Message Queue            | Event Bus (Pub/Sub)             |
| ------------- | ------------------------ | ------------------------------- |
| **Pattern**   | Point-to-point           | Publish-subscribe               |
| **Consumers** | One consumer per message | Multiple subscribers            |
| **Delivery**  | Exactly once             | At least once (all subscribers) |
| **Example**   | RabbitMQ queues          | Azure Service Bus Topics        |
| **Use Case**  | Task distribution        | Event notification              |

### Terms Explained

**Producer**: Service that sends messages
**Consumer**: Service that receives and processes messages
**Queue**: Storage for messages (FIFO)
**Exchange**: Routes messages to queues (RabbitMQ term)
**Topic**: Pub/sub channel (Azure Service Bus term)
**Subscription**: Consumer connection to topic
**Dead Letter Queue (DLQ)**: Failed messages storage

---

## When to Use Message Queues

### ✅ **Use When**

1. **Long-running operations**

   ```
   User uploads file → Queue → Background worker processes
   ```

2. **High traffic spikes**

   ```
   Black Friday orders → Queue → Process at sustainable rate
   ```

3. **Decoupling services**

   ```
   OrderService → Queue → EmailService, InventoryService, ShippingService
   ```

4. **Retry logic needed**

   ```
   Payment fails → Retry 3 times → Move to DLQ if still failing
   ```

5. **Multiple consumers**
   ```
   Image upload → Queue → Thumbnail worker, Watermark worker, Analysis worker
   ```

### ❌ **Don't Use When**

1. **Need immediate response**

   ```
   User login → Need instant success/failure response
   ```

2. **Simple CRUD operations**

   ```
   GET /api/users → Just query database directly
   ```

3. **Real-time requirements**

   ```
   Chat messages → Use SignalR/WebSockets instead
   ```

4. **Low complexity, low scale**
   ```
   Small apps with < 100 users → Overkill
   ```

---

## RabbitMQ Fundamentals

### Core Components

```
Producer → Exchange → Queue → Consumer
              ↓
           Routing Key
```

**Exchange Types**:

- **Direct**: Route by exact routing key match
- **Fanout**: Broadcast to all queues
- **Topic**: Route by pattern match (e.g., `order.*`)
- **Headers**: Route by message headers

### Basic RabbitMQ Example

```csharp
// Install: Install-Package RabbitMQ.Client

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

// Connection setup
var factory = new ConnectionFactory
{
    HostName = "localhost",
    UserName = "guest",
    Password = "guest",
    Port = 5672
};

// --- PRODUCER ---
public class OrderProducer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public OrderProducer()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // ✅ Declare queue (idempotent - safe to call multiple times)
        _channel.QueueDeclare(
            queue: "orders",
            durable: true,      // Survives broker restart
            exclusive: false,   // Not limited to this connection
            autoDelete: false,  // Don't delete when no consumers
            arguments: null);
    }

    public async Task SendOrderAsync(Order order)
    {
        var message = JsonSerializer.Serialize(order);
        var body = Encoding.UTF8.GetBytes(message);

        // ✅ Persistent message (survives restart)
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(
            exchange: "",
            routingKey: "orders",
            basicProperties: properties,
            body: body);

        Console.WriteLine($"[Producer] Sent: {message}");
    }
}

// --- CONSUMER ---
public class OrderConsumer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public OrderConsumer()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: "orders",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        // ✅ Fair dispatch (take 1 message at a time)
        _channel.BasicQos(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false);
    }

    public void StartConsuming()
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                var order = JsonSerializer.Deserialize<Order>(message);
                Console.WriteLine($"[Consumer] Processing: {order?.Id}");

                // ✅ Process order
                ProcessOrder(order);

                // ✅ Acknowledge message (removes from queue)
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Consumer] Error: {ex.Message}");

                // ❌ Reject and requeue (or send to DLQ)
                _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        _channel.BasicConsume(
            queue: "orders",
            autoAck: false,  // ✅ Manual acknowledgment
            consumer: consumer);

        Console.WriteLine("[Consumer] Waiting for messages...");
    }

    private void ProcessOrder(Order? order)
    {
        // Business logic here
        Thread.Sleep(1000);  // Simulate work
    }
}

public record Order(int Id, string ProductName, decimal Amount);
```

### Publish-Subscribe Pattern (Fanout)

```csharp
// Setup: One exchange, multiple queues

// Producer
public class EventPublisher
{
    private readonly IModel _channel;

    public EventPublisher()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();

        // ✅ Declare fanout exchange
        _channel.ExchangeDeclare("order.events", ExchangeType.Fanout);
    }

    public void PublishOrderCreated(Order order)
    {
        var message = JsonSerializer.Serialize(order);
        var body = Encoding.UTF8.GetBytes(message);

        // ✅ Publish to exchange (not directly to queue)
        _channel.BasicPublish(
            exchange: "order.events",
            routingKey: "",  // Ignored for fanout
            basicProperties: null,
            body: body);

        Console.WriteLine($"[Publisher] Published: {message}");
    }
}

// Multiple consumers
public class EmailService
{
    public EmailService()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare("order.events", ExchangeType.Fanout);

        // ✅ Each service creates its own queue
        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queueName, "order.events", routingKey: "");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            Console.WriteLine($"[EmailService] Sending email for: {message}");
            channel.BasicAck(ea.DeliveryTag, false);
        };

        channel.BasicConsume(queueName, autoAck: false, consumer);
    }
}

public class InventoryService
{
    public InventoryService()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare("order.events", ExchangeType.Fanout);

        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(queueName, "order.events", routingKey: "");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            Console.WriteLine($"[InventoryService] Updating stock for: {message}");
            channel.BasicAck(ea.DeliveryTag, false);
        };

        channel.BasicConsume(queueName, autoAck: false, consumer);
    }
}

// Result: One message → All services receive it
```

---

## Azure Service Bus

### Queue Example

```csharp
// Install: Install-Package Azure.Messaging.ServiceBus

using Azure.Messaging.ServiceBus;

// Connection string from Azure Portal
const string connectionString = "Endpoint=sb://myservicebus.servicebus.windows.net/;...";
const string queueName = "orders";

// --- SENDER ---
public class ServiceBusSender
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;

    public ServiceBusSender()
    {
        _client = new ServiceBusClient(connectionString);
        _sender = _client.CreateSender(queueName);
    }

    public async Task SendMessageAsync(Order order)
    {
        var message = new ServiceBusMessage(JsonSerializer.Serialize(order))
        {
            ContentType = "application/json",
            MessageId = Guid.NewGuid().ToString(),
            Subject = "Order.Created"
        };

        // ✅ Custom properties
        message.ApplicationProperties.Add("Priority", "High");
        message.ApplicationProperties.Add("CustomerId", order.CustomerId);

        await _sender.SendMessageAsync(message);
        Console.WriteLine($"[Sender] Sent: {order.Id}");
    }

    public async Task SendBatchAsync(List<Order> orders)
    {
        // ✅ Batch for efficiency
        using var batch = await _sender.CreateMessageBatchAsync();

        foreach (var order in orders)
        {
            var message = new ServiceBusMessage(JsonSerializer.Serialize(order));

            if (!batch.TryAddMessage(message))
            {
                // Batch full - send and create new batch
                await _sender.SendMessagesAsync(batch);
                batch.Clear();
                batch.TryAddMessage(message);
            }
        }

        if (batch.Count > 0)
            await _sender.SendMessagesAsync(batch);
    }
}

// --- RECEIVER ---
public class ServiceBusReceiver : BackgroundService
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusProcessor _processor;

    public ServiceBusReceiver()
    {
        _client = new ServiceBusClient(connectionString);

        var options = new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 5,              // Process 5 messages concurrently
            AutoCompleteMessages = false,        // Manual completion
            MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5)
        };

        _processor = _client.CreateProcessor(queueName, options);

        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _processor.StartProcessingAsync(stoppingToken);

        // Keep running until cancellation
        await Task.Delay(Timeout.Infinite, stoppingToken);

        await _processor.StopProcessingAsync();
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        var order = JsonSerializer.Deserialize<Order>(body);

        Console.WriteLine($"[Receiver] Processing: {order?.Id}");

        try
        {
            // ✅ Business logic
            await ProcessOrderAsync(order);

            // ✅ Complete message (removes from queue)
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Receiver] Error: {ex.Message}");

            // ❌ Abandon message (will be retried)
            await args.AbandonMessageAsync(args.Message);
        }
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        Console.WriteLine($"[Receiver] Error: {args.Exception.Message}");
        return Task.CompletedTask;
    }

    private async Task ProcessOrderAsync(Order? order)
    {
        // Simulate processing
        await Task.Delay(1000);
    }
}
```

### Topic and Subscriptions (Pub/Sub)

```csharp
const string topicName = "order-events";

// --- PUBLISHER ---
public class TopicPublisher
{
    private readonly ServiceBusSender _sender;

    public TopicPublisher()
    {
        var client = new ServiceBusClient(connectionString);
        _sender = client.CreateSender(topicName);
    }

    public async Task PublishOrderCreatedAsync(Order order)
    {
        var message = new ServiceBusMessage(JsonSerializer.Serialize(order))
        {
            Subject = "Order.Created",
            ContentType = "application/json"
        };

        // ✅ Properties for filtering
        message.ApplicationProperties.Add("OrderStatus", "Created");
        message.ApplicationProperties.Add("TotalAmount", order.TotalAmount);

        await _sender.SendMessageAsync(message);
    }
}

// --- SUBSCRIBERS ---
// Subscription 1: Email service (all orders)
public class EmailSubscriber : BackgroundService
{
    private readonly ServiceBusProcessor _processor;

    public EmailSubscriber()
    {
        var client = new ServiceBusClient(connectionString);
        _processor = client.CreateProcessor(topicName, "email-subscription");
        _processor.ProcessMessageAsync += async args =>
        {
            var order = JsonSerializer.Deserialize<Order>(args.Message.Body.ToString());
            Console.WriteLine($"[Email] Sending email for order: {order?.Id}");
            await args.CompleteMessageAsync(args.Message);
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _processor.StartProcessingAsync(stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}

// Subscription 2: High-value orders only (filter)
// In Azure Portal, create subscription with filter:
// TotalAmount > 1000

public class VipOrderSubscriber : BackgroundService
{
    private readonly ServiceBusProcessor _processor;

    public VipOrderSubscriber()
    {
        var client = new ServiceBusClient(connectionString);
        _processor = client.CreateProcessor(topicName, "vip-subscription");
        _processor.ProcessMessageAsync += async args =>
        {
            var order = JsonSerializer.Deserialize<Order>(args.Message.Body.ToString());
            Console.WriteLine($"[VIP] Special handling for order: {order?.Id}");
            await args.CompleteMessageAsync(args.Message);
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _processor.StartProcessingAsync(stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
```

---

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

## Best Practices

### Message Design

- ✅ **Idempotent** - Safe to process multiple times
- ✅ **Self-contained** - Include all needed data
- ✅ **Versioned** - Support schema evolution
- ✅ **Small** - Only essential data
- ✅ **Immutable** - Don't modify after creation

### Error Handling

- ✅ **Retry with exponential backoff**
- ✅ **Dead letter queue** for failed messages
- ✅ **Logging** for troubleshooting
- ✅ **Alerts** for DLQ growth
- ✅ **Idempotency** to handle duplicates

### Performance

- ✅ **Batch sending** when possible
- ✅ **Concurrent consumers** for throughput
- ✅ **Message prefetching** (but not too much)
- ✅ **Connection pooling**
- ✅ **Monitor queue depth**

### Reliability

- ✅ **Persistent messages** (survive restart)
- ✅ **Manual acknowledgment** (not auto)
- ✅ **Transaction support** when needed
- ✅ **Message TTL** (time-to-live)
- ✅ **Duplicate detection**

---

## Common Pitfalls

### ❌ **Auto-Acknowledge Messages**

```csharp
// ❌ BAD: Message removed before processing
consumer.Received += (model, ea) =>
{
    ProcessMessage(ea.Body);  // If this throws, message is lost
};
channel.BasicConsume(queue, autoAck: true, consumer);  // ❌ Auto-ack

// ✅ GOOD: Manual acknowledgment after success
consumer.Received += (model, ea) =>
{
    try
    {
        ProcessMessage(ea.Body);
        channel.BasicAck(ea.DeliveryTag, false);  // ✅ Ack after success
    }
    catch (Exception ex)
    {
        channel.BasicNack(ea.DeliveryTag, false, requeue: true);  // ✅ Requeue on failure
    }
};
channel.BasicConsume(queue, autoAck: false, consumer);
```

### ❌ **No Retry Limit**

```csharp
// ❌ BAD: Infinite retry loop
catch (Exception ex)
{
    channel.BasicNack(ea.DeliveryTag, false, requeue: true);  // ❌ Always requeue
}

// ✅ GOOD: Limit retries, use DLQ
var retryCount = (int)(ea.BasicProperties.Headers?["x-retry-count"] ?? 0);

if (retryCount >= 3)
{
    // ✅ Move to dead letter queue
    channel.BasicNack(ea.DeliveryTag, false, requeue: false);
}
else
{
    // ✅ Increment retry count and requeue
    ea.BasicProperties.Headers["x-retry-count"] = retryCount + 1;
    channel.BasicNack(ea.DeliveryTag, false, requeue: true);
}
```

### ❌ **Not Idempotent**

```csharp
// ❌ BAD: Duplicate charges user twice
public async Task ProcessPayment(PaymentMessage message)
{
    await _paymentService.ChargeCustomerAsync(message.CustomerId, message.Amount);
}

// ✅ GOOD: Check if already processed
public async Task ProcessPayment(PaymentMessage message)
{
    if (await _paymentRepository.ExistsAsync(message.PaymentId))
    {
        return;  //✅ Already processed
    }

    await _paymentService.ChargeCustomerAsync(message.CustomerId, message.Amount);
    await _paymentRepository.SaveAsync(message.PaymentId);
}
```

---

## Related Files

- [RealTime/SignalRBasics.cs](../../Learning/RealTime/SignalRBasics.cs) - Real-time communication (alternative to polling)
- [Resilience/PollyRetryPatterns.cs](../../Learning/Resilience/PollyRetryPatterns.cs) - Retry logic for message processing

---

## See Also

- [Real-Time Communication](../RealTime.md) - SignalR for instant updates
- [Resilience](../Resilience.md) - Retry and circuit breaker patterns
- [Async and Multithreading](../Async-Multithreading.md) - Background task patterns
- [Logging and Observability](../Logging-Observability.md) - Troubleshooting distributed systems
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [Deployment-DevOps.md](../Deployment-DevOps.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: This topic covers Message Architecture and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know Message Architecture and I would just follow best practices."
- Strong answer: "For Message Architecture, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Message Architecture in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.

## Topic Files

- [Why Message-Based Architecture?](Why-Message-Based-Architecture.md)
- [Core Concepts](Core-Concepts.md)
- [When to Use Message Queues](When-to-Use-Message-Queues.md)
- [RabbitMQ Fundamentals](RabbitMQ-Fundamentals.md)
- [Azure Service Bus](Azure-Service-Bus.md)
- [Event Hubs](Event-Hubs.md)
- [Event Grid](Event-Grid.md)
- [Background Workers in .NET](Background-Workers-in-DotNet.md)
- [Background Processing with IHostedService](Background-Processing-with-IHostedService.md)
- [Event-Driven Patterns](Event-Driven-Patterns.md)
- [Contract-First Messaging](Contract-First-Messaging.md)
- [Failure-Recovery Testing](Failure-Recovery-Testing.md)
- [Best Practices](Best-Practices.md)
- [Common Pitfalls](Common-Pitfalls.md)





