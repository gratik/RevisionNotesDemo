# Transaction Patterns

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Data-Access](../README.md)

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


## Interview Answer Block
30-second answer:
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

