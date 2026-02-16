// ==============================================================================
// CONCURRENCY AND CONSISTENCY PATTERNS
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class ConcurrencyConsistencyPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n--- CONCURRENCY AND CONSISTENCY PATTERNS ---\n");
        ShowOptimisticConcurrency();
        ShowIsolationChoices();
        ShowLockingGuidance();
    }

    private static void ShowOptimisticConcurrency()
    {
        Console.WriteLine("Optimistic concurrency with rowversion:");
        Console.WriteLine("- Include rowversion in read model.");
        Console.WriteLine("- Update with predicate on key + rowversion.");
        Console.WriteLine("- If rows affected = 0, return conflict for retry/reload.\n");
    }

    private static void ShowIsolationChoices()
    {
        Console.WriteLine("Isolation choices:");
        Console.WriteLine("- Read committed for most transactional paths.");
        Console.WriteLine("- Snapshot or RCSI when read/write blocking is a recurring bottleneck.");
        Console.WriteLine("- Serializable only for strict correctness windows.\n");
    }

    private static void ShowLockingGuidance()
    {
        Console.WriteLine("Locking guidance:");
        Console.WriteLine("- Keep write transactions short.");
        Console.WriteLine("- Touch tables in consistent order.");
        Console.WriteLine("- Avoid broad lock hints unless backed by measured evidence.\n");
    }
}

