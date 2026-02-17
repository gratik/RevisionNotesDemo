namespace RevisionNotesDemo.OperationalExcellence;

public static class IncidentResponseRunbook
{
    public static void RunAll()
    {
        Console.WriteLine("\n=== INCIDENT RESPONSE RUNBOOK ===\n");
        Console.WriteLine("- Detect: alert with clear symptom, service, and severity.");
        Console.WriteLine("- Triage: confirm blast radius, user impact, and active release context.");
        Console.WriteLine("- Mitigate: rollback, feature-flag off, or traffic shift.");
        Console.WriteLine("- Recover: verify SLO stabilization and document timeline + actions.\n");
    }
}
