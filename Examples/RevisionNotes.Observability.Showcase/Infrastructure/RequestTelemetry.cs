using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace RevisionNotes.Observability.Showcase.Infrastructure;

public sealed class RequestTelemetry
{
    private readonly object _gate = new();

    public ActivitySource ActivitySource { get; } = new("RevisionNotes.Observability.Showcase");
    public Meter Meter { get; } = new("RevisionNotes.Observability.Showcase", "1.0.0");

    public Counter<long> RequestCounter { get; }
    public Histogram<double> RequestDurationMs { get; }

    public long TotalRequests { get; private set; }
    public long ErrorRequests { get; private set; }
    public DateTimeOffset LastUpdatedUtc { get; private set; } = DateTimeOffset.UtcNow;

    public RequestTelemetry()
    {
        RequestCounter = Meter.CreateCounter<long>("http.server.request.count", description: "Total HTTP requests");
        RequestDurationMs = Meter.CreateHistogram<double>("http.server.request.duration.ms", unit: "ms", description: "HTTP request duration in milliseconds");
    }

    public void RecordSuccess()
    {
        lock (_gate)
        {
            TotalRequests++;
            LastUpdatedUtc = DateTimeOffset.UtcNow;
        }
    }

    public void RecordFailure()
    {
        lock (_gate)
        {
            TotalRequests++;
            ErrorRequests++;
            LastUpdatedUtc = DateTimeOffset.UtcNow;
        }
    }
}