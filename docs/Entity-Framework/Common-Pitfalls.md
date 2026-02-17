# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Relational data modeling and basic LINQ provider behavior.
- Related examples: docs/Entity-Framework/README.md
> Subject: [Entity-Framework](../README.md)

## Common Pitfalls

### ❌ Not Using AsNoTracking for Read-Only

```csharp
// ❌ BAD: Tracking entities you won't modify
public async Task<List<UserDto>> GetUsersAsync()
{
    var users = await _context.Users.ToListAsync();  // ❌ Unnecessary tracking
    return users.Select(u => new UserDto { /* ... */ }).ToList();
}

// ✅ GOOD: No tracking for read-only
public async Task<List<UserDto>> GetUsersAsync()
{
    return await _context.Users
        .AsNoTracking()
        .Select(u => new UserDto { /* ... */ })
        .ToListAsync();
}
```

### ❌ N+1 Query Problem

```csharp
// ❌ BAD: N+1 queries
var users = await _context.Users.ToListAsync();
foreach (var user in users)
{
    var orderCount = await _context.Orders.CountAsync(o => o.UserId == user.Id);  // ❌ Query per user
}

// ✅ GOOD: Single query
var users = await _context.Users
    .Select(u => new
    {
        User = u,
        OrderCount = u.Orders.Count()
    })
    .ToListAsync();
```

### ❌ Forgetting to Await

```csharp
// ❌ BAD: Not awaiting async query
var users = _context.Users.ToListAsync();  // ❌ Returns Task<List<User>>, not List<User>

// ✅ GOOD: Await the query
var users = await _context.Users.ToListAsync();
```

### ❌ Tracking Too Many Entities

```csharp
// ❌ BAD: Loading all users in memory
var users = await _context.Users.ToListAsync();  // ❌ Could be millions of rows

// ✅ GOOD: Paginate or stream
var users = await _context.Users
    .Skip(page * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

### ❌ Not Using Transactions for Multiple Operations

```csharp
// ❌ BAD: Multiple SaveChanges (separate transactions)
await _context.Users.AddAsync(user);
await _context.SaveChangesAsync();  // Transaction 1

await _context.Orders.AddAsync(order);
await _context.SaveChangesAsync();  // Transaction 2
// If second fails, first already committed

// ✅ GOOD: Single SaveChanges (single transaction)
await _context.Users.AddAsync(user);
await _context.Orders.AddAsync(order);
await _context.SaveChangesAsync();  // ✅ Single transaction, all or nothing

// ✅ GOOD: Explicit transaction for complex operations
using var transaction = await _context.Database.BeginTransactionAsync();
try
{
    await _context.Users.AddAsync(user);
    await _context.SaveChangesAsync();
    
    await _context.Orders.AddAsync(order);
    await _context.SaveChangesAsync();
    
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

---


## Interview Answer Block
30-second answer:
- Common Pitfalls is about ORM-based data modeling and persistence. It matters because query shape and tracking behavior strongly affect performance.
- Use it when building data access layers with maintainable domain mappings.

2-minute answer:
- Start with the problem Common Pitfalls solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: developer productivity vs query/control precision.
- Close with one failure mode and mitigation: N+1 queries and incorrect tracking strategy.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Common Pitfalls but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Common Pitfalls, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Common Pitfalls and map it to one concrete implementation in this module.
- 3 minutes: compare Common Pitfalls with an alternative, then walk through one failure mode and mitigation.