// ==============================================================================
// ENTITY FRAMEWORK CORE - CHANGE TRACKING EXAMPLES
// Reference: Revision Notes - Entity Framework Core (Section 8.6.1)
// ==============================================================================
//
// WHAT IS CHANGE TRACKING?
// ------------------------
// EF Core tracks entity state to decide which SQL to generate on SaveChanges.
// Entities are marked as Detached, Added, Modified, Deleted, or Unchanged.
//
// WHY IT MATTERS
// --------------
// - Correctness: EF updates the right rows and columns
// - Performance: tracking adds overhead for read-only queries
// - Disconnected scenarios: APIs must set state explicitly
// - Concurrency: avoids overwriting unintended fields
//
// WHEN TO USE
// -----------
// - YES: Normal CRUD with DbContext per request
// - YES: Disconnected updates (API receives DTOs)
// - YES: Read-only queries with AsNoTracking
//
// WHEN NOT TO USE
// ---------------
// - NO: Do not use tracking for large, read-only lists
// - NO: Do not keep long-lived contexts with many tracked entities
//
// REAL-WORLD EXAMPLE
// ------------------
// Product update API:
// - Client sends ProductDto with Id and Price
// - Server attaches entity and marks only Price as modified
// - Avoids overwriting Name/Stock fields not sent by client
// ==============================================================================

using Microsoft.EntityFrameworkCore;

namespace RevisionNotesDemo.DataAccess.EntityFramework;

/// <summary>
/// Sample entity representing a product in inventory.
/// Used to demonstrate change tracking behavior.
/// 
/// NOTE: In real applications, you'd have more properties like:
///       - CreatedDate, ModifiedDate (for auditing)
///       - IsDeleted (for soft deletes)
///       - Version/RowVersion (for concurrency)
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

/// <summary>
/// DbContext for change tracking examples.
/// 
/// PRODUCTION NOTE: In real apps, configure DbContext via dependency injection
///                  in Startup.cs/Program.cs, not in OnConfiguring.
/// 
/// EXAMPLE:
///   services.AddDbContext<YourContext>(options =>
///       options.UseSqlServer(connectionString));
/// </summary>
public class ChangeTrackingDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // InMemoryDatabase is great for examples and unit tests
        // For production, use UseSqlServer, UseNpgsql, etc.
        optionsBuilder.UseInMemoryDatabase("ChangeTrackingDb");
    }
}

public class ChangeTrackingExamples
{
    /// <summary>
    /// EXAMPLE 1: Understanding Entity States - The Foundation of Change Tracking
    /// 
    /// THE 5 ENTITY STATES:
    ///   1. Detached  - Not tracked by context (new entities, or explicitly detached)
    ///   2. Added     - Will be INSERTed when SaveChanges is called
    ///   3. Unchanged - Retrieved from DB, no changes detected
    ///   4. Modified  - Retrieved from DB, changes detected (generates UPDATE)
    ///   5. Deleted   - Will be DELETEd when SaveChanges is called
    /// 
    /// WHY THIS MATTERS:
    ///   - EF uses states to know what SQL to generate (INSERT/UPDATE/DELETE)
    ///   - Understanding states is crucial for disconnected scenarios (APIs)
    ///   - Performance: tracking has overhead (~30-40% for read-only queries)
    /// 
    /// REAL-WORLD: In web APIs, entities come from JSON (Detached state).
    ///             You must manually set state to Modified/Added/Deleted.
    /// </summary>
    public static async Task DemonstrateEntityStates()
    {
        using var context = new ChangeTrackingDbContext();

        // Create new entity (not yet tracked)
        var product = new Product { Name = "Laptop", Price = 999.99m, Stock = 10 };

        // State: Detached (context doesn't know about this entity)
        Console.WriteLine($"Initial State: {context.Entry(product).State}");

        // Add to context (tells EF to track and INSERT on next SaveChanges)
        context.Products.Add(product);
        // State: Added (EF will generate: INSERT INTO Products...)
        Console.WriteLine($"After Add: {context.Entry(product).State}");

        // Save to database (executes INSERT)
        await context.SaveChangesAsync();
        // State: Unchanged (entity now matches database, no pending changes)
        Console.WriteLine($"After SaveChanges: {context.Entry(product).State}");

        // Modify a property (EF automatically detects this!)
        product.Price = 899.99m;
        // State: Modified (EF will generate: UPDATE Products SET Price = 899.99...)
        Console.WriteLine($"After modification: {context.Entry(product).State}");

        // Save changes (executes UPDATE)
        await context.SaveChangesAsync();
        // State: Back to Unchanged (all changes saved)
        Console.WriteLine($"After SaveChanges: {context.Entry(product).State}");

        // TIP: Use context.ChangeTracker.Entries() to see all tracked entities
    }

    /// <summary>
    /// EXAMPLE 2: AsNoTracking - The #1 Performance Optimization for Read-Only Queries
    /// 
    /// PERFORMANCE IMPACT: 30-40% faster, less memory, no snapshot overhead
    /// 
    /// WHEN TO USE:
    ///   ✅ Reports, dashboards, analytics
    ///   ✅ List views, search results
    ///   ✅ API GET endpoints that only read data
    ///   ✅ Any query where you won't update the entities
    /// 
    /// WHEN NOT TO USE:
    ///   ❌ When you plan to update entities later
    ///   ❌ When you need change detection
    /// 
    /// HOW IT WORKS:
    ///   - Normal tracking: EF creates a snapshot of each entity to detect changes
    ///   - AsNoTracking: Skips snapshot creation = faster, less memory
    /// 
    /// GOTCHA: Entities returned by AsNoTracking are not tracked!
    ///         If you modify them and call SaveChanges, nothing happens.
    /// 
    /// TIP: For read-heavy apps, set NoTracking as default:
    ///      context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    /// </summary>
    public static async Task AsNoTrackingExample()
    {
        using var context = new ChangeTrackingDbContext();

        // Seed some test data
        context.Products.Add(new Product { Name = "Mouse", Price = 29.99m, Stock = 50 });
        await context.SaveChangesAsync();

        // Read-only query with AsNoTracking
        // EF Core will NOT track these entities (no change detection overhead)
        var products = await context.Products
            .AsNoTracking()  // ← This makes it 30-40% faster!
            .Where(p => p.Stock > 0)
            .ToListAsync();

        Console.WriteLine($"AsNoTracking: {products.Count} products (30-40% faster)");

        // Verify: These entities are NOT tracked
        // context.Entry(products[0]).State would be Detached (if we checked)
    }

    /// <summary>
    /// EXAMPLE 3: Manual State Management - Essential for Web APIs and Disconnected Scenarios
    /// 
    /// THE PROBLEM:
    ///   In web APIs, entities come from JSON (client sends updated entity).
    ///   These entities are Detached - the context doesn't know if they're new or modified.
    /// 
    /// THE SOLUTION:
    ///   Manually tell EF the entity state using context.Entry(entity).State
    /// 
    /// COMMON SCENARIOS:
    ///   1. Web API receives updated entity → Set state to Modified
    ///   2. Web API receives new entity → Set state to Added
    ///   3. Web API receives delete request → Set state to Deleted
    /// 
    /// TWO APPROACHES:
    ///   A) Update ALL columns: context.Entry(entity).State = EntityState.Modified
    ///   B) Update SPECIFIC columns: Attach + mark properties as modified
    /// 
    /// WHY APPROACH B IS BETTER:
    ///   - Only updates changed columns (concurrent updates won't overwrite other fields)
    ///   - More efficient SQL: UPDATE Products SET Price = @p0 WHERE Id = @p1
    ///   - Prevents accidentally overwriting fields not sent by client
    /// 
    /// REAL-WORLD PATTERN:
    ///   [HttpPut("products/{id}")]
    ///   public async Task<IActionResult> Update(int id, ProductDto dto) {
    ///       var product = new Product { Id = id, Price = dto.Price };
    ///       context.Attach(product);
    ///       context.Entry(product).Property(p => p.Price).IsModified = true;
    ///       await context.SaveChangesAsync();
    ///   }
    /// </summary>
    public static async Task ManualStateManagement()
    {
        // Simulate receiving entity from API client (e.g., from JSON body)
        // In reality: var product = JsonSerializer.Deserialize<Product>(json);
        var product = new Product { Id = 1, Name = "Updated Product", Price = 199.99m, Stock = 20 };

        using var context = new ChangeTrackingDbContext();

        // APPROACH A: Mark entire entity as modified (updates ALL columns)
        // SQL: UPDATE Products SET Name = @p0, Price = @p1, Stock = @p2 WHERE Id = @p3
        context.Entry(product).State = EntityState.Modified;

        // APPROACH B: Attach and mark SPECIFIC properties as modified (BETTER)
        // This is safer - only updates the Price column
        // SQL: UPDATE Products SET Price = @p0 WHERE Id = @p1
        context.Attach(product);  // Tell EF to track this entity
        context.Entry(product).Property(p => p.Price).IsModified = true;  // Only Price changed

        Console.WriteLine("Manual state management completed");

        // NOTE: We're not calling SaveChangesAsync here (this is just a demo)
        // In real code: await context.SaveChangesAsync();
    }

    /// <summary>
    /// EXAMPLE 4: Disable Tracking at Context Level - Great for Read-Heavy Applications
    /// 
    /// WHEN TO USE THIS PATTERN:
    ///   - Read-heavy applications (reports, analytics, dashboards)
    ///   - APIs that mostly serve GET requests
    ///   - When 90%+ of queries are read-only
    /// 
    /// BENEFIT:
    ///   - Don't need to remember to add .AsNoTracking() to every query
    ///   - Cleaner code, better default for read scenarios
    ///   - Can still opt-in to tracking for specific queries:
    ///     context.Products.AsTracking().Where(...)
    /// 
    /// CONFIGURATION OPTIONS:
    ///   - QueryTrackingBehavior.TrackAll (default)
    ///   - QueryTrackingBehavior.NoTracking (all queries use AsNoTracking)
    ///   - QueryTrackingBehavior.NoTrackingWithIdentityResolution (EF Core 5+)
    /// 
    /// REAL-WORLD EXAMPLE:
    ///   In a reporting API, set this in your DbContext constructor:
    ///   public ReportsDbContext(DbContextOptions options) : base(options) {
    ///       ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    ///   }
    /// </summary>
    public static async Task DisableTrackingContextLevel()
    {
        using var context = new ChangeTrackingDbContext();

        // Set default tracking behavior for ALL queries on this context
        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        // Now all queries automatically use AsNoTracking (no need to specify it)
        var products = await context.Products.ToListAsync();
        Console.WriteLine("All queries use NoTracking by default");

        // If you need tracking for a specific query, opt-in:
        // var trackedProduct = await context.Products.AsTracking().FirstOrDefaultAsync();

        // TIP: In ASP.NET Core, configure this in AddDbContext:
        // services.AddDbContext<YourContext>(options => {
        //     options.UseSqlServer(conn);
        //     options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        // });
    }

    public static async Task RunAllExamples()
    {
        Console.WriteLine("\n=== ENTITY FRAMEWORK - CHANGE TRACKING EXAMPLES ===\n");
        await DemonstrateEntityStates();
        await AsNoTrackingExample();
        await ManualStateManagement();
        await DisableTrackingContextLevel();
        Console.WriteLine("\nChange Tracking examples completed!\n");
    }
}
