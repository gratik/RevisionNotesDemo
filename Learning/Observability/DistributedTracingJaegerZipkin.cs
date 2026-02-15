// ============================================================================
// DISTRIBUTED TRACING (JAEGER/ZIPKIN STYLE)
// ============================================================================
// WHAT IS THIS?
// -------------
// Distributed tracing records a request path across services as parent/child
// spans with timing and status metadata.
//
// WHY IT MATTERS
// --------------
// ✅ Pinpoints latency contributors across service boundaries
// ✅ Improves incident triage by exposing causal request flow
// ✅ Makes retry/circuit-breaker effects observable
//
// WHEN TO USE
// -----------
// ✅ Multi-service systems with asynchronous dependencies
// ✅ Teams troubleshooting cross-service regressions
//
// WHEN NOT TO USE
// ---------------
// ❌ Single-process apps where plain logs are sufficient
//
// REAL-WORLD EXAMPLE
// ------------------
// Frontend -> API Gateway -> Orders API -> Payment API -> DB trace tree used
// to locate p95 spikes in payment authorization.
// ============================================================================

namespace RevisionNotesDemo.Observability;

public static class DistributedTracingJaegerZipkin
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Distributed Tracing (Jaeger/Zipkin)                 ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        ShowTraceModel();
        ShowPropagation();
        ShowAnalysisWorkflow();
        ShowTraceExample();
    }

    private static void ShowTraceModel()
    {
        Console.WriteLine("1) TRACE MODEL");
        Console.WriteLine("- Trace = one end-to-end request");
        Console.WriteLine("- Span = timed unit of work within that trace");
        Console.WriteLine("- Parent/child relationships show dependency graph\n");
    }

    private static void ShowPropagation()
    {
        Console.WriteLine("2) CONTEXT PROPAGATION");
        Console.WriteLine("- Propagate W3C trace headers between services");
        Console.WriteLine("- HTTP: traceparent + tracestate");
        Console.WriteLine("- Messaging: include trace context in message metadata");
        Console.WriteLine("- Missing propagation creates broken traces and blind spots\n");
    }

    private static void ShowAnalysisWorkflow()
    {
        Console.WriteLine("3) ANALYSIS WORKFLOW");
        Console.WriteLine("- Filter by route and high latency percentile");
        Console.WriteLine("- Compare normal vs degraded traces");
        Console.WriteLine("- Identify widest span and repeated retry chains");
        Console.WriteLine("- Validate fix by watching span duration trend\n");
    }

    private static void ShowTraceExample()
    {
        Console.WriteLine("4) TRACE EXAMPLE");

        var trace = new TraceGraph("trace-91af");
        trace.AddSpan(new TraceSpanNode("gateway", 6, "OK"));
        trace.AddSpan(new TraceSpanNode("orders-api", 14, "OK"));
        trace.AddSpan(new TraceSpanNode("payment-api", 120, "OK"));
        trace.AddSpan(new TraceSpanNode("sql-payments", 95, "OK"));

        var slowest = trace.Spans.MaxBy(s => s.DurationMs);

        Console.WriteLine($"- Trace id: {trace.TraceId}");
        Console.WriteLine($"- Span count: {trace.Spans.Count}");
        Console.WriteLine($"- Slowest span: {slowest?.Name} ({slowest?.DurationMs}ms)\n");
    }
}

public sealed class TraceGraph
{
    public TraceGraph(string traceId)
    {
        TraceId = traceId;
    }

    public string TraceId { get; }

    public List<TraceSpanNode> Spans { get; } = [];

    public void AddSpan(TraceSpanNode span)
    {
        Spans.Add(span);
    }
}

public sealed record TraceSpanNode(string Name, int DurationMs, string Status);
