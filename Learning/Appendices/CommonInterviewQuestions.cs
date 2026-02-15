// ============================================================================
// COMMON INTERVIEW QUESTIONS
// Reference: Revision Notes - Appendix C
// ============================================================================
// WHAT IS THIS?
// -------------
// A compact list of common C# and .NET interview questions.
//
// WHY IT MATTERS
// --------------
// ✅ Aids focused review and recall
// ✅ Helps practice clear explanations
//
// WHEN TO USE
// -----------
// ✅ Interview prep or quick self-assessment
// ✅ Revising fundamentals before a screen
//
// WHEN NOT TO USE
// ---------------
// ❌ As a replacement for deep, hands-on learning
// ❌ As a sole source of truth for design decisions
//
// REAL-WORLD EXAMPLE
// ------------------
// Practice explaining IQueryable vs IEnumerable.
// ============================================================================

namespace RevisionNotesDemo.Appendices;

public static class CommonInterviewQuestionsDemo
{
    private record InterviewQuestion(string Question, string KeyPoints);

    private static readonly InterviewQuestion[] Questions =
    {
        new("What is the difference between IEnumerable and IQueryable?",
            "IQueryable builds expressions for remote execution; IEnumerable runs in-memory."),
        new("When would you use async/await?",
            "For I/O-bound work; avoid blocking threads for network or disk operations."),
        new("Explain SOLID in one sentence.",
            "Principles that keep code modular, testable, and easy to change."),
        new("What is dependency injection and why use it?",
            "It inverts dependencies to improve testability and flexibility."),
        new("How do you prevent memory leaks in .NET?",
            "Dispose resources, unsubscribe events, avoid long-lived references."),
        new("What is a circuit breaker?",
            "It stops calls to failing services, allowing recovery and fast failure."),
        new("When should you use records?",
            "For immutable, value-based data models and concise DTOs."),
        new("How do you handle exceptions in Web APIs?",
            "Use centralized middleware with consistent ProblemDetails responses."),
        new("What are DI lifetimes?",
            "Singleton: one instance, Scoped: per request, Transient: per use."),
        new("Why is logging with templates preferred?",
            "Structured fields enable filtering and analytics, with better performance.")
    };

    public static void RunDemo()
    {
        Console.WriteLine("\n=== COMMON INTERVIEW QUESTIONS ===\n");

        for (var i = 0; i < Questions.Length; i++)
        {
            var q = Questions[i];
            Console.WriteLine($"Q{i + 1}: {q.Question}");
            Console.WriteLine($"A: {q.KeyPoints}\n");
        }
    }
}