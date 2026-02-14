// ==============================================================================
// ENTITY FRAMEWORK CORE - PERFORMANCE OPTIMIZATION
// Reference: Revision Notes - Entity Framework Core (Section 8.6.4)
// ==============================================================================
// PURPOSE: Master EF Core performance optimization techniques to make your app
//          fast and scalable. Poor EF performance is one of the top reasons
//          for slow web applications. This file shows you how to fix it.
//
// WHAT YOU'LL LEARN:
//   • Projection (Select only needed columns) - 50-90% less data transfer
//   • AsNoTracking for read-only queries - 30-40% faster
//   • Avoiding N+1 queries with Include - 10-100x faster
//   • Split queries for multiple includes - Prevents cartesian explosion
//   • Compiled queries - Pre-compile frequently-used queries
//   • Batch operations - Up to 50x faster than loops
//   • Paging with Skip/Take - Essential for large datasets
//   • Indexing - 100x faster lookups
//
// PERFORMANCE MINDSET:
//   1. Measure first (use profiling tools)
//   2. Optimize database queries before code
//   3. Network calls are expensive (minimize round trips)
//   4. Database is faster than your app at filtering/sorting
//
// COMMON BOTTLENECKS:
//   ❌ N+1 queries (loading related data in loops)
//   ❌ SELECT * when only need few columns
//   ❌ Change tracking for read-only queries
//   ❌ SaveChanges in loops
//   ❌ No indexes on queried columns
//   ❌ Loading entire table when only need page
//
// PROFILING TOOLS:
//   • MiniProfiler (see SQL in browser)
//   • Application Insights (Azure)
//   • SQL Server Profiler
//   • EF Core logging (options.LogTo(Console.WriteLine))
//
// KEY CONCEPTS: Projection, AsNoTracking, avoiding N+1, split queries, compiled queries, indexing
// ==============================================================================

using Microsoft.EntityFrameworkCore;

namespace RevisionNotesDemo.DataAccess.EntityFramework;

public class PerformanceCustomer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ICollection<PerformanceOrder> Orders { get; set; } = new List<PerformanceOrder>();
}

public class PerformanceOrder
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
    public int CustomerId { get; set; }
    public PerformanceCustomer Customer { get; set; } = null!;
}

public class PerformanceDbContext : DbContext
{
    public DbSet<PerformanceCustomer> Customers { get; set; }
    public DbSet<PerformanceOrder> Orders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("PerformanceDb")
            .EnableSensitiveDataLogging()
            .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Index for performance
        modelBuilder.Entity<PerformanceCustomer>()
            .HasIndex(c => c.Email)
            .IsUnique();

        // Composite index
        modelBuilder.Entity<PerformanceOrder>()
            .HasIndex(o => new { o.CustomerId, o.OrderDate });
    }
}

public class PerformanceOptimizationExamples
{
    /// <summary>
    /// EXAMPLE 1: Projection - Select Only What You Need
    /// 
    /// THE PROBLEM:
    ///   Fetching all columns when you only need a few wastes:
    ///   - Network bandwidth (transferring unnecessary data)
    ///   - Memory (storing data you won't use)
    ///   - CPU (serializing/deserializing)
    /// 
    /// THE SOLUTION:
    ///   Use .Select() to project only needed columns
    /// 
    /// PERFORMANCE IMPACT:
    ///   - 50-90% less data transferred
    ///   - Automatically uses AsNoTracking (bonus!)
    ///   - Faster queries (database does less work)
    /// 
    /// WHEN TO USE:
    ///   ✅ API responses (DTOs)
    ///   ✅ List views (only show name, not all 50 properties)
    ///   ✅ Reports (aggregate data)
    ///   ✅ Dropdowns (Id + Name only)
    /// 
    /// REAL-WORLD EXAMPLE:
    ///   Customer table has 20 columns (Name, Email, Address, Phone, etc.)
    ///   Your list view only shows Name and Email
    ///   ❌ Loading all 20 columns wastes 90% of data transfer
    ///   ✅ Project only Name and Email
    /// 
    /// TIP: Use anonymous types for internal use, DTOs for API responses
    /// </summary>
    public static async Task ProjectionExample()
    {
        using var context = new PerformanceDbContext();

        Console.WriteLine("\n❌ BAD - Fetches all columns:");
        // This generates: SELECT *, Email FROM Customers
        // Transfers ALL customer data (wasted bandwidth)
        var customersBad = await context.Customers.ToListAsync();
        // Even though we only use Name, we transferred Address, Phone, etc.

        Console.WriteLine("\n✅ GOOD - Projection (only needed columns):");
        // This generates: SELECT Id, Name, Email FROM Customers
        // Only transfers columns we actually use (efficient!)
        var customersGood = await context.Customers
            .Select(c => new { c.Id, c.Name, c.Email })  // ← Project only needed columns
            .ToListAsync();

        Console.WriteLine($"Projection fetches only {customersGood.Count} needed columns");
        Console.WriteLine("Saved 50-90% data transfer!");

        // BONUS: Projection automatically uses AsNoTracking (no tracking overhead)

        // TIP: For API responses, project to DTO:
        // var customers = await context.Customers
        //     .Select(c => new CustomerDto { Id = c.Id, Name = c.Name })
        //     .ToListAsync();
    }

    /// <summary>
    /// EXAMPLE 2: AsNoTracking (30-40% faster)
    /// </summary>
    public static async Task AsNoTrackingPerformance()
    {
        using var context = new PerformanceDbContext();

        Console.WriteLine("\n❌ With Tracking (slower for read-only):");
        var tracked = await context.Customers.ToListAsync();

        Console.WriteLine("\n✅ AsNoTracking (30-40% faster):");
        var noTracking = await context.Customers
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// EXAMPLE 3: Avoiding N+1 Query Problem - The #1 Performance Killer
    /// 
    /// REVIEW: N+1 is when you load a collection, then query related data in a loop.
    ///   - 1 query to load entities
    ///   - N queries (one per entity) for related data
    ///   - Total: 1 + N queries (DISASTER with 100+ entities)
    /// 
    /// THE FIX: Use .Include() to load everything in 1 query with JOIN
    /// 
    /// PERFORMANCE IMPACT:
    ///   ❌ BAD: 101 queries for 100 customers = 1-5 seconds
    ///   ✅ GOOD: 1 query with JOIN = 50-100ms
    ///   → 10-100x faster!
    /// 
    /// HOW TO DETECT:
    ///   - Enable SQL logging: options.LogTo(Console.WriteLine)
    ///   - Look for many similar sequential queries
    ///   - Use MiniProfiler to see "Duplicate queries" warning
    /// 
    /// REAL-WORLD: This is the most common EF performance bug in production.
    ///             Fixing N+1 can turn a 5-second page load into 50ms.
    /// 
    /// NESTED INCLUDES:
    ///   Need Orders.OrderItems too?
    ///   .Include(c => c.Orders).ThenInclude(o => o.OrderItems)
    /// </summary>
    public static async Task AvoidingN1Queries()
    {
        using var context = new PerformanceDbContext();

        // Seed test data with related entities
        var customer = new PerformanceCustomer
        {
            Name = "John Doe",
            Email = "john@example.com",
            Orders = new List<PerformanceOrder>
            {
                new PerformanceOrder { OrderDate = DateTime.Now, Total = 99.99m },
                new PerformanceOrder { OrderDate = DateTime.Now.AddDays(-1), Total = 149.99m }
            }
        };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        Console.WriteLine("\n❌ BAD - N+1 queries:");
        // Load customers (Query 1)
        var customers1 = await context.Customers.ToListAsync();
        foreach (var c in customers1)
        {
            // Query 2, 3, 4... one PER customer (N+1 PROBLEM!)
            var orderCount = await context.Orders
                .Where(o => o.CustomerId == c.Id)
                .CountAsync();  // Separate query for EACH customer!
            // With 100 customers = 101 database round trips = SLOW!
        }

        Console.WriteLine("\n✅ GOOD - Single query with Include:");
        // Load customers AND orders in ONE query with JOIN
        var customers2 = await context.Customers
            .Include(c => c.Orders)  // ← JOIN Orders (1 query total!)
            .ToListAsync();
        // Now Orders are already loaded, no more queries needed
        foreach (var c in customers2)
        {
            var orderCount = c.Orders.Count;  // No database query!
            // Data already in memory
        }

        // IMPORTANT: Always test with realistic data (100+ records)
        // N+1 is not obvious with only 3 test records!
    }

    /// <summary>
    /// EXAMPLE 4: Split Queries (for multiple includes)
    /// </summary>
    public static async Task SplitQueriesExample()
    {
        using var context = new PerformanceDbContext();

        Console.WriteLine("\n❌ Single query with multiple includes (cartesian explosion):");
        var customers1 = await context.Customers
            .Include(c => c.Orders)
            .ToListAsync();

        Console.WriteLine("\n✅ Split queries (EF Core 5+):");
        var customers2 = await context.Customers
            .Include(c => c.Orders)
            .AsSplitQuery()
            .ToListAsync();
    }

    /// <summary>
    /// EXAMPLE 5: Compiled Queries
    /// </summary>
    private static readonly Func<PerformanceDbContext, int, Task<PerformanceCustomer?>>
        GetCustomerById = EF.CompileAsyncQuery(
            (PerformanceDbContext db, int id) =>
                db.Customers
                    .Include(c => c.Orders)
                    .FirstOrDefault(c => c.Id == id)
        );

    public static async Task CompiledQueryExample()
    {
        using var context = new PerformanceDbContext();

        Console.WriteLine("\n✅ Compiled query (compiled once, reused):");
        var customer = await GetCustomerById(context, 1);

        Console.WriteLine("Compiled query executed efficiently");
    }

    /// <summary>
    /// EXAMPLE 6: Batch Operations - Never SaveChanges in a Loop!
    /// 
    /// THE PROBLEM:
    ///   Calling SaveChanges inside a loop = each iteration hits database
    ///   - 100 iterations = 100 database round trips
    ///   - Each round trip has network latency (~10-50ms)
    ///   - Total time: 1-5 seconds for 100 records
    /// 
    /// THE SOLUTION:
    ///   Batch all changes, call SaveChanges ONCE
    ///   - 1 database round trip
    ///   - All changes in single transaction (atomic)
    ///   - Total time: 50-200ms for 100 records
    /// 
    /// PERFORMANCE IMPACT:
    ///   10-50x faster! (not exaggerating)
    /// 
    /// HOW IT WORKS:
    ///   EF Core batches multiple INSERTs/UPDATEs into single SQL statement:
    ///   INSERT INTO Customers VALUES (...), (...), (...)
    /// 
    /// GOTCHA: There's a limit (~1000 records recommended)
    ///         For larger batches, consider:
    ///         - EF Core Bulk Extensions library
    ///         - SqlBulkCopy for SQL Server
    ///         - Batch in chunks (e.g., SaveChanges every 1000 records)
    /// 
    /// REAL-WORLD: Data imports, nightly batch jobs, migrations
    /// </summary>
    public static async Task BatchOperationsExample()
    {
        using var context = new PerformanceDbContext();

        // Create 100 test customers
        var customers = Enumerable.Range(1, 100).Select(i => new PerformanceCustomer
        {
            Name = $"Customer {i}",
            Email = $"customer{i}@example.com"
        }).ToList();

        Console.WriteLine("\n❌ BAD - Multiple round trips:");
        Console.WriteLine("  foreach (customer in customers) {");
        Console.WriteLine("      context.Add(customer);");
        Console.WriteLine("      context.SaveChanges();  // ← Database call EACH time!");
        Console.WriteLine("  }");
        Console.WriteLine("  Result: 100 separate database calls (1-5 seconds)");
        // Don't actually do this - it's slow!

        Console.WriteLine("\n✅ GOOD - Single batch:");
        // Add all customers to context (tracked, but not saved yet)
        context.Customers.AddRange(customers);  // Or: customers.ForEach(c => context.Add(c))

        // Single SaveChanges = single database call!
        await context.SaveChangesAsync();  // ← Batches all INSERTs together

        Console.WriteLine($"Batch inserted {customers.Count} customers");
        Console.WriteLine("Result: 1 database call (50-200ms)");

        // TIP: For very large batches (10,000+), consider chunking:
        // foreach (var chunk in customers.Chunk(1000)) {
        //     context.AddRange(chunk);
        //     await context.SaveChangesAsync();
        // }
    }

    /// <summary>
    /// EXAMPLE 7: Paging
    /// </summary>
    public static async Task PagingExample()
    {
        using var context = new PerformanceDbContext();

        int page = 1, pageSize = 10;

        Console.WriteLine("\n✅ Paging (always for large sets):");
        var customers = await context.Customers
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        Console.WriteLine($"Retrieved page {page} with {pageSize} items");
    }

    /// <summary>
    /// EXAMPLE 8: Performance Checklist - Quick Reference for Code Reviews
    /// 
    /// Use this checklist when reviewing EF Core code or optimizing slow queries.
    /// These are the most impactful optimizations (80/20 rule).
    /// 
    /// PRIORITY 1 - FIX THESE FIRST (biggest impact):
    ///   ✅ Avoid N+1 queries (use Include)
    ///   ✅ Batch operations (single SaveChanges)
    ///   ✅ Add indexes on frequently queried columns
    ///   ✅ Page large result sets (Skip/Take)
    /// 
    /// PRIORITY 2 - EASY WINS (30-40% improvements):
    ///   ✅ Use AsNoTracking for read-only queries
    ///   ✅ Use projection (Select) instead of fetching all columns
    ///   ✅ Filter in database (Where before ToList)
    /// 
    /// PRIORITY 3 - ADVANCED (specific scenarios):
    ///   ✅ Use Split queries for multiple includes
    ///   ✅ Use Compiled queries for frequently executed queries
    ///   ✅ Use filtered includes (EF Core 5+)
    ///   ✅ Monitor queries with logging/profiling
    /// 
    /// RED FLAGS (code smells):
    ///   ❌ SaveChanges inside foreach loop
    ///   ❌ Database queries inside foreach loop
    ///   ❌ .ToList() followed by .Where() (filter in memory)
    ///   ❌ Loading all columns when only need few
    ///   ❌ No AsNoTracking on read-only queries
    ///   ❌ No paging for list views
    ///   ❌ No indexes on WHERE/ORDER BY columns
    /// 
    /// PROFILING:
    ///   Always measure before and after optimization!
    ///   Tools: MiniProfiler, Application Insights, EF Core logging
    /// </summary>
    public static void PerformanceChecklist()
    {
        Console.WriteLine("\n=== PERFORMANCE CHECKLIST ===\n");
        Console.WriteLine("✅ Use AsNoTracking for read-only queries (30-40% faster)");
        Console.WriteLine("✅ Use projection (Select) instead of fetching all columns (50-90% less data)");
        Console.WriteLine("✅ Use Include to avoid N+1 queries (10-100x faster)");
        Console.WriteLine("✅ Use Split queries for multiple includes (prevents cartesian explosion)");
        Console.WriteLine("✅ Use Compiled queries for frequently executed queries (cached execution plan)");
        Console.WriteLine("✅ Use AddRange + single SaveChanges for batch operations (10-50x faster)");
        Console.WriteLine("✅ Always page large result sets with Skip/Take (essential for lists)");
        Console.WriteLine("✅ Add indexes to frequently queried columns (100x faster lookups)");
        Console.WriteLine("✅ Use filtered includes (EF Core 5+) to load partial collections");
        Console.WriteLine("✅ Monitor queries with logging/profiling (find bottlenecks)");
        Console.WriteLine("\nRED FLAGS:");
        Console.WriteLine("❌ SaveChanges in loop (batch instead)");
        Console.WriteLine("❌ Database queries in loop (use Include or Contains)");
        Console.WriteLine("❌ .ToList() then .Where() (filter in SQL, not memory)");
        Console.WriteLine("❌ No paging for large datasets (always use Skip/Take)");
    }

    public static async Task RunAllExamples()
    {
        Console.WriteLine("\n=== ENTITY FRAMEWORK - PERFORMANCE OPTIMIZATION ===\n");
        await ProjectionExample();
        await AsNoTrackingPerformance();
        await AvoidingN1Queries();
        await SplitQueriesExample();
        await CompiledQueryExample();
        await BatchOperationsExample();
        await PagingExample();
        PerformanceChecklist();
        Console.WriteLine("\nPerformance Optimization examples completed!\n");
    }
}
