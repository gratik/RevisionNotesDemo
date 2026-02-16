// ==============================================================================
// NORMALIZATION AND PRAGMATIC DENORMALIZATION
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class NormalizationAndDenormalizationPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n--- NORMALIZATION AND DENORMALIZATION ---\n");
        ShowNormalization();
        ShowWhenToDenormalize();
        ShowGoodBadExample();
    }

    private static void ShowNormalization()
    {
        Console.WriteLine("Normalization baseline:");
        Console.WriteLine("- 1NF: atomic values");
        Console.WriteLine("- 2NF: no partial dependency on composite keys");
        Console.WriteLine("- 3NF: no transitive dependency on non-key columns\n");
    }

    private static void ShowWhenToDenormalize()
    {
        Console.WriteLine("When denormalization is justified:");
        Console.WriteLine("- Read-heavy workloads with stable query shape.");
        Console.WriteLine("- Precomputed summaries/materialized projections.");
        Console.WriteLine("- Reporting models where eventual consistency is acceptable.\n");
    }

    private static void ShowGoodBadExample()
    {
        Console.WriteLine("❌ BAD: denormalize early without measured bottleneck.");
        Console.WriteLine("✅ GOOD: normalize first, measure, then denormalize targeted hot paths.");
        Console.WriteLine("✅ GOOD: keep source-of-truth normalized and project to read models.\n");
    }
}

