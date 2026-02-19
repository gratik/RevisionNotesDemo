# Entity Framework Core Best Practices

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: LINQ and relational database basics
- Related examples: Learning/DataAccess/EntityFramework/EntityFrameworkBestPractices.cs, Learning/DataAccess/EntityFramework/PerformanceOptimizationExamples.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../README.md)

## Module Metadata

- **Prerequisites**: Data Access
- **When to Study**: After Data Access overview when focusing on ORM-heavy systems.
- **Related Files**: `../Learning/DataAccess/EntityFramework/*.cs`
- **Estimated Time**: 120-150 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](../Learning-Path.md) | [Track Start](../Configuration.md)
- **Next Step**: [Security.md](../Security.md)
<!-- STUDY-NAV-END -->


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

## Multi-Tenancy with Global Query Filters

### The Problem: Manual Tenant Filtering

❌ **BAD:** Manually filter by tenant in every query (error-prone):

```csharp
public async Task<List<Order>> GetOrdersAsync()
{
    // Easy to forget this filter!
    return await _context.Orders
        .Where(o => o.TenantId == _currentTenantId)
        .ToListAsync();
}
```

**Problems:**
- Data leak risk if filter forgotten
- Repetitive code
- Error-prone

### The Solution: Global Query Filters

✅ **GOOD:** Configure filter once, applies automatically:

```csharp
public class AppDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ✅ Configure global query filter
        modelBuilder.Entity<Order>()
            .HasQueryFilter(o => o.TenantId == _tenantProvider.GetCurrentTenantId());
        
        // This filter is AUTOMATICALLY applied to ALL queries!
    }
}

// Service code - no manual filtering needed!
public async Task<List<Order>> GetOrdersAsync()
{
    // ✅ Automatic: WHERE TenantId = @CurrentTenant
    return await _context.Orders.ToListAsync();
}
```

### Tenant Provider Setup

```csharp
public interface ITenantProvider
{
    int GetCurrentTenantId();
}

public class HttpTenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContext;
    
    public int GetCurrentTenantId()
    {
        var tenantIdClaim = _httpContext.HttpContext?.User
            .FindFirst("TenantId")?.Value;
        
        if (int.TryParse(tenantIdClaim, out var tenantId))
            return tenantId;
        
        throw new UnauthorizedAccessException("No tenant context");
    }
}

// Program.cs
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, HttpTenantProvider>();
```

### Combined Filters

Combine tenant filters with soft deletes, active flags, etc.:

```csharp
modelBuilder.Entity<Order>()
    .HasQueryFilter(o => 
        o.TenantId == _tenantProvider.GetCurrentTenantId() &&
        !o.IsDeleted &&
        o.IsActive);
```

### Bypassing Filters

Use `IgnoreQueryFilters()` when needed (e.g., admin views):

```csharp
var allOrders = await _context.Orders
    .IgnoreQueryFilters()
    .ToListAsync();
```

---

## Shadow Properties & Table Splitting

### Shadow Properties: Clean Domain Models

**Problem:** Audit fields pollute domain model:

❌ **BAD:**
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    
    // ❌ Infrastructure concerns in domain
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string ModifiedBy { get; set; }
}
```

**Solution:** Shadow properties keep domain clean:

✅ **GOOD:**
```csharp
// Clean domain model
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    // No audit fields!
}

// DbContext configuration
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>(entity => {
        // Add shadow properties (not in C# class)
        entity.Property<DateTime>("CreatedAt")
            .HasDefaultValueSql("GETUTCDATE()");
        entity.Property<string>("CreatedBy")
            .HasMaxLength(100);
        entity.Property<DateTime?>("ModifiedAt");
        entity.Property<string>("ModifiedBy")
            .HasMaxLength(100);
    });
}
```

### Querying Shadow Properties

```csharp
var recentProducts = await _context.Products
    .Where(p => EF.Property<DateTime>(p, "CreatedAt") > DateTime.UtcNow.AddDays(-7))
    .ToListAsync();
```

### Automatic Audit Trail

Override `SaveChanges` to populate shadow properties automatically:

```csharp
public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
{
    var entries = ChangeTracker.Entries()
        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
    
    var currentUser = _userProvider.GetCurrentUserId();
    var now = DateTime.UtcNow;
    
    foreach (var entry in entries)
    {
        if (entry.State == EntityState.Added)
        {
            entry.Property("CreatedAt").CurrentValue = now;
            entry.Property("CreatedBy").CurrentValue = currentUser;
        }
        
        if (entry.State == EntityState.Modified)
        {
            entry.Property("ModifiedAt").CurrentValue = now;
            entry.Property("ModifiedBy").CurrentValue = currentUser;
        }
    }
    
    return await base.SaveChangesAsync(ct);
}
```

### Table Splitting: Performance Optimization

Split one table into multiple entities for performance:

```csharp
// Main entity (frequently loaded)
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public ProductDetails Details { get; set; }
}

// Details entity (loaded on demand)
public class ProductDetails
{
    public int ProductId { get; set; }
    public string LongDescription { get; set; }  // Large text
    public byte[] Image { get; set; }           // Large binary
    public Product Product { get; set; }
}

// Configuration (both map to same table)
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>(entity => {
        entity.ToTable("Products");
        entity.HasOne(p => p.Details)
            .WithOne(d => d.Product)
            .HasForeignKey<ProductDetails>(d => d.ProductId);
    });
    
    modelBuilder.Entity<ProductDetails>(entity => {
        entity.ToTable("Products");  // Same table!
    });
}
```

**Benefits:**
- Faster list queries (load small entity only)
- Load details only when needed
- Single table in database
- Transparent to application

---

## Related Files

- [DataAccess/EntityFramework/EntityFrameworkBestPractices.cs](../../Learning/DataAccess/EntityFramework/EntityFrameworkBestPractices.cs)
- [DataAccess/EntityFramework/ChangeTrackingExamples.cs](../../Learning/DataAccess/EntityFramework/ChangeTrackingExamples.cs)
- [DataAccess/EntityFramework/RelationshipsNavigationExamples.cs](../../Learning/DataAccess/EntityFramework/RelationshipsNavigationExamples.cs)
- [DataAccess/EntityFramework/PerformanceOptimizationExamples.cs](../../Learning/DataAccess/EntityFramework/PerformanceOptimizationExamples.cs)
- [DataAccess/EntityFramework/MigrationsInDepthExamples.cs](../../Learning/DataAccess/EntityFramework/MigrationsInDepthExamples.cs)
- [DataAccess/EntityFramework/MultiTenancyPatterns.cs](../../Learning/DataAccess/EntityFramework/MultiTenancyPatterns.cs)
- [DataAccess/EntityFramework/ShadowPropertiesExamples.cs](../../Learning/DataAccess/EntityFramework/ShadowPropertiesExamples.cs)

---

## See Also

- [Data Access](../Data-Access.md) - EF Core vs Dapper vs ADO.NET
- [Performance](../Performance.md) - Query optimization techniques
- [Async Programming](../Async-Multithreading.md) - Async database queries
- [Design Patterns](../Design-Patterns.md) - Repository and Unit of Work patterns
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [Security.md](../Security.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: This topic covers Entity Framework and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know Entity Framework and I would just follow best practices."
- Strong answer: "For Entity Framework, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Entity Framework in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.

## Topic Files

- [Tracking vs No-Tracking Queries](Tracking-vs-No-Tracking-Queries.md)
- [Query Performance](Query-Performance.md)
- [Relationships and Navigation Properties](Relationships-and-Navigation-Properties.md)
- [DbContext Configuration](DbContext-Configuration.md)
- [Migrations](Migrations.md)
- [Advanced Patterns](Advanced-Patterns.md)
- [Best Practices](Best-Practices.md)
- [Common Pitfalls](Common-Pitfalls.md)
- [Multi-Tenancy with Global Query Filters](Multi-Tenancy-with-Global-Query-Filters.md)
- [Shadow Properties & Table Splitting](Shadow-Properties-Table-Splitting.md)



