// ==============================================================================
// INDEX ARCHITECTURE PATTERNS
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class IndexArchitecturePatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n--- INDEX ARCHITECTURE PATTERNS ---\n");
        ShowClusteredStrategy();
        ShowNonclusteredStrategy();
        ShowOperationalGuidance();
    }

    private static void ShowClusteredStrategy()
    {
        Console.WriteLine("Clustered index strategy:");
        Console.WriteLine("- Choose narrow, stable, monotonic-ish keys when possible.");
        Console.WriteLine("- Avoid frequent updates to clustered key columns.");
        Console.WriteLine("- Validate fragmentation behavior under real write patterns.\n");
    }

    private static void ShowNonclusteredStrategy()
    {
        Console.WriteLine("Nonclustered index strategy:");
        Console.WriteLine("- Index on high-selectivity filter/join columns.");
        Console.WriteLine("- Use INCLUDE columns for covering high-frequency queries.");
        Console.WriteLine("- Use filtered indexes for sparse flags/status columns.\n");

        Console.WriteLine("❌ BAD: index every column.");
        Console.WriteLine("✅ GOOD: index from observed query workload and usage stats.\n");
    }

    private static void ShowOperationalGuidance()
    {
        Console.WriteLine("Operational guidance:");
        Console.WriteLine("- Track seeks/scans/lookups via DMVs.");
        Console.WriteLine("- Drop unused indexes with evidence.");
        Console.WriteLine("- Tune fill factor only when split patterns justify it.\n");
    }
}

