// ==============================================================================
// ENTITY FRAMEWORK: MULTI-TENANCY PATTERNS
// ==============================================================================
// PURPOSE: Isolate data by tenant in SaaS applications
// WHY: Security, compliance, data isolation required for SaaS
// USE WHEN: Building multi-tenant applications with shared database
// ==============================================================================
// WHAT IS MULTI-TENANCY?
// Multi-tenancy is an architecture where a single application instance serves
// multiple customers (tenants), with each tenant's data logically isolated from
// others. In a shared database model, all tenants' data is in the same tables,
// distinguished by a TenantId column.
//
// THE CHALLENGE:
// Every database query MUST filter by TenantId to prevent data leaks.
// Manually adding .Where(x => x.TenantId == currentTenant) to every query is:
// • Error-prone (easy to forget one query = data leak = compliance violation)
// • Repetitive (hundreds of queries in a typical app)
// • Hard to maintain (every developer must remember every time)
// • Security-critical (one missed filter can expose competitor data)
//
// THE SOLUTION: GLOBAL QUERY FILTERS
// EF Core's Global Query Filters automatically add WHERE clauses to ALL queries.
// Configure once in OnModelCreating(), applies automatically everywhere.
//
// WHY IT MATTERS:
// • SECURITY: Zero chance of data leak (filter applied automatically)
// • COMPLIANCE: GDPR, SOC 2 require data isolation - filters enforce it
// • DRY: Configure once, not in every query (eliminates 1000s of .Where() calls)
// • MAINTAINABILITY: One place to change filtering logic
// • DEVELOPER SAFETY: New developers can't accidentally forget filter
//
// WHEN TO USE:
// ✅ SaaS applications with multiple customers sharing one database
// ✅ Multi-organization systems (schools, hospitals, franchises)
// ✅ B2B platforms where customers see only their data
// ✅ Compliance-critical applications (healthcare, finance)
//
// MULTI-TENANCY MODELS:
// 1. Separate Database per Tenant - Highest isolation, most expensive
// 2. Separate Schema per Tenant - Good isolation, moderate cost
// 3. Shared Database + TenantId (THIS PATTERN) - Most cost-effective, requires filters
//
// REAL-WORLD EXAMPLE:
// Slack: Each workspace is a tenant. When loading messages, MUST only show
// messages for the current workspace. Global Query Filter ensures that
// context.Messages.ToListAsync() automatically includes "WHERE TenantId = 'workspace-123'"
// across the entire codebase, preventing cross-workspace data leaks.
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.EntityFramework;

public static class MultiTenancyPatterns
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== EF CORE: MULTI-TENANCY PATTERNS ===\n");
        
        Example1_WithoutFilters();
        Example2_GlobalQueryFilters();
        Example3_TenantContext();
        Example4_CombinedFilters();
        
        Console.WriteLine("\n💡 Key Takeaways:");
        Console.WriteLine("   ✅ Use Global Query Filters for automatic filtering");
        Console.WriteLine("   ✅ Configure once, apply everywhere");
        Console.WriteLine("   ✅ Inject tenant context per request");
        Console.WriteLine("   ✅ Combine with soft deletes, active flags");
        Console.WriteLine("   ✅ Critical for SaaS data security");
    }
    
    private static void Example1_WithoutFilters()
    {
        Console.WriteLine("=== EXAMPLE 1: Without Filters (Dangerous!) ===\n");
        
        Console.WriteLine("❌ BAD: Manual tenant filtering everywhere\n");
        // PROBLEM: Easy to forget filter
        // public async Task<List<Order>> GetOrdersAsync() {
        //     // ❌ NO TENANT FILTER - EXPOSES ALL TENANTS' DATA!
        //     return await _context.Orders.ToListAsync();
        // }
        //
        // PROBLEM: Repetitive code
        // public async Task<Order?> GetOrderAsync(int orderId) {
        //     return await _context.Orders
        //         .Where(o => o.TenantId == _currentTenantId)  // Repeated everywhere
        //         .FirstOrDefaultAsync(o => o.Id == orderId);
        // }
        
        Console.WriteLine("\n💥 Problems:");
        Console.WriteLine("   • Data leak risk");
        Console.WriteLine("   • Repetitive code");
        Console.WriteLine("   • Easy to forget");
        Console.WriteLine("   • Error-prone");
    }
    
    private static void Example2_GlobalQueryFilters()
    {
        Console.WriteLine("\n=== EXAMPLE 2: Global Query Filters (Automatic) ===\n");
        
        Console.WriteLine("✅ GOOD: Configure filters once, apply to ALL queries\n");
        // public class AppDbContext : DbContext {
        //     private readonly ITenantProvider _tenantProvider;
        //     
        //     protected override void OnModelCreating(ModelBuilder modelBuilder) {
        //         // ✅ Configure global query filter
        //         modelBuilder.Entity<Order>()
        //             .HasQueryFilter(o => o.TenantId == _tenantProvider.GetCurrentTenantId());
        //         
        //         // This filter is AUTOMATICALLY applied to ALL queries!
        //     }
        // }
        //
        // Service (NO manual filtering needed!)
        // public async Task<List<Order>> GetOrdersAsync() {
        //     // ✅ Automatic: WHERE TenantId = @CurrentTenant
        //     return await _context.Orders.ToListAsync();
        // }
        
        Console.WriteLine("\n📊 Benefits:");
        Console.WriteLine("   • Zero chance of forgetting tenant filter");
        Console.WriteLine("   • Clean service code");
        Console.WriteLine("   • Secure by default");
        Console.WriteLine("   • One place to change logic");
    }
    
    private static void Example3_TenantContext()
    {
        Console.WriteLine("\n=== EXAMPLE 3: Tenant Context Setup ===\n");
        
        Console.WriteLine("✅ Register tenant provider as scoped\n");
        // Program.cs
        // builder.Services.AddHttpContextAccessor();
        // builder.Services.AddScoped<ITenantProvider, HttpTenantProvider>();
        // builder.Services.AddDbContext<AppDbContext>(...);
        //
        // public interface ITenantProvider {
        //     int GetCurrentTenantId();
        // }
        //
        // public class HttpTenantProvider : ITenantProvider {
        //     private readonly IHttpContextAccessor _httpContext;
        //     
        //     public int GetCurrentTenantId() {
        //         var tenantIdClaim = _httpContext.HttpContext?.User
        //             .FindFirst("TenantId")?.Value;
        //         if (int.TryParse(tenantIdClaim, out var tenantId))
        //             return tenantId;
        //         throw new UnauthorizedAccessException("No tenant context");
        //     }
        // }
        
        Console.WriteLine("\n📊 Flow:");
        Console.WriteLine("   1. User authenticates (JWT with TenantId claim)");
        Console.WriteLine("   2. HttpTenantProvider reads TenantId from claims");
        Console.WriteLine("   3. DbContext uses provider in query filter");
        Console.WriteLine("   4. All queries automatically filtered");
    }
    
    private static void Example4_CombinedFilters()
    {
        Console.WriteLine("\n=== EXAMPLE 4: Combined Filters ===\n");
        
        Console.WriteLine("✅ Combine tenant filters with soft deletes, active flags\n");
        // modelBuilder.Entity<Order>()
        //     .HasQueryFilter(o => 
        //         o.TenantId == _tenantProvider.GetCurrentTenantId() &&
        //         !o.IsDeleted &&
        //         o.IsActive);
        //
        // Now ALL queries automatically filter by:
        // - Current tenant
        // - Not deleted
        // - Active records only
        
        Console.WriteLine("\n📊 Common Combinations:");
        Console.WriteLine("   • TenantId + IsDeleted (soft delete)");
        Console.WriteLine("   • TenantId + IsActive");
        Console.WriteLine("   • TenantId + IsPublished");
        Console.WriteLine("   • Multiple entities with same filter");
        
        Console.WriteLine("\n💡 Advanced:");
        Console.WriteLine("   • Use IgnoreQueryFilters() to bypass when needed");
        Console.WriteLine("   • Example: Admin view showing all tenants");
        // var allOrders = await _context.Orders
        //     .IgnoreQueryFilters()
        //     .ToListAsync();
    }
}
