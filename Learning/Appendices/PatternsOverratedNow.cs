// ============================================================================
// PATTERNS THAT ARE OVERUSED TODAY
// Reference: Revision Notes - Appendix A
// ============================================================================
// WHAT IS THIS?
// -------------
// A practical review of frequently overapplied patterns, including when they
// help, when they harm, and a scoring rubric for deciding if they are justified.
//
// WHY IT MATTERS
// --------------
// ✅ Prevents abstraction-heavy code with low business value
// ✅ Improves readability, onboarding speed, and operability
// ✅ Keeps architecture aligned with measurable constraints
//
// WHEN TO USE
// -----------
// ✅ Design reviews and architecture discussions
// ✅ Refactoring plans where complexity has grown organically
// ✅ ADR conversations requiring explicit tradeoffs
//
// WHEN NOT TO USE
// ---------------
// ❌ As rigid doctrine detached from product constraints
// ❌ To reject patterns that genuinely solve current pain points
//
// REAL-WORLD EXAMPLE
// ------------------
// Replacing service locator with constructor injection and reducing per-entity
// repositories improved testability and cut boilerplate in a feature module.
// ============================================================================

namespace RevisionNotesDemo.Appendices;

public static class PatternsOverratedNowDemo
{
    private sealed record PatternReview(
        string Name,
        string TypicalMisuse,
        string HiddenCost,
        string BetterDefault,
        string LegitimateUse,
        string ReviewQuestion,
        string SmellSignal);

    private static readonly IReadOnlyList<PatternReview> Reviews =
    [
        new(
            Name: "Singleton",
            TypicalMisuse: "Used as global mutable state for convenience",
            HiddenCost: "Tight coupling, hard test isolation, order-dependent bugs",
            BetterDefault: "Register explicit service lifetimes via DI",
            LegitimateUse: "Stateless infrastructure service with stable lifecycle",
            ReviewQuestion: "Does this object truly represent one process-wide resource?",
            SmellSignal: "State changes from multiple call paths"),

        new(
            Name: "Abstract Factory",
            TypicalMisuse: "Added before multiple product families exist",
            HiddenCost: "Excessive types and indirection for simple construction",
            BetterDefault: "Simple factory method + options/DI",
            LegitimateUse: "Multiple cohesive product families must vary together",
            ReviewQuestion: "Do we have at least two concrete product families today?",
            SmellSignal: "One implementation + many interfaces"),

        new(
            Name: "Service Locator",
            TypicalMisuse: "Used to avoid constructor parameter updates",
            HiddenCost: "Hidden dependencies and runtime resolution failures",
            BetterDefault: "Constructor injection with explicit contracts",
            LegitimateUse: "Only as boundary adapter in legacy integration",
            ReviewQuestion: "Can a new engineer discover dependencies from constructor only?",
            SmellSignal: "Runtime KeyNotFound/invalid resolve errors"),

        new(
            Name: "Repository Per Entity",
            TypicalMisuse: "Duplicating DbSet-like methods without domain behavior",
            HiddenCost: "Boilerplate, query fragmentation, fake abstraction",
            BetterDefault: "Use DbContext + focused query services",
            LegitimateUse: "Domain aggregate rules span multiple data stores",
            ReviewQuestion: "Does this repository add business meaning beyond CRUD?",
            SmellSignal: "Many pass-through methods with no policy"),

        new(
            Name: "Mediator Everywhere",
            TypicalMisuse: "Every interaction routed through handlers by default",
            HiddenCost: "Traceability overhead and ceremony for trivial operations",
            BetterDefault: "Direct service call where flow is straightforward",
            LegitimateUse: "Complex workflows needing pipeline behaviors",
            ReviewQuestion: "Are cross-cutting behaviors the main value here?",
            SmellSignal: "Single-line handlers with no orchestration"),

        new(
            Name: "Event Sourcing by Default",
            TypicalMisuse: "Chosen before audit/replay needs are validated",
            HiddenCost: "Operational complexity and schema evolution burden",
            BetterDefault: "State persistence with explicit audit log",
            LegitimateUse: "Strong replay/audit requirements with mature ops",
            ReviewQuestion: "Is event replay a core business requirement, not preference?",
            SmellSignal: "Event store exists but replay is never exercised")
    ];

    public static void RunDemo()
    {
        Console.WriteLine("\n=== PATTERNS THAT ARE OVERUSED TODAY ===\n");

        PrintDecisionRubric();
        PrintScoringModel();
        PrintReviews();
        PrintCaseStudy();
        PrintRefactoringChecklist();
        PrintAdoptionGuardrails();
    }

    private static void PrintDecisionRubric()
    {
        Console.WriteLine("1) DECISION RUBRIC (APPLY BEFORE ADDING A PATTERN)\n");
        Console.WriteLine("- Problem pressure: Is there recurring pain today?");
        Console.WriteLine("- Simpler option: Did we reject a lower-complexity alternative?");
        Console.WriteLine("- Operability: Can teams debug and maintain it quickly?");
        Console.WriteLine("- Longevity: Will this still help in 6-12 months?");
        Console.WriteLine("- Team fit: Does the team have enough depth for this pattern?\n");
    }

    private static void PrintScoringModel()
    {
        Console.WriteLine("2) PATTERN SCORING MODEL (0-10)\n");

        var scores = new Dictionary<string, int>
        {
            ["Current pain severity"] = 8,
            ["Alternative simplicity"] = 3,
            ["Operational burden"] = 6,
            ["Long-term value"] = 7
        };

        foreach (var score in scores)
        {
            Console.WriteLine($"- {score.Key}: {score.Value}/10");
        }

        var recommendation = scores["Current pain severity"] + scores["Long-term value"]
            - scores["Alternative simplicity"] - scores["Operational burden"];

        Console.WriteLine($"\n- Composite signal: {recommendation}");
        Console.WriteLine("- Positive composite suggests pattern may be justified.\n");
    }

    private static void PrintReviews()
    {
        Console.WriteLine("3) PATTERN REVIEWS\n");

        foreach (var review in Reviews)
        {
            Console.WriteLine($"- {review.Name}");
            Console.WriteLine($"  Misuse: {review.TypicalMisuse}");
            Console.WriteLine($"  Hidden cost: {review.HiddenCost}");
            Console.WriteLine($"  Better default: {review.BetterDefault}");
            Console.WriteLine($"  Legitimate use: {review.LegitimateUse}");
            Console.WriteLine($"  Review question: {review.ReviewQuestion}");
            Console.WriteLine($"  Smell signal: {review.SmellSignal}\n");
        }
    }

    private static void PrintCaseStudy()
    {
        Console.WriteLine("4) MINI CASE STUDY\n");
        Console.WriteLine("Context: Checkout module had service locator + 14 repositories.");
        Console.WriteLine("Action: Moved to constructor injection + 4 query-focused services.");
        Console.WriteLine("Result: 30% less boilerplate, clearer dependencies, easier tests.");
        Console.WriteLine("Secondary gain: incident triage time dropped due to explicit call graph.\n");
    }

    private static void PrintRefactoringChecklist()
    {
        Console.WriteLine("5) REFACTORING CHECKLIST\n");
        Console.WriteLine("- Replace hidden dependencies with constructor contracts.");
        Console.WriteLine("- Collapse duplicate abstractions with no domain value.");
        Console.WriteLine("- Keep pattern only if it solves validated operational pain.");
        Console.WriteLine("- Document retained patterns with explicit rationale.");
        Console.WriteLine("- Add tests proving behavior survives simplification.\n");
    }

    private static void PrintAdoptionGuardrails()
    {
        Console.WriteLine("6) ADOPTION GUARDRAILS\n");
        Console.WriteLine("- Introduce one architectural pattern at a time per domain area.");
        Console.WriteLine("- Re-evaluate after one release cycle with telemetry data.");
        Console.WriteLine("- Treat architecture debt like product debt: track and prioritize.");
        Console.WriteLine("- Avoid pattern-driven rewrites without measurable outcomes.\n");

        Console.WriteLine("Guiding rule: use patterns to remove risk, not to signal sophistication.");
    }
}
