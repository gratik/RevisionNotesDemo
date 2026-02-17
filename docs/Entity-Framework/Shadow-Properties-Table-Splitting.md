# Shadow Properties & Table Splitting

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Relational data modeling and basic LINQ provider behavior.
- Related examples: docs/Entity-Framework/README.md
> Subject: [Entity-Framework](../README.md)

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


## Interview Answer Block
30-second answer:
- Shadow Properties & Table Splitting is about ORM-based data modeling and persistence. It matters because query shape and tracking behavior strongly affect performance.
- Use it when building data access layers with maintainable domain mappings.

2-minute answer:
- Start with the problem Shadow Properties & Table Splitting solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: developer productivity vs query/control precision.
- Close with one failure mode and mitigation: N+1 queries and incorrect tracking strategy.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Shadow Properties & Table Splitting but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Shadow Properties & Table Splitting, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Shadow Properties & Table Splitting and map it to one concrete implementation in this module.
- 3 minutes: compare Shadow Properties & Table Splitting with an alternative, then walk through one failure mode and mitigation.