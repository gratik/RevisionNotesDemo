# Best Practices

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


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

## Detailed Guidance

Best Practices guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Best Practices before implementation work begins.
- Keep boundaries explicit so Best Practices decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Best Practices in production-facing code.
- When performance, correctness, or maintainability depends on consistent Best Practices decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Best Practices as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Best Practices is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Best Practices are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

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

