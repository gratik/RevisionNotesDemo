# Query Performance

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Relational data modeling and basic LINQ provider behavior.
- Related examples: docs/Entity-Framework/README.md
> Subject: [Entity-Framework](../README.md)

## Query Performance

### N+1 Problem

```csharp
// ❌ BAD: N+1 query problem
var users = await _context.Users.ToListAsync();  // 1 query
foreach (var user in users)
{
    var orders = await _context.Orders
        .Where(o => o.UserId == user.Id)
        .ToListAsync();  // N queries (one per user)
}
// Total: 1 + N queries

// ✅ GOOD: Single query with Include
var users = await _context.Users
    .Include(u => u.Orders)  // ✅ Eager loading
    .ToListAsync();
// Total: 1 query
```

### Eager Loading vs Explicit Loading vs Lazy Loading

```csharp
// ✅ Eager Loading - Load related data upfront
var users = await _context.Users
    .Include(u => u.Orders)
        .ThenInclude(o => o.Items)  // ✅ Load nested relationships
    .ToListAsync();

// ✅ Explicit Loading - Load related data on demand
var user = await _context.Users.FirstAsync();
await _context.Entry(user)
    .Collection(u => u.Orders)
    .LoadAsync();

// ❌ Lazy Loading - Automatic but can cause N+1 problems
// Not recommended for most scenarios
```

### Projection for Performance

```csharp
// ❌ BAD: Loading entire entity when only need few fields
var users = await _context.Users
    .Include(u => u.Orders)
    .ToListAsync();
// Loads all columns from Users and Orders

// ✅ GOOD: Project only needed fields
var users = await _context.Users
    .Select(u => new UserDto
    {
        Id = u.Id,
        Name = u.Name,
        OrderCount = u.Orders.Count
    })
    .ToListAsync();
// Only loads Id, Name, and counts orders
```

---

## Detailed Guidance

SQL query guidance focuses on predictable execution plans, safe parameterization, and measured performance tuning.

### Design Notes
- Define success criteria for Query Performance before implementation work begins.
- Keep boundaries explicit so Query Performance decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Query Performance in production-facing code.
- When performance, correctness, or maintainability depends on consistent Query Performance decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Query Performance as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Query Performance is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Query Performance are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Query Performance is about ORM-based data modeling and persistence. It matters because query shape and tracking behavior strongly affect performance.
- Use it when building data access layers with maintainable domain mappings.

2-minute answer:
- Start with the problem Query Performance solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: developer productivity vs query/control precision.
- Close with one failure mode and mitigation: N+1 queries and incorrect tracking strategy.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Query Performance but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Query Performance, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Query Performance and map it to one concrete implementation in this module.
- 3 minutes: compare Query Performance with an alternative, then walk through one failure mode and mitigation.