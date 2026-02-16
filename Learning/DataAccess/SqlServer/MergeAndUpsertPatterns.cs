// ==============================================================================
// MERGE AND UPSERT PATTERNS
// ==============================================================================
// WHAT IS THIS?
// Practical guidance for SQL Server MERGE usage and safer upsert alternatives.
//
// WHY IT MATTERS
// - Upsert logic is common and can introduce race conditions if done incorrectly.
// - MERGE can be concise, but needs strict guardrails and testing.
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class MergeAndUpsertPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n--- MERGE AND UPSERT PATTERNS ---\n");
        ShowMergeBasics();
        ShowSaferAlternatives();
        ShowWhenToUseWhich();
    }

    private static void ShowMergeBasics()
    {
        Console.WriteLine("MERGE basics (single statement upsert/sync):");
        Console.WriteLine("  MERGE dbo.Customers AS target");
        Console.WriteLine("  USING @IncomingCustomers AS source");
        Console.WriteLine("    ON target.CustomerId = source.CustomerId");
        Console.WriteLine("  WHEN MATCHED THEN");
        Console.WriteLine("    UPDATE SET target.Name = source.Name, target.Email = source.Email");
        Console.WriteLine("  WHEN NOT MATCHED BY TARGET THEN");
        Console.WriteLine("    INSERT (CustomerId, Name, Email) VALUES (source.CustomerId, source.Name, source.Email);");
        Console.WriteLine();

        Console.WriteLine("MERGE guardrails:");
        Console.WriteLine("- Ensure source set has unique business keys before merge.");
        Console.WriteLine("- Use appropriate locking/concurrency strategy for high-contention tables.");
        Console.WriteLine("- Validate affected row counts and unexpected action mixes.");
        Console.WriteLine("- Include regression tests for duplicate key and concurrent writer scenarios.\n");
    }

    private static void ShowSaferAlternatives()
    {
        Console.WriteLine("Safer explicit two-step upsert pattern:");
        Console.WriteLine("1) UPDATE existing rows");
        Console.WriteLine("   UPDATE t");
        Console.WriteLine("   SET t.Name = s.Name, t.Email = s.Email");
        Console.WriteLine("   FROM dbo.Customers t");
        Console.WriteLine("   JOIN @IncomingCustomers s ON s.CustomerId = t.CustomerId;");
        Console.WriteLine();
        Console.WriteLine("2) INSERT missing rows");
        Console.WriteLine("   INSERT dbo.Customers (CustomerId, Name, Email)");
        Console.WriteLine("   SELECT s.CustomerId, s.Name, s.Email");
        Console.WriteLine("   FROM @IncomingCustomers s");
        Console.WriteLine("   WHERE NOT EXISTS (");
        Console.WriteLine("     SELECT 1 FROM dbo.Customers t WHERE t.CustomerId = s.CustomerId");
        Console.WriteLine("   );\n");

        Console.WriteLine("❌ BAD upsert pattern (race prone):");
        Console.WriteLine("  IF EXISTS (SELECT 1 FROM dbo.Customers WHERE CustomerId = @Id)");
        Console.WriteLine("    UPDATE ...");
        Console.WriteLine("  ELSE");
        Console.WriteLine("    INSERT ...");
        Console.WriteLine("Reason: separate existence check can race under concurrency.\n");
    }

    private static void ShowWhenToUseWhich()
    {
        Console.WriteLine("When to use:");
        Console.WriteLine("- Use MERGE when set-based synchronization is clear, tested, and monitored.");
        Console.WriteLine("- Use explicit UPDATE+INSERT when you want simpler troubleshooting.");
        Console.WriteLine("- Always enforce uniqueness with constraints/indexes regardless of pattern.");
        Console.WriteLine("- Prefer TVPs for batch upsert input from C#.\n");
    }
}
