# Query Performance

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


