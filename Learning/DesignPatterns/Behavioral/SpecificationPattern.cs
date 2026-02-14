// ==============================================================================
// SPECIFICATION PATTERN
// Reference: Revision Notes - Design Patterns
// ==============================================================================
// PURPOSE: Encapsulates business rules/conditions as reusable objects
// BENEFIT: Composable, testable, reusable business logic; works great with Repository pattern
// USE WHEN: Complex filtering logic, business rules need to be combined/reused
// ==============================================================================

namespace RevisionNotesDemo.DesignPatterns.Behavioral;

// ========================================================================
// SPECIFICATION INTERFACE
// ========================================================================

/// <summary>
/// Specification interface for business rules
/// </summary>
public interface ISpecification<T>
{
    bool IsSatisfiedBy(T entity);

    // Composition methods
    ISpecification<T> And(ISpecification<T> other);
    ISpecification<T> Or(ISpecification<T> other);
    ISpecification<T> Not();
}

// ========================================================================
// BASE SPECIFICATION (Composite Pattern)
// ========================================================================

public abstract class Specification<T> : ISpecification<T>
{
    public abstract bool IsSatisfiedBy(T entity);

    public ISpecification<T> And(ISpecification<T> other) =>
        new AndSpecification<T>(this, other);

    public ISpecification<T> Or(ISpecification<T> other) =>
        new OrSpecification<T>(this, other);

    public ISpecification<T> Not() =>
        new NotSpecification<T>(this);
}

// ========================================================================
// COMPOSITE SPECIFICATIONS
// ========================================================================

public class AndSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override bool IsSatisfiedBy(T entity) =>
        _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);
}

public class OrSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public OrSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override bool IsSatisfiedBy(T entity) =>
        _left.IsSatisfiedBy(entity) || _right.IsSatisfiedBy(entity);
}

public class NotSpecification<T> : Specification<T>
{
    private readonly ISpecification<T> _spec;

    public NotSpecification(ISpecification<T> spec)
    {
        _spec = spec;
    }

    public override bool IsSatisfiedBy(T entity) =>
        !_spec.IsSatisfiedBy(entity);
}

// ========================================================================
// DOMAIN MODEL
// ========================================================================

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public bool IsPaid { get; set; }
    public int ItemCount { get; set; }
    public string Status { get; set; } = "Pending";  // Pending, Shipped, Delivered

    public override string ToString() =>
        $"[{Id}] {CustomerName} - ${TotalAmount:F2} - {Status} " +
        $"({OrderDate:MM/dd}) - {(IsPaid ? "✅ Paid" : "❌ Unpaid")} - {ItemCount} items";
}

// ========================================================================
// BUSINESS RULE SPECIFICATIONS  
// ========================================================================

public class PaidOrderSpecification : Specification<Order>
{
    public override bool IsSatisfiedBy(Order order) => order.IsPaid;
}

public class LargeOrderSpecification : Specification<Order>
{
    private readonly decimal _threshold;

    public LargeOrderSpecification(decimal threshold = 500)
    {
        _threshold = threshold;
    }

    public override bool IsSatisfiedBy(Order order) => order.TotalAmount >= _threshold;
}

public class RecentOrderSpecification : Specification<Order>
{
    private readonly int _daysAgo;

    public RecentOrderSpecification(int daysAgo = 30)
    {
        _daysAgo = daysAgo;
    }

    public override bool IsSatisfiedBy(Order order) =>
        order.OrderDate >= DateTime.Now.AddDays(-_daysAgo);
}

public class PendingOrderSpecification : Specification<Order>
{
    public override bool IsSatisfiedBy(Order order) =>
        order.Status == "Pending";
}

public class BulkOrderSpecification : Specification<Order>
{
    private readonly int _minItems;

    public BulkOrderSpecification(int minItems = 10)
    {
        _minItems = minItems;
    }

    public override bool IsSatisfiedBy(Order order) =>
        order.ItemCount >= _minItems;
}

// ========================================================================
// REPOSITORY WITH SPECIFICATION SUPPORT
// ========================================================================

public class OrderRepository
{
    private readonly List<Order> _orders = new();

    public OrderRepository()
    {
        // Seed data
        _orders.AddRange(new[]
        {
            new Order { Id = 1, CustomerName = "Alice", TotalAmount = 750m, OrderDate = DateTime.Now.AddDays(-5), IsPaid = true, ItemCount = 8, Status = "Shipped" },
            new Order { Id = 2, CustomerName = "Bob", TotalAmount = 250m, OrderDate = DateTime.Now.AddDays(-45), IsPaid = false, ItemCount = 3, Status = "Pending" },
            new Order { Id = 3, CustomerName = "Charlie", TotalAmount = 1200m, OrderDate = DateTime.Now.AddDays(-2), IsPaid = true, ItemCount = 15, Status = "Delivered" },
            new Order { Id = 4, CustomerName = "Diana", TotalAmount = 100m, OrderDate = DateTime.Now.AddDays(-10), IsPaid = true, ItemCount = 2, Status = "Delivered" },
            new Order { Id = 5, CustomerName = "Eve", TotalAmount = 600m, OrderDate = DateTime.Now.AddDays(-8), IsPaid = false, ItemCount = 12, Status = "Pending" },
            new Order { Id = 6, CustomerName = "Frank", TotalAmount = 450m, OrderDate = DateTime.Now.AddDays(-60), IsPaid = true, ItemCount = 5, Status = "Delivered" }
        });
    }

    public IEnumerable<Order> Find(ISpecification<Order> specification)
    {
        return _orders.Where(o => specification.IsSatisfiedBy(o));
    }

    public IEnumerable<Order> GetAll() => _orders;
}

// ========================================================================
// DEMONSTRATION
// ========================================================================

public class SpecificationDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== SPECIFICATION PATTERN DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Design Patterns\n");

        var repository = new OrderRepository();

        Console.WriteLine("--- All Orders ---");
        foreach (var order in repository.GetAll())
            Console.WriteLine($"  {order}");
        Console.WriteLine();

        // 1. Simple specifications
        Console.WriteLine("--- 1. Simple Specifications ---");

        var paidSpec = new PaidOrderSpecification();
        var paidOrders = repository.Find(paidSpec);
        Console.WriteLine("[SPEC] ✅ Paid orders:");
        foreach (var order in paidOrders)
            Console.WriteLine($"  {order}");
        Console.WriteLine();

        var largeOrderSpec = new LargeOrderSpecification(500);
        var largeOrders = repository.Find(largeOrderSpec);
        Console.WriteLine("[SPEC] 💰 Large orders (>= $500):");
        foreach (var order in largeOrders)
            Console.WriteLine($"  {order}");
        Console.WriteLine();

        // 2. Composite specifications (AND)
        Console.WriteLine("--- 2. Composite: AND ---");
        var recentSpec = new RecentOrderSpecification(30);
        var paidAndRecent = paidSpec.And(recentSpec);
        var results = repository.Find(paidAndRecent);
        Console.WriteLine("[SPEC] ✅ Paid AND recent (< 30 days):");
        foreach (var order in results)
            Console.WriteLine($"  {order}");
        Console.WriteLine();

        // 3. Composite specifications (OR)
        Console.WriteLine("--- 3. Composite: OR ---");
        var largeOrBulk = new LargeOrderSpecification(500).Or(new BulkOrderSpecification(10));
        var results2 = repository.Find(largeOrBulk);
        Console.WriteLine("[SPEC] 💰 Large (>= $500) OR bulk (>= 10 items):");
        foreach (var order in results2)
            Console.WriteLine($"  {order}");
        Console.WriteLine();

        // 4. Composite specifications (NOT)
        Console.WriteLine("--- 4. Composite: NOT ---");
        var notPaid = paidSpec.Not();
        var unpaidOrders = repository.Find(notPaid);
        Console.WriteLine("[SPEC] ❌ Unpaid orders:");
        foreach (var order in unpaidOrders)
            Console.WriteLine($"  {order}");
        Console.WriteLine();

        // 5. Complex composition
        Console.WriteLine("--- 5. Complex Composition ---");
        // Find: (Large AND Paid) OR (Bulk AND Unpaid)
        var largePaid = new LargeOrderSpecification(500).And(new PaidOrderSpecification());
        var bulkUnpaid = new BulkOrderSpecification(10).And(new PaidOrderSpecification().Not());
        var complexSpec = largePaid.Or(bulkUnpaid);

        var complexResults = repository.Find(complexSpec);
        Console.WriteLine("[SPEC] 🎯 Complex: (Large AND Paid) OR (Bulk AND Unpaid):");
        foreach (var order in complexResults)
            Console.WriteLine($"  {order}");
        Console.WriteLine();

        // 6. Pending high-value orders
        Console.WriteLine("--- 6. Business Rule: High-Risk Orders ---");
        var highRisk = new PendingOrderSpecification()
            .And(new LargeOrderSpecification(500))
            .And(new PaidOrderSpecification().Not());

        var riskOrders = repository.Find(highRisk);
        Console.WriteLine("[SPEC] ⚠️  Pending + Large + Unpaid (high risk):");
        foreach (var order in riskOrders)
            Console.WriteLine($"  {order}");
        Console.WriteLine();

        Console.WriteLine("💡 Specification Pattern Benefits:");
        Console.WriteLine("   ✅ Encapsulates business rules as objects");
        Console.WriteLine("   ✅ Composable - combine with And/Or/Not");
        Console.WriteLine("   ✅ Reusable - same spec across different contexts");
        Console.WriteLine("   ✅ Testable - each rule can be unit tested");
        Console.WriteLine("   ✅ Declarative - reads like business requirements");
        Console.WriteLine("   ✅ Works great with Repository pattern");
    }
}