# Advanced Patterns

> Subject: [Entity-Framework](../README.md)

## Advanced Patterns

### Raw SQL Queries

```csharp
// ✅ Raw SQL for complex queries
var users = await _context.Users
    .FromSqlRaw("SELECT * FROM Users WHERE IsActive = 1")
    .ToListAsync();

// ✅ Parameterized to prevent SQL injection
var email = "test@example.com";
var user = await _context.Users
    .FromSqlRaw("SELECT * FROM Users WHERE Email = {0}", email)
    .FirstOrDefaultAsync();

// ✅ Execute non-query
await _context.Database.ExecuteSqlRawAsync(
    "UPDATE Users SET LastLogin = GETDATE() WHERE Id = {0}", userId);
```

### Global Query Filters

```csharp
// ✅ Apply filter globally (e.g., soft delete)
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<User>()
        .HasQueryFilter(u => !u.IsDeleted);
}

// All queries automatically filter deleted users
var users = await _context.Users.ToListAsync();  // ✅ Only non-deleted

// ✅ Ignore filter when needed
var allUsers = await _context.Users
    .IgnoreQueryFilters()
    .ToListAsync();  // ✅ Includes deleted
```

### Compiled Queries

```csharp
// ✅ Compile frequently-used query for performance
private static readonly Func<AppDbContext, int, Task<User?>> _getUserById =
    EF.CompileAsyncQuery((AppDbContext context, int id) =>
        context.Users.FirstOrDefault(u => u.Id == id));

public async Task<User?> GetUserByIdAsync(int id)
{
    return await _getUserById(_context, id);
}
// 10-30% faster for hot paths
```

---

## Detailed Guidance

Advanced Patterns guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Advanced Patterns before implementation work begins.
- Keep boundaries explicit so Advanced Patterns decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Advanced Patterns in production-facing code.
- When performance, correctness, or maintainability depends on consistent Advanced Patterns decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Advanced Patterns as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Advanced Patterns is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Advanced Patterns are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

