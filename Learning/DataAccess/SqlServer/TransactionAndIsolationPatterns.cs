// ==============================================================================
// SQL SERVER TRANSACTION, ISOLATION, AND DEADLOCK PATTERNS
// ==============================================================================
//
// WHAT ARE TRANSACTIONS?
// A transaction groups operations into an all-or-nothing unit of work.
// If one step fails, the whole transaction rolls back to preserve consistency.
//
// WHY IT MATTERS
// - Prevents partial updates (data integrity)
// - Handles concurrent access safely
// - Supports rollback and recovery
// - Required for financial and inventory systems
// ==============================================================================

using Microsoft.Data.SqlClient;
using System.Transactions;

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class TransactionAndIsolationPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n--- TRANSACTION AND ISOLATION PATTERNS ---\n");
        Console.WriteLine("Includes:");
        Console.WriteLine("- Basic SQL transaction boundaries");
        Console.WriteLine("- Isolation level tradeoffs");
        Console.WriteLine("- TransactionScope for distributed boundaries");
        Console.WriteLine("- Deadlock retry strategy (error 1205)\n");
        Console.WriteLine("Review classes:");
        Console.WriteLine("- BasicTransactionExamples");
        Console.WriteLine("- IsolationLevelExamples");
        Console.WriteLine("- TransactionScopeExamples");
        Console.WriteLine("- DeadlockHandlingExamples\n");
    }
}

/// <summary>
/// EXAMPLE 1: BASIC TRANSACTIONS - All-or-Nothing Operations
/// </summary>
public class BasicTransactionExamples
{
    private readonly string _connectionString = "ConnectionString";

    // ❌ BAD: No transaction - partial updates possible
    public async Task<bool> TransferMoney_NoTransaction(int fromId, int toId, decimal amount)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var debitSql = "UPDATE Accounts SET Balance = Balance - @Amount WHERE Id = @Id";
        using (var command = new SqlCommand(debitSql, connection))
        {
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@Id", fromId);
            await command.ExecuteNonQueryAsync();
        }

        var creditSql = "UPDATE Accounts SET Balance = Balance + @Amount WHERE Id = @Id";
        using (var command = new SqlCommand(creditSql, connection))
        {
            command.Parameters.AddWithValue("@Amount", amount);
            command.Parameters.AddWithValue("@Id", toId);
            await command.ExecuteNonQueryAsync();
        }

        return true;
    }

    // ✅ GOOD: Transaction ensures atomicity
    public async Task<bool> TransferMoney_WithTransaction(int fromId, int toId, decimal amount)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

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

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    // ✅ BETTER: Async transaction (SQL Server 2012+)
    public async Task<bool> TransferMoney_AsyncTransaction(int fromId, int toId, decimal amount)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = (SqlTransaction)await connection.BeginTransactionAsync();

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

/// <summary>
/// EXAMPLE 2: ISOLATION LEVELS - Controlling Concurrent Access
/// </summary>
public class IsolationLevelExamples
{
    private readonly string _connectionString = "ConnectionString";

    public async Task<List<Product>> GetProducts_ReadUncommitted()
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        using var command = new SqlCommand("SELECT * FROM Products", connection, transaction);
        using var reader = await command.ExecuteReaderAsync();

        var products = new List<Product>();
        while (await reader.ReadAsync())
        {
            products.Add(new Product { Id = reader.GetInt32(0) });
        }

        return products;
    }

    public async Task<Product?> GetProduct_ReadCommitted(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        using var command = new SqlCommand("SELECT * FROM Products WHERE Id = @Id", connection, transaction);
        command.Parameters.AddWithValue("@Id", id);
        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Product { Id = reader.GetInt32(0) };
        }

        return null;
    }

    public async Task<bool> UpdateProduct_RepeatableRead(int id, decimal newPrice)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction(System.Data.IsolationLevel.RepeatableRead);

        try
        {
            decimal currentPrice;
            using (var readCmd = new SqlCommand("SELECT Price FROM Products WHERE Id = @Id", connection, transaction))
            {
                readCmd.Parameters.AddWithValue("@Id", id);
                currentPrice = (decimal)(await readCmd.ExecuteScalarAsync() ?? 0m);
            }

            if (currentPrice > newPrice * 0.5m)
            {
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

    public async Task<bool> ProcessOrder_Serializable(int productId, int quantity)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction(System.Data.IsolationLevel.Serializable);

        try
        {
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
            return false;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}

/// <summary>
/// EXAMPLE 3: TRANSACTIONSCOPE - Distributed Transactions
/// </summary>
public class TransactionScopeExamples
{
    private readonly string _connectionString1 = "Server=.;Database=Db1;Trusted_Connection=true";
    private readonly string _connectionString2 = "Server=.;Database=Db2;Trusted_Connection=true";

    public async Task<bool> CreateOrderAndAudit_TransactionScope(Order order)
    {
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
            TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            using (var connection1 = new SqlConnection(_connectionString1))
            {
                await connection1.OpenAsync();
                using var command = new SqlCommand("INSERT INTO Orders VALUES (@Total)", connection1);
                command.Parameters.AddWithValue("@Total", order.Total);
                await command.ExecuteNonQueryAsync();
            }

            using (var connection2 = new SqlConnection(_connectionString2))
            {
                await connection2.OpenAsync();
                using var command = new SqlCommand("INSERT INTO AuditLog VALUES (@Message)", connection2);
                command.Parameters.AddWithValue("@Message", "Order created");
                await command.ExecuteNonQueryAsync();
            }

            scope.Complete();
            return true;
        }
        catch
        {
            throw;
        }
    }

    public class Order
    {
        public decimal Total { get; set; }
    }
}

/// <summary>
/// EXAMPLE 4: DEADLOCK HANDLING - Avoiding and Recovering from Deadlocks
/// </summary>
public class DeadlockHandlingExamples
{
    private readonly string _connectionString = "ConnectionString";

    public async Task<bool> UpdateWithRetry(int productId, int quantity, int maxRetries = 3)
    {
        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                return await UpdateProduct(productId, quantity);
            }
            catch (SqlException ex) when (ex.Number == 1205)
            {
                if (attempt == maxRetries - 1)
                {
                    throw;
                }

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
