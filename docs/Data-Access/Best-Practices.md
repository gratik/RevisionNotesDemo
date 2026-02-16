# Best Practices

> Subject: [Data-Access](../README.md)

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


