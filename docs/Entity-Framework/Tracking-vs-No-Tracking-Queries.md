# Tracking vs No-Tracking Queries

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Relational data modeling and basic LINQ provider behavior.
- Related examples: docs/Entity-Framework/README.md
> Subject: [Entity-Framework](../README.md)

## Tracking vs No-Tracking Queries

### Change Tracking

```csharp
// ✅ Tracking query (default) - EF tracks changes
var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == 1);
user.Name = "Updated Name";
await _context.SaveChangesAsync();  // ✅ EF detects and saves changes

// ✅ No-tracking query - read-only, better performance
var users = await _context.Users
    .AsNoTracking()
    .Where(u => u.IsActive)
    .ToListAsync();
// Changes to users won't be saved
```

### When to Use Each

| Scenario | Use Tracking | Use No-Tracking |
|----------|--------------|-----------------|
| **Read-only queries** | ❌ | ✅ |
| **API GET endpoints** | ❌ | ✅ |
| **Will update entities** | ✅ | ❌ |
| **Displaying data** | ❌ | ✅ |
| **Background jobs reading data** | ❌ | ✅ |

```csharp
// ✅ Read-only API endpoint
[HttpGet]
public async Task<IActionResult> GetUsers()
{
    var users = await _context.Users
        .AsNoTracking()  // ✅ No tracking needed
        .ToListAsync();
    
    return Ok(users);
}

// ✅ Update endpoint needs tracking
[HttpPut("{id}")]
public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
{
    var user = await _context.Users.FindAsync(id);  // ✅ Tracked by default
    if (user == null) return NotFound();
    
    user.Name = request.Name;
    await _context.SaveChangesAsync();  // ✅ Changes detected and saved
    
    return Ok(user);
}
```

---

## Detailed Guidance

Tracking vs No-Tracking Queries guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Tracking vs No-Tracking Queries before implementation work begins.
- Keep boundaries explicit so Tracking vs No-Tracking Queries decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Tracking vs No-Tracking Queries in production-facing code.
- When performance, correctness, or maintainability depends on consistent Tracking vs No-Tracking Queries decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Tracking vs No-Tracking Queries as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Tracking vs No-Tracking Queries is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Tracking vs No-Tracking Queries are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Tracking vs No-Tracking Queries is about ORM-based data modeling and persistence. It matters because query shape and tracking behavior strongly affect performance.
- Use it when building data access layers with maintainable domain mappings.

2-minute answer:
- Start with the problem Tracking vs No-Tracking Queries solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: developer productivity vs query/control precision.
- Close with one failure mode and mitigation: N+1 queries and incorrect tracking strategy.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Tracking vs No-Tracking Queries but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Tracking vs No-Tracking Queries, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Tracking vs No-Tracking Queries and map it to one concrete implementation in this module.
- 3 minutes: compare Tracking vs No-Tracking Queries with an alternative, then walk through one failure mode and mitigation.