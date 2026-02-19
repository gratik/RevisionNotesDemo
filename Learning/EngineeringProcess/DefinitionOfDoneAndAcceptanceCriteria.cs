namespace RevisionNotesDemo.EngineeringProcess;

public static class DefinitionOfDoneAndAcceptanceCriteria
{
    public static void RunAll()
    {
        Console.WriteLine("\n=== DEFINITION OF DONE AND ACCEPTANCE CRITERIA ===\n");
        Console.WriteLine("- DoD includes tests, docs, monitoring, and rollback readiness.");
        Console.WriteLine("- Acceptance criteria specify observable behavior, not implementation details.");
        Console.WriteLine("- Include non-functional criteria: latency, reliability, and supportability.");
        Console.WriteLine("- Reject completion when criteria are unverifiable.\n");
    }
}
