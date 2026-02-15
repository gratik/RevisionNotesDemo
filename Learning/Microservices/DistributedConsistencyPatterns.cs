// ============================================================================
// DISTRIBUTED CONSISTENCY PATTERNS
// ============================================================================
// WHAT IS THIS?
// -------------
// Core patterns for reliable state transitions across services:
// outbox/inbox, idempotency keys, deduplication, and saga failure recovery.
//
// WHY IT MATTERS
// --------------
// ✅ Prevents data drift across async boundaries
// ✅ Reduces duplicate side effects from retries
// ✅ Makes failures recoverable and auditable
//
// WHEN TO USE
// -----------
// ✅ Event-driven integrations and at-least-once delivery systems
// ✅ Business flows that span multiple services
//
// WHEN NOT TO USE
// ---------------
// ❌ Single database monolith with local ACID transactions only
//
// REAL-WORLD EXAMPLE
// ------------------
// Order service writes order + outbox atomically; payment service consumes
// event with inbox dedupe; retries are safe via idempotency keys.
// ============================================================================

using System.Collections.Concurrent;

namespace RevisionNotesDemo.Microservices;

public static class DistributedConsistencyPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║ Distributed Consistency Patterns                      ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        ShowPatternMap();
        ShowOutboxAndInboxFlow();
        ShowIdempotencyAndDeduplication();
        ShowSagaRecovery();
        ShowExactlyOnceReality();
    }

    private static void ShowPatternMap()
    {
        Console.WriteLine("1) PATTERN MAP");
        Console.WriteLine("- Outbox: guarantee event publication after local commit");
        Console.WriteLine("- Inbox: guarantee each message is processed once logically");
        Console.WriteLine("- Idempotency Key: guarantee safe client/API retries");
        Console.WriteLine("- Saga Recovery: compensate partial success\n");
    }

    private static void ShowOutboxAndInboxFlow()
    {
        Console.WriteLine("2) OUTBOX + INBOX FLOW");

        var outbox = new OutboxStore();
        var inbox = new InboxStore();

        var message = new IntegrationMessage(Guid.NewGuid(), "OrderPlaced", "ord-1001");

        outbox.Save(message);
        var deliveryOne = inbox.TryProcess(message.MessageId);
        var deliveryTwo = inbox.TryProcess(message.MessageId); // duplicate delivery

        Console.WriteLine($"- Outbox queued: {outbox.Count}");
        Console.WriteLine($"- First delivery accepted: {deliveryOne}");
        Console.WriteLine($"- Duplicate delivery accepted: {deliveryTwo}");
        Console.WriteLine("- Result: at-least-once transport, exactly-once business effect\n");
    }

    private static void ShowIdempotencyAndDeduplication()
    {
        Console.WriteLine("3) IDEMPOTENCY + DEDUPLICATION");

        var api = new PaymentApiIdempotencyStore();
        const string key = "pay-req-88";

        var first = api.Execute(key, 49.99m);
        var retry = api.Execute(key, 49.99m);

        Console.WriteLine($"- First result: {first}");
        Console.WriteLine($"- Retry result: {retry}");
        Console.WriteLine("- Side effect count for key: 1\n");
    }

    private static void ShowSagaRecovery()
    {
        Console.WriteLine("4) SAGA FAILURE RECOVERY");
        Console.WriteLine("- Step A: Reserve inventory ✅");
        Console.WriteLine("- Step B: Charge payment ✅");
        Console.WriteLine("- Step C: Arrange shipment ❌");
        Console.WriteLine("- Compensation: Refund payment, release inventory, mark order canceled\n");
    }

    private static void ShowExactlyOnceReality()
    {
        Console.WriteLine("5) EXACTLY-ONCE REALITY");
        Console.WriteLine("- Transport can be at-most-once or at-least-once");
        Console.WriteLine("- Exactly-once end-to-end is a system illusion with strict boundaries");
        Console.WriteLine("- Practical target: exactly-once BUSINESS effect via idempotency + dedupe\n");
    }
}

internal sealed record IntegrationMessage(Guid MessageId, string EventName, string AggregateId);

internal sealed class OutboxStore
{
    private readonly List<IntegrationMessage> _messages = [];

    public int Count => _messages.Count;

    public void Save(IntegrationMessage message)
    {
        _messages.Add(message);
    }
}

internal sealed class InboxStore
{
    private readonly HashSet<Guid> _processed = [];

    public bool TryProcess(Guid messageId)
    {
        return _processed.Add(messageId);
    }
}

internal sealed class PaymentApiIdempotencyStore
{
    private readonly ConcurrentDictionary<string, string> _responses = new();

    public string Execute(string idempotencyKey, decimal amount)
    {
        return _responses.GetOrAdd(
            idempotencyKey,
            _ => $"Payment accepted for {amount:F2} at {DateTimeOffset.UtcNow:O}");
    }
}
