// ==============================================================================
// BULK INGESTION PATTERNS
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class BulkIngestionPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n--- BULK INGESTION PATTERNS ---\n");
        CompareIngestionMethods();
        ShowBatchingGuidance();
        ShowValidationAndSafety();
    }

    private static void CompareIngestionMethods()
    {
        Console.WriteLine("Method comparison:");
        Console.WriteLine("- TVP: great for medium batch API-driven inserts/updates.");
        Console.WriteLine("- SqlBulkCopy: best for very large high-throughput ingestion.");
        Console.WriteLine("- Row-by-row inserts: simplest, but slowest and most lock-expensive.\n");
    }

    private static void ShowBatchingGuidance()
    {
        Console.WriteLine("Batching guidance:");
        Console.WriteLine("- Use bounded batch size to avoid long transactions.");
        Console.WriteLine("- Keep an idempotency key/checkpoint for resumable loads.");
        Console.WriteLine("- Stage then merge into target for complex validations.\n");
    }

    private static void ShowValidationAndSafety()
    {
        Console.WriteLine("Validation and safety:");
        Console.WriteLine("- Validate schema and mandatory fields before ingestion.");
        Console.WriteLine("- Track rejects and poison records separately.");
        Console.WriteLine("- Measure throughput, lock waits, and log growth during bulk loads.\n");
    }
}

