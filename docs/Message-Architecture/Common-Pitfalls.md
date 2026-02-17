# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Distributed systems basics, queues/topics, and eventual consistency concepts.
- Related examples: docs/Message-Architecture/README.md
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
- Common Pitfalls is about asynchronous messaging and event-driven coordination. It matters because it improves decoupling and throughput in distributed systems.
- Use it when reliable integration between independently evolving services.

2-minute answer:
- Start with the problem Common Pitfalls solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: scalability and decoupling vs operational complexity.
- Close with one failure mode and mitigation: underestimating retries, ordering, and idempotency concerns.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Common Pitfalls but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Common Pitfalls, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Common Pitfalls and map it to one concrete implementation in this module.
- 3 minutes: compare Common Pitfalls with an alternative, then walk through one failure mode and mitigation.