// ==============================================================================
// TRANSACTION PATTERNS - ACID Guarantees and Consistency
// ==============================================================================
//
// WHAT ARE TRANSACTIONS?
// ----------------------
// A transaction groups operations into an all-or-nothing unit of work.
// If one step fails, the whole transaction rolls back to preserve consistency.
//
// WHY IT MATTERS
// --------------
// - Prevents partial updates (data integrity)
// - Handles concurrent access safely
// - Supports rollback and recovery
// - Required for financial and inventory systems
//
// WHEN TO USE
// -----------
// - YES: Multiple related updates must succeed together
// - YES: Money transfers, inventory updates, order processing
// - YES: Cross-table writes that must stay consistent
//
// WHEN NOT TO USE
// ---------------
// - NO: Single simple updates (SaveChanges is already atomic)
// - NO: Long-running operations that would hold locks too long
//
// REAL-WORLD EXAMPLE
// ------------------
// Bank transfer:
// - Debit account A and credit account B in a single transaction
// - If credit fails, debit is rolled back
// ==============================================================================

using System.Data;
using Microsoft.Data.SqlClient;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace RevisionNotesDemo.DataAccess;

/// <summary>
/// EXAMPLE 1: BASIC TRANSACTIONS - All-or-Nothing Operations
/// 
/// THE PROBLEM:
/// Transfer money: Debit account A, credit account B.
/// If credit fails, must rollback debit.
/// 
/// THE SOLUTION:
/// Wrap operations in transaction. Commit all or rollback all.
/// 
/// WHY IT MATTERS:
/// - Data consistency (no partial transfers)
/// - Atomicity guarantee
/// - Automatic rollback on exceptions
/// </summary>
public class BasicTransactionExamples
{
    private readonly string _connectionString = "ConnectionString";

    // ❌ BAD: No transaction - partial updates possible
    public async Task<bool> TransferMoney_NoTransaction(int fromId, int toId, decimal amount)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // Debit from account
        var debitSql = "UPDATE Accounts SET Balance = Balance - @Amount WHERE Id = @Id";
        using (var command = new SqlCommand(debitSql, connection))
        {
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@Id", fromId);
            await command.ExecuteNonQueryAsync();
        }

        // ❌ If exception here, money is debited but not credited!
        // throw new Exception("Crash!");

        // Credit to account
        var creditSql = "UPDATE Accounts SET Balance = Balance + @Amount WHERE Id = @Id";
        using (var command = new SqlCommand(creditSql, connection))
        {
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@Id", toId);
            await command.ExecuteNonQueryAsync();
        }

        return true;  // ❌ Money could be lost or duplicated!
    }

    // ✅ GOOD: Transaction ensures atomicity
    public async Task<bool> TransferMoney_WithTransaction(int fromId, int toId, decimal amount)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();  // ✅ Start transaction

        try
        {
            // Debit from account
            var debitSql = "UPDATE Accounts SET Balance = Balance - @Amount WHERE Id = @Id";
            using (var command = new SqlCommand(debitSql, connection, transaction))  // ✅ Pass transaction
            {
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@Id", fromId);
                await command.ExecuteNonQueryAsync();
            }

            // Credit to account
            var creditSql = "UPDATE Accounts SET Balance = Balance + @Amount WHERE Id = @Id";
            using (var command = new SqlCommand(creditSql, connection, transaction))  // ✅ Pass transaction
            {
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@Id", toId);
                await command.ExecuteNonQueryAsync();
            }

            transaction.Commit();  // ✅ Both succeeded, commit
            return true;
        }
        catch
        {
            transaction.Rollback();  // ✅ Error occurred, rollback both
            throw;
        }
    }

    // ✅ BETTER: Async transaction (SQL Server 2012+)
    public async Task<bool> TransferMoney_AsyncTransaction(int fromId, int toId, decimal amount)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();  // ✅ Async begin

        try
        {
            var debitSql = "UPDATE Accounts SET Balance = Balance - @Amount WHERE Id = @Id";
            using (var command = new SqlCommand(debitSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@Id", fromId);
                await command.ExecuteNonQueryAsync();
            }

            var creditSql = "UPDATE Accounts SET Balance = Balance + @Amount WHERE Id = @Id";
            using (var command = new SqlCommand(creditSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@Id", toId);
                await command.ExecuteNonQueryAsync();
            }

            await transaction.CommitAsync();  // ✅ Async commit
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();  // ✅ Async rollback
            throw;
        }
    }
}

/// <summary>
/// EXAMPLE 2: EF CORE TRANSACTIONS - DbContext Transaction Management
/// 
/// THE PROBLEM:
/// SaveChanges commits immediately. Need multiple SaveChanges in one transaction.
/// 
/// THE SOLUTION:
/// Use Database.BeginTransaction() for explicit control.
/// </summary>
public class EfCoreTransactionExamples
{
    // ✅ GOOD: EF Core transaction with multiple saves
    public async Task<bool> CreateOrderWithInventory(AppDbContext context, Order order, List<OrderItem> items)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // 1. Create order
            context.Orders.Add(order);
            await context.SaveChangesAsync();  // ✅ First save

            // 2. Add order items
            foreach (var item in items)
            {
                item.OrderId = order.Id;
                context.OrderItems.Add(item);

                // 3. Update inventory
                var product = await context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                    if (product.Stock < 0)
                        throw new InvalidOperationException("Insufficient stock");
                }
            }

            await context.SaveChangesAsync();  // ✅ Second save

            await transaction.CommitAsync();  // ✅ All succeeded
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();  // ✅ Undo all changes
            throw;
        }
    }

    // ✅ GOOD: Share transaction across contexts
    public async Task<bool> ShareTransaction(AppDbContext context1, AuditDbContext context2)
    {
        using var transaction = await context1.Database.BeginTransactionAsync();

        try
        {
            // Use same transaction in both contexts
            await context2.Database.UseTransactionAsync(transaction.GetDbTransaction());  // ✅ Share

            // Changes in context1
            context1.Orders.Add(new Order { Total = 100 });
            await context1.SaveChangesAsync();

            // Changes in context2
            context2.AuditLogs.Add(new AuditLog { Message = "Order created" });
            await context2.SaveChangesAsync();

            await transaction.CommitAsync();  // ✅ Both databases committed
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
        // ✅ SaveChanges automatically wraps in transaction
        context.Orders.Add(order);
        await context.SaveChangesAsync();  // ✅ Atomic by default

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

    public class Order { public int Id { get; set; } public decimal Total { get; set; } }
    public class OrderItem { public int Id { get; set; } public int OrderId { get; set; } public int ProductId { get; set; } public int Quantity { get; set; } }
    public class Product { public int Id { get; set; } public int Stock { get; set; } }
    public class AuditLog { public int Id { get; set; } public string Message { get; set; } = ""; }
}

/// <summary>
/// EXAMPLE 3: ISOLATION LEVELS - Controlling Concurrent Access
/// 
/// THE PROBLEM:
/// Multiple transactions accessing same data simultaneously.
/// Need to balance consistency vs. performance.
/// 
/// THE SOLUTION:
/// Choose appropriate isolation level.
/// 
/// WHY IT MATTERS:
/// - Higher isolation = more consistency, less concurrency
/// - Lower isolation = better performance, more anomalies
/// </summary>
public class IsolationLevelExamples
{
    private readonly string _connectionString = "ConnectionString";

    // ✅ Read Uncommitted: Fastest, allows dirty reads
    public async Task<List<Product>> GetProducts_ReadUncommitted()
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // ✅ Lowest isolation: Can read uncommitted changes from other transactions
        using var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

        using var command = new SqlCommand("SELECT * FROM Products", connection, transaction);
        using var reader = await command.ExecuteReaderAsync();

        var products = new List<Product>();
        while (await reader.ReadAsync())
        {
            products.Add(new Product { Id = reader.GetInt32(0) });
        }

        // Use case: Dashboard stats where exact precision doesn't matter
        // GOTCHA: Can see "dirty reads" - changes not yet committed

        return products;
    }

    // ✅ Read Committed: Default, prevents dirty reads
    public async Task<Product?> GetProduct_ReadCommitted(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // ✅ Default: Only read committed data
        using var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

        using var command = new SqlCommand("SELECT * FROM Products WHERE Id = @Id", connection, transaction);
        command.Parameters.AddWithValue("@Id", id);
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return new Product { Id = reader.GetInt32(0) };

        // GOTCHA: Still allows "non-repeatable reads" - data can change between reads

        return null;
    }

    // ✅ Repeatable Read: Prevents non-repeatable reads
    public async Task<bool> UpdateProduct_RepeatableRead(int id, decimal newPrice)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // ✅ Locks rows to prevent changes during transaction
        using var transaction = connection.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);

        try
        {
            // Read product
            decimal currentPrice;
            using (var readCmd = new SqlCommand("SELECT Price FROM Products WHERE Id = @Id", connection, transaction))
            {
                readCmd.Parameters.AddWithValue("@Id", id);
                currentPrice = (decimal)(await readCmd.ExecuteScalarAsync() ?? 0m);
            }

            // Business logic
            if (currentPrice > newPrice * 0.5m)  // Max 50% discount
            {
                // Update price
                using var updateCmd = new SqlCommand(
                    "UPDATE Products SET Price = @Price WHERE Id = @Id",
                    connection, transaction);
                updateCmd.Parameters.AddWithValue("@Price", newPrice);
                updateCmd.Parameters.AddWithValue("@Id", id);
                await updateCmd.ExecuteNonQueryAsync();

                transaction.Commit();
                return true;
            }

            transaction.Rollback();
            return false;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // ✅ Serializable: Highest isolation, full consistency
    public async Task<bool> ProcessOrder_Serializable(int productId, int quantity)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        // ✅ Highest isolation: Acts as if transactions run serially
        using var transaction = connection.BeginTransaction(System.Data.IsolationLevel.Serializable);

        try
        {
            // Check stock
            int stock;
            using (var checkCmd = new SqlCommand(
                "SELECT Stock FROM Products WHERE Id = @Id",
                connection, transaction))
            {
                checkCmd.Parameters.AddWithValue("@Id", productId);
                stock = (int)(await checkCmd.ExecuteScalarAsync() ?? 0);
            }

            if (stock >= quantity)
            {
                // Deduct stock
                using var updateCmd = new SqlCommand(
                    "UPDATE Products SET Stock = Stock - @Quantity WHERE Id = @Id",
                    connection, transaction);
                updateCmd.Parameters.AddWithValue("@Quantity", quantity);
                updateCmd.Parameters.AddWithValue("@Id", productId);
                await updateCmd.ExecuteNonQueryAsync();

                transaction.Commit();
                return true;
            }

            transaction.Rollback();
            return false;  // Insufficient stock
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public class Product { public int Id { get; set; } public decimal Price { get; set; } public int Stock { get; set; } }
}

// ISOLATION LEVELS SUMMARY:
//
// +--------------------+-------------+-----------------+----------------+-------------+
// | Isolation Level    | Dirty Read  | Non-Repeatable  | Phantom Read   | Performance |
// |                    |             | Read            |                |             |
// +--------------------+-------------+-----------------+----------------+-------------+
// | Read Uncommitted   | ✗ Possible  | ✗ Possible      | ✗ Possible     | ✅ Fastest  |
// | Read Committed     | ✅ Prevented| ✗ Possible      | ✗ Possible     | ✅ Fast     |
// | Repeatable Read    | ✅ Prevented| ✅ Prevented    | ✗ Possible     | ⚠️ Slower   |
// | Serializable       | ✅ Prevented| ✅ Prevented    | ✅ Prevented   | ❌ Slowest  |
// +--------------------+-------------+-----------------+----------------+-------------+
//
// WHEN TO USE:
// - Read Uncommitted: Dashboards, reports (approx ok)
// - Read Committed: Default for most operations
// - Repeatable Read: Financial operations
// - Serializable: Critical inventory/booking systems

/// <summary>
/// EXAMPLE 4: TRANSACTIONSCOPE - Distributed Transactions
/// 
/// THE PROBLEM:
/// Need transaction across multiple databases or resources.
/// 
/// THE SOLUTION:
/// TransactionScope coordinates distributed transactions.
/// 
/// GOTCHA: Requires MSDTC (complex setup), prefer single-database transactions.
/// </summary>
public class TransactionScopeExamples
{
    private readonly string _connectionString1 = "Server=.;Database=Db1;Trusted_Connection=true";
    private readonly string _connectionString2 = "Server=.;Database=Db2;Trusted_Connection=true";

    // ✅ GOOD: TransactionScope for ambient transaction
    public async Task<bool> CreateOrderAndAudit_TransactionScope(Order order)
    {
        // ✅ Ambient transaction - automatically enrolled
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);  // ✅ REQUIRED for async

        try
        {
            // Operation 1: Main database
            using (var connection1 = new SqlConnection(_connectionString1))
            {
                await connection1.OpenAsync();
                using var command = new SqlCommand("INSERT INTO Orders VALUES (@Total)", connection1);
                command.Parameters.AddWithValue("@Total", order.Total);
                await command.ExecuteNonQueryAsync();
            }

            // Operation 2: Audit database
            using (var connection2 = new SqlConnection(_connectionString2))
            {
                await connection2.OpenAsync();
                using var command = new SqlCommand("INSERT INTO AuditLog VALUES (@Message)", connection2);
                command.Parameters.AddWithValue("@Message", "Order created");
                await command.ExecuteNonQueryAsync();
            }

            scope.Complete();  // ✅ Commit both databases
            return true;
        }
        catch
        {
            // ✅ Automatic rollback if Complete() not called
            throw;
        }
    }

    // ⚠️ IMPORTANT: TransactionScopeAsyncFlowOption.Enabled
    // Without it, transaction doesn't flow across await boundaries!

    public class Order { public decimal Total { get; set; } }
}

/// <summary>
/// EXAMPLE 5: DEADLOCK HANDLING - Avoiding and Recovering from Deadlocks
/// 
/// THE PROBLEM:
/// Transaction A locks Table 1, waits for Table 2.
/// Transaction B locks Table 2, waits for Table 1.
/// = Deadlock! One transaction rolled back by SQL Server.
/// 
/// THE SOLUTION:
/// Consistent lock order, retry logic, minimize lock duration.
/// </summary>
public class DeadlockHandlingExamples
{
    private readonly string _connectionString = "ConnectionString";

    // ✅ GOOD: Retry logic for deadlock victim
    public async Task<bool> UpdateWithRetry(int productId, int quantity, int maxRetries = 3)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                return await UpdateProduct(productId, quantity);
            }
            catch (SqlException ex) when (ex.Number == 1205)  // ✅ Deadlock error code
            {
                if (attempt == maxRetries - 1)
                    throw;

                // ✅ Exponential backoff
                await Task.Delay(TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)));
            }
        }

        return false;
    }

    private async Task<bool> UpdateProduct(int productId, int quantity)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            // ✅ BEST PRACTICE: Always access tables in same order across app
            // Good: Always Products → OrderItems → Orders
            // Bad: Sometimes Orders → Products, other times Products → Orders

            using var command = new SqlCommand(
                "UPDATE Products SET Stock = Stock - @Quantity WHERE Id = @Id",
                connection, (SqlTransaction)transaction);
            command.Parameters.AddWithValue("@Quantity", quantity);
            command.Parameters.AddWithValue("@Id", productId);
            await command.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

// SUMMARY - Transaction Best Practices:
//
// ✅ DO:
// - Use transactions for multiple related operations
// - Commit or rollback explicitly
// - Keep transactions short (minimize lock duration)
// - Use appropriate isolation level (default: Read Committed)
// - Handle SqlException with error code 1205 (deadlock)
// - Access tables in consistent order
// - Use TransactionScopeAsyncFlowOption.Enabled for async
//
// ❌ DON'T:
// - Leave transactions open during user input
// - Use Serializable unless absolutely necessary
// - Ignore deadlock exceptions (implement retry)
// - Mix ambient and explicit transactions
// - Hold transactions across API calls
//
// DECISION TREE:
// Q: Single database, single operation?
//    → No transaction needed (SaveChanges is atomic)
// Q: Multiple operations, same database?
//    → Use DbTransaction or Database.BeginTransaction()
// Q: Multiple databases?
//    → Use TransactionScope (or avoid if possible)
// Q: High contention?
//    → Lower isolation level + retry logic
// Q: Financial/critical data?
//    → Higher isolation level (RepeatableRead/Serializable)
