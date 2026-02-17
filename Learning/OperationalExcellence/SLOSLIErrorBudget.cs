namespace RevisionNotesDemo.OperationalExcellence;

public static class SLOSLIErrorBudget
{
    public static void RunAll()
    {
        Console.WriteLine("\n=== SLO SLI ERROR BUDGET ===\n");
        Console.WriteLine("- Define SLIs for latency, error rate, and availability.");
        Console.WriteLine("- Set SLO targets and a clear budget policy.");
        Console.WriteLine("- Gate risky changes when budget burn is high.");
        Console.WriteLine("- Use weekly reliability review to tune thresholds and runbooks.\n");
    }
}
