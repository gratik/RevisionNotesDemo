# Structured Logging

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Logging basics, distributed tracing concepts, and monitoring fundamentals.
- Related examples: docs/Logging-Observability/README.md
> Subject: [Logging-Observability](../README.md)

## Structured Logging

### String Interpolation vs Templates

```csharp
// ❌ BAD: String interpolation (not structured)
_logger.LogInformation($"Order {orderId} processed in {elapsed}ms");
// Logged as: "Order 123 processed in 45ms" (just a string)

// ✅ GOOD: Template with parameters (structured)
_logger.LogInformation("Order {OrderId} processed in {ElapsedMs}ms", orderId, elapsed);
// Logged as: { Message: "Order...", OrderId: 123, ElapsedMs: 45 }
// Can query: WHERE OrderId = 123 OR WHERE ElapsedMs > 100
```

### Benefits of Structured Logging

**Queryable**: Search by specific field values
```sql
-- Find all logs for specific order
SELECT * FROM Logs WHERE OrderId = 123

-- Find slow operations
SELECT * FROM Logs WHERE ElapsedMs > 1000
```

**Performance**: No string concatenation
```csharp
// ✅ Template compiled once, parameters passed efficiently
_logger.LogInformation("Processing {Count} items", items.Count);
```

---

## Detailed Guidance

Structured Logging guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Structured Logging before implementation work begins.
- Keep boundaries explicit so Structured Logging decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Structured Logging in production-facing code.
- When performance, correctness, or maintainability depends on consistent Structured Logging decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Structured Logging as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Structured Logging is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Structured Logging are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Structured Logging is about telemetry design for diagnostics and operations. It matters because good observability shortens detection and recovery times.
- Use it when correlating logs, traces, and metrics across service boundaries.

2-minute answer:
- Start with the problem Structured Logging solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: high-cardinality detail vs telemetry cost/noise.
- Close with one failure mode and mitigation: missing correlation context during incident response.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Structured Logging but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Structured Logging, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Structured Logging and map it to one concrete implementation in this module.
- 3 minutes: compare Structured Logging with an alternative, then walk through one failure mode and mitigation.