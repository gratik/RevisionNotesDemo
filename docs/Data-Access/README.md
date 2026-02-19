# Data Access Patterns (ADO.NET, Dapper, EF Core)

## Metadata
- Owner: Maintainers
- Last updated: February 16, 2026
- Prerequisites: SQL basics, repository patterns
- Related examples: Learning/DataAccess/AdoNetPatterns.cs, Learning/DataAccess/DapperExamples.cs, Learning/DataAccess/SqlServer/SqlServerQueriesAndOperations.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../README.md)

## Module Metadata

- **Prerequisites**: LINQ Queries, Async Multithreading
- **When to Study**: After API basics; before deeper persistence specialization.
- **Related Files**: `../Learning/DataAccess/*.cs`
- **Estimated Time**: 120-150 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](../Learning-Path.md) | [Track Start](../Configuration.md)
- **Next Step**: [Entity-Framework.md](../Entity-Framework.md)
<!-- STUDY-NAV-END -->


## Overview

This guide covers three primary data access approaches in .NET: ADO.NET (raw SQL),
Dapper (micro-ORM), and Entity Framework Core (full ORM). Each has distinct performance
characteristics, complexity tradeoffs, and ideal use cases. This document focuses on
ADO.NET and Dapper patterns; see [Entity-Framework.md](../Entity-Framework.md) for EF Core.

## Folder Boundary (Important)

To avoid overlap in this repository:

- `Learning/DataAccess` is the **canonical implementation track** for data access code.
- `Learning/Database` is the **concept/comparison catalog** for quick-reference study.

If you are adding or updating runnable implementation content, use `Learning/DataAccess` first.

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

## SQL Server Deep Dive

### Useful Topics to Master

- Query design: SARGable predicates, projection discipline, parameterization
- Schema design: key strategy, constraints, normalized vs denormalized tradeoffs
- Programmability: stored procedures, inline TVFs, user-defined table types (TVPs)
- Table design: type sizing, nullability, defaults, and check constraints
- Normalization strategy with pragmatic denormalization for read models
- Modeling patterns: soft delete, temporal/audit, and multi-tenant options
- Intermediate sets: CTE vs #temp table vs @table variable decision-making
- Upsert semantics: `MERGE` vs explicit `UPDATE + INSERT` patterns
- Graph transfer: efficient load/update for customers, orders, order-items
- Partitioning and data lifecycle strategy
- Index architecture and usage-driven maintenance
- Statistics/cardinality management and plan-stability tuning
- Execution-plan analysis workflow
- Concurrency controls (rowversion, isolation, locking)
- Bulk ingestion strategy (TVP, SqlBulkCopy, staged loads)
- Monitoring runbook and SQL security controls
- Performance tuning: Query Store, plans, indexes, wait analysis, and regressions
- Monitoring and operations: backups, restore drills, deadlock/timeout trend analysis
- Troubleshooting: blocking, parameter sniffing, tempdb pressure, and plan instability

### Best Practices

- Use range predicates on indexed columns instead of wrapping columns in functions.
- Keep transactions short and explicit.
- Design indexes from real workload patterns, not theory.
- Prefer inline TVFs over multi-statement TVFs for critical paths.
- Validate every change with before/after metrics (logical reads, CPU, p95 latency).

### Bad Practices

- `SELECT *` in API hot paths.
- Scalar UDFs in large scans/joins.
- No foreign keys "for speed" (causes silent data integrity drift).
- Over-indexing write-heavy tables without usage review.
- Tuning by guesswork without Query Store or wait stats.

### SQL Server Deployment Models: Differences and Restrictions

| Model | What you manage | Typical strengths | Typical restrictions/tradeoffs |
| --- | --- | --- | --- |
| Self-managed SQL Server (on-prem or self-managed VM) | You manage full stack: OS, SQL engine, HA/DR, patching, backups | Full control and broadest feature surface | Highest operational burden and incident ownership |
| Hosted SQL Server instance (IaaS hosted by provider/cloud VM) | Host manages infrastructure; SQL operations often still mostly yours | High compatibility for lift-and-shift workloads | Still significant DBA/ops ownership; cloud infra costs and sizing decisions |
| Azure SQL Managed Instance | Microsoft manages more platform operations | High SQL Server compatibility with managed service benefits | Less instance/OS control than self-managed SQL Server; validate feature parity per workload |
| Azure SQL Database | Microsoft-managed PaaS per database/elastic pool model | Strongest PaaS automation, scaling, and operational simplicity | More feature constraints vs full instance model; design for per-database boundaries |

Migration note:
- Validate SQL Agent job model, cross-database dependencies, server-level settings, extensibility assumptions, and backup/restore workflow before choosing target.

---

## SQL Server Topic Docs

- [SQL Server Docs Index](SqlServer/README.md)
- [SQL Server Queries and Operations](SqlServer/SqlServerQueriesAndOperations.md)
- [Table Design Fundamentals](SqlServer/TableDesignFundamentals.md)
- [Normalization and Denormalization Patterns](SqlServer/NormalizationAndDenormalizationPatterns.md)
- [Relational Data Modeling Patterns](SqlServer/RelationalDataModelingPatterns.md)
- [Partitioning and Data Lifecycle Patterns](SqlServer/PartitioningAndDataLifecyclePatterns.md)
- [Index Architecture Patterns](SqlServer/IndexArchitecturePatterns.md)
- [Statistics and Cardinality Patterns](SqlServer/StatisticsAndCardinalityPatterns.md)
- [Execution Plan Analysis Lab](SqlServer/ExecutionPlanAnalysisLab.md)
- [Stored Procedure Design Standards](SqlServer/StoredProcedureDesignStandards.md)
- [Concurrency Consistency Patterns](SqlServer/ConcurrencyConsistencyPatterns.md)
- [Bulk Ingestion Patterns](SqlServer/BulkIngestionPatterns.md)
- [Operational Monitoring Runbook](SqlServer/OperationalMonitoringRunbook.md)
- [SQL Security Patterns](SqlServer/SqlSecurityPatterns.md)
- [CTE vs Temp/Table Variable Patterns](SqlServer/CteTempTableAndTableVariablePatterns.md)
- [MERGE and Upsert Patterns](SqlServer/MergeAndUpsertPatterns.md)
- [SQL Graph Data Transfer Patterns](SqlServer/SqlGraphDataTransferPatterns.md)
- [Transaction and Isolation Patterns](SqlServer/TransactionAndIsolationPatterns.md)

---

## Related Files

### ADO.NET

- [DataAccess/AdoNetPatterns.cs](../../Learning/DataAccess/AdoNetPatterns.cs) - Connection management, SqlCommand patterns, DataReader
- [DataAccess/SqlServer/SqlServerQueriesAndOperations.cs](../../Learning/DataAccess/SqlServer/SqlServerQueriesAndOperations.cs) - SQL Server query quality, keys/indexes, tuning, monitoring, and platform differences
- [DataAccess/SqlServer/TableDesignFundamentals.cs](../../Learning/DataAccess/SqlServer/TableDesignFundamentals.cs) - Table design standards for data types, keys, and constraints
- [DataAccess/SqlServer/NormalizationAndDenormalizationPatterns.cs](../../Learning/DataAccess/SqlServer/NormalizationAndDenormalizationPatterns.cs) - Normalization baseline and denormalization decision guidance
- [DataAccess/SqlServer/RelationalDataModelingPatterns.cs](../../Learning/DataAccess/SqlServer/RelationalDataModelingPatterns.cs) - Modeling patterns for soft delete, temporal, and tenant isolation
- [DataAccess/SqlServer/PartitioningAndDataLifecyclePatterns.cs](../../Learning/DataAccess/SqlServer/PartitioningAndDataLifecyclePatterns.cs) - Partitioning and data retention lifecycle patterns
- [DataAccess/SqlServer/IndexArchitecturePatterns.cs](../../Learning/DataAccess/SqlServer/IndexArchitecturePatterns.cs) - Index architecture and workload-first indexing guidance
- [DataAccess/SqlServer/StatisticsAndCardinalityPatterns.cs](../../Learning/DataAccess/SqlServer/StatisticsAndCardinalityPatterns.cs) - Statistics quality and cardinality estimation patterns
- [DataAccess/SqlServer/ExecutionPlanAnalysisLab.cs](../../Learning/DataAccess/SqlServer/ExecutionPlanAnalysisLab.cs) - Execution plan inspection workflow and optimization checklist
- [DataAccess/SqlServer/StoredProcedureDesignStandards.cs](../../Learning/DataAccess/SqlServer/StoredProcedureDesignStandards.cs) - Stored procedure design conventions and error handling
- [DataAccess/SqlServer/ConcurrencyConsistencyPatterns.cs](../../Learning/DataAccess/SqlServer/ConcurrencyConsistencyPatterns.cs) - Concurrency control and isolation pattern guidance
- [DataAccess/SqlServer/BulkIngestionPatterns.cs](../../Learning/DataAccess/SqlServer/BulkIngestionPatterns.cs) - Bulk ingestion approach selection and batching patterns
- [DataAccess/SqlServer/OperationalMonitoringRunbook.cs](../../Learning/DataAccess/SqlServer/OperationalMonitoringRunbook.cs) - SQL monitoring and incident response runbook
- [DataAccess/SqlServer/SqlSecurityPatterns.cs](../../Learning/DataAccess/SqlServer/SqlSecurityPatterns.cs) - SQL Server security controls and least-privilege model
- [DataAccess/SqlServer/CteTempTableAndTableVariablePatterns.cs](../../Learning/DataAccess/SqlServer/CteTempTableAndTableVariablePatterns.cs) - CTE/temp-table/table-variable tradeoffs and bad-vs-good examples
- [DataAccess/SqlServer/MergeAndUpsertPatterns.cs](../../Learning/DataAccess/SqlServer/MergeAndUpsertPatterns.cs) - MERGE usage guidance and safer upsert alternatives
- [DataAccess/SqlServer/SqlGraphDataTransferPatterns.cs](../../Learning/DataAccess/SqlServer/SqlGraphDataTransferPatterns.cs) - Graph transfer and diff-based update patterns for C# and SQL
- [DataAccess/SqlServer/TransactionAndIsolationPatterns.cs](../../Learning/DataAccess/SqlServer/TransactionAndIsolationPatterns.cs) - SQL Server transaction boundaries, isolation levels, and deadlock handling
- [DataAccess/SqlServer/README.md](../../Learning/DataAccess/SqlServer/README.md) - SQL Server concept index and runnable file map

### Dapper

- [DataAccess/DapperExamples.cs](../../Learning/DataAccess/DapperExamples.cs) - Basic queries, multi-mapping, QueryMultiple, parameters

### Advanced Data Patterns (NoSQL, Caching, Scaling)

- [DataAccess/DatabaseShardingAndScaling.cs](../../Learning/DataAccess/DatabaseShardingAndScaling.cs) - Horizontal scaling strategies
- [DataAccess/GraphDatabasePatterns.cs](../../Learning/DataAccess/GraphDatabasePatterns.cs) - Graph database concepts and patterns
- [DataAccess/MongoDBWithDotNet.cs](../../Learning/DataAccess/MongoDBWithDotNet.cs) - Document-oriented databases
- [DataAccess/ReadReplicasAndCQRS.cs](../../Learning/DataAccess/ReadReplicasAndCQRS.cs) - Read replicas and CQRS pattern
- [DataAccess/RedisPatterns.cs](../../Learning/DataAccess/RedisPatterns.cs) - In-memory caching and data structures
- [DataAccess/TimeSeriesDatabases.cs](../../Learning/DataAccess/TimeSeriesDatabases.cs) - Time-series data optimization

### Entity Framework Core

- [Entity-Framework.md](../Entity-Framework.md) - Full EF Core documentation
- [DataAccess/EntityFramework/](../../Learning/DataAccess/EntityFramework/) - EF Core examples (7 files)
- [DataAccess/EntityFramework/EfCoreTransactionPatterns.cs](../../Learning/DataAccess/EntityFramework/EfCoreTransactionPatterns.cs) - EF Core transaction boundaries and cross-context transaction sharing

---

## See Also

- [Entity Framework](../Entity-Framework.md) - Full ORM with change tracking and migrations
- [Performance](../Performance.md) - Query optimization and benchmarking
- [Security](../Security.md) - SQL injection prevention, connection string security
- [Resilience](../Resilience.md) - Retry policies for transient failures
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-15

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [Entity-Framework.md](../Entity-Framework.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: Good data access minimizes unnecessary I/O, keeps transaction boundaries explicit, and makes query behavior observable.
- 2-minute deep dive: I choose EF, Dapper, or ADO.NET based on workload and control requirements, prevent N+1/query bloat, and enforce atomic write + outbox patterns for reliability.
- Common follow-up: How do you decide between EF and Dapper?
- Strong response: EF for productivity/domain modeling, Dapper for hot-path read/write control, with measured benchmarks guiding the final choice.
- Tradeoff callout: Premature micro-optimization can reduce maintainability without measurable benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know Data Access and I would just follow best practices."
- Strong answer: "For Data Access, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Data Access in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.

## Topic Files

- [Choosing the Right Tool](Choosing-the-Right-Tool.md)
- [ADO.NET Fundamentals](ADONET-Fundamentals.md)
- [Dapper: The Micro-ORM](Dapper-The-Micro-ORM.md)
- [Transaction Patterns](Transaction-Patterns.md)
- [Best Practices](Best-Practices.md)
- [Common Pitfalls](Common-Pitfalls.md)
- [SQL Server Deep Dive](SQL-Server-Deep-Dive.md)




