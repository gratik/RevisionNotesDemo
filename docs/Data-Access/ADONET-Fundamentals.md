# ADO.NET Fundamentals

> Subject: [Data-Access](../README.md)

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


