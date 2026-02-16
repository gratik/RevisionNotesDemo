# Common Pitfalls

> Subject: [Message-Architecture](../README.md)

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


