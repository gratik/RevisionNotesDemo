// ==============================================================================
// ENTITY FRAMEWORK CORE - BEST PRACTICES & ANTI-PATTERNS  
// Reference: Revision Notes - Entity Framework Core (Section 8.6.5)
// ==============================================================================
// PURPOSE: Your essential guide to avoiding the most common and costly EF Core
//          mistakes. Each example shows the WRONG way (❌) vs the RIGHT way (✅)
//          with performance metrics and real-world impact.
//
// WHY THIS FILE EXISTS:
//   These are the top 8 mistakes that cause 90% of EF Core performance problems
//   in production. Learning these patterns will save you hours of debugging and
//   dramatically improve your app's performance.
//
// WHAT YOU'LL LEARN:
//   1. N+1 Query Problem - The #1 performance killer (10-100x slower!)
//   2. Tracking Overhead - Why read-only queries are 30-40% slower by default
//   3. Projection - How to reduce data transfer by 75%
//   4. Batch Operations - 10-50x faster than loops
//   5. Filter Location - Database vs memory (massive difference)
//   6. Query Batching - Avoid loops with smart queries
//   7. Context Lifetime - Prevent memory leaks
//   8. Efficient Updates - Update without loading
//
// REAL-WORLD IMPACT:
//   • Fixing N+1 queries: 5 seconds → 50ms
//   • Using AsNoTracking: 30-40% faster read queries
//   • Batch operations: 10-50x faster inserts
//   • Projection: 75% less network data
//
// HOW TO USE THIS FILE:
//   1. Read through each example (they build on each other)
//   2. Run the examples to see the difference
//   3. Apply these patterns to your own code
//   4. Use the checklist at the end for code reviews
//
// TESTING ADVICE:
//   Always test with realistic data volumes (100+ records).
//   Performance problems don't show up with 3 test records!
//
// FILE: Learning/DataAccess/EntityFramework/EntityFrameworkBestPractices.cs
// ==============================================================================

using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace RevisionNotesDemo.DataAccess.EntityFramework;

/// <summary>
/// Sample Blog entity for demonstrating best practices.
/// Represents a blog with multiple posts (one-to-many relationship).
/// 
/// REAL-WORLD ANALOGY:
///   Think: Customer → Orders, Author → Books, Department → Employees
///   Same relationship pattern applies everywhere.
/// </summary>
public class Blog
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    // Collection navigation property - one blog has many posts
    public ICollection<Post> Posts { get; set; } = new List<Post>();
}

/// <summary>
/// Post entity - the "many" side of the Blog-Post relationship.
/// Each post belongs to exactly one blog.
/// </summary>
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    // Foreign key
    public int BlogId { get; set; }

    // Reference navigation property - post belongs to one blog
    public Blog Blog { get; set; } = null!;
}

/// <summary>
/// DbContext for best practices examples.
/// 
/// NOTE: Uses InMemoryDatabase for simplicity.
///       In production, configure via dependency injection in Startup.cs/Program.cs:
///       services.AddDbContext<YourContext>(options => 
///           options.UseSqlServer(connectionString));
/// </summary>
public class BestPracticesDbContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // InMemoryDatabase: Great for demos, testing, prototyping
        // Production: Use UseSqlServer, UseNpgsql, UseSqlite, etc.
        optionsBuilder.UseInMemoryDatabase("BestPracticesDb");
    }
}

/// <summary>
/// Comprehensive examples showing good vs bad EF Core practices.
/// 
/// STRUCTURE:
///   Each example follows the pattern:
///   1. ❌ BAD - Shows the anti-pattern (what NOT to do)
///   2. ✅ GOOD - Shows the correct approach
///   3. Explanation - WHY it matters, performance impact
/// 
/// LEARNING APPROACH:
///   Run examples sequentially - they're ordered by importance.
///   The first few examples (N+1, AsNoTracking, Projection) have
///   the biggest impact on real-world applications.
/// </summary>
public class EntityFrameworkBestPractices
{
    /// <summary>
    /// Helper method to seed test data into the database.
    /// Creates 3 blogs: 2 active (one with posts) and 1 inactive.
    /// 
    /// IDEMPOTENT: Safe to call multiple times (checks if data exists)
    /// </summary>
    private static void SeedData(BestPracticesDbContext context)
    {
        if (context.Blogs.Any()) return;

        context.Blogs.AddRange(
            new Blog
            {
                Name = "Tech Blog",
                Description = "Technology",
                IsActive = true,
                Posts = new List<Post>
                {
                    new Post { Title = "Post 1", Content = "Content 1" },
                    new Post { Title = "Post 2", Content = "Content 2" }
                }
            },
            new Blog { Name = "Food Blog", Description = "Recipes", IsActive = true },
            new Blog { Name = "Old Blog", Description = "Archived", IsActive = false }
        );
        context.SaveChanges();
    }

    /// <summary>
    /// EXAMPLE 1: N+1 Query Problem - THE #1 PERFORMANCE KILLER
    /// 
    /// THE PROBLEM:
    ///   Loading entities, then accessing related data in a loop triggers
    ///   1 query + N queries (one per entity) = disaster with large datasets.
    /// 
    /// WHY IT'S THE #1 MISTAKE:
    ///   • Affects 80% of EF Core applications
    ///   • Can make apps 10-100x slower
    ///   • Not obvious with small test datasets
    ///   • Easy to accidentally introduce
    /// 
    /// REAL-WORLD IMPACT:
    ///   • 100 blogs = 101 database queries!
    ///   • Each query has ~10-50ms latency
    ///   • Total: 1-5 seconds instead of 50ms
    ///   • Can bring down production systems
    /// 
    /// THE FIX:
    ///   Use .Include() to load related data in ONE query with JOIN.
    ///   SQL: SELECT * FROM Blogs b LEFT JOIN Posts p ON b.Id = p.BlogId
    /// 
    /// HOW TO DETECT:
    ///   • Enable SQL logging: options.LogTo(Console.WriteLine)
    ///   • Use profiling tools (MiniProfiler, Application Insights)
    ///   • Look for many sequential similar queries in logs
    /// 
    /// NESTED RELATIONSHIPS:
    ///   Need Posts.Comments too?
    ///   .Include(b => b.Posts).ThenInclude(p => p.Comments)
    /// </summary>
    public static async Task Example1_N1QueryProblem()
    {
        Console.WriteLine("\n=== EXAMPLE 1: N+1 Query Problem ===");

        using var context = new BestPracticesDbContext();
        SeedData(context);

        Console.WriteLine("\n❌ BAD: N+1 queries (1 + N separate database calls)");
        // This loads blogs only (Query 1)
        var blogs1 = await context.Blogs.ToListAsync();
        foreach (var blog in blogs1)
        {
            // Accessing Posts triggers a separate query for EACH blog (Queries 2, 3, 4...)
            var postCount = blog.Posts.Count; // ← SEPARATE DATABASE QUERY!
            Console.WriteLine($"   {blog.Name}: {postCount} posts");
        }
        Console.WriteLine("   Problem: Made 1 + 3 = 4 database queries");
        Console.WriteLine("   With 100 blogs: 1 + 100 = 101 queries (DISASTER!)");

        Console.WriteLine("\n✅ GOOD: Single query with Include");
        // Load blogs AND posts in ONE query with SQL JOIN
        var blogs2 = await context.Blogs
            .Include(b => b.Posts)  // ← Eager load posts with JOIN
            .ToListAsync();

        foreach (var blog in blogs2)
        {
            // Posts already loaded - NO database query!
            Console.WriteLine($"   {blog.Name}: {blog.Posts.Count} posts");
        }
        Console.WriteLine("   Benefit: Only 1 database query with JOIN");
        Console.WriteLine("   Performance: 10-100x faster with large datasets!");

        // TIP: Always test with realistic data (100+ records)
        // N+1 is not obvious with only 3 test records!
    }

    /// <summary>
    /// EXAMPLE 2: Tracking Overhead - Free 30-40% Performance Gain
    /// 
    /// THE PROBLEM:
    ///   By default, EF Core tracks ALL entities you load (for change detection).
    ///   For read-only queries (reports, lists, GET endpoints), this is wasted overhead.
    /// 
    /// WHAT IS TRACKING:
    ///   EF creates a "snapshot" of each entity to detect changes.
    ///   Takes memory + CPU time for something you don't need.
    /// 
    /// WHEN TO USE AsNoTracking:
    ///   ✅ Reports, dashboards, analytics
    ///   ✅ List views, search results
    ///   ✅ API GET endpoints
    ///   ✅ Any query where you won't update entities
    ///   ❌ When you plan to update entities later
    /// 
    /// PERFORMANCE IMPACT:
    ///   • 30-40% faster queries
    ///   • Less memory usage
    ///   • Especially noticeable with large result sets
    /// 
    /// AUTOMATIC AsNoTracking:
    ///   Projection (Select) automatically uses AsNoTracking.
    ///   So: .Select(b => new { b.Name }) is already optimized.
    /// 
    /// CONTEXT-LEVEL SETTING:
    ///   For read-heavy apps, set default behavior:
    ///   context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    /// </summary>
    public static async Task Example2_TrackingOverhead()
    {
        Console.WriteLine("\n=== EXAMPLE 2: Tracking Overhead ===");

        using var context = new BestPracticesDbContext();
        SeedData(context);

        Console.WriteLine("\n❌ BAD: Default tracking for read-only scenario");
        // EF Core creates snapshots of all entities (wasted work for read-only)
        var blogs1 = await context.Blogs.ToListAsync();
        Console.WriteLine($"   Loaded {blogs1.Count} blogs WITH tracking (slower)");
        Console.WriteLine("   EF created snapshots for change detection (unnecessary overhead)");

        Console.WriteLine("\n✅ GOOD: AsNoTracking for read-only queries");
        // Skip change tracking - just load the data
        var blogs2 = await context.Blogs
            .AsNoTracking()  // ← 30-40% faster!
            .ToListAsync();
        Console.WriteLine($"   Loaded {blogs2.Count} blogs WITHOUT tracking (30-40% faster)");
        Console.WriteLine("   No snapshots, less memory, faster query");

        // IMPORTANT: AsNoTracking entities are "detached"
        // If you modify them and call SaveChanges(), nothing happens!
    }

    /// <summary>
    /// EXAMPLE 3: Projection - Load Only What You Need
    /// 
    /// THE PROBLEM:
    ///   Loading entire entities when you only need 2-3 properties wastes:
    ///   • Database resources (larger result sets)
    ///   • Network bandwidth (more data transferred)
    ///   • Application memory (storing unused data)
    ///   • Serialization cost (sending to frontend)
    /// 
    /// THE SOLUTION:
    ///   Use .Select() to load ONLY the columns you need.
    ///   
    /// BENEFITS:
    ///   • 75% less data transferred (typical)
    ///   • Faster queries (SQL: SELECT Id, Name vs SELECT *)
    ///   • Less memory usage
    ///   • Automatic AsNoTracking (bonus!)
    /// 
    /// WHEN TO USE:
    ///   ✅ List views (only need Id, Name, Status)
    ///   ✅ Dropdowns (Id, Name)
    ///   ✅ API responses (don't expose internal fields)
    ///   ✅ Reports (specific aggregations)
    ///   ❌ When you need the full entity
    /// 
    /// REAL-WORLD EXAMPLE:
    ///   Blog entity: 20 properties including large Description field
    ///   Blog list: only need Id, Name (2 properties)
    ///   Result: 90% less data transferred!
    /// 
    /// ADVANCED:
    ///   Can project to DTOs, anonymous types, or tuples:
    ///   .Select(b => new BlogListDto { Id = b.Id, Name = b.Name })
    /// </summary>
    public static async Task Example3_Projection()
    {
        Console.WriteLine("\n=== EXAMPLE 3: Projection ===");

        using var context = new BestPracticesDbContext();
        SeedData(context);

        Console.WriteLine("\n❌ BAD: Loading entire entities when you only need 2 fields");
        // Loads ALL properties: Id, Name, Description, IsActive, Posts
        var blogs1 = await context.Blogs.ToListAsync();
        foreach (var blog in blogs1)
        {
            Console.WriteLine($"   {blog.Id}: {blog.Name}");  // Only using 2 properties!
        }
        Console.WriteLine("   Problem: Loaded all fields (Id, Name, Description, IsActive, Posts)");
        Console.WriteLine("   Wasted 75% of network bandwidth and memory");

        Console.WriteLine("\n✅ GOOD: Project to only needed fields");
        // SQL: SELECT Id, Name FROM Blogs (much smaller result set)
        var blogs2 = await context.Blogs
            .Select(b => new { b.Id, b.Name })  // ← Only load what's needed
            .ToListAsync();

        foreach (var blog in blogs2)
        {
            Console.WriteLine($"   {blog.Id}: {blog.Name}");
        }
        Console.WriteLine("   Benefit: 75% less data transferred");
        Console.WriteLine("   SQL: SELECT Id, Name (not SELECT *)");
        Console.WriteLine("   BONUS: Projection automatically uses AsNoTracking!");
    }

    /// <summary>
    /// EXAMPLE 4: Batch Operations - The Power of Single SaveChanges
    /// 
    /// THE PROBLEM:
    ///   Calling SaveChanges() inside a loop makes hundreds of database round-trips.
    ///   INCREDIBLY slow with large datasets (10-50x slower).
    /// 
    /// THE SOLUTION:
    ///   Add all entities to context, then call SaveChanges() ONCE.
    ///   EF batches them into efficient SQL.
    /// 
    /// WHY IT MATTERS:
    ///   • Database round-trip ~10-50ms
    ///   • 1000 inserts with SaveChanges in loop: 10-50 seconds
    ///   • 1000 inserts with single SaveChanges: 0.5-1 second
    ///   • Result: 10-50x faster!
    /// 
    /// REAL-WORLD SCENARIOS:
    ///   • Bulk imports (CSV, Excel)
    ///   • Data migrations
    ///   • Batch processing
    ///   • Seeding data
    /// 
    /// TRANSACTION SAFETY:
    ///   Single SaveChanges() is ONE transaction.
    ///   All succeed or all fail (ACID properties).
    /// 
    /// VERY LARGE BATCHES:
    ///   For 10,000+ records, batch in chunks:
    ///   for (int i = 0; i < items.Count; i += 1000) {
    ///       context.AddRange(items.Skip(i).Take(1000));
    ///       await context.SaveChangesAsync();
    ///   }
    /// 
    /// SQL SERVER SPECIFIC:
    ///   Consider BulkInsert libraries (EFCore.BulkExtensions)
    ///   for massive datasets (100k+ records).
    /// </summary>
    public static async Task Example4_BatchOperations()
    {
        Console.WriteLine("\n=== EXAMPLE 4: Batch Operations ===");

        Console.WriteLine("\n❌ BAD: SaveChanges inside loop (multiple round-trips)");
        using (var context = new BestPracticesDbContext())
        {
            var sw = Stopwatch.StartNew();
            for (int i = 1; i <= 5; i++)
            {
                context.Blogs.Add(new Blog { Name = $"Blog {i}", IsActive = true });
                await context.SaveChangesAsync(); // ← Database round-trip each time!
            }
            sw.Stop();
            Console.WriteLine($"   Made 5 separate database calls ({sw.ElapsedMilliseconds}ms)");
            Console.WriteLine("   Problem: With 1000 entities = 1000 round-trips (10-50 seconds!)");
        }

        Console.WriteLine("\n✅ GOOD: Batch with single SaveChanges");
        using (var context = new BestPracticesDbContext())
        {
            var sw = Stopwatch.StartNew();
            var blogs = new List<Blog>();
            for (int i = 1; i <= 5; i++)
            {
                blogs.Add(new Blog { Name = $"Blog {i}", IsActive = true });
            }
            // Add all entities to context (just preparing, no DB hit yet)
            context.Blogs.AddRange(blogs);

            // Single database round-trip with batched SQL
            await context.SaveChangesAsync(); // ← ONE round-trip for all 5!
            sw.Stop();
            Console.WriteLine($"   Made 1 database call ({sw.ElapsedMilliseconds}ms) - 10-50x faster!");
            Console.WriteLine("   Transaction: All succeed or all fail (ACID)");
        }
    }

    /// <summary>
    /// EXAMPLE 5: Filter Location - Database vs Memory (CRITICAL!)
    /// 
    /// THE PROBLEM:
    ///   Filtering in memory loads ALL records, then filters.
    ///   Devastating with large tables (loads 1M records to find 10!).
    /// 
    /// THE DIFFERENCE:
    ///   ❌ .ToList().Where()   - Load ALL, then filter (memory)
    ///   ✅ .Where().ToList()   - Filter FIRST, then load (database)
    /// 
    /// WHY ORDER MATTERS:
    ///   LINQ is lazy - query builds until you "execute" it:
    ///   • ToList(), ToArray(), Count(), First() trigger execution
    ///   • Where(), Select(), OrderBy() just build the query
    /// 
    /// RULE OF THUMB:
    ///   Always put filtering BEFORE ToList/ToArray.
    ///   Let the database do what it's optimized for!
    /// 
    /// REAL-WORLD DISASTER:
    ///   Products table: 1 million products
    ///   .ToList().Where(p => p.IsActive)
    ///   Result: Load 1M records, crash server, timeout
    ///   
    ///   .Where(p => p.IsActive).ToList()
    ///   Result: SQL WHERE IsActive = 1, load 50k records, fast!
    /// 
    /// WHAT RUNS WHERE:
    ///   • Simple expressions: Run in database (Id == 5, Name.Contains)
    ///   • Complex methods: Run in memory (custom C# methods)
    ///   • Use EF.Functions for database functions (LIKE, DATEADD, etc.)
    /// </summary>
    public static async Task Example5_FilterLocation()
    {
        Console.WriteLine("\n=== EXAMPLE 5: Filter Location ===");

        using var context = new BestPracticesDbContext();
        SeedData(context);

        Console.WriteLine("\n❌ BAD: Filter in memory (.ToList().Where())");
        // Step 1: ToList() executes query - loads ALL blogs
        var allBlogs = await context.Blogs.ToListAsync();  // SQL: SELECT * FROM Blogs

        // Step 2: Where() filters in C# memory
        var activeBlogs1 = allBlogs.Where(b => b.IsActive).ToList();

        Console.WriteLine($"   Loaded ALL {allBlogs.Count} blogs, then filtered to {activeBlogs1.Count}");
        Console.WriteLine("   Problem: With 1M records, loads 1M to find 100k!");
        Console.WriteLine("   Result: Out of memory, timeout, server crash");

        Console.WriteLine("\n✅ GOOD: Filter in database (.Where().ToList())");
        // SQL: SELECT * FROM Blogs WHERE IsActive = 1
        var activeBlogs2 = await context.Blogs
            .Where(b => b.IsActive)  // ← Becomes SQL WHERE clause
            .ToListAsync();

        Console.WriteLine($"   Loaded only {activeBlogs2.Count} active blogs");
        Console.WriteLine("   Benefit: Database filters efficiently with indexes");
        Console.WriteLine("   Result: Fast, scalable, memory-efficient");

        // KEY INSIGHT: Order matters!
        // .Where().ToList() = filter in SQL
        // .ToList().Where() = filter in C#
    }

    /// <summary>
    /// EXAMPLE 6: Batch Queries - Avoid Loops with Smart SQL
    /// 
    /// THE PROBLEM:
    ///   Running queries in loops = N database round-trips.
    ///   Similar to N+1 but different cause.
    /// 
    /// THE SOLUTION:
    ///   Use .Contains() to match multiple values in ONE query.
    ///   SQL: WHERE BlogId IN (1, 2, 3)
    /// 
    /// WHEN YOU'LL NEED THIS:
    ///   • Loading entities by list of IDs
    ///   • Filtering by multiple values
    ///   • "Get orders for these customer IDs"
    ///   • "Load products with these SKUs"
    /// 
    /// PERFORMANCE:
    ///   100 IDs with loop: 100 queries
    ///   100 IDs with Contains: 1 query
    ///   Result: 100x faster!
    /// 
    /// GOTCHA - LARGE LISTS:
    ///   SQL Server has ~2100 parameter limit.
    ///   For 5000+ IDs, batch in chunks:
    ///   foreach (var chunk in ids.Chunk(2000))
    ///       results.AddRange(context.Posts.Where(p => chunk.Contains(p.BlogId)));
    /// 
    /// ALTERNATIVE PATTERNS:
    ///   • .Any(ids.Contains(p.BlogId))  - same as Contains
    ///   • Temporary table for huge lists (10k+ items)
    /// </summary>
    public static async Task Example6_BatchQueries()
    {
        Console.WriteLine("\n=== EXAMPLE 6: Batch Queries ===");

        using var context = new BestPracticesDbContext();
        SeedData(context);

        var ids = new[] { 1, 2, 3 };

        Console.WriteLine("\n❌ BAD: Query in loop (N database round-trips)");
        foreach (var id in ids)
        {
            // Separate query for EACH ID - disaster with large lists
            var blog = await context.Blogs.FindAsync(id);
            Console.WriteLine($"   Found: {blog?.Name}");
        }
        Console.WriteLine($"   Problem: Made {ids.Length} separate queries");
        Console.WriteLine("   With 100 IDs = 100 database round-trips");

        Console.WriteLine("\n✅ GOOD: Single query with Contains (WHERE IN)");
        // SQL: WHERE Id IN (1, 2, 3)
        var blogs = await context.Blogs
            .Where(b => ids.Contains(b.Id))  // ← Single query!
            .ToListAsync();

        foreach (var blog in blogs)
        {
            Console.WriteLine($"   Found: {blog.Name}");
        }
        Console.WriteLine("   Benefit: Made 1 database query");
        Console.WriteLine("   SQL: WHERE Id IN (1, 2, 3)");
        Console.WriteLine("   Performance: 100x faster with large ID lists");
        Console.WriteLine("\n   GOTCHA: SQL Server has ~2100 parameter limit");
        Console.WriteLine("   For 5000+ IDs, batch in chunks of 2000");
    }

    /// <summary>
    /// EXAMPLE 7: DbContext Lifetime - Prevent Memory Leaks
    /// 
    /// THE PROBLEM:
    ///   DbContext accumulates change tracking data.
    ///   Long-lived contexts cause memory leaks.
    /// 
    /// WHY IT MATTERS:
    ///   • DbContext tracks EVERY entity you load
    ///   • Memory usage grows indefinitely
    ///   • Performance degrades over time
    ///   • Eventually: OutOfMemoryException
    /// 
    /// THE RULE:
    ///   DbContext should be SHORT-LIVED.
    ///   Create → Use → Dispose
    /// 
    /// RECOMMENDED PATTERNS:
    ///   
    ///   1. USING STATEMENT (Best for manual creation):
    ///      using (var context = new MyDbContext()) {
    ///          // Use context
    ///      } // Automatically disposed
    ///   
    ///   2. DEPENDENCY INJECTION - Scoped (Best for web apps):
    ///      services.AddDbContext<MyContext>(options => ...);
    ///      
    ///      ASP.NET creates one context per HTTP request.
    ///      Disposed when request completes.
    /// 
    /// WHEN NOT TO DISPOSE MANUALLY:
    ///   If using DI, framework handles disposal.
    ///   Don't call Dispose() on injected contexts!
    /// 
    /// ANTI-PATTERNS:
    ///   ❌ Static DbContext
    ///   ❌ Singleton DbContext
    ///   ❌ Class-level DbContext field (long-lived)
    ///   ❌ Caching DbContext
    /// 
    /// THREAD SAFETY:
    ///   DbContext is NOT thread-safe.
    ///   Each thread needs its own context instance.
    /// </summary>
    public static void Example7_ContextLifetime()
    {
        Console.WriteLine("\n=== EXAMPLE 7: DbContext Lifetime ===");

        Console.WriteLine("\n❌ BAD: Long-lived context");
        Console.WriteLine("   var context = new DbContext(); // Lives forever");
        Console.WriteLine("   // Many operations over time...");
        Console.WriteLine("   Problem:");
        Console.WriteLine("   • Accumulates tracking data");
        Console.WriteLine("   • Memory grows indefinitely");
        Console.WriteLine("   • Performance degrades");
        Console.WriteLine("   • Eventually: OutOfMemoryException");

        Console.WriteLine("\n✅ GOOD: Short-lived context with 'using' statement");
        Console.WriteLine("   using (var context = new DbContext())");
        Console.WriteLine("   {");
        Console.WriteLine("       // Use context");
        Console.WriteLine("   } // ← Automatically disposed");
        Console.WriteLine("   Benefits:");
        Console.WriteLine("   • Fresh context for each operation");
        Console.WriteLine("   • Automatic cleanup");
        Console.WriteLine("   • Change tracker cleared");
        Console.WriteLine("   • Memory released");

        Console.WriteLine("\n   PRODUCTION PATTERN (ASP.NET):");
        Console.WriteLine("   services.AddDbContext<MyContext>(Scoped);");
        Console.WriteLine("   • One context per HTTP request");
        Console.WriteLine("   • Automatic disposal after request");
        Console.WriteLine("   • Framework manages lifecycle");

        Console.WriteLine("\n   ANTI-PATTERNS TO AVOID:");
        Console.WriteLine("   ❌ Static DbContext");
        Console.WriteLine("   ❌ Singleton DbContext");
        Console.WriteLine("   ❌ Long-lived class field");
        Console.WriteLine("   ❌ Sharing across threads");
    }

    /// <summary>
    /// EXAMPLE 8: Efficient Updates - Update Without Loading
    /// 
    /// THE PROBLEM:
    ///   Load entity → Modify → Save = 2 database round-trips
    ///   Inefficient for simple updates.
    /// 
    /// THE SOLUTION:
    ///   Use Attach + Property modification for direct updates.
    ///   Only 1 database roundtrip!
    /// 
    /// WHEN TO USE:
    ///   ✅ You have the ID and new values
    ///   ✅ Simple field updates
    ///   ✅ API PATCH endpoints
    ///   ✅ Bulk status updates
    ///   ❌ Complex business logic (load entity)
    ///   ❌ Need to validate current state
    /// 
    /// HOW IT WORKS:
    ///   1. Create entity with ID
    ///   2. Attach to context (marks as Unchanged)
    ///   3. Modify property (marks as Modified)
    ///   4. SaveChanges (generates UPDATE query)
    /// 
    /// GENERATED SQL:
    ///   UPDATE Blogs SET Name = @p0 WHERE Id = @p1
    ///   Only updates changed properties!
    /// 
    /// MULTIPLE PROPERTIES:
    ///   blog.Name = "New Name";
    ///   blog.Description = "New Desc";
    ///   UPDATE Blogs SET Name = @p0, Description = @p1 WHERE Id = @p2
    /// 
    /// ALTERNATIVE - ExecuteUpdate (EF 7+):
    ///   await context.Blogs
    ///       .Where(b => b.Id == 1)
    ///       .ExecuteUpdateAsync(s => s.SetProperty(b => b.Name, "New Name"));
    ///   
    ///   Even faster - direct SQL without change tracking!
    /// 
    /// GOTCHA:
    ///   Only works if you know the ID.
    ///   For complex updates with validations, load the entity.
    /// </summary>
    public static async Task Example8_EfficientUpdates()
    {
        Console.WriteLine("\n=== EXAMPLE 8: Efficient Updates ===");

        using var context = new BestPracticesDbContext();
        SeedData(context);

        Console.WriteLine("\n❌ BAD: Load entity just to update one field (2 round-trips)");
        // Round-trip #1: SELECT * FROM Blogs WHERE Id = 1
        var blog1 = await context.Blogs.FindAsync(1);
        if (blog1 != null)
        {
            blog1.Name = "Updated Name";
            // Round-trip #2: UPDATE Blogs SET Name = ... WHERE Id = 1
            await context.SaveChangesAsync();
        }
        Console.WriteLine("   Problem: 2 database round-trips");
        Console.WriteLine("   Round-trip #1: SELECT * FROM Blogs WHERE Id = 1");
        Console.WriteLine("   Round-trip #2: UPDATE Blogs SET Name = ... WHERE Id = 1");

        Console.WriteLine("\n✅ GOOD: Attach and mark property as modified (1 round-trip)");
        // Create entity with ID and new value
        var blog2 = new Blog { Id = 1, Name = "Efficiently Updated" };

        // Attach to context (marks as Unchanged)
        context.Attach(blog2);

        // Mark specific property as modified
        context.Entry(blog2).Property(b => b.Name).IsModified = true;

        // Generates: UPDATE Blogs SET Name = @p0 WHERE Id = @p1
        await context.SaveChangesAsync();

        Console.WriteLine("   Benefit: Only 1 database round-trip");
        Console.WriteLine("   SQL: UPDATE Blogs SET Name = @p0 WHERE Id = @p1");
        Console.WriteLine("   No SELECT needed!");

        Console.WriteLine("\n   WHEN TO USE:");
        Console.WriteLine("   ✅ You have the ID and new values");
        Console.WriteLine("   ✅ Simple field updates");
        Console.WriteLine("   ✅ API PATCH endpoints");
        Console.WriteLine("   ❌ Complex validations");
        Console.WriteLine("   ❌ Need to check current state");
    }

    /// <summary>
    /// Run all examples sequentially.
    /// 
    /// LEARNING PATH:
    ///   Examples are ordered by impact - the first few have
    ///   the biggest effect on real-world performance.
    /// 
    /// USAGE:
    ///   Call this method to see all examples run in sequence.
    ///   Or call individual examples: await Example1_N1QueryProblem();
    /// </summary>
    public static async Task RunAllExamples()
    {
        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine("ENTITY FRAMEWORK CORE - BEST PRACTICES VS ANTI-PATTERNS");
        Console.WriteLine(new string('=', 70));

        await Example1_N1QueryProblem();
        await Example2_TrackingOverhead();
        await Example3_Projection();
        await Example4_BatchOperations();
        await Example5_FilterLocation();
        await Example6_BatchQueries();
        Example7_ContextLifetime();
        await Example8_EfficientUpdates();

        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine("KEY TAKEAWAYS - YOUR EF CORE PERFORMANCE CHECKLIST:");
        Console.WriteLine(new string('=', 70));
        Console.WriteLine("\n✅ ALWAYS:");
        Console.WriteLine("  1. Use .Include() to avoid N+1 queries (10-100x faster)");
        Console.WriteLine("  2. Use .AsNoTracking() for read-only queries (30-40% faster)");
        Console.WriteLine("  3. Use .Select() projection to load only needed columns (75% less data)");
        Console.WriteLine("  4. Batch operations with single SaveChanges (10-50x faster)");
        Console.WriteLine("  5. Filter with .Where() BEFORE .ToList() (database, not memory)");
        Console.WriteLine("  6. Use .Contains() for multiple IDs - avoid query loops (100x faster)");
        Console.WriteLine("  7. Keep DbContext short-lived with 'using' statements");
        Console.WriteLine("  8. Use Attach for updates when you have the ID");

        Console.WriteLine("\n❌ NEVER:");
        Console.WriteLine("  1. Access navigation properties without .Include()");
        Console.WriteLine("  2. Use default tracking for read-only queries");
        Console.WriteLine("  3. Call .ToList() then filter in memory");
        Console.WriteLine("  4. Call SaveChanges() inside loops");
        Console.WriteLine("  5. Run queries inside loops");
        Console.WriteLine("  6. Use long-lived DbContext (static, singleton, class field)");
        Console.WriteLine("  7. Load full entity just to update one field");

        Console.WriteLine("\n🔍 TESTING:");
        Console.WriteLine("  • Always test with realistic data (100+ records)");
        Console.WriteLine("  • Enable SQL logging: options.LogTo(Console.WriteLine)");
        Console.WriteLine("  • Use profiling tools (MiniProfiler, Application Insights)");
        Console.WriteLine("  • Watch for N+1 patterns in logs");

        Console.WriteLine("\n📚 LEARN MORE:");
        Console.WriteLine("  • Revision Notes - Section 8.6.5 (Best Practices)");
        Console.WriteLine("  • README.md - Entity Framework section");
        Console.WriteLine("  • Microsoft Docs: EF Core Performance");
        Console.WriteLine(new string('=', 70) + "\n");
    }
}
