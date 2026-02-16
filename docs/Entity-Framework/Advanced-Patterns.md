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


