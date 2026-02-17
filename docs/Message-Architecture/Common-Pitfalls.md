# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


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

