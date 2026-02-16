// ==============================================================================
// SQL GRAPH DATA TRANSFER PATTERNS (CUSTOMERS -> ORDERS -> ORDER ITEMS)
// ==============================================================================
// WHAT IS THIS?
// End-to-end patterns for reading object graphs from SQL and writing changes back.
//
// WHY IT MATTERS
// - Naive graph loading causes N+1 round-trips and poor latency.
// - Naive graph updates can over-write data, cause deadlocks, or lose intent.
// - Set-based batched patterns are faster and safer under concurrency.
// ==============================================================================

using System.Data;
using Microsoft.Data.SqlClient;

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class SqlGraphDataTransferPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n--- SQL GRAPH DATA TRANSFER PATTERNS ---\n");
        ShowReadPatterns();
        ShowWritePatterns();
        ShowCSharpDeltaExample();
    }

    private static void ShowReadPatterns()
    {
        Console.WriteLine("READ graph patterns:\n");

        Console.WriteLine("❌ BAD: N+1 reads");
        Console.WriteLine("  1 query for customers");
        Console.WriteLine("  + N queries for orders");
        Console.WriteLine("  + N*M queries for order items\n");

        Console.WriteLine("✅ GOOD: batched graph read with 3 result sets");
        Console.WriteLine("  SELECT CustomerId, Name FROM dbo.Customers WHERE CustomerId IN (...)");
        Console.WriteLine("  SELECT OrderId, CustomerId, Status FROM dbo.Orders WHERE CustomerId IN (...)");
        Console.WriteLine("  SELECT OrderItemId, OrderId, ProductId, Quantity");
        Console.WriteLine("  FROM dbo.OrderItems WHERE OrderId IN (...)\n");

        Console.WriteLine("C# assembly approach:");
        Console.WriteLine("- Read each result set once.");
        Console.WriteLine("- Build dictionaries by key (CustomerId, OrderId).");
        Console.WriteLine("- Attach children in memory without additional SQL round trips.\n");
    }

    private static void ShowWritePatterns()
    {
        Console.WriteLine("WRITE graph patterns:\n");

        Console.WriteLine("❌ BAD: delete-all then reinsert-all on every update");
        Console.WriteLine("Problems: lock amplification, identity churn, auditing noise, race risk.\n");

        Console.WriteLine("✅ GOOD: diff-based update");
        Console.WriteLine("- Determine adds/updates/deletes in memory.");
        Console.WriteLine("- Send changes in batches (TVP or staging table).");
        Console.WriteLine("- Apply in one transaction with deterministic lock order.\n");

        Console.WriteLine("Recommended SQL write boundary:");
        Console.WriteLine("1) Upsert parent customers");
        Console.WriteLine("2) Upsert child orders");
        Console.WriteLine("3) Upsert order items");
        Console.WriteLine("4) Delete removed rows (if true replacement semantics)");
        Console.WriteLine("5) Commit transaction\n");
    }

    private static void ShowCSharpDeltaExample()
    {
        Console.WriteLine("C# delta update example (good pattern):\n");

        var existing = SampleData.ExistingGraph();
        var incoming = SampleData.IncomingGraph();

        var delta = GraphDeltaBuilder.Build(existing, incoming);

        Console.WriteLine($"Customers: add={delta.CustomersToInsert.Count}, update={delta.CustomersToUpdate.Count}, delete={delta.CustomerIdsToDelete.Count}");
        Console.WriteLine($"Orders: add={delta.OrdersToInsert.Count}, update={delta.OrdersToUpdate.Count}, delete={delta.OrderIdsToDelete.Count}");
        Console.WriteLine($"OrderItems: add={delta.ItemsToInsert.Count}, update={delta.ItemsToUpdate.Count}, delete={delta.ItemIdsToDelete.Count}\n");

        Console.WriteLine("SQL transport best practices:");
        Console.WriteLine("- Use TVPs for batched input");
        Console.WriteLine("- Keep transaction short and set-based");
        Console.WriteLine("- Return rowversion/updated timestamps for optimistic concurrency");
        Console.WriteLine("- Validate affected-row counts and surface conflicts to API callers\n");
    }
}

public static class GraphDeltaBuilder
{
    public static GraphDelta Build(List<CustomerGraph> existing, List<CustomerGraph> incoming)
    {
        var existingCustomers = existing.ToDictionary(c => c.CustomerId);
        var incomingCustomers = incoming.ToDictionary(c => c.CustomerId);

        var customersToInsert = incoming.Where(c => !existingCustomers.ContainsKey(c.CustomerId)).ToList();
        var customersToUpdate = incoming
            .Where(c => existingCustomers.TryGetValue(c.CustomerId, out var ex) && (ex.Name != c.Name || ex.Email != c.Email))
            .ToList();
        var customerIdsToDelete = existing.Where(c => !incomingCustomers.ContainsKey(c.CustomerId)).Select(c => c.CustomerId).ToList();

        var existingOrders = existing.SelectMany(c => c.Orders).ToDictionary(o => o.OrderId);
        var incomingOrders = incoming.SelectMany(c => c.Orders).ToDictionary(o => o.OrderId);

        var ordersToInsert = incomingOrders.Values.Where(o => !existingOrders.ContainsKey(o.OrderId)).ToList();
        var ordersToUpdate = incomingOrders.Values
            .Where(o => existingOrders.TryGetValue(o.OrderId, out var ex) && (ex.Status != o.Status || ex.CustomerId != o.CustomerId))
            .ToList();
        var orderIdsToDelete = existingOrders.Values.Where(o => !incomingOrders.ContainsKey(o.OrderId)).Select(o => o.OrderId).ToList();

        var existingItems = existing.SelectMany(c => c.Orders).SelectMany(o => o.Items).ToDictionary(i => i.OrderItemId);
        var incomingItems = incoming.SelectMany(c => c.Orders).SelectMany(o => o.Items).ToDictionary(i => i.OrderItemId);

        var itemsToInsert = incomingItems.Values.Where(i => !existingItems.ContainsKey(i.OrderItemId)).ToList();
        var itemsToUpdate = incomingItems.Values
            .Where(i => existingItems.TryGetValue(i.OrderItemId, out var ex) &&
                        (ex.Quantity != i.Quantity || ex.UnitPrice != i.UnitPrice || ex.ProductId != i.ProductId || ex.OrderId != i.OrderId))
            .ToList();
        var itemIdsToDelete = existingItems.Values.Where(i => !incomingItems.ContainsKey(i.OrderItemId)).Select(i => i.OrderItemId).ToList();

        return new GraphDelta(
            customersToInsert, customersToUpdate, customerIdsToDelete,
            ordersToInsert, ordersToUpdate, orderIdsToDelete,
            itemsToInsert, itemsToUpdate, itemIdsToDelete);
    }
}

public sealed record GraphDelta(
    List<CustomerGraph> CustomersToInsert,
    List<CustomerGraph> CustomersToUpdate,
    List<long> CustomerIdsToDelete,
    List<OrderGraph> OrdersToInsert,
    List<OrderGraph> OrdersToUpdate,
    List<long> OrderIdsToDelete,
    List<OrderItemGraph> ItemsToInsert,
    List<OrderItemGraph> ItemsToUpdate,
    List<long> ItemIdsToDelete);

public sealed record CustomerGraph(long CustomerId, string Name, string Email, List<OrderGraph> Orders);
public sealed record OrderGraph(long OrderId, long CustomerId, string Status, List<OrderItemGraph> Items);
public sealed record OrderItemGraph(long OrderItemId, long OrderId, long ProductId, int Quantity, decimal UnitPrice);

internal static class SampleData
{
    public static List<CustomerGraph> ExistingGraph() =>
    [
        new CustomerGraph(
            1, "Contoso Retail", "ops@contoso.com",
            [
                new OrderGraph(
                    1001, 1, "Pending",
                    [
                        new OrderItemGraph(5001, 1001, 201, 2, 10m),
                        new OrderItemGraph(5002, 1001, 202, 1, 25m)
                    ])
            ])
    ];

    public static List<CustomerGraph> IncomingGraph() =>
    [
        new CustomerGraph(
            1, "Contoso Retail", "operations@contoso.com",
            [
                new OrderGraph(
                    1001, 1, "Paid",
                    [
                        new OrderItemGraph(5001, 1001, 201, 3, 10m),
                        new OrderItemGraph(5003, 1001, 203, 1, 15m)
                    ])
            ]),
        new CustomerGraph(
            2, "Fabrikam Foods", "finance@fabrikam.com",
            [])
    ];
}

public static class SqlGraphTransportTemplates
{
    // Template only: demonstrates how graph deltas should be shipped to SQL.
    public static async Task ExecuteDeltaAsync(SqlConnection connection, GraphDelta delta, CancellationToken cancellationToken)
    {
        using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            // Typical production pattern:
            // 1) Convert delta collections to DataTable TVPs.
            // 2) Execute stored procedures that apply set-based upsert/delete.
            // 3) Commit if all succeed.

            await Task.CompletedTask;
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    // Utility shape for TVP creation in production code paths.
    public static DataTable BuildCustomerTvp(IEnumerable<CustomerGraph> customers)
    {
        var table = new DataTable();
        table.Columns.Add("CustomerId", typeof(long));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Email", typeof(string));

        foreach (var c in customers)
        {
            table.Rows.Add(c.CustomerId, c.Name, c.Email);
        }

        return table;
    }
}
