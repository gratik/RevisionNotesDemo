// ============================================================================
// END-TO-END CASE STUDY (Order-to-Delivery)
// ============================================================================
// WHAT IS THIS?
// -------------
// A cohesive example that walks through one feature from requirements to
// architecture, implementation, testing, and deployment checkpoints.
//
// WHY IT MATTERS
// --------------
// ✅ Shows how isolated concepts connect in a real delivery workflow
// ✅ Demonstrates architecture and delivery tradeoffs in one place
// ✅ Provides a reusable template for future feature design
//
// WHEN TO USE
// -----------
// ✅ During feature kickoff and design reviews
// ✅ For onboarding engineers into end-to-end ownership
//
// WHEN NOT TO USE
// ---------------
// ❌ As a strict blueprint for every team/product context
//
// REAL-WORLD EXAMPLE
// ------------------
// E-commerce "Place Order" flow: API receives request, validates business
// rules, persists order, publishes integration event, and tracks deployment
// readiness gates.
// ============================================================================

using System.Collections.Concurrent;

namespace RevisionNotesDemo.Architecture;

public static class EndToEndCaseStudy
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║ End-to-End Case Study: Order-to-Delivery              ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        ShowRequirements();
        ShowArchitecture();
        ShowImplementation();
        ShowTestingPlan();
        ShowDeploymentReadiness();
    }

    private static void ShowRequirements()
    {
        Console.WriteLine("1) REQUIREMENTS");
        Console.WriteLine("- Customer can place an order with 1..N items");
        Console.WriteLine("- Payment must be authorized before fulfillment");
        Console.WriteLine("- Duplicate submissions must be idempotent");
        Console.WriteLine("- API p95 latency target: < 250ms\n");
    }

    private static void ShowArchitecture()
    {
        Console.WriteLine("2) ARCHITECTURE");
        Console.WriteLine("- API: Receives command + idempotency key");
        Console.WriteLine("- Domain: Validates order invariants");
        Console.WriteLine("- Data: Persists order aggregate + outbox event");
        Console.WriteLine("- Messaging: Publishes OrderPlaced event asynchronously\n");
    }

    private static void ShowImplementation()
    {
        Console.WriteLine("3) IMPLEMENTATION SNAPSHOT");

        var service = new PlaceOrderService();

        var command = new PlaceOrderCommand(
            CustomerId: "cust-001",
            IdempotencyKey: "req-123",
            Items: [new OrderItem("sku-chair", 1), new OrderItem("sku-lamp", 2)]);

        var first = service.Place(command);
        var second = service.Place(command); // same idempotency key

        Console.WriteLine($"- First call status: {first.Status}");
        Console.WriteLine($"- Second call status: {second.Status} (deduplicated)");
        Console.WriteLine($"- Stored orders: {service.OrderCount}");
        Console.WriteLine($"- Outbox messages: {service.OutboxCount}\n");
    }

    private static void ShowTestingPlan()
    {
        Console.WriteLine("4) TESTING STRATEGY");
        Console.WriteLine("- Unit: domain invariants (empty order rejected)");
        Console.WriteLine("- Integration: order + outbox are saved atomically");
        Console.WriteLine("- API: duplicate idempotency key returns same order id");
        Console.WriteLine("- Non-functional: load test p95 latency and error budget\n");
    }

    private static void ShowDeploymentReadiness()
    {
        Console.WriteLine("5) DEPLOYMENT READINESS");
        Console.WriteLine("- Health checks: liveness/readiness endpoints");
        Console.WriteLine("- Observability: trace id + structured logs for order id");
        Console.WriteLine("- Rollout: canary 5% -> 25% -> 100%");
        Console.WriteLine("- Rollback: one-command deployment rollback path\n");
    }
}

internal sealed class PlaceOrderService
{
    private readonly ConcurrentDictionary<string, PlaceOrderResult> _idempotencyStore = new();
    private readonly List<OrderRecord> _orders = [];
    private readonly List<OutboxRecord> _outbox = [];

    public int OrderCount => _orders.Count;

    public int OutboxCount => _outbox.Count;

    public PlaceOrderResult Place(PlaceOrderCommand command)
    {
        if (_idempotencyStore.TryGetValue(command.IdempotencyKey, out var existing))
        {
            return existing with { Status = "Accepted (Idempotent Replay)" };
        }

        if (command.Items.Count == 0)
        {
            return new PlaceOrderResult(string.Empty, "Rejected");
        }

        var orderId = $"ord-{_orders.Count + 1:000}";

        _orders.Add(new OrderRecord(orderId, command.CustomerId, DateTimeOffset.UtcNow));
        _outbox.Add(new OutboxRecord(Guid.NewGuid(), "OrderPlaced", orderId, DateTimeOffset.UtcNow));

        var result = new PlaceOrderResult(orderId, "Accepted");
        _idempotencyStore[command.IdempotencyKey] = result;

        return result;
    }
}

internal sealed record PlaceOrderCommand(string CustomerId, string IdempotencyKey, IReadOnlyList<OrderItem> Items);

internal sealed record OrderItem(string Sku, int Quantity);

internal sealed record PlaceOrderResult(string OrderId, string Status);

internal sealed record OrderRecord(string OrderId, string CustomerId, DateTimeOffset CreatedUtc);

internal sealed record OutboxRecord(Guid MessageId, string EventName, string AggregateId, DateTimeOffset CreatedUtc);
