# Performance Considerations

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Logging basics, distributed tracing concepts, and monitoring fundamentals.
- Related examples: docs/Logging-Observability/README.md
> Subject: [Logging-Observability](../README.md)

## Performance Considerations

### Log Guards

```csharp
// ❌ BAD: Expensive operation even if logging disabled
_logger.LogDebug($"User data: {SerializeToJson(user)}");
// SerializeToJson runs even if Debug logging is off!

// ✅ GOOD: Check if enabled first
if (_logger.IsEnabled(LogLevel.Debug))
{
    _logger.LogDebug("User data: {UserJson}", SerializeToJson(user));
}

// ✅ BETTER: Use LoggerMessage for high-performance logging
private static readonly Action<ILogger, string, Exception?> _logUserData =
    LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(1, "UserData"),
        "User data: {UserJson}");

public void LogUserData(string json)
{
    _logUserData(_logger, json, null);
}
```

### High-Performance Logging with LoggerMessage

```csharp
public static class LogMessages
{
    // ✅ Compiled once, reused (zero allocations)
    private static readonly Action<ILogger, int, int, Exception?> _orderProcessed =
        LoggerMessage.Define<int, int>(
            LogLevel.Information,
            new EventId(1001, "OrderProcessed"),
            "Order {OrderId} processed in {ElapsedMs}ms");
    
    public static void LogOrderProcessed(this ILogger logger, int orderId, int elapsedMs)
    {
        _orderProcessed(logger, orderId, elapsedMs, null);
    }
}

// Usage
_logger.LogOrderProcessed(123, 45);
// 10x faster than regular logging in hot paths
```

---

## Detailed Guidance

Performance guidance focuses on bottleneck-first optimization supported by representative measurements and guardrails.

### Design Notes
- Define success criteria for Performance Considerations before implementation work begins.
- Keep boundaries explicit so Performance Considerations decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Performance Considerations in production-facing code.
- When performance, correctness, or maintainability depends on consistent Performance Considerations decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Performance Considerations as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Performance Considerations is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Performance Considerations are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Performance Considerations is about telemetry design for diagnostics and operations. It matters because good observability shortens detection and recovery times.
- Use it when correlating logs, traces, and metrics across service boundaries.

2-minute answer:
- Start with the problem Performance Considerations solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: high-cardinality detail vs telemetry cost/noise.
- Close with one failure mode and mitigation: missing correlation context during incident response.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Performance Considerations but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Performance Considerations, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Performance Considerations and map it to one concrete implementation in this module.
- 3 minutes: compare Performance Considerations with an alternative, then walk through one failure mode and mitigation.