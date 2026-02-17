# Multi-Tenancy with Global Query Filters

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


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
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

