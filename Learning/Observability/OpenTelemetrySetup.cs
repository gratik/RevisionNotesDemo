// ============================================================================
// OPEN TELEMETRY SETUP
// ============================================================================
// WHAT IS THIS?
// -------------
// OpenTelemetry is a vendor-neutral standard for traces, metrics, and logs.
// It separates instrumentation from backend choice via exporters.
//
// WHY IT MATTERS
// --------------
// ✅ Avoids lock-in to one monitoring vendor
// ✅ Promotes consistent telemetry schema across services
// ✅ Enables correlation between logs, traces, and metrics
//
// WHEN TO USE
// -----------
// ✅ Microservices/distributed systems with multiple teams
// ✅ Platforms needing backend flexibility (Grafana, Azure, Datadog, etc.)
//
// WHEN NOT TO USE
// ---------------
// ❌ Single-process prototypes where basic logging is enough
//
// REAL-WORLD EXAMPLE
// ------------------
// API service emits HTTP/server spans, DB client spans, request-duration
// histogram, and error logs with shared trace identifiers.
// ============================================================================

namespace RevisionNotesDemo.Observability;

public static class OpenTelemetrySetup
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  OpenTelemetry Setup                                  ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        ShowPipelineShape();
        ShowResourceConventions();
        ShowSamplingAndCardinality();
        ShowInstrumentationExample();
    }

    private static void ShowPipelineShape()
    {
        Console.WriteLine("1) PIPELINE SHAPE");
        Console.WriteLine("- Instrumentation -> SDK -> Processor -> Exporter");
        Console.WriteLine("- Signals: traces, metrics, logs");
        Console.WriteLine("- Configure once per service with environment/resource attributes\n");
    }

    private static void ShowResourceConventions()
    {
        Console.WriteLine("2) RESOURCE CONVENTIONS");
        Console.WriteLine("- service.name: revision-notes-api");
        Console.WriteLine("- service.version: 1.4.0");
        Console.WriteLine("- deployment.environment: prod");
        Console.WriteLine("- cloud.region: eastus");
        Console.WriteLine("- Keep keys stable so dashboards and alerts stay reliable\n");
    }

    private static void ShowSamplingAndCardinality()
    {
        Console.WriteLine("3) SAMPLING + CARDINALITY");
        Console.WriteLine("- Head sampling controls volume/cost (for example: 10% in prod)");
        Console.WriteLine("- Keep span/metric labels low-cardinality");
        Console.WriteLine("- Never tag spans with raw user input or full URLs with IDs\n");
    }

    private static void ShowInstrumentationExample()
    {
        Console.WriteLine("4) INSTRUMENTATION EXAMPLE");

        var trace = new OTelTrace("trace-a1f2");
        trace.StartSpan("HTTP POST /orders", "server");
        trace.StartSpan("SQL INSERT orders", "client");
        trace.EndSpan("SQL INSERT orders", durationMs: 12, status: "OK");
        trace.EndSpan("HTTP POST /orders", durationMs: 43, status: "OK");

        Console.WriteLine($"- Trace id: {trace.TraceId}");
        Console.WriteLine($"- Span count: {trace.Spans.Count}");
        Console.WriteLine($"- Root span status: {trace.Spans[0].Status}\n");
    }
}

public sealed class OTelTrace
{
    public OTelTrace(string traceId)
    {
        TraceId = traceId;
    }

    public string TraceId { get; }

    public List<OTelSpan> Spans { get; } = [];

    public void StartSpan(string name, string kind)
    {
        Spans.Add(new OTelSpan(name, kind, "InProgress", 0));
    }

    public void EndSpan(string name, int durationMs, string status)
    {
        var span = Spans.LastOrDefault(s => s.Name == name && s.Status == "InProgress");
        if (span is null)
        {
            return;
        }

        var index = Spans.IndexOf(span);
        Spans[index] = span with { DurationMs = durationMs, Status = status };
    }
}

public sealed record OTelSpan(string Name, string Kind, string Status, int DurationMs);
