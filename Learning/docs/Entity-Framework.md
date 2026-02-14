# Entity Framework Core Best Practices

> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../../README.md)

## Overview

Entity Framework Core is a powerful ORM for .NET, but misuse can lead to N+1 queries, memory leaks,
and performance issues. This guide covers tracking behavior, query optimization, relationships,
migrations, and common pitfalls to avoid.

---

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

## Relationships and Navigation Properties

### One-to-Many

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Navigation property
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    // Navigation property
    public User User { get; set; } = null!;
}

// Configuration
modelBuilder.Entity<Order>()
    .HasOne(o => o.User)
    .WithMany(u => u.Orders)
    .HasForeignKey(o => o.UserId);
```

### Many-to-Many

```csharp
// ✅ Many-to-many with join entity (explicit)
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
}

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
}

public class StudentCourse
{
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    
    public DateTime EnrolledDate { get; set; }  // Extra data on join table
}

// Configuration
modelBuilder.Entity<StudentCourse>()
    .HasKey(sc => new { sc.StudentId, sc.CourseId });

// ✅ Query many-to-many
var student = await _context.Students
    .Include(s => s.StudentCourses)
        .ThenInclude(sc => sc.Course)
    .FirstAsync(s => s.Id == 1);
```

---

## DbContext Configuration

### DbContext Lifetime

```csharp
// ✅ GOOD: Scoped lifetime (default in ASP.NET Core)
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// ❌ BAD: Don't create DbContext manually in production
using var context = new AppDbContext();  // ❌ Not recommended

// ✅ GOOD: Inject DbContext
public class UserService
{
    private readonly AppDbContext _context;
    
    public UserService(AppDbContext context)
    {
        _context = context;
    }
}
```

### Connection String Management

```csharp
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyDb;Trusted_Connection=true"
  }
}

// ✅ Configure in Program.cs
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

---

## Migrations

### Creating Migrations

```bash
# Create migration
dotnet ef migrations add InitialCreate

# Apply migration
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script
```

### Migration Best Practices

```csharp
// ✅ GOOD: Descriptive migration names
dotnet ef migrations add AddUserEmailIndex
dotnet ef migrations add AddOrderStatusColumn

// ✅ Always review generated migration
public partial class AddUserEmailIndex : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);
    }
    
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Users_Email",
            table: "Users");
    }
}
```

---

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

## Best Practices

### ✅ Query Performance
- Use `AsNoTracking()` for read-only queries
- Use `Include()` to prevent N+1 queries
- Use projection (Select) to load only needed fields
- Avoid lazy loading in most scenarios
- Use compiled queries for hot paths

### ✅ DbContext Usage
- Register as Scoped (default in ASP.NET Core)
- Don't cache entities across requests
- Dispose DbContext properly (automatic with DI)
- Don't create DbContext manually in production
- One SaveChanges per unit of work

### ✅ Relationships
- Always configure relationships explicitly
- Use eager loading (`Include`) when you need related data
- Use explicit loading when conditionally needed
- Consider projection to avoid loading entire graphs

### ✅ Migrations
- Create migrations with descriptive names
- Review generated migrations before applying
- Test migrations on staging before production
- Keep migrations in source control
- Use `dotnet ef migrations script` for production

---

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

## Related Files

- [DataAccess/EntityFramework/EntityFrameworkBestPractices.cs](../DataAccess/EntityFramework/EntityFrameworkBestPractices.cs)
- [DataAccess/EntityFramework/ChangeTrackingExamples.cs](../DataAccess/EntityFramework/ChangeTrackingExamples.cs)
- [DataAccess/EntityFramework/RelationshipsNavigationExamples.cs](../DataAccess/EntityFramework/RelationshipsNavigationExamples.cs)
- [DataAccess/EntityFramework/PerformanceOptimizationExamples.cs](../DataAccess/EntityFramework/PerformanceOptimizationExamples.cs)
- [DataAccess/EntityFramework/MigrationsInDepthExamples.cs](../DataAccess/EntityFramework/MigrationsInDepthExamples.cs)

---

## See Also

- [Data Access](Data-Access.md) - EF Core vs Dapper vs ADO.NET
- [Performance](Performance.md) - Query optimization techniques
- [Async Programming](Async-Multithreading.md) - Async database queries
- [Design Patterns](Design-Patterns.md) - Repository and Unit of Work patterns
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14
