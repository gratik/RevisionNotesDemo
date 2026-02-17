namespace RevisionNotesDemo.EngineeringProcess;

public static class CodeReviewStandards
{
    public static void RunAll()
    {
        Console.WriteLine("\n=== CODE REVIEW STANDARDS ===\n");
        Console.WriteLine("- Prioritize correctness, regression risk, and operability over style-only feedback.");
        Console.WriteLine("- Require evidence for tests covering happy and failure paths.");
        Console.WriteLine("- Check security, performance, and observability implications.");
        Console.WriteLine("- Keep review comments actionable, specific, and scoped to outcome.\n");
    }
}
