// ==============================================================================
// EXECUTION PLAN ANALYSIS LAB
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class ExecutionPlanAnalysisLab
{
    public static void RunAll()
    {
        Console.WriteLine("\n--- EXECUTION PLAN ANALYSIS LAB ---\n");
        ShowPlanReadingChecklist();
        ShowCommonSmells();
        ShowFixWorkflow();
    }

    private static void ShowPlanReadingChecklist()
    {
        Console.WriteLine("Plan reading checklist:");
        Console.WriteLine("- Identify highest-cost operators first.");
        Console.WriteLine("- Validate estimated vs actual row count divergence.");
        Console.WriteLine("- Look for scans, key lookups, sorts, and hash spill warnings.");
        Console.WriteLine("- Inspect memory grant size and spill indicators.\n");
    }

    private static void ShowCommonSmells()
    {
        Console.WriteLine("Common plan smells:");
        Console.WriteLine("- Key lookup repeated per row from non-covering index.");
        Console.WriteLine("- Implicit conversion on join/filter columns.");
        Console.WriteLine("- Scalar function calls on large rowsets.");
        Console.WriteLine("- Missing predicates on partitioning key.\n");
    }

    private static void ShowFixWorkflow()
    {
        Console.WriteLine("Fix workflow:");
        Console.WriteLine("1. Capture baseline (duration, reads, CPU).");
        Console.WriteLine("2. Apply one change (query rewrite OR index change).");
        Console.WriteLine("3. Compare execution plan and metrics.");
        Console.WriteLine("4. Keep only measurable improvements.\n");
    }
}

