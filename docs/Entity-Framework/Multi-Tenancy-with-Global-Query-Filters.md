# Multi-Tenancy with Global Query Filters

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Relational data modeling and basic LINQ provider behavior.
- Related examples: docs/Entity-Framework/README.md
> Subject: [Entity-Framework](../README.md)

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


## Interview Answer Block
30-second answer:
- Multi-Tenancy with Global Query Filters is about ORM-based data modeling and persistence. It matters because query shape and tracking behavior strongly affect performance.
- Use it when building data access layers with maintainable domain mappings.

2-minute answer:
- Start with the problem Multi-Tenancy with Global Query Filters solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: developer productivity vs query/control precision.
- Close with one failure mode and mitigation: N+1 queries and incorrect tracking strategy.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Multi-Tenancy with Global Query Filters but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Multi-Tenancy with Global Query Filters, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Multi-Tenancy with Global Query Filters and map it to one concrete implementation in this module.
- 3 minutes: compare Multi-Tenancy with Global Query Filters with an alternative, then walk through one failure mode and mitigation.