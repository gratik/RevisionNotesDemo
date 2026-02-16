// ==============================================================================
// STATISTICS AND CARDINALITY PATTERNS
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class StatisticsAndCardinalityPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n--- STATISTICS AND CARDINALITY PATTERNS ---\n");
        ShowStatsBasics();
        ShowCardinalityFailureModes();
        ShowMitigationPatterns();
    }

    private static void ShowStatsBasics()
    {
        Console.WriteLine("Statistics basics:");
        Console.WriteLine("- Optimizer relies on stats histograms to estimate row counts.");
        Console.WriteLine("- Stale stats can produce poor joins/memory grants.");
        Console.WriteLine("- Auto-update helps, but high-churn tables may need targeted updates.\n");
    }

    private static void ShowCardinalityFailureModes()
    {
        Console.WriteLine("Common failure modes:");
        Console.WriteLine("- Severe underestimation -> spills and key lookup storms.");
        Console.WriteLine("- Severe overestimation -> oversized memory grants and concurrency loss.");
        Console.WriteLine("- Parameter sensitivity -> unstable plans by input distribution.\n");
    }

    private static void ShowMitigationPatterns()
    {
        Console.WriteLine("Mitigation patterns:");
        Console.WriteLine("- Refresh stats on high-change objects.");
        Console.WriteLine("- Rewrite predicates for better selectivity visibility.");
        Console.WriteLine("- Consider option recompile or query hints only with evidence.");
        Console.WriteLine("- Use Query Store to detect regressions over time.\n");
    }
}

