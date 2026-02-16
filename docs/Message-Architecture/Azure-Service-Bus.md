# Azure Service Bus

> Subject: [Message-Architecture](../README.md)

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


