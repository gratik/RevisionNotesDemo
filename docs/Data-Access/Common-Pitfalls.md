# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: SQL fundamentals, connection lifecycle, and transaction basics.
- Related examples: docs/Data-Access/README.md
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

## Detailed Guidance

Common Pitfalls guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Common Pitfalls before implementation work begins.
- Keep boundaries explicit so Common Pitfalls decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Common Pitfalls in production-facing code.
- When performance, correctness, or maintainability depends on consistent Common Pitfalls decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Common Pitfalls as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Common Pitfalls is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Common Pitfalls are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Common Pitfalls is about data persistence and query strategy selection. It matters because it drives correctness, latency, and transactional integrity.
- Use it when choosing between EF Core, Dapper, and ADO.NET by workload.

2-minute answer:
- Start with the problem Common Pitfalls solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: abstraction convenience vs explicit SQL control.
- Close with one failure mode and mitigation: ignoring transaction and indexing behavior in production paths.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Common Pitfalls but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Common Pitfalls, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Common Pitfalls and map it to one concrete implementation in this module.
- 3 minutes: compare Common Pitfalls with an alternative, then walk through one failure mode and mitigation.