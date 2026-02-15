// ============================================================================
// HEXAGONAL ARCHITECTURE (PORTS AND ADAPTERS)
// ============================================================================
// WHAT IS THIS?
// -------------
// A pattern where business logic lives in the center and communicates through
// explicit ports (interfaces). Adapters implement ports for web, database,
// queues, or third-party APIs.
//
// WHY IT MATTERS
// --------------
// ✅ Keeps domain logic independent from frameworks/vendors
// ✅ Enables fast unit tests against in-memory adapters
// ✅ Reduces rewrite risk when infrastructure changes
//
// WHEN TO USE
// -----------
// ✅ Systems integrating with multiple external services
// ✅ Teams that need stable business rules over long product life
//
// WHEN NOT TO USE
// ---------------
// ❌ Very small apps where adapter overhead adds no practical value
//
// REAL-WORLD EXAMPLE
// ------------------
// Subscription billing: core use case charges via payment port and records
// status via repository port. Stripe/Mock adapters can be swapped safely.
// ============================================================================

namespace RevisionNotesDemo.Architecture;

public static class HexagonalArchitectureExamples
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Hexagonal Architecture (Ports and Adapters)          ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        ShowCoreConcepts();
        ShowAdapterSwap();
        ShowInboundOutboundModel();
        ShowCommonPitfalls();
    }

    private static void ShowCoreConcepts()
    {
        Console.WriteLine("1) CORE CONCEPTS");
        Console.WriteLine("- Domain/use cases depend on ports, never concrete adapters");
        Console.WriteLine("- Inbound adapters translate input (HTTP/CLI/message) into commands");
        Console.WriteLine("- Outbound adapters implement ports for storage, payment, and messaging\n");
    }

    private static void ShowAdapterSwap()
    {
        Console.WriteLine("2) ADAPTER SWAP DEMO");

        var inMemoryRepo = new HexInMemorySubscriptionRepository();
        var fakeGateway = new HexFakePaymentGateway(success: true);
        var useCase = new HexActivateSubscriptionUseCase(inMemoryRepo, fakeGateway);

        var first = useCase.Execute(new HexActivateSubscriptionCommand("sub-001", 19.99m));
        Console.WriteLine($"- Fake adapter result: {first.Status}");

        var stripeGateway = new HexStripeStylePaymentGateway();
        var useCaseWithStripe = new HexActivateSubscriptionUseCase(inMemoryRepo, stripeGateway);
        var second = useCaseWithStripe.Execute(new HexActivateSubscriptionCommand("sub-002", 29.99m));
        Console.WriteLine($"- Stripe-style adapter result: {second.Status}");

        Console.WriteLine($"- Persisted subscriptions: {inMemoryRepo.Count}\n");
    }

    private static void ShowInboundOutboundModel()
    {
        Console.WriteLine("3) INBOUND/OUTBOUND MODEL");
        Console.WriteLine("- Inbound port: Execute(command) on use case");
        Console.WriteLine("- Outbound ports: payment + persistence interfaces");
        Console.WriteLine("- Adapters remain replaceable as long as port contracts hold\n");
    }

    private static void ShowCommonPitfalls()
    {
        Console.WriteLine("4) COMMON PITFALLS");
        Console.WriteLine("- Putting HTTP DTOs directly in domain entities");
        Console.WriteLine("- Creating generic ports that mirror framework abstractions");
        Console.WriteLine("- Leaking vendor error types through port signatures\n");
    }
}

public sealed record HexActivateSubscriptionCommand(string SubscriptionId, decimal Amount);

public sealed record HexActivateSubscriptionResult(string SubscriptionId, string Status);

public interface IHexSubscriptionRepository
{
    void Save(HexSubscription subscription);
}

public interface IHexPaymentGateway
{
    bool Charge(string subscriptionId, decimal amount);
}

public sealed class HexSubscription
{
    public HexSubscription(string id, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Subscription id is required.", nameof(id));
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");
        }

        Id = id;
        Amount = amount;
        ActivatedUtc = DateTimeOffset.UtcNow;
    }

    public string Id { get; }

    public decimal Amount { get; }

    public DateTimeOffset ActivatedUtc { get; }
}

public sealed class HexActivateSubscriptionUseCase
{
    private readonly IHexSubscriptionRepository _repository;
    private readonly IHexPaymentGateway _payments;

    public HexActivateSubscriptionUseCase(IHexSubscriptionRepository repository, IHexPaymentGateway payments)
    {
        _repository = repository;
        _payments = payments;
    }

    public HexActivateSubscriptionResult Execute(HexActivateSubscriptionCommand command)
    {
        var charged = _payments.Charge(command.SubscriptionId, command.Amount);
        if (!charged)
        {
            return new HexActivateSubscriptionResult(command.SubscriptionId, "Rejected");
        }

        var subscription = new HexSubscription(command.SubscriptionId, command.Amount);
        _repository.Save(subscription);

        return new HexActivateSubscriptionResult(command.SubscriptionId, "Activated");
    }
}

public sealed class HexInMemorySubscriptionRepository : IHexSubscriptionRepository
{
    private readonly List<HexSubscription> _subscriptions = [];

    public int Count => _subscriptions.Count;

    public void Save(HexSubscription subscription)
    {
        _subscriptions.Add(subscription);
    }
}

public sealed class HexFakePaymentGateway : IHexPaymentGateway
{
    private readonly bool _success;

    public HexFakePaymentGateway(bool success)
    {
        _success = success;
    }

    public bool Charge(string subscriptionId, decimal amount)
    {
        return _success;
    }
}

public sealed class HexStripeStylePaymentGateway : IHexPaymentGateway
{
    public bool Charge(string subscriptionId, decimal amount)
    {
        return amount <= 1000m;
    }
}
