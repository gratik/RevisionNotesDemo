// ==============================================================================
// ENTITY FRAMEWORK CORE - MIGRATIONS IN DEPTH
// Reference: Revision Notes - Entity Framework Core (Section 8.6.3)
// ==============================================================================
// PURPOSE: Master EF Core migrations - the code-first approach to evolving your
//          database schema over time. This is how professional teams manage
//          database changes across environments (dev, staging, production).
//
// WHAT YOU'LL LEARN:
//   • Creating migrations (dotnet ef migrations add)
//   • Applying migrations (dotnet ef database update)
//   • Rolling back migrations (going back to previous version)
//   • Generating SQL scripts for production deployment
//   • Seeding data with HasData
//   • Production deployment strategies
//
// WHY MIGRATIONS ARE IMPORTANT:
//   - Track database schema changes in source control (Git)
//   - Apply changes consistently across all environments
//   - Rollback capability (undo migrations if needed)
//   - Audit trail of all schema changes
//   - Team collaboration (everyone gets same schema changes)
//
// CODE-FIRST WORKFLOW:
//   1. Change your entity classes (add property, new entity, etc.)
//   2. Create migration: dotnet ef migrations add AddNewProperty
//   3. Review generated migration code (Up/Down methods)
//   4. Apply migration: dotnet ef database update
//   5. Commit migration files to Git
//
// PRODUCTION BEST PRACTICES:
//   ✅ Generate SQL scripts (review manually before running)
//   ✅ Test migrations on staging environment first
//   ✅ Have rollback plan (Down migrations or database backup)
//   ❌ Don't run context.Database.Migrate() in production (risky)
//   ❌ Don't modify existing migrations (create new ones)
//
// KEY CONCEPTS: Creating migrations, applying, rollback, seeding data, production deployment
// ==============================================================================

using Microsoft.EntityFrameworkCore;

namespace RevisionNotesDemo.DataAccess.EntityFramework;

public class MigrationProduct
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class MigrationsDbContext : DbContext
{
    public DbSet<MigrationProduct> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("MigrationsDb");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed data using HasData
        modelBuilder.Entity<MigrationProduct>().HasData(
            new MigrationProduct { Id = 1, Name = "Laptop", Price = 999.99m },
            new MigrationProduct { Id = 2, Name = "Mouse", Price = 29.99m }
        );

        // Configure table
        modelBuilder.Entity<MigrationProduct>()
            .ToTable("Products")
            .HasKey(p => p.Id);

        // Configure columns
        modelBuilder.Entity<MigrationProduct>()
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        // Create index
        modelBuilder.Entity<MigrationProduct>()
            .HasIndex(p => p.Name);
    }
}

public class MigrationsInDepthExamples
{
    /// <summary>
    /// EXAMPLE 1: Common Migration Commands - Your Essential Toolkit
    /// 
    /// These are CLI commands you'll use daily. Keep this as a reference!
    /// 
    /// PREREQUISITES:
    ///   - Install EF Core tools: dotnet tool install --global dotnet-ef
    ///   - Or update: dotnet tool update --global dotnet-ef
    /// 
    /// WORKFLOW:
    ///   1. Modify entity classes (add/remove properties, new entities)
    ///   2. Create migration to capture changes
    ///   3. Review generated migration code
    ///   4. Apply migration to update database
    ///   5. Commit migration files to Git
    /// 
    /// MIGRATION FILES:
    ///   - 20240213120000_InitialCreate.cs - The migration class (Up/Down)
    ///   - 20240213120000_InitialCreate.Designer.cs - Model snapshot
    ///   - YourDbContextModelSnapshot.cs - Current model state
    /// 
    /// TROUBLESHOOTING:
    ///   - "No DbContext found" → Specify: --context YourDbContext
    ///   - "Build failed" → Fix compilation errors first
    ///   - "No migrations to apply" → You're up to date!
    /// </summary>
    public static void MigrationCommands()
    {
        Console.WriteLine("\n=== MIGRATION COMMANDS ===\n");

        Console.WriteLine("CREATE MIGRATION:");
        Console.WriteLine("  dotnet ef migrations add InitialCreate");
        Console.WriteLine("  dotnet ef migrations add AddCustomerEmail");
        Console.WriteLine("  // Creates migration files in Migrations folder");
        Console.WriteLine("  // Naming: Use descriptive names (not 'Migration1')");

        Console.WriteLine("\nAPPLY MIGRATIONS:");
        Console.WriteLine("  dotnet ef database update");
        Console.WriteLine("  // Applies all pending migrations to database");
        Console.WriteLine("  dotnet ef database update SpecificMigration");
        Console.WriteLine("  // Apply up to specific migration (partial update)");

        Console.WriteLine("\nGENERATE SQL SCRIPT (PRODUCTION):");
        Console.WriteLine("  dotnet ef migrations script");
        Console.WriteLine("  // Generates SQL for all migrations");
        Console.WriteLine("  dotnet ef migrations script --idempotent");
        Console.WriteLine("  // Safe to run multiple times (checks if already applied)");
        Console.WriteLine("  dotnet ef migrations script FromMigration ToMigration");
        Console.WriteLine("  // Generate SQL for specific range");

        Console.WriteLine("\nROLLBACK:");
        Console.WriteLine("  dotnet ef database update PreviousMigration");
        Console.WriteLine("  // Rollback to specific migration (runs Down methods)");
        Console.WriteLine("  dotnet ef database update 0");
        Console.WriteLine("  // Rollback ALL migrations (empty database)");

        Console.WriteLine("\nREMOVE MIGRATION:");
        Console.WriteLine("  dotnet ef migrations remove");
        Console.WriteLine("  // Remove last migration (ONLY if not applied yet!)");
        Console.WriteLine("  // CANNOT remove if already applied to database");

        Console.WriteLine("\nMIGRATION BUNDLE (EF Core 6+):");
        Console.WriteLine("  dotnet ef migrations bundle");
        Console.WriteLine("  // Creates self-contained executable");
        Console.WriteLine("  ./efbundle --connection \"Server=...;\"");
        Console.WriteLine("  // Run bundle to apply migrations");
    }

    /// <summary>
    /// EXAMPLE 2: Applying Migrations Programmatically - For Specific Scenarios Only
    /// 
    /// WHEN TO USE:
    ///   ✅ Development: Auto-apply migrations on app startup
    ///   ✅ Automated tests: Ensure test database is up-to-date
    ///   ✅ Single-tenant apps: Each tenant has their own database
    ///   ❌ Production (multi-tenant): Too risky! Use SQL scripts instead
    /// 
    /// TWO METHODS:
    ///   1. MigrateAsync() - Apply pending migrations (safe, incremental)
    ///   2. EnsureCreatedAsync() - Create database if not exists (no migrations!)
    /// 
    /// IMPORTANT DIFFERENCE:
    ///   - MigrateAsync: Uses migrations, tracks history, can rollback
    ///   - EnsureCreatedAsync: Creates schema directly, NO migration history!
    ///     ⚠️ Once you use EnsureCreatedAsync, you can't use migrations!
    /// 
    /// PRODUCTION CONCERNS:
    ///   - Multiple instances running MigrateAsync simultaneously = conflict
    ///   - Long-running migrations can cause startup timeouts
    ///   - Migration failures in production = downtime
    /// 
    /// BEST PRACTICE:
    ///   Run migrations as separate deployment step, not in application code.
    /// </summary>
    public static async Task ApplyMigrationsInCode()
    {
        using var context = new MigrationsDbContext();

        // Apply any pending migrations to the database
        // This is safe and incremental (only applies what's needed)
        await context.Database.MigrateAsync();

        // Alternative for simple scenarios (testing, prototyping)
        // WARNING: Creates schema directly, ignores migrations!
        // await context.Database.EnsureCreatedAsync();

        Console.WriteLine("Migrations applied programmatically");

        // TIP: Check if migrations are pending:
        // var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        // if (pendingMigrations.Any()) {
        //     await context.Database.MigrateAsync();
        // }
    }

    /// <summary>
    /// EXAMPLE 3: Seeding Data
    /// </summary>
    public static async Task SeedingDataExample()
    {
        using var context = new MigrationsDbContext();
        await context.Database.EnsureCreatedAsync();

        // Data seeded via HasData in OnModelCreating
        var products = await context.Products.ToListAsync();

        Console.WriteLine($"Seeded {products.Count} products:");
        foreach (var product in products)
        {
            Console.WriteLine($"  - {product.Name}: ${product.Price}");
        }
    }

    /// <summary>
    /// EXAMPLE 4: Migration File Structure (Conceptual)
    /// </summary>
    public static void MigrationFileStructure()
    {
        Console.WriteLine("\n=== MIGRATION FILE STRUCTURE ===\n");

        Console.WriteLine("// 20240213120000_InitialCreate.cs");
        Console.WriteLine("public partial class InitialCreate : Migration");
        Console.WriteLine("{");
        Console.WriteLine("    protected override void Up(MigrationBuilder migrationBuilder)");
        Console.WriteLine("    {");
        Console.WriteLine("        migrationBuilder.CreateTable(");
        Console.WriteLine("            name: \"Products\",");
        Console.WriteLine("            columns: table => new {");
        Console.WriteLine("                Id = table.Column<int>(nullable: false),");
        Console.WriteLine("                Name = table.Column<string>(maxLength: 200, nullable: false)");
        Console.WriteLine("            });");
        Console.WriteLine("    }");
        Console.WriteLine("    ");
        Console.WriteLine("    protected override void Down(MigrationBuilder migrationBuilder)");
        Console.WriteLine("    {");
        Console.WriteLine("        migrationBuilder.DropTable(name: \"Products\");");
        Console.WriteLine("    }");
        Console.WriteLine("}");
    }

    /// <summary>
    /// EXAMPLE 5: Production Deployment Strategies
    /// </summary>
    public static void ProductionStrategies()
    {
        Console.WriteLine("\n=== PRODUCTION DEPLOYMENT STRATEGIES ===\n");

        Console.WriteLine("1. SQL SCRIPTS (Recommended):");
        Console.WriteLine("   - Generate script: dotnet ef migrations script");
        Console.WriteLine("   - Review script manually");
        Console.WriteLine("   - Execute via your deployment pipeline");
        Console.WriteLine("   - Full control, audit trail");

        Console.WriteLine("\n2. MIGRATION BUNDLES (EF Core 6+):");
        Console.WriteLine("   - Self-contained executable");
        Console.WriteLine("   - dotnet ef migrations bundle");
        Console.WriteLine("   - Deploy and run bundle");

        Console.WriteLine("\n3. AUTOMATIC (Use with caution):");
        Console.WriteLine("   - context.Database.MigrateAsync() in startup");
        Console.WriteLine("   - Simple but risky for production");
    }

    public static async Task RunAllExamples()
    {
        Console.WriteLine("\n=== ENTITY FRAMEWORK - MIGRATIONS IN DEPTH ===\n");
        MigrationCommands();
        await ApplyMigrationsInCode();
        await SeedingDataExample();
        MigrationFileStructure();
        ProductionStrategies();
        Console.WriteLine("\nMigrations examples completed!\n");
    }
}
