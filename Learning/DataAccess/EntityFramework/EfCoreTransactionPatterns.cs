// ==============================================================================
// EF CORE TRANSACTION PATTERNS
// ==============================================================================
// WHAT IS THIS?
// EF Core-specific transaction patterns for multi-step writes and shared transactions.
//
// WHY IT MATTERS
// - SaveChanges is atomic per call, but multi-step workflows need explicit boundaries.
// - Sharing transactions across contexts is common in integration and auditing paths.
// ==============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace RevisionNotesDemo.DataAccess.EntityFramework;

/// <summary>
/// EF CORE TRANSACTIONS - DbContext Transaction Management
/// </summary>
public class EfCoreTransactionExamples
{
    // ✅ GOOD: EF Core transaction with multiple saves
    public async Task<bool> CreateOrderWithInventory(AppDbContext context, Order order, List<OrderItem> items)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            foreach (var item in items)
            {
                item.OrderId = order.Id;
                context.OrderItems.Add(item);

                var product = await context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                    if (product.Stock < 0)
                    {
                        throw new InvalidOperationException("Insufficient stock");
                    }
                }
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // ✅ GOOD: Share transaction across contexts
    public async Task<bool> ShareTransaction(AppDbContext context1, AuditDbContext context2)
    {
        using var transaction = await context1.Database.BeginTransactionAsync();

        try
        {
            await context2.Database.UseTransactionAsync(transaction.GetDbTransaction());

            context1.Orders.Add(new Order { Total = 100 });
            await context1.SaveChangesAsync();

            context2.AuditLogs.Add(new AuditLog { Message = "Order created" });
            await context2.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // ✅ GOOD: Automatic transaction for single SaveChanges
    public async Task<Order> CreateOrder_AutomaticTransaction(AppDbContext context, Order order)
    {
        context.Orders.Add(order);
        await context.SaveChangesAsync();
        return order;
    }

    // Supporting classes
    public class AppDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
    }

    public class AuditDbContext : DbContext
    {
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    }

    public class Order
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public int Stock { get; set; }
    }

    public class AuditLog
    {
        public int Id { get; set; }
        public string Message { get; set; } = "";
    }
}
