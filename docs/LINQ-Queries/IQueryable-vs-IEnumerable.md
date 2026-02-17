# IQueryable vs IEnumerable

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Collections, lambdas, and deferred execution basics.
- Related examples: docs/LINQ-Queries/README.md
> Subject: [LINQ-Queries](../README.md)

## IQueryable vs IEnumerable

### The Critical Difference

**IEnumerable<T>**: In-memory execution (LINQ-to-Objects)
- Executes in your application (client-side)
- Uses `Func<T, bool>` (compiled delegates)
- Best for: In-memory collections (List, Array)

**IQueryable<T>**: Remote execution (LINQ-to-SQL, LINQ-to-EF)
- Executes on database server (server-side)
- Uses `Expression<Func<T, bool>>` (expression trees)
- Translates to SQL
- Best for: Database queries

### Quick Example

```csharp
// ❌ BAD: Loads entire table into memory!
IEnumerable<Customer> customers = _dbContext.Customers.ToList();  // ❌ Loads ALL
var active = customers.Where(c => c.IsActive).Take(10);
// SQL: SELECT * FROM Customers (loads everything!)

// ✅ GOOD: Filters in database
IQueryable<Customer> customers = _dbContext.Customers;
var active = customers.Where(c => c.IsActive).Take(10).ToList();
// SQL: SELECT TOP 10 * FROM Customers WHERE IsActive = 1
```

---

## Detailed Guidance

LINQ guidance focuses on readable query composition while avoiding hidden multiple enumerations and expensive client-side execution.

### Design Notes
- Define success criteria for IQueryable vs IEnumerable before implementation work begins.
- Keep boundaries explicit so IQueryable vs IEnumerable decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring IQueryable vs IEnumerable in production-facing code.
- When performance, correctness, or maintainability depends on consistent IQueryable vs IEnumerable decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying IQueryable vs IEnumerable as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where IQueryable vs IEnumerable is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for IQueryable vs IEnumerable are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- IQueryable vs IEnumerable is about query composition over in-memory and provider-backed data. It matters because correct query shape impacts both readability and performance.
- Use it when implementing filtering, projection, and aggregation in business workflows.

2-minute answer:
- Start with the problem IQueryable vs IEnumerable solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: concise query syntax vs hidden complexity in execution behavior.
- Close with one failure mode and mitigation: accidental multiple enumeration or provider-side translation surprises.
## Interview Bad vs Strong Answer
Bad answer:
- Defines IQueryable vs IEnumerable but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose IQueryable vs IEnumerable, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define IQueryable vs IEnumerable and map it to one concrete implementation in this module.
- 3 minutes: compare IQueryable vs IEnumerable with an alternative, then walk through one failure mode and mitigation.