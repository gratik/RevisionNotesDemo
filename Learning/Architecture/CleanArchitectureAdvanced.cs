// ============================================================================
// CLEAN ARCHITECTURE (ADVANCED)
// ============================================================================
// WHAT IS THIS?
// -------------
// Practical advanced guidance for Clean Architecture in real services:
// dependency direction, use-case orchestration, policy enforcement, and
// infrastructure boundaries.
//
// WHY IT MATTERS
// --------------
// ✅ Protects business rules from framework churn
// ✅ Keeps use-case logic testable without ASP.NET/EF dependencies
// ✅ Makes infrastructure swaps possible with lower migration risk
//
// WHEN TO USE
// -----------
// ✅ Medium/large systems with evolving requirements and teams
// ✅ Services where business rules are more stable than delivery tech
//
// WHEN NOT TO USE
// ---------------
// ❌ Tiny throwaway apps where architectural overhead dominates
// ❌ Teams that cannot maintain boundaries consistently
//
// REAL-WORLD EXAMPLE
// ------------------
// "Place order" use case: domain validates invariants, application orchestrates,
// infrastructure persists and publishes events through ports.
// ============================================================================

namespace RevisionNotesDemo.Architecture;

public static class CleanArchitectureAdvanced
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Clean Architecture (Advanced)                        ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        ShowDependencyRule();
        ShowUseCaseOrchestration();
        ShowPolicyEnforcement();
        ShowBoundaryTradeoffs();
        ShowAntiPatterns();
    }

    private static void ShowDependencyRule()
    {
        Console.WriteLine("1) DEPENDENCY RULE");
        Console.WriteLine("- Source dependencies point inward (Domain <- Application <- Infrastructure)");
        Console.WriteLine("- Domain should not reference EF, HTTP, queues, or framework-specific types");
        Console.WriteLine("- Outer layers adapt to inner contracts through interfaces\n");
    }

    private static void ShowUseCaseOrchestration()
    {
        Console.WriteLine("2) USE-CASE ORCHESTRATION EXAMPLE");

        var repository = new InMemoryOrderRepository();
        var publisher = new InMemoryEventPublisher();
        var handler = new CleanPlaceOrderHandler(repository, publisher, new SystemClock());

        var command = new CleanPlaceOrderCommand("cust-42", [new CleanOrderLine("sku-chair", 1), new CleanOrderLine("sku-lamp", 2)]);
        var result = handler.Handle(command);

        Console.WriteLine($"- Created order id: {result.OrderId}");
        Console.WriteLine($"- Persisted orders: {repository.Count}");
        Console.WriteLine($"- Published events: {publisher.Events.Count}");
        Console.WriteLine("- Application layer orchestrated without infrastructure details\n");
    }

    private static void ShowPolicyEnforcement()
    {
        Console.WriteLine("3) POLICY ENFORCEMENT (PIPELINE STYLE)");

        var auth = new AuthorizationPolicy("orders:write");
        var validation = new CleanOrderValidationPolicy();

        var good = validation.Validate(new CleanPlaceOrderCommand("cust-1", [new CleanOrderLine("sku-book", 1)]));
        var bad = validation.Validate(new CleanPlaceOrderCommand("cust-1", []));

        Console.WriteLine($"- Permission required: {auth.RequiredPermission}");
        Console.WriteLine($"- Valid command accepted: {good}");
        Console.WriteLine($"- Invalid command accepted: {bad}");
        Console.WriteLine("- Cross-cutting policies remain outside entities but before handler logic\n");
    }

    private static void ShowBoundaryTradeoffs()
    {
        Console.WriteLine("4) BOUNDARY TRADEOFFS");
        Console.WriteLine("- More boundaries => better replaceability, but more boilerplate");
        Console.WriteLine("- Avoid repository-per-entity if it adds no value over a focused port");
        Console.WriteLine("- Keep DTO mapping at boundaries, not inside entities");
        Console.WriteLine("- Prefer use-case specific ports over generic catch-all interfaces\n");
    }

    private static void ShowAntiPatterns()
    {
        Console.WriteLine("5) COMMON ANTI-PATTERNS");
        Console.WriteLine("- Domain entities directly calling HttpClient or DbContext");
        Console.WriteLine("- Application layer returning EF tracked entities to API layer");
        Console.WriteLine("- Leaky abstractions where ports expose framework-specific details");
        Console.WriteLine("- Ceremony-only architecture with no explicit business policy\n");
    }
}

public sealed record CleanPlaceOrderCommand(string CustomerId, IReadOnlyList<CleanOrderLine> Lines);

public sealed record CleanOrderLine(string Sku, int Quantity);

public sealed record CleanPlaceOrderResult(string OrderId);

public sealed class Order
{
    public Order(string id, string customerId, IReadOnlyList<CleanOrderLine> lines, DateTimeOffset createdUtc)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            throw new ArgumentException("Customer id is required.", nameof(customerId));
        }

        if (lines.Count == 0)
        {
            throw new InvalidOperationException("Order must contain at least one line.");
        }

        Id = id;
        CustomerId = customerId;
        Lines = lines;
        CreatedUtc = createdUtc;
    }

    public string Id { get; }

    public string CustomerId { get; }

    public IReadOnlyList<CleanOrderLine> Lines { get; }

    public DateTimeOffset CreatedUtc { get; }
}

public interface IOrderRepository
{
    void Add(Order order);
}

public interface IIntegrationEventPublisher
{
    void Publish(string eventName, string aggregateId);
}

public interface IClock
{
    DateTimeOffset UtcNow();
}

public sealed class CleanPlaceOrderHandler
{
    private readonly IOrderRepository _orders;
    private readonly IIntegrationEventPublisher _events;
    private readonly IClock _clock;

    public CleanPlaceOrderHandler(IOrderRepository orders, IIntegrationEventPublisher events, IClock clock)
    {
        _orders = orders;
        _events = events;
        _clock = clock;
    }

    public CleanPlaceOrderResult Handle(CleanPlaceOrderCommand command)
    {
        var orderId = $"ord-{Guid.NewGuid():N}";
        var order = new Order(orderId, command.CustomerId, command.Lines, _clock.UtcNow());

        _orders.Add(order);
        _events.Publish("OrderPlaced", orderId);

        return new CleanPlaceOrderResult(orderId);
    }
}

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _storage = [];

    public int Count => _storage.Count;

    public void Add(Order order)
    {
        _storage.Add(order);
    }
}

public sealed class InMemoryEventPublisher : IIntegrationEventPublisher
{
    public List<string> Events { get; } = [];

    public void Publish(string eventName, string aggregateId)
    {
        Events.Add($"{eventName}:{aggregateId}");
    }
}

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow()
    {
        return DateTimeOffset.UtcNow;
    }
}

public sealed class AuthorizationPolicy
{
    public AuthorizationPolicy(string requiredPermission)
    {
        RequiredPermission = requiredPermission;
    }

    public string RequiredPermission { get; }
}

public sealed class CleanOrderValidationPolicy
{
    public bool Validate(CleanPlaceOrderCommand command)
    {
        return !string.IsNullOrWhiteSpace(command.CustomerId)
            && command.Lines.Count > 0
            && command.Lines.All(x => x.Quantity > 0);
    }
}
