using RevisionNotesDemo.Appendices;

namespace RevisionNotesDemo.UnitTests;

public class AppendicesOutputTests
{
    [Fact]
    public void PatternsOverratedNowRunDemoPrintsComprehensiveSections()
    {
        var output = CaptureConsole(() => PatternsOverratedNowDemo.RunDemo());

        Assert.Contains("DECISION RUBRIC", output);
        Assert.Contains("PATTERN SCORING MODEL", output);
        Assert.Contains("ADOPTION GUARDRAILS", output);
        Assert.Contains("Guiding rule:", output);
    }

    [Fact]
    public void QuickReferenceTablesRunDemoPrintsOperationalGuidance()
    {
        var output = CaptureConsole(() => QuickReferenceTablesDemo.RunDemo());

        Assert.Contains("API DESIGN", output);
        Assert.Contains("OPERATIONS", output);
        Assert.Contains("INCIDENT TRIAGE FLOW", output);
        Assert.Contains("team defaults", output);
    }

    [Fact]
    public void CommonInterviewQuestionsRunDemoPrintsPrepFrameworkAndPlan()
    {
        var output = CaptureConsole(() => CommonInterviewQuestionsDemo.RunDemo());

        Assert.Contains("ANSWER FRAMEWORK", output);
        Assert.Contains("CORE QUESTIONS", output);
        Assert.Contains("PRACTICE LOOP", output);
        Assert.Contains("7-DAY PREPARATION PLAN", output);
    }

    private static string CaptureConsole(Action action)
    {
        var originalOut = Console.Out;
        using var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            action();
            writer.Flush();
            return writer.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
