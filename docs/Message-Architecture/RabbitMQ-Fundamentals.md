# RabbitMQ Fundamentals

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Distributed systems basics, queues/topics, and eventual consistency concepts.
- Related examples: docs/Message-Architecture/README.md
> Subject: [Message-Architecture](../README.md)

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


## Interview Answer Block
30-second answer:
- RabbitMQ Fundamentals is about asynchronous messaging and event-driven coordination. It matters because it improves decoupling and throughput in distributed systems.
- Use it when reliable integration between independently evolving services.

2-minute answer:
- Start with the problem RabbitMQ Fundamentals solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: scalability and decoupling vs operational complexity.
- Close with one failure mode and mitigation: underestimating retries, ordering, and idempotency concerns.
## Interview Bad vs Strong Answer
Bad answer:
- Defines RabbitMQ Fundamentals but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose RabbitMQ Fundamentals, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define RabbitMQ Fundamentals and map it to one concrete implementation in this module.
- 3 minutes: compare RabbitMQ Fundamentals with an alternative, then walk through one failure mode and mitigation.