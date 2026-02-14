// ============================================================================
// STRUCT VS CLASS
// Reference: Revision Notes - Page 9
// ============================================================================
// Struct: Value type, stack, no inheritance, better for small immutable data
// Class: Reference type, heap, supports inheritance
// ============================================================================

namespace RevisionNotesDemo.MemoryManagement;

// From Revision Notes - Page 9: Small, immutable struct
public readonly struct Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public override string ToString() => $"{Amount} {Currency}";
}

public class Order
{
    public Money Total { get; private set; }
    public int OrderId { get; set; }

    public Order(Money total, int orderId)
    {
        Total = total;
        OrderId = orderId;
    }
}

public class StructVsClassDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== STRUCT VS CLASS DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Page 9\n");

        // Struct - value type behavior
        var total = new Money(19.99m, "GBP");
        var order = new Order(total, 1001);

        Console.WriteLine($"[STRUCT] Order #{order.OrderId}: {order.Total}");

        Console.WriteLine("\n💡 From Revision Notes:");
        Console.WriteLine("   - Struct: Value type, stack, no inheritance");
        Console.WriteLine("   - Class: Reference type, heap, supports inheritance");
        Console.WriteLine("   - Use struct for small, immutable data like coordinates, money");
        Console.WriteLine("   - Use class for objects with identity and behavior");
    }
}
