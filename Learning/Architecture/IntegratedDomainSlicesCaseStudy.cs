// ============================================================================
// INTEGRATED DOMAIN SLICES CASE STUDY
// ============================================================================
// WHAT IS THIS?
// -------------
// A multi-slice extension of the end-to-end case study showing how one product
// feature moves through API, data, resilience, observability, security, and
// deployment checkpoints in cohesive domain slices.
//
// WHY IT MATTERS
// --------------
// ✅ Connects cross-cutting concerns into concrete implementation slices
// ✅ Gives a reusable delivery template for feature teams
// ✅ Makes tradeoffs visible from request path to production rollout
//
// WHEN TO USE
// -----------
// ✅ During architecture workshops and release planning
// ✅ For onboarding engineers into full-lifecycle ownership
// ============================================================================

namespace RevisionNotesDemo.Architecture;

public static class IntegratedDomainSlicesCaseStudy
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║ Integrated Domain Slices: Order Platform             ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        var blueprints = BuildBlueprints();
        PrintBlueprints(blueprints);

        var workflow = new IntegratedOrderWorkflow();
        workflow.RunDemo();
    }

    private static IReadOnlyList<DomainSliceBlueprint> BuildBlueprints()
    {
        return
        [
            new DomainSliceBlueprint(
                SliceName: "Slice A - Checkout API",
                Goal: "Accept a place-order command with idempotent semantics and audit-safe persistence.",
                ApiContract: "POST /orders with idempotency key and validated command payload.",
                DataModel: "Order aggregate + transactional outbox event in same write boundary.",
                Resilience: "Input guards, optimistic conflict handling, bounded retries for transient storage errors.",
                Observability: "Structured logs with orderId, traceId, latency histogram, error-rate SLO.",
                Security: "JWT auth + role checks + anti-replay request fingerprinting.",
                Deployment: "Canary release with readiness gate and rollback threshold."),
            new DomainSliceBlueprint(
                SliceName: "Slice B - Fulfillment Worker",
                Goal: "Consume OrderPlaced events and reserve inventory without duplicate side effects.",
                ApiContract: "Message contract: OrderPlaced(orderId, tenantId, items, occurredUtc).",
                DataModel: "Inbox deduplication key + inventory reservation records.",
                Resilience: "Exponential backoff retries + dead-letter handoff after max attempts.",
                Observability: "Queue lag metric, processing duration, failed attempt count by reason.",
                Security: "Managed identity for queue/storage access and scoped secret retrieval.",
                Deployment: "Blue/green worker deployment with drain + handoff."),
            new DomainSliceBlueprint(
                SliceName: "Slice C - Customer Notification",
                Goal: "Publish customer confirmation reliably after fulfillment state transition.",
                ApiContract: "Notification command envelope with channel preferences and locale.",
                DataModel: "Notification log table + channel delivery status history.",
                Resilience: "Circuit breaker for provider outage + fallback provider routing.",
                Observability: "Delivery success rate, provider latency, template render failures.",
                Security: "PII masking in logs, encryption-at-rest for message payloads.",
                Deployment: "Feature-flag rollout by tenant cohort with staged enablement.")
        ];
    }

    private static void PrintBlueprints(IEnumerable<DomainSliceBlueprint> blueprints)
    {
        Console.WriteLine("Domain slice blueprints:\n");

        foreach (var slice in blueprints)
        {
            Console.WriteLine($"{slice.SliceName}");
            Console.WriteLine($"- Goal: {slice.Goal}");
            Console.WriteLine($"- API/Contract: {slice.ApiContract}");
            Console.WriteLine($"- Data: {slice.DataModel}");
            Console.WriteLine($"- Resilience: {slice.Resilience}");
            Console.WriteLine($"- Observability: {slice.Observability}");
            Console.WriteLine($"- Security: {slice.Security}");
            Console.WriteLine($"- Deployment: {slice.Deployment}\n");
        }
    }
}

internal sealed class IntegratedOrderWorkflow
{
    private readonly SliceTelemetryCollector _telemetry = new();
    private readonly HashSet<string> _fulfillmentInbox = [];
    private int _notificationAttempts;

    public void RunDemo()
    {
        Console.WriteLine("Integrated workflow simulation:");

        var placeOrder = RunCheckoutSlice(orderId: "ord-9001", idempotencyKey: "req-9001");
        Console.WriteLine($"- Checkout slice result: {placeOrder.Status}");

        var fulfillment = RunFulfillmentSlice(placeOrder.OrderId);
        Console.WriteLine($"- Fulfillment slice result: {fulfillment.Status} after {fulfillment.Attempts} attempt(s)");

        var notification = RunNotificationSlice(placeOrder.OrderId);
        Console.WriteLine($"- Notification slice result: {notification.Status} after {notification.Attempts} attempt(s)\n");

        _telemetry.PrintSummary();
    }

    private SliceExecutionResult RunCheckoutSlice(string orderId, string idempotencyKey)
    {
        var latencyMs = 82;
        var replaySafe = !string.IsNullOrWhiteSpace(idempotencyKey);
        var status = replaySafe ? "Accepted with outbox event persisted" : "Rejected";
        _telemetry.Track("checkout", latencyMs, status);
        return new SliceExecutionResult(orderId, status, Attempts: 1);
    }

    private SliceExecutionResult RunFulfillmentSlice(string orderId)
    {
        if (_fulfillmentInbox.Contains(orderId))
        {
            _telemetry.Track("fulfillment", 12, "Idempotent replay ignored");
            return new SliceExecutionResult(orderId, "Idempotent replay ignored", Attempts: 1);
        }

        var attempts = RetryPolicySimulator.Execute(maxAttempts: 3, shouldFailAttempt: attempt => attempt == 1);
        _fulfillmentInbox.Add(orderId);

        var status = attempts < 3 ? "Reserved inventory and published FulfillmentStarted" : "Moved to dead-letter";
        _telemetry.Track("fulfillment", 145, status);
        return new SliceExecutionResult(orderId, status, attempts);
    }

    private SliceExecutionResult RunNotificationSlice(string orderId)
    {
        _notificationAttempts = RetryPolicySimulator.Execute(
            maxAttempts: 2,
            shouldFailAttempt: attempt => attempt == 1);

        var status = _notificationAttempts == 1
            ? "Delivered through primary provider"
            : "Delivered through fallback provider";

        _telemetry.Track("notification", 61, status);
        return new SliceExecutionResult(orderId, status, _notificationAttempts);
    }
}

internal sealed class SliceTelemetryCollector
{
    private readonly List<SliceTelemetryEntry> _entries = [];

    public void Track(string slice, int latencyMs, string status)
    {
        _entries.Add(new SliceTelemetryEntry(slice, latencyMs, status));
    }

    public void PrintSummary()
    {
        Console.WriteLine("Operational summary:");
        foreach (var group in _entries.GroupBy(x => x.Slice))
        {
            var averageLatency = (int)group.Average(x => x.LatencyMs);
            var latestStatus = group.Last().Status;
            Console.WriteLine($"- {group.Key}: avg latency {averageLatency}ms, latest status '{latestStatus}'");
        }
    }
}

internal static class RetryPolicySimulator
{
    public static int Execute(int maxAttempts, Func<int, bool> shouldFailAttempt)
    {
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            if (!shouldFailAttempt(attempt))
            {
                return attempt;
            }
        }

        return maxAttempts;
    }
}

internal sealed record DomainSliceBlueprint(
    string SliceName,
    string Goal,
    string ApiContract,
    string DataModel,
    string Resilience,
    string Observability,
    string Security,
    string Deployment);

internal sealed record SliceExecutionResult(string OrderId, string Status, int Attempts);

internal sealed record SliceTelemetryEntry(string Slice, int LatencyMs, string Status);
