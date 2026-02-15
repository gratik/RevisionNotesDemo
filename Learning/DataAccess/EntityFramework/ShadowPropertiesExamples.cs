// ==============================================================================
// ENTITY FRAMEWORK: SHADOW PROPERTIES & TABLE SPLITTING
// ==============================================================================
// PURPOSE: Keep domain models clean while adding infrastructure concerns
// WHY: Separate domain from persistence, add audit without polluting entities
// USE WHEN: Need tracking, audit, or optimization without changing domain
// ==============================================================================
// WHAT ARE SHADOW PROPERTIES?
// Shadow Properties are database columns that exist in the database but are NOT
// C# properties on your entity classes. They're defined entirely in EF Core's
// configuration (OnModelCreating). Your domain model stays clean while the
// database includes audit/tracking fields.
//
// THE PROBLEM: AUDIT FIELD POLLUTION
// Most applications need audit fields (CreatedAt, CreatedBy, ModifiedAt, ModifiedBy)
// for compliance, debugging, and data lineage. Adding these to every entity:
// • Clutters domain model with infrastructure concerns (violates SRP)
// • Mixes business logic with persistence concerns
// • Makes entities harder to understand and test
// • Pollutes domain layer with framework dependencies
//
// THE SOLUTION: SHADOW PROPERTIES
// Define audit columns in EF configuration, not in C# classes:
// • Domain entities remain pure business logic
// • Database still has all required audit columns
// • EF automatically populates values via SaveChanges override
// • Clean separation of domain and persistence
//
// WHY IT MATTERS:
// • CLEAN ARCHITECTURE: Domain layer not polluted with infrastructure
// • SIMPLER ENTITIES: Only business properties visible in code
// • AUTOMATIC AUDITING: Override SaveChanges once, applies to all entities
// • TESTABILITY: Domain tests don't need to set audit fields
// • COMPLIANCE: Audit trail without domain complexity
//
// WHAT IS TABLE SPLITTING?
// Table Splitting maps multiple entities to the SAME database table. Useful when
// an entity has rarely-used large columns (images, descriptions).
//
// Example: Product table with Image (large binary). Create two entities:
// • Product (Id, Name, Price) - loaded frequently
// • ProductDetails (Id, Image, LongDescription) - loaded on demand
// Both map to same "Products" table, but you control what loads when.
//
// WHY TABLE SPLITTING MATTERS:
// • PERFORMANCE: List queries don't load large columns (faster, less memory)
// • SELECTIVE LOADING: Load details only when viewing product detail page
// • SINGLE TABLE: Simpler database schema (no joins needed)
// • TRANSPARENT: Application code doesn't know it's the same table
//
// WHEN TO USE:
// ✅ SHADOW PROPERTIES: Need audit fields on all entities (compliance)
// ✅ SHADOW PROPERTIES: Want clean domain models (DDD, Clean Architecture)
// ✅ TABLE SPLITTING: Entity has rarely-used large columns
// ✅ TABLE SPLITTING: Performance issue from loading unnecessary data
//
// REAL-WORLD EXAMPLE:
// E-commerce: Product entity needs CreatedAt, CreatedBy for legal compliance.
// Instead of polluting Product class, use shadow properties. Override SaveChanges
// to auto-populate. Result: Product class is pure business logic (Name, Price, Stock),
// but database has audit columns and they're automatically populated.
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.EntityFramework;

public static class ShadowPropertiesExamples
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== EF CORE: SHADOW PROPERTIES & TABLE SPLITTING ===\n");
        
        Example1_AuditFieldsPollution();
        Example2_ShadowProperties();
        Example3_TableSplitting();
        Example4_AutomaticAudit();
        
        Console.WriteLine("\n💡 Key Takeaways:");
        Console.WriteLine("   ✅ Shadow properties keep domain clean");
        Console.WriteLine("   ✅ Audit fields added without polluting entities");
        Console.WriteLine("   ✅ Table splitting optimizes performance");
        Console.WriteLine("   ✅ Automatic tracking via SaveChanges override");
        Console.WriteLine("   ✅ Separation of domain and infrastructure");
    }
    
    private static void Example1_AuditFieldsPollution()
    {
        Console.WriteLine("=== EXAMPLE 1: Audit Fields Pollution ===\n");
        
        Console.WriteLine("❌ BAD: Audit fields pollute domain model\n");
        // public class Product {
        //     public int Id { get; set; }
        //     public string Name { get; set; }
        //     public decimal Price { get; set; }
        //     
        //     // ❌ Infrastructure concerns in domain
        //     public DateTime CreatedAt { get; set; }
        //     public string CreatedBy { get; set; }
        //     public DateTime? ModifiedAt { get; set; }
        //     public string ModifiedBy { get; set; }
        // }
        
        Console.WriteLine("\n💥 Problems:");
        Console.WriteLine("   • Domain model cluttered");
        Console.WriteLine("   • Infrastructure mixed with business logic");
        Console.WriteLine("   • Hard to maintain");
        Console.WriteLine("   • Violates SRP");
    }
    
    private static void Example2_ShadowProperties()
    {
        Console.WriteLine("\n=== EXAMPLE 2: Shadow Properties (Clean Domain) ===\n");
        
        Console.WriteLine("✅ GOOD: Clean domain, audit in DB only\n");
        // Clean domain model
        // public class Product {
        //     public int Id { get; set; }
        //     public string Name { get; set; }
        //     public decimal Price { get; set; }
        //     // No audit fields!
        // }
        //
        // DbContext configuration
        // protected override void OnModelCreating(ModelBuilder modelBuilder) {
        //     modelBuilder.Entity<Product>(entity => {
        //         // Add shadow properties (not in C# class)
        //         entity.Property<DateTime>("CreatedAt").HasDefaultValueSql("GETUTCDATE()");
        //         entity.Property<string>("CreatedBy").HasMaxLength(100);
        //         entity.Property<DateTime?>("ModifiedAt");
        //         entity.Property<string>("ModifiedBy").HasMaxLength(100);
        //     });
        // }
        //
        // Query shadow properties
        // var recentProducts = await _context.Products
        //     .Where(p => EF.Property<DateTime>(p, "CreatedAt") > DateTime.UtcNow.AddDays(-7))
        //     .ToListAsync();
        
        Console.WriteLine("\n📊 Benefits:");
        Console.WriteLine("   • Clean domain model");
        Console.WriteLine("   • DB still has audit columns");
        Console.WriteLine("   • Automatic tracking possible");
        Console.WriteLine("   • Easy to apply to all entities");
    }
    
    private static void Example3_TableSplitting()
    {
        Console.WriteLine("\n=== EXAMPLE 3: Table Splitting (Performance) ===\n");
        
        Console.WriteLine("✅ TABLE SPLITTING: One table, multiple entities\n");
        // Problem: Product has rarely-used large columns
        // Solution: Split into two entities
        //
        // Main entity (frequently loaded)
        // public class Product {
        //     public int Id { get; set; }
        //     public string Name { get; set; }
        //     public decimal Price { get; set; }
        //     public ProductDetails Details { get; set; }
        // }
        //
        // Details entity (loaded on demand)
        // public class ProductDetails {
        //     public int ProductId { get; set; }
        //     public string LongDescription { get; set; }  // Large text
        //     public byte[] Image { get; set; }           // Large binary
        //     public Product Product { get; set; }
        // }
        //
        // Configuration (both map to same table)
        // modelBuilder.Entity<Product>(entity => {
        //     entity.ToTable("Products");
        //     entity.HasOne(p => p.Details)
        //         .WithOne(d => d.Product)
        //         .HasForeignKey<ProductDetails>(d => d.ProductId);
        // });
        // modelBuilder.Entity<ProductDetails>(entity => {
        //     entity.ToTable("Products");  // Same table!
        // });
        
        Console.WriteLine("\n📊 Benefits:");
        Console.WriteLine("   • Faster list queries (small entity)");
        Console.WriteLine("   • Load details only when needed");
        Console.WriteLine("   • Single table in database");
        Console.WriteLine("   • Transparent to application");
    }
    
    private static void Example4_AutomaticAudit()
    {
        Console.WriteLine("\n=== EXAMPLE 4: Automatic Audit Trail ===\n");
        
        Console.WriteLine("✅ Override SaveChanges for automatic audit\n");
        // public override async Task<int> SaveChangesAsync(CancellationToken ct = default) {
        //     var entries = ChangeTracker.Entries()
        //         .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
        //     
        //     var currentUser = _userProvider.GetCurrentUserId();
        //     var now = DateTime.UtcNow;
        //     
        //     foreach (var entry in entries) {
        //         if (entry.State == EntityState.Added) {
        //             entry.Property("CreatedAt").CurrentValue = now;
        //             entry.Property("CreatedBy").CurrentValue = currentUser;
        //         }
        //         
        //         if (entry.State == EntityState.Modified) {
        //             entry.Property("ModifiedAt").CurrentValue = now;
        //             entry.Property("ModifiedBy").CurrentValue = currentUser;
        //         }
        //     }
        //     
        //     return await base.SaveChangesAsync(ct);
        // }
        
        Console.WriteLine("\n📊 Flow:");
        Console.WriteLine("   1. SaveChanges called");
        Console.WriteLine("   2. Inspect ChangeTracker entries");
        Console.WriteLine("   3. Set shadow property values");
        Console.WriteLine("   4. Call base.SaveChanges");
        Console.WriteLine("   5. Audit fields automatically populated");
        
        Console.WriteLine("\n💡 Advanced:");
        Console.WriteLine("   • Implement IAuditable interface");
        Console.WriteLine("   • Apply to specific entities only");
        Console.WriteLine("   • Combine with multi-tenancy");
        Console.WriteLine("   • Log changes to separate audit table");
    }
}
