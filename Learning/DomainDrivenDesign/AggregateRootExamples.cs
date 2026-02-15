// ==============================================================================
// DOMAIN-DRIVEN DESIGN: AGGREGATE ROOTS
// ==============================================================================
// PURPOSE: Define transactional consistency boundaries in domain
// WHY: Ensures business rules across multiple entities are maintained
// USE WHEN: Multiple related entities must change together atomically
// ==============================================================================
// WHAT IS AN AGGREGATE?
// An Aggregate is a cluster of domain entities and value objects that are
// treated as a single unit for data changes. The Aggregate Root is the ONLY
// entity in the cluster that external objects are allowed to hold references to.
//
// AGGREGATE ROOT: The gateway entity that controls all access to the aggregate.
// - Only entry point for modifications (enforces invariants)
// - Has global identity that doesn't change
// - Responsible for maintaining consistency within aggregate boundary
// - Example: Order aggregate (Order root + OrderLines children)
//
// WHY IT MATTERS:
// • CONSISTENCY BOUNDARIES: Defines what must be consistent at all times
// • TRANSACTION BOUNDARIES: Aggregate = one transaction (save together)
// • ENCAPSULATION: External code cannot bypass rules by modifying children
// • CLEAR OWNERSHIP: One entity owns and controls related entities
// • SIMPLIFIED PERSISTENCE: One repository per aggregate (not per entity)
//
// THE PROBLEM WITHOUT AGGREGATES:
// Without clear boundaries, any code can modify any entity directly:
// - Business rules scattered across services (not enforced)
// - Inconsistent state possible (OrderLines changed but Total not updated)
// - Unclear transactional boundaries (should we save Order and Lines together?)
// - Direct access to child entities bypasses parent's validation
//
// AGGREGATE DESIGN RULES:
// 1. Keep aggregates SMALL (2-3 entities max) for performance
// 2. Reference other aggregates by ID only (not direct object references)
// 3. One repository per aggregate root (not per entity)
// 4. Enforce all invariants within the aggregate boundary
// 5. Use eventual consistency between aggregates (domain events)
//
// WHEN TO USE:
// ✅ Multiple entities must change together atomically
// ✅ Business rules span multiple entities
// ✅ Need clear transactional boundaries
// ✅ Want to prevent direct modification of child entities
//
// REAL-WORLD EXAMPLE:
// E-commerce Order aggregate: Order (root) controls OrderLines (children).
// Invariant: Order.Total must ALWAYS equal sum of OrderLines.
// External code calls order.AddLine(product, qty) - Order enforces rules,
// updates children, recalculates total, raises events. Cannot bypass Order
// to modify OrderLines directly, which would break the Total invariant.
// ==============================================================================

namespace RevisionNotesDemo.DomainDrivenDesign;

public static class AggregateRootExamples
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== DDD: AGGREGATES ===\n");
        
        Example1_WithoutAggregate();
        Example2_WithAggregate();
        Example3_AggregateRules();
        Example4_RepositoryPattern();
        
        Console.WriteLine("\n💡 Key Takeaways:");
        Console.WriteLine("   ✅ Aggregate = consistency boundary");
        Console.WriteLine("   ✅ Access children only through root");
        Console.WriteLine("   ✅ Keep aggregates small (2-3 entities max)");
        Console.WriteLine("   ✅ Reference other aggregates by ID only");
        Console.WriteLine("   ✅ One repository per aggregate");
    }
    
    private static void Example1_WithoutAggregate()
    {
        Console.WriteLine("=== EXAMPLE 1: Without Aggregates (Problems) ===\n");
        
        Console.WriteLine("❌ BAD: Anyone can modify related entities directly\n");
        // PROBLEM: Can break invariants
        // var order = await _orderRepo.GetAsync(1);
        // order.Lines[0].Quantity = 100;  // Changed quantity
        // // Total NOT updated! Data inconsistent
        //
        // PROBLEM: Can bypass business rules
        // var line = new OrderLine { OrderId = 1, Quantity = -5 };  // Negative!
        // _lineRepo.Add(line);  // Saved invalid state
        
        Console.WriteLine("\n💥 Problems:");
        Console.WriteLine("   • Business rules not enforced");
        Console.WriteLine("   • Inconsistent state possible");
        Console.WriteLine("   • Unclear what to save together");
        Console.WriteLine("   • No transactional boundary");
    }
    
    private static void Example2_WithAggregate()
    {
        Console.WriteLine("\n=== EXAMPLE 2: With Aggregate Root ===\n");
        
        Console.WriteLine("✅ GOOD: Order is Aggregate Root, controls OrderLines\n");
        // public class Order : AggregateRoot<OrderId> {
        //     public Money Total { get; private set; }
        //     private readonly List<OrderLine> _lines = new();
        //     public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();
        //     
        //     public void AddLine(ProductId productId, Money price, int quantity) {
        //         if (Status != OrderStatus.Draft)
        //             throw new InvalidOperationException();
        //         if (quantity <= 0)
        //             throw new ArgumentException("Quantity must be positive");
        //         
        //         var line = OrderLine.Create(productId, price, quantity);
        //         _lines.Add(line);
        //         RecalculateTotal();
        //         RaiseDomainEvent(new ProductAddedToOrder(Id, productId, quantity));
        //     }
        //     
        //     private void RecalculateTotal() {
        //         Total = _lines.Aggregate(Money.Zero, (sum, line) => sum.Add(line.LineTotal));
        //     }
        // }
        
        Console.WriteLine("\n📊 Benefits:");
        Console.WriteLine("   • Invariants always maintained");
        Console.WriteLine("   • Cannot create invalid state");
        Console.WriteLine("   • Clear transaction boundary");
        Console.WriteLine("   • Business rules in one place");
    }
    
    private static void Example3_AggregateRules()
    {
        Console.WriteLine("\n=== EXAMPLE 3: Aggregate Design Rules ===\n");
        
        Console.WriteLine("✅ Rule 1: Keep aggregates small (2-3 entities max)\n");
        // GOOD: Order + OrderLines (small aggregate)
        // BAD: Customer with all orders (huge aggregate)
        
        Console.WriteLine("✅ Rule 2: Reference other aggregates by ID only\n");
        // public class Order {
        //     public CustomerId CustomerId { get; private set; }  // ✅ By ID
        //     // public Customer Customer { get; set; }  // ❌ Direct reference
        // }
        
        Console.WriteLine("✅ Rule 3: One repository per aggregate root\n");
        // public interface IOrderRepository {
        //     Task<Order> GetAsync(OrderId id);
        //     Task SaveAsync(Order order);  // Saves order + lines together
        // }
        
        Console.WriteLine("✅ Rule 4: Enforce invariants within aggregate boundary\n");
        // Invariant: Total = sum of lines
        // Every modification maintains this invariant
        
        Console.WriteLine("✅ Rule 5: Changes between aggregates use eventual consistency\n");
        // Use domain events for cross-aggregate updates
        // public void Submit() {
        //     Status = OrderStatus.Submitted;
        //     RaiseDomainEvent(new OrderSubmitted(Id, _lines));
        // }
    }
    
    private static void Example4_RepositoryPattern()
    {
        Console.WriteLine("\n=== EXAMPLE 4: Repository for Aggregate ===\n");
        
        Console.WriteLine("✅ Repository saves entire aggregate\n");
        // public class OrderRepository : IOrderRepository {
        //     public async Task<Order?> GetAsync(OrderId id, CancellationToken ct = default) {
        //         return await _context.Orders
        //             .Include(o => o.Lines)  // Load entire aggregate
        //             .FirstOrDefaultAsync(o => o.Id == id, ct);
        //     }
        //     
        //     public async Task SaveAsync(Order order, CancellationToken ct = default) {
        //         if (order.Id.IsNew)
        //             _context.Orders.Add(order);
        //         else
        //             _context.Orders.Update(order);
        //         await _context.SaveChangesAsync(ct);  // Lines saved automatically
        //     }
        // }
        
        Console.WriteLine("\n📊 Benefits:");
        Console.WriteLine("   • Simple persistence (one save call)");
        Console.WriteLine("   • Transaction = aggregate = consistency");
        Console.WriteLine("   • Clear boundaries");
        Console.WriteLine("   • Easy to test");
    }
}
