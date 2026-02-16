# Common Pitfalls

> Subject: [Data-Access](../README.md)

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


