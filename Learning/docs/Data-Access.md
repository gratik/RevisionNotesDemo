# Data Access Patterns (ADO.NET, Dapper, EF Core)

> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../../README.md)

## Overview

This guide covers three primary data access approaches in .NET: ADO.NET (raw SQL),
Dapper (micro-ORM), and Entity Framework Core (full ORM). Each has distinct performance
characteristics, complexity tradeoffs, and ideal use cases. This document focuses on
ADO.NET and Dapper patterns; see [Entity-Framework.md](Entity-Framework.md) for EF Core.

---

## Choosing the Right Tool

### Quick Comparison

| Feature               | ADO.NET                | Dapper                    | EF Core                    |
| --------------------- | ---------------------- | ------------------------- | -------------------------- |
| **Performance**       | Fastest (baseline)     | Very Fast (90-95% of ADO) | Slower (tracking overhead) |
| **Learning Curve**    | Steepest               | Moderate                  | Easiest                    |
| **SQL Control**       | Full                   | Full                      | Limited (LINQ translated)  |
| **Object Mapping**    | Manual                 | Automatic                 | Automatic + Navigation     |
| **Change Tracking**   | None                   | None                      | Built-in                   |
| **Migrations**        | Manual                 | Manual                    | Automatic                  |
| **Best For**          | Bulk ops, stored procs | Micro-services, APIs      | Complex domains, CRUD apps |
| **Lines of Code**     | Most                   | Moderate                  | Least                      |
| **Query Flexibility** | Complete               | Complete                  | Limited by LINQ            |

### Performance Characteristics (10,000 rows)

- **ADO.NET**: ~50ms (baseline, manual mapping)
- **Dapper**: ~55ms (10% overhead for automatic mapping)
- **EF Core Tracked**: ~450ms (9x slower, change tracking + complex SQL)
- **EF Core NoTracking**: ~150ms (3x slower, still generates joins)

### Decision Tree

```
Need complex relationships + automatic migrations? → EF Core
    ↓ No
Performance critical (< 100ms SLA)? → Dapper or ADO.NET
    ↓ Bulk operations (10k+ rows)?
    Yes → ADO.NET (SqlBulkCopy)
    No → Dapper (cleaner code, good performance)
```

---

## ADO.NET Fundamentals

### Connection Management

**The Foundation:** All .NET data access is built on ADO.NET's `SqlConnection`.

```csharp
// ✅ GOOD: Using statement ensures disposal
using var connection = new SqlConnection(connString);
await connection.OpenAsync();

// Query database
using var command = new SqlCommand("SELECT Name FROM Users", connection);
using var reader = await command.ExecuteReaderAsync();

while (await reader.ReadAsync())
{
    var name = reader.GetString(0);
}
// ✅ Connection automatically returned to pool
```

```csharp
// ❌ BAD: Connection never closed - pool exhaustion!
var connection = new SqlConnection(connString);
await connection.OpenAsync();

var command = new SqlCommand("SELECT Name FROM Users", connection);
var reader = await command.ExecuteReaderAsync();
// ❌ Connection leaked - will exhaust pool after ~100 requests
```

**Connection Pooling:**

- ADO.NET automatically pools connections by connection string
- `using` returns connection to pool (doesn't destroy it)
- Default: Min=0, Max=100 connections per pool
- Different connection strings = separate pools (watch trailing semicolons!)

```csharp
// Custom pool settings
var connString = "Server=.;Database=MyDb;Trusted_Connection=true;" +
                 "Min Pool Size=5;Max Pool Size=200;Pooling=true";
```

### SqlCommand Execution Methods

**Choose the right method for your operation:**

```csharp
// ExecuteReader: SELECT queries (returns rows)
using var reader = await command.ExecuteReaderAsync();
while (await reader.ReadAsync())
{
    var id = reader.GetInt32(0);
    var name = reader.GetString(1);
}

// ExecuteNonQuery: INSERT/UPDATE/DELETE (returns row count)
var rowsAffected = await command.ExecuteNonQueryAsync();

// ExecuteScalar: Single value (COUNT, SUM, new ID)
var count = (int)await command.ExecuteScalarAsync();
```

### Parameterized Queries (SQL Injection Prevention)

```csharp
// ❌ DANGEROUS: SQL Injection vulnerability!
var sql = $"SELECT * FROM Users WHERE Name = '{userName}'";
// If userName = "'; DROP TABLE Users; --" → DISASTER

// ✅ SAFE: Parameterized query
var sql = "SELECT * FROM Users WHERE Name = @Name AND Age > @Age";
using var command = new SqlCommand(sql, connection);
command.Parameters.AddWithValue("@Name", userName);
command.Parameters.AddWithValue("@Age", 18);

var reader = await command.ExecuteReaderAsync();
```

---

## Dapper: The Micro-ORM

**The Sweet Spot:** SQL control + automatic object mapping without EF overhead.

### Basic Queries

```csharp
using Dapper;

// Query list of objects
var users = await connection.QueryAsync<User>(
    "SELECT Id, FirstName, LastName, Email FROM Users");

// Query single object
var user = await connection.QueryFirstOrDefaultAsync<User>(
    "SELECT * FROM Users WHERE Id = @Id",
    new { Id = userId });

// Execute command (INSERT/UPDATE/DELETE)
var rowsAffected = await connection.ExecuteAsync(
    "UPDATE Users SET LastLogin = @Now WHERE Id = @Id",
    new { Now = DateTime.UtcNow, Id = userId });

// Return scalar value (new ID)
var newId = await connection.QuerySingleAsync<int>(
    @"INSERT INTO Users (Name, Email) VALUES (@Name, @Email);
      SELECT CAST(SCOPE_IDENTITY() AS INT);",
    new { Name = "John", Email = "john@example.com" });
```

### Multi-Mapping (JOIN Queries)

**One of Dapper's most powerful features:**

```csharp
// Map to two objects from a JOIN
var orders = await connection.QueryAsync<Order, Customer, Order>(
    sql: @"SELECT o.*, c.*
           FROM Orders o
           INNER JOIN Customers c ON o.CustomerId = c.Id",
    map: (order, customer) =>
    {
        order.Customer = customer;
        return order;
    },
    splitOn: "Id"  // Where second object starts (Customer.Id)
);

// Map to three objects
var orders = await connection.QueryAsync<Order, Customer, Product, Order>(
    sql: @"SELECT o.*, c.*, p.*
           FROM Orders o
           INNER JOIN Customers c ON o.CustomerId = c.Id
           INNER JOIN Products p ON o.ProductId = p.Id",
    map: (order, customer, product) =>
    {
        order.Customer = customer;
        order.Product = product;
        return order;
    },
    splitOn: "Id,Id"  // Split points for Customer and Product
);
```

### Multiple Result Sets (QueryMultiple)

**Execute multiple queries in one round-trip:**

```csharp
using var multi = await connection.QueryMultipleAsync(@"
    SELECT * FROM Customers WHERE Id = @CustomerId;
    SELECT * FROM Orders WHERE CustomerId = @CustomerId;
    SELECT * FROM Addresses WHERE CustomerId = @CustomerId;",
    new { CustomerId = customerId }
);

var customer = await multi.ReadFirstAsync<Customer>();
var orders = await multi.ReadAsync<Order>();
var addresses = await multi.ReadAsync<Address>();
```

### Dynamic Parameters

```csharp
var parameters = new DynamicParameters();
parameters.Add("@Name", "John");
parameters.Add("@OutputId", dbType: DbType.Int32, direction: ParameterDirection.Output);

await connection.ExecuteAsync(
    "INSERT INTO Users (Name) VALUES (@Name); SELECT @OutputId = SCOPE_IDENTITY();",
    parameters);

var newId = parameters.Get<int>("@OutputId");
```

---

## Transaction Patterns

### Basic Transactions (All-or-Nothing)

```csharp
using var connection = new SqlConnection(connString);
await connection.OpenAsync();

using var transaction = await connection.BeginTransactionAsync();

try
{
    // Multiple operations - all succeed or all fail
    await connection.ExecuteAsync(
        "UPDATE Accounts SET Balance = Balance - @Amount WHERE Id = @FromId",
        new { Amount = 100, FromId = 1 },
        transaction);

    await connection.ExecuteAsync(
        "UPDATE Accounts SET Balance = Balance + @Amount WHERE Id = @ToId",
        new { Amount = 100, ToId = 2 },
        transaction);

    await transaction.CommitAsync();  // ✅ Both succeeded
}
catch
{
    await transaction.RollbackAsync();  // ✅ Either failed, rollback both
    throw;
}
```

### Transaction Isolation Levels

| Level                        | Dirty Reads | Non-Repeatable Reads | Phantom Reads | Performance              |
| ---------------------------- | ----------- | -------------------- | ------------- | ------------------------ |
| **Read Uncommitted**         | Yes         | Yes                  | Yes           | Fastest                  |
| **Read Committed** (default) | No          | Yes                  | Yes           | Fast                     |
| **Repeatable Read**          | No          | No                   | Yes           | Moderate                 |
| **Serializable**             | No          | No                   | No            | Slowest                  |
| **Snapshot**                 | No          | No                   | No            | Fast (requires DB setup) |

```csharp
// Set isolation level
using var transaction = await connection.BeginTransactionAsync(
    IsolationLevel.ReadCommitted);

// Higher isolation = more locks = slower but more consistent
using var transaction = await connection.BeginTransactionAsync(
    IsolationLevel.Serializable);
```

**Choosing Isolation Level:**

- **Read Committed** (default): Good for most scenarios
- **Repeatable Read**: When you need consistent reads within transaction
- **Serializable**: When you need complete isolation (rare, slow)
- **Snapshot**: Best of both worlds but requires `ALTER DATABASE` setup

### Deadlock Handling

```csharp
public async Task<T> ExecuteWithDeadlockRetry<T>(Func<Task<T>> operation, int maxRetries = 3)
{
    for (int attempt = 0; attempt < maxRetries; attempt++)
    {
        try
        {
            return await operation();
        }
        catch (SqlException ex) when (ex.Number == 1205)  // Deadlock
        {
            if (attempt == maxRetries - 1) throw;

            // Exponential backoff
            await Task.Delay(TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)));
        }
    }
    throw new InvalidOperationException("Should never reach here");
}
```

---

## Best Practices

### ✅ Connection Management

- **Always** use `using` statements for automatic disposal
- Let connection pooling handle reuse (don't cache connections)
- Keep connections open for minimum time
- **Never** store connections in static fields or class members

### ✅ SQL Injection Prevention

- **Always** use parameterized queries
- Use Dapper's anonymous objects: `new { Id = userId }`
- **Never** use string interpolation: `$"SELECT * WHERE Id = {id}"`
- **Never** concatenate user input into SQL strings

### ✅ Transaction Best Practices

- Keep transactions as short as possible
- Use appropriate isolation levels (default is usually fine)
- Handle deadlocks with retry logic and exponential backoff
- **Never** hold transactions across HTTP calls or user interactions
- Commit explicitly after all operations succeed

### ✅ Performance Best Practices

- Use async methods (`ExecuteReaderAsync`, `QueryAsync`)
- Use `AsNoTracking()` with EF Core for read-only queries
- Consider Dapper for read-heavy workloads
- Use `SqlBulkCopy` for bulk inserts (10k+ rows)
- Index frequently queried columns

### ✅ Error Handling

- Wrap database operations in try-catch
- Log connection strings (without passwords!)
- Include SQL and parameters in logs (sanitized)
- Retry transient errors (timeouts, deadlocks)

---

## Common Pitfalls

### ❌ Connection Leaks

```csharp
// ❌ BAD: Connection never closed
var conn = new SqlConnection(connString);
conn.Open();
var command = new SqlCommand("SELECT * FROM Users", conn);
// Forgot to dispose - connection leaked!

// ✅ GOOD: Automatic disposal
using var conn = new SqlConnection(connString);
await conn.OpenAsync();
using var command = new SqlCommand("SELECT * FROM Users", conn);
```

### ❌ SQL Injection

```csharp
// ❌ DANGEROUS
var sql = $"SELECT * FROM Users WHERE Name = '{userName}'";
await connection.QueryAsync<User>(sql);

// ✅ SAFE
var sql = "SELECT * FROM Users WHERE Name = @Name";
await connection.QueryAsync<User>(sql, new { Name = userName });
```

### ❌ Transaction Not Passed to Commands

```csharp
// ❌ BAD: Command not enrolled in transaction!
using var transaction = connection.BeginTransaction();
await connection.ExecuteAsync("UPDATE Users SET Name = 'John'");  // No transaction!
await transaction.CommitAsync();  // Doesn't affect the update!

// ✅ GOOD: Pass transaction to command
using var transaction = connection.BeginTransaction();
await connection.ExecuteAsync("UPDATE Users SET Name = 'John'", transaction: transaction);
await transaction.CommitAsync();
```

### ❌ Holding Transactions Too Long

```csharp
// ❌ BAD: Transaction held during HTTP call
using var transaction = connection.BeginTransaction();
await connection.ExecuteAsync("UPDATE Inventory SET Qty = Qty - 1", transaction: transaction);
await _httpClient.GetAsync("https://external-api.com");  // ❌ Locks held during HTTP!
await transaction.CommitAsync();

// ✅ GOOD: Complete transaction before external calls
using var transaction = connection.BeginTransaction();
await connection.ExecuteAsync("UPDATE Inventory SET Qty = Qty - 1", transaction: transaction);
await transaction.CommitAsync();  // ✅ Release locks first

await _httpClient.GetAsync("https://external-api.com");  // Then external call
```

### ❌ Not Using Connection Pooling

```csharp
// ❌ BAD: Disabling connection pooling (slow!)
var connString = "Server=.;Database=MyDb;Pooling=false";

// ✅ GOOD: Let pooling work (default)
var connString = "Server=.;Database=MyDb";  // Pooling=true by default
```

---

## Related Files

### ADO.NET

- [DataAccess/AdoNetPatterns.cs](../DataAccess/AdoNetPatterns.cs) - Connection management, SqlCommand patterns, DataReader
- [DataAccess/TransactionPatterns.cs](../DataAccess/TransactionPatterns.cs) - Transactions, isolation levels, deadlock handling

### Dapper

- [DataAccess/DapperExamples.cs](../DataAccess/DapperExamples.cs) - Basic queries, multi-mapping, QueryMultiple, parameters

### Advanced Data Patterns (NoSQL, Caching, Scaling)

- [DataAccess/DatabaseShardingAndScaling.cs](../DataAccess/DatabaseShardingAndScaling.cs) - Horizontal scaling strategies
- [DataAccess/GraphDatabasePatterns.cs](../DataAccess/GraphDatabasePatterns.cs) - Graph database concepts and patterns
- [DataAccess/MongoDBWithDotNet.cs](../DataAccess/MongoDBWithDotNet.cs) - Document-oriented databases
- [DataAccess/ReadReplicasAndCQRS.cs](../DataAccess/ReadReplicasAndCQRS.cs) - Read replicas and CQRS pattern
- [DataAccess/RedisPatterns.cs](../DataAccess/RedisPatterns.cs) - In-memory caching and data structures
- [DataAccess/TimeSeriesDatabases.cs](../DataAccess/TimeSeriesDatabases.cs) - Time-series data optimization

### Entity Framework Core

- [Entity-Framework.md](Entity-Framework.md) - Full EF Core documentation
- [DataAccess/EntityFramework/](../DataAccess/EntityFramework/) - EF Core examples (7 files)

---

## See Also

- [Entity Framework](Entity-Framework.md) - Full ORM with change tracking and migrations
- [Performance](Performance.md) - Query optimization and benchmarking
- [Security](Security.md) - SQL injection prevention, connection string security
- [Resilience](Resilience.md) - Retry policies for transient failures
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-15
