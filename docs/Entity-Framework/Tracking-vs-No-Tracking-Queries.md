# Tracking vs No-Tracking Queries

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


