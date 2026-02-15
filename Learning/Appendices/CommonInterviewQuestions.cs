// ============================================================================
// COMMON INTERVIEW QUESTIONS
// Reference: Revision Notes - Appendix C
// ============================================================================
// WHAT IS THIS?
// -------------
// Structured interview prep covering core .NET topics with concise answer
// templates, follow-up depth points, and practical examples.
//
// WHY IT MATTERS
// --------------
// ✅ Improves clarity and confidence in technical interviews
// ✅ Encourages principle-based answers over memorized definitions
// ✅ Helps connect concepts to production tradeoffs
//
// WHEN TO USE
// -----------
// ✅ Pre-interview refresh and mock interview practice
// ✅ Team knowledge sharing sessions
//
// WHEN NOT TO USE
// ---------------
// ❌ As a replacement for implementation practice
// ❌ As an exhaustive system-design reference
//
// REAL-WORLD EXAMPLE
// ------------------
// Explain IQueryable vs IEnumerable with SQL translation implications and
// mention when deferred execution helps performance.
// ============================================================================

namespace RevisionNotesDemo.Appendices;

public static class CommonInterviewQuestionsDemo
{
    private sealed record InterviewQuestion(
        string Category,
        string Question,
        string ShortAnswer,
        string DeepDive,
        string PracticalExample,
        string FollowUp);

    private static readonly IReadOnlyList<InterviewQuestion> Questions =
    [
        new(
            Category: "Language + LINQ",
            Question: "What is the difference between IEnumerable and IQueryable?",
            ShortAnswer: "IEnumerable executes in-memory; IQueryable builds expressions for provider-side execution.",
            DeepDive: "Use IQueryable until projection/filtering is complete, then materialize with ToListAsync.",
            PracticalExample: "Filtering orders in SQL avoids loading full table into app memory.",
            FollowUp: "How would you detect unintended client-side evaluation?"),

        new(
            Category: "Concurrency",
            Question: "When should you use async/await?",
            ShortAnswer: "Use it for I/O-bound operations to free request threads.",
            DeepDive: "Avoid sync-over-async and propagate async all the way up call chains.",
            PracticalExample: "HTTP + DB calls in web APIs should be async to improve throughput.",
            FollowUp: "What are common deadlock causes in legacy sync contexts?"),

        new(
            Category: "Architecture",
            Question: "Explain SOLID in one sentence.",
            ShortAnswer: "SOLID helps design modules that change safely and are easy to test.",
            DeepDive: "Give one concrete example, like interface segregation reducing unnecessary coupling.",
            PracticalExample: "Split a large payment interface into capture/refund/read contracts.",
            FollowUp: "Which SOLID principle is most violated in your recent codebase?"),

        new(
            Category: "Dependency Injection",
            Question: "What are DI lifetimes and common mistakes?",
            ShortAnswer: "Singleton = app lifetime, Scoped = request lifetime, Transient = new each resolve.",
            DeepDive: "Never inject scoped services into singleton without safe factory/abstraction.",
            PracticalExample: "DbContext should be scoped, not singleton, in ASP.NET Core.",
            FollowUp: "How do you debug captive dependency issues quickly?"),

        new(
            Category: "Reliability",
            Question: "What is a circuit breaker and why use it?",
            ShortAnswer: "It fails fast when dependency failures cross threshold to prevent cascading outages.",
            DeepDive: "Combine with timeout + retry + jitter and instrument open/half-open transitions.",
            PracticalExample: "Payment provider outage should not exhaust all API worker threads.",
            FollowUp: "How do retries interact with circuit breakers under load?"),

        new(
            Category: "Web API",
            Question: "How do you handle exceptions in production Web APIs?",
            ShortAnswer: "Use centralized middleware with ProblemDetails and correlation id.",
            DeepDive: "Return safe client messages, log full server context, and classify expected vs unexpected errors.",
            PracticalExample: "Validation errors map to 400; unhandled faults map to 500 with trace id.",
            FollowUp: "What should never be included in client-facing errors?"),

        new(
            Category: "Data + Performance",
            Question: "How do you prevent memory leaks in .NET?",
            ShortAnswer: "Dispose unmanaged resources, unsubscribe events, avoid unnecessary long-lived references.",
            DeepDive: "Use profilers to detect retained objects and track allocation hotspots.",
            PracticalExample: "Background singleton holding per-request objects causes retained-memory growth.",
            FollowUp: "Which metrics indicate a leak versus temporary pressure?"),

        new(
            Category: "Modern C#",
            Question: "When should you use records?",
            ShortAnswer: "For immutable, value-based models such as DTOs and message contracts.",
            DeepDive: "Prefer classes when identity/lifecycle mutation is central.",
            PracticalExample: "API response models are good record candidates; aggregates often are not.",
            FollowUp: "How do records affect equality and collection behavior?"),

        new(
            Category: "Security",
            Question: "How would you secure a public API?",
            ShortAnswer: "Authenticate, authorize per resource, validate input, and enforce rate limits.",
            DeepDive: "Add threat modeling, secret management, and telemetry-driven detection.",
            PracticalExample: "JWT scopes + per-tenant checks + ProblemDetails + WAF policies.",
            FollowUp: "What controls would you prioritize first in a legacy API?"),

        new(
            Category: "Cloud + DevOps",
            Question: "What makes a deployment production-safe?",
            ShortAnswer: "Immutable artifacts, staged rollout, health checks, and fast rollback.",
            DeepDive: "Use CI/CD gates, canary strategy, and post-deploy SLO verification.",
            PracticalExample: "Blue/green release with automated rollback on p95 regression.",
            FollowUp: "How do you prove rollback readiness before an incident?"),

        new(
            Category: "System Design",
            Question: "How do you design an idempotent order API?",
            ShortAnswer: "Use client idempotency keys with dedupe store and deterministic response replay.",
            DeepDive: "Persist key + result atomically and define expiration semantics.",
            PracticalExample: "Same key returns same order id for safe retry during timeouts.",
            FollowUp: "How do you prevent key-space abuse and replay attacks?")
    ];

    public static void RunDemo()
    {
        Console.WriteLine("\n=== COMMON INTERVIEW QUESTIONS ===\n");

        PrintAnswerFramework();
        PrintQuestions();
        PrintPracticeLoop();
        PrintWeeklyPreparationPlan();
    }

    private static void PrintAnswerFramework()
    {
        Console.WriteLine("1) ANSWER FRAMEWORK (60-90 SECONDS)");
        Console.WriteLine("- Definition: one-sentence concept summary");
        Console.WriteLine("- Tradeoff: when it helps vs hurts");
        Console.WriteLine("- Example: real production scenario");
        Console.WriteLine("- Risk control: how to implement safely");
        Console.WriteLine("- Follow-up: one deeper technical point\n");
    }

    private static void PrintQuestions()
    {
        Console.WriteLine("2) CORE QUESTIONS\n");

        for (var i = 0; i < Questions.Count; i++)
        {
            var item = Questions[i];

            Console.WriteLine($"Q{i + 1} [{item.Category}]: {item.Question}");
            Console.WriteLine($"A (short): {item.ShortAnswer}");
            Console.WriteLine($"Deep dive: {item.DeepDive}");
            Console.WriteLine($"Example: {item.PracticalExample}");
            Console.WriteLine($"Follow-up prompt: {item.FollowUp}\n");
        }
    }

    private static void PrintPracticeLoop()
    {
        Console.WriteLine("3) PRACTICE LOOP");
        Console.WriteLine("- Record mock answers and trim filler language.");
        Console.WriteLine("- Add one measurable production result per answer.");
        Console.WriteLine("- Prepare failure scenarios and tradeoff discussions.");
        Console.WriteLine("- Rehearse whiteboard explanations for system questions.\n");
    }

    private static void PrintWeeklyPreparationPlan()
    {
        Console.WriteLine("4) 7-DAY PREPARATION PLAN");

        var plan = new[]
        {
            "Day 1: Language fundamentals + LINQ",
            "Day 2: Async, threading, and performance",
            "Day 3: Web API, security, and error handling",
            "Day 4: Data access, transactions, and caching",
            "Day 5: Architecture and design patterns",
            "Day 6: System design and incident case studies",
            "Day 7: Full mock interview with feedback"
        };

        foreach (var step in plan)
        {
            Console.WriteLine($"- {step}");
        }

        Console.WriteLine();
    }
}
