// ============================================================================
// APPLICATION INSIGHTS INTEGRATION
// ============================================================================
// WHAT IS THIS?
// -------------
// Azure Application Insights is a managed observability backend for requests,
// dependencies, traces, exceptions, custom events, and metrics.
//
// WHY IT MATTERS
// --------------
// ✅ Fast setup for ASP.NET workloads on Azure
// ✅ Built-in correlation across app/service dependencies
// ✅ Strong operational workflows with alerts, Live Metrics, and KQL
//
// WHEN TO USE
// -----------
// ✅ Azure-hosted systems that need centralized telemetry quickly
// ✅ Teams using Azure Monitor alerts and dashboards
//
// WHEN NOT TO USE
// ---------------
// ❌ Strictly self-hosted observability stacks with non-Azure constraints
//
// REAL-WORLD EXAMPLE
// ------------------
// Checkout API tracks request rates, failed dependencies, and business events
// ("OrderPlaced", "PaymentDeclined") with correlation IDs.
// ============================================================================

namespace RevisionNotesDemo.Observability;

public static class ApplicationInsightsIntegration
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Application Insights Integration                     ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        ShowTelemetryModel();
        ShowCorrelationPattern();
        ShowAlertingApproach();
        ShowKustoExamples();
    }

    private static void ShowTelemetryModel()
    {
        Console.WriteLine("1) TELEMETRY MODEL");
        Console.WriteLine("- Requests: inbound HTTP/RPC operations");
        Console.WriteLine("- Dependencies: SQL/HTTP/Service Bus calls");
        Console.WriteLine("- Exceptions + traces: diagnostic context");
        Console.WriteLine("- Custom events/metrics: business KPIs\n");
    }

    private static void ShowCorrelationPattern()
    {
        Console.WriteLine("2) CORRELATION PATTERN");

        var envelope = new AppInsightsTelemetryEnvelope(
            OperationId: "op-7d2e",
            RequestId: "req-1221",
            TraceId: "trace-7d2e",
            UserId: "user-19");

        Console.WriteLine($"- OperationId: {envelope.OperationId}");
        Console.WriteLine($"- RequestId: {envelope.RequestId}");
        Console.WriteLine($"- TraceId: {envelope.TraceId}");
        Console.WriteLine("- Use operation + trace identifiers on every telemetry item\n");
    }

    private static void ShowAlertingApproach()
    {
        Console.WriteLine("3) ALERTING APPROACH");
        Console.WriteLine("- Error-rate alert: > 2% failed requests in 5 minutes");
        Console.WriteLine("- Latency alert: p95 > 500ms for checkout endpoint");
        Console.WriteLine("- Dependency alert: SQL failure count above baseline");
        Console.WriteLine("- Route alerts to on-call with clear runbook links\n");
    }

    private static void ShowKustoExamples()
    {
        Console.WriteLine("4) KQL EXAMPLES");
        Console.WriteLine("- requests | where success == false | summarize count() by name");
        Console.WriteLine("- dependencies | summarize avg(duration) by target");
        Console.WriteLine("- exceptions | where cloud_RoleName == \"checkout-api\"\n");
    }
}

public sealed record AppInsightsTelemetryEnvelope(string OperationId, string RequestId, string TraceId, string UserId);
