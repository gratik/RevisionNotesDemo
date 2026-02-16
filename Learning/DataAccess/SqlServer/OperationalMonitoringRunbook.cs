// ==============================================================================
// OPERATIONAL MONITORING RUNBOOK
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class OperationalMonitoringRunbook
{
    public static void RunAll()
    {
        Console.WriteLine("\n--- OPERATIONAL MONITORING RUNBOOK ---\n");
        ShowDailyChecks();
        ShowIncidentFlow();
        ShowRecoveryReadiness();
    }

    private static void ShowDailyChecks()
    {
        Console.WriteLine("Daily checks:");
        Console.WriteLine("- Query Store regressions and top resource queries.");
        Console.WriteLine("- Backup success/failure and restore-point freshness.");
        Console.WriteLine("- Blocking/deadlock trend and wait profile changes.");
        Console.WriteLine("- Capacity trend: CPU, memory grants, storage growth.\n");
    }

    private static void ShowIncidentFlow()
    {
        Console.WriteLine("Incident flow:");
        Console.WriteLine("1. Confirm affected endpoint/query set.");
        Console.WriteLine("2. Capture waits + top queries + active blockers.");
        Console.WriteLine("3. Compare with known baseline.");
        Console.WriteLine("4. Apply reversible mitigation and measure.\n");
    }

    private static void ShowRecoveryReadiness()
    {
        Console.WriteLine("Recovery readiness:");
        Console.WriteLine("- Validate RPO/RTO targets with regular restore drills.");
        Console.WriteLine("- Keep failover and rollback playbooks up to date.");
        Console.WriteLine("- Track mean-time-to-detect and mean-time-to-recover.\n");
    }
}

