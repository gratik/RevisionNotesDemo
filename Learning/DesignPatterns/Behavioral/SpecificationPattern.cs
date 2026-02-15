// ==============================================================================
// SPECIFICATION PATTERN - Composable Business Rules
// Reference: Revision Notes - Design Patterns
// ==============================================================================
//
// WHAT IS THE SPECIFICATION PATTERN?
// -----------------------------------
// Encapsulates business rules and domain logic as reusable, combinable objects.
// Allows you to chain/combine specifications using AND, OR, NOT operators to build
// complex filtering logic. Separates query logic from domain models.
//
// Think of it as: "Job search filters - combine rules like 'Remote' AND ('Salary > $100k'
// OR 'Equity > $50k') AND NOT 'Requires PhD'. Each rule is a specification that can be
// reused and combined differently for different searches."
//
// Core Concepts:
//   • Specification: Encapsulates a single business rule
//   • IsSatisfiedBy(): Tests if entity meets criteria
//   • Composition: Combine specs with And(), Or(), Not()
//   • Reusability: Same spec used in multiple contexts
//   • Testability: Test each rule independently
//
// WHY IT MATTERS
// --------------
// ✅ REUSABLE BUSINESS LOGIC: Write rule once, use everywhere
// ✅ COMPOSABLE: Combine simple rules into complex queries
// ✅ TESTABLE: Test each specification independently
// ✅ READABLE: Self-documenting code (new PremiumCustomerSpec())
// ✅ DRY: Avoid duplicating filtering logic
// ✅ DOMAIN-DRIVEN: Business rules separated from infrastructure
//
// WHEN TO USE IT
// --------------
// ✅ Complex filtering/selection logic
// ✅ Business rules need to be combined in various ways
// ✅ Same rule used in multiple places (UI, validation, queries)
// ✅ Need to translate business rules to database queries (LINQ)
// ✅ Working with Repository pattern
// ✅ Domain-Driven Design (DDD)
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Simple one-time filters (lambda is simpler)
// ❌ Rules never combined
// ❌ Overhead not justified (2-3 simple conditions)
// ❌ Performance critical (direct queries faster)
//
// REAL-WORLD EXAMPLE - E-commerce Product Filtering
// -------------------------------------------------
// Amazon/Shopify product search with filters:
//   • Filters:
//     - Price range: $50 - $100
//     - In stock: Yes
//     - Brand: Nike OR Adidas
//     - Rating: ≥ 4 stars
//     - Ships to: Canada
//
// WITHOUT SPECIFICATION:
//   ❌ var products = allProducts
//         .Where(p => p.Price >= 50 && p.Price <= 100)  // Duplicated logic
//         .Where(p => p.InStock)
//         .Where(p => p.Brand == "Nike" || p.Brand == "Adidas")
//         .Where(p => p.Rating >= 4)
//         .Where(p => p.ShipsTo.Contains("Canada"));
//   ❌ Same filters needed in:
//     - Product listing page
//     - Search results
//     - Recommendations
//     - Admin reports
//   ❌ Duplication everywhere!
//   ❌ Change price range logic = update 10 places
//
// WITH SPECIFICATION:
//   ✅ class PriceRangeSpec : Specification<Product> {
//         private readonly decimal _min, _max;
//         public PriceRangeSpec(decimal min, decimal max) {
//             _min = min; _max = max;
//         }
//         public override bool IsSatisfiedBy(Product p) =>
//             p.Price >= _min && p.Price <= _max;
//     }
//   
//   ✅ class InStockSpec : Specification<Product> {
//         public override bool IsSatisfiedBy(Product p) => p.InStock;
//     }
//   
//   ✅ class BrandSpec : Specification<Product> {
//         private readonly string _brand;
//         public BrandSpec(string brand) => _brand = brand;
//         public override bool IsSatisfiedBy(Product p) => p.Brand == _brand;
//     }
//   
//   ✅ class MinRatingSpec : Specification<Product> {
//         private readonly decimal _minRating;
//         public MinRatingSpec(decimal rating) => _minRating = rating;
//         public override bool IsSatisfiedBy(Product p) => p.Rating >= _minRating;
//     }
//   
//   ✅ Usage (Composition):
//     var spec = new PriceRangeSpec(50, 100)
//         .And(new InStockSpec())
//         .And(new BrandSpec("Nike").Or(new BrandSpec("Adidas")))
//         .And(new MinRatingSpec(4));
//     
//     var products = repository.Find(spec);  // Apply to repository
//     
//     // Or in-memory:
//     var filtered = allProducts.Where(p => spec.IsSatisfiedBy(p));
//   
//   ✅ Reuse anywhere:
//     - Listing: repository.Find(inStockSpec)
//     - Search: repository.Find(searchSpec.And(inStockSpec))
//     - Validation: if (premiumSpec.IsSatisfiedBy(customer)) { ... }
//
// ANOTHER EXAMPLE - Customer Segmentation
// ---------------------------------------
// Marketing campaign targeting:
//   • Target: Premium customers (High LTV, Active, Good credit)
//   • Segments:
//     - HighValueSpec: TotalSpent > $10,000
//     - ActiveSpec: LastPurchase < 30 days ago
//     - GoodCreditSpec: CreditScore > 700
//     - LoyaltyMemberSpec: MembershipYears > 2
//
// Compose for different campaigns:
//   var premiumSpec = new HighValueSpec()
//       .And(new ActiveSpec())
//       .And(new GoodCreditSpec());
//   
//   var loyaltyRewardSpec = new LoyaltyMemberSpec()
//       .And(new ActiveSpec());
//   
//   var winBackSpec = new HighValueSpec()
//       .And(new ActiveSpec().Not());  // Was valuable, now inactive
//
// ANOTHER EXAMPLE - Order Eligibility
// -----------------------------------
// Check if order qualifies for free shipping:
//   interface IOrderSpec {
//       bool IsSatisfiedBy(Order order);
//   }
//   
//   var freeShippingSpec = new MinimumOrderValueSpec(50)
//       .Or(new PremiumMemberSpec())
//       .Or(new PromoCodeSpec("FREESHIP"));
//   
//   if (freeShippingSpec.IsSatisfiedBy(order)) {
//       order.ShippingCost = 0;
//   }
//
// SPECIFICATION WITH REPOSITORY PATTERN
// -------------------------------------
// Translate specification to database query:
//   interface ISpecification<T> {
//       bool IsSatisfiedBy(T entity);               // In-memory check
//       Expression<Func<T, bool>> ToExpression();  // Database query
//   }
//   
//   class PriceRangeSpec : Specification<Product> {
//       public override Expression<Func<Product, bool>> ToExpression() {
//           return p => p.Price >= _min && p.Price <= _max;  // EF translates to SQL
//       }
//   }
//   
//   class Repository<T> {
//       public IEnumerable<T> Find(ISpecification<T> spec) {
//           return _dbSet.Where(spec.ToExpression());  // Translates to SQL!
//       }
//   }
//   
//   Usage:
//     var products = repository.Find(new PriceRangeSpec(50, 100));
//     // SQL: SELECT * FROM Products WHERE Price >= 50 AND Price <= 100
//
// COMPOSITE SPECIFICATIONS
// ------------------------
// Built-in composition:
//   class AndSpecification<T> : Specification<T> {
//       private ISpecification<T> _left, _right;
//       public AndSpecification(ISpecification<T> left, ISpecification<T> right) {
//           _left = left; _right = right;
//       }
//       public override bool IsSatisfiedBy(T entity) =>
//           _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);
//   }
//   
//   class OrSpecification<T> : Specification<T> {
//       public override bool IsSatisfiedBy(T entity) =>
//           _left.IsSatisfiedBy(entity) || _right.IsSatisfiedBy(entity);
//   }
//   
//   class NotSpecification<T> : Specification<T> {
//       private ISpecification<T> _spec;
//       public override bool IsSatisfiedBy(T entity) =>
//           !_spec.IsSatisfiedBy(entity);
//   }
//
// FLUENT API
// ----------
// Extension methods for readability:
//   public static class SpecificationExtensions {
//       public static ISpecification<T> And<T>(
//           this ISpecification<T> left,
//           ISpecification<T> right) =>
//               new AndSpecification<T>(left, right);
//   }
//   
//   Usage:
//     var spec = new HighValueSpec()
//         .And(new ActiveSpec())
//         .And(new GoodCreditSpec());
//
// REAL-WORLD LIBRARIES
// --------------------
// Libraries using Specification pattern:
//   • **Ardalis.Specification**: Specification pattern for EF Core
//   • **LinqSpecs**: Specification pattern for LINQ
//   • **NSpecifications**: General-purpose specifications
//
// Example with Ardalis.Specification:
//   public class ActiveCustomersSpec : Specification<Customer> {
//       public ActiveCustomersSpec() {
//           Query.Where(c => c.IsActive)
//                .OrderBy(c => c.Name);
//       }
//   }
//
// SPECIFICATION VS LAMBDA EXPRESSIONS
// -----------------------------------
// Specification vs Lambda:
//   • Specification: Reusable, named, testable, composable
//   • Lambda: Inline, anonymous, one-time use
//
// When to use Specification:
//   ✅ Rule used in multiple places
//   ✅ Complex business logic
//   ✅ Need to test rule independently
//   ✅ Need to combine rules dynamically
//
// When to use Lambda:
//   ✅ Simple one-time filter
//   ✅ Query-specific logic
//   ✅ No reuse needed
//
// BEST PRACTICES
// --------------
// ✅ Name specifications clearly (what they test, not how)
// ✅ Keep specifications focused (Single Responsibility)
// ✅ Make specifications immutable
// ✅ Unit test each specification independently
// ✅ Use composition over creating complex specs
// ✅ Provide ToExpression() for database queries
// ✅ Consider specification builder/factory for common combinations
//
// SPECIFICATION VS SIMILAR PATTERNS
// ---------------------------------
// Specification vs Strategy:
//   • Specification: Evaluates to true/false (selection)
//   • Strategy: Executes algorithm (action)
//
// Specification vs Composite:
//   • Specification: Boolean logic composition (AND, OR, NOT)
//   • Composite: Tree structure composition
//
// ==========================================================================================================================================================

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