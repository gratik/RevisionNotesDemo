// ============================================================================
// OPENTELEMETRY + APPLICATION INSIGHTS INTEGRATION
// ============================================================================
// WHAT IS THIS?
// Demonstrates how OpenTelemetry instrumentation and Azure Application Insights
// work together for production-grade trace + metric + log correlation.
//
// WHY IT MATTERS
// ✅ Standard instrumentation with vendor-neutral model
// ✅ Azure-native storage/querying with App Insights + Log Analytics
// ✅ Faster incident triage using correlation id and trace context
// ============================================================================

namespace RevisionNotesDemo.Observability;

public static class OpenTelemetryAndApplicationInsightsIntegration
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  OpenTelemetry + Application Insights Integration     ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        ShowIntegrationModel();
        ShowCorrelationStrategy();
        ShowOperationalQueries();
    }

    private static void ShowIntegrationModel()
    {
        Console.WriteLine("1) INTEGRATION MODEL");
        Console.WriteLine("- Instrument services with OpenTelemetry.");
        Console.WriteLine("- Export traces/metrics to Azure Monitor -> Application Insights.");
        Console.WriteLine("- Keep structured logs with correlation fields for KQL workflows.\n");
    }

    private static void ShowCorrelationStrategy()
    {
        Console.WriteLine("2) CORRELATION STRATEGY");
        Console.WriteLine("- Accept/return X-Correlation-ID in APIs.");
        Console.WriteLine("- Let traceparent propagate automatically for HTTP.");
        Console.WriteLine("- Add correlation + traceparent to Service Bus messages for async hops.\n");
    }

    private static void ShowOperationalQueries()
    {
        Console.WriteLine("3) OPERATIONAL QUERIES");
        Console.WriteLine("- Query by customDimensions.correlationId for support tickets.");
        Console.WriteLine("- Pivot to operation_Id for full distributed trace timeline.");
        Console.WriteLine("- Build alerts from error rate, p95 latency, and queue lag thresholds.\n");
    }
}
