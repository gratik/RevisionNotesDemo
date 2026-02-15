// ============================================================================
// QUICK REFERENCE TABLES
// Reference: Revision Notes - Appendix B
// ============================================================================
// WHAT IS THIS?
// -------------
// Actionable decision tables for common .NET tradeoffs in runtime behavior,
// data access, API design, resilience, security, and operations.
//
// WHY IT MATTERS
// --------------
// ✅ Speeds up day-to-day technical decisions
// ✅ Provides consistent engineering defaults
// ✅ Improves review quality with shared heuristics
//
// WHEN TO USE
// -----------
// ✅ PR reviews, incident response, architecture discussions
// ✅ Team onboarding and coding standard alignment
//
// WHEN NOT TO USE
// ---------------
// ❌ As a substitute for benchmarking or threat modeling
// ❌ When product constraints invalidate generic defaults
//
// REAL-WORLD EXAMPLE
// ------------------
// Choosing async I/O, pagination strategy, and caching policy for a busy API.
// ============================================================================

namespace RevisionNotesDemo.Appendices;

public static class QuickReferenceTablesDemo
{
    private sealed record DecisionRow(string Scenario, string Prefer, string Avoid, string Why);

    private static readonly IReadOnlyList<DecisionRow> RuntimeRows =
    [
        new("I/O-bound endpoint", "async/await end-to-end", ".Result/.Wait()", "Prevents thread starvation under load"),
        new("CPU-bound heavy transform", "Task.Run with bounded concurrency", "Unbounded parallel loops", "Controls contention and latency spikes"),
        new("Hot-path logging", "Structured templates + event ids", "String interpolation in loops", "Lowers allocations and improves queryability"),
        new("Transient external failures", "Retry with jitter + timeout", "Infinite retries", "Avoids cascading failures and retry storms")
    ];

    private static readonly IReadOnlyList<DecisionRow> DataRows =
    [
        new("Read-only EF query", "AsNoTracking", "Tracking by default", "Reduces memory and change-tracker overhead"),
        new("Large result sets", "Pagination + projection", "Load full entities", "Cuts payload size and memory pressure"),
        new("Mixed reads/writes", "Scoped DbContext per request", "Singleton DbContext", "Prevents stale state and threading issues"),
        new("Frequent lookups", "Cache with TTL + size limit", "Unbounded cache", "Balances performance and memory usage")
    ];

    private static readonly IReadOnlyList<DecisionRow> ApiRows =
    [
        new("Versioning strategy", "URL or header versioning with policy", "Silent breaking changes", "Preserves client compatibility"),
        new("Error contract", "ProblemDetails with correlation id", "Ad hoc error payloads", "Improves operability and support"),
        new("Idempotent writes", "Idempotency key + conflict handling", "Blind retries on POST", "Prevents duplicate side effects"),
        new("Query endpoints", "Filter + pagination + projection", "Return all records", "Protects latency and memory")
    ];

    private static readonly IReadOnlyList<DecisionRow> SecurityRows =
    [
        new("API authentication", "Short-lived access tokens", "Long-lived bearer tokens", "Reduces impact of token leakage"),
        new("Browser sessions", "Secure + HttpOnly + SameSite cookies", "Client-readable auth cookies", "Mitigates XSS/CSRF abuse"),
        new("Secret management", "Vault + managed identity", "Secrets in source/appsettings", "Improves rotation and auditability"),
        new("Error responses", "ProblemDetails without internals", "Stack traces to clients", "Avoids information disclosure")
    ];

    private static readonly IReadOnlyList<DecisionRow> OperationsRows =
    [
        new("Deployment rollout", "Canary or blue/green", "Big bang deploy", "Lowers blast radius"),
        new("Observability baseline", "Logs + metrics + traces", "Logs only", "Faster root-cause analysis"),
        new("Capacity strategy", "Autoscale + SLO alerts", "Reactive manual scaling", "Maintains reliability under burst load"),
        new("Recovery readiness", "Tested rollback/runbooks", "Unverified rollback assumptions", "Reduces outage duration")
    ];

    public static void RunDemo()
    {
        Console.WriteLine("\n=== QUICK REFERENCE TABLES ===\n");

        PrintTable("1) RUNTIME AND RESILIENCE", RuntimeRows);
        PrintTable("2) DATA ACCESS", DataRows);
        PrintTable("3) API DESIGN", ApiRows);
        PrintTable("4) SECURITY", SecurityRows);
        PrintTable("5) OPERATIONS", OperationsRows);
        PrintPriorityChecklist();
        PrintIncidentTriageFlow();
    }

    private static void PrintTable(string title, IReadOnlyList<DecisionRow> rows)
    {
        Console.WriteLine(title);
        Console.WriteLine(new string('-', title.Length));

        foreach (var row in rows)
        {
            Console.WriteLine($"- Scenario: {row.Scenario}");
            Console.WriteLine($"  Prefer:   {row.Prefer}");
            Console.WriteLine($"  Avoid:    {row.Avoid}");
            Console.WriteLine($"  Why:      {row.Why}\n");
        }
    }

    private static void PrintPriorityChecklist()
    {
        Console.WriteLine("6) PRIORITY CHECKLIST");
        Console.WriteLine("---------------------");
        Console.WriteLine("- Correctness first, then reliability, then performance.");
        Console.WriteLine("- Prefer measurable improvements over speculative optimization.");
        Console.WriteLine("- Add observability before introducing complexity.");
        Console.WriteLine("- Revisit defaults with production telemetry each quarter.");
        Console.WriteLine("- Promote proven defaults into engineering standards.\n");
    }

    private static void PrintIncidentTriageFlow()
    {
        Console.WriteLine("7) INCIDENT TRIAGE FLOW");
        Console.WriteLine("-----------------------");

        var flow = new[]
        {
            "Check error budget/SLO impact",
            "Identify failing dependency from traces",
            "Apply safe mitigation (rollback, feature flag, scale)",
            "Capture root cause and preventive action"
        };

        for (var i = 0; i < flow.Length; i++)
        {
            Console.WriteLine($"- Step {i + 1}: {flow[i]}");
        }

        Console.WriteLine("\nTip: treat these as team defaults, then adapt based on context and metrics.");
    }
}
