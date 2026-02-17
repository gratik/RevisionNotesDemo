# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Logging basics, distributed tracing concepts, and monitoring fundamentals.
- Related examples: docs/Logging-Observability/README.md
> Subject: [Logging-Observability](../README.md)

## Common Pitfalls

### ❌ String Interpolation

```csharp
// ❌ BAD: Not structured, always allocates string
_logger.LogInformation($"Processing order {orderId}");

// ✅ GOOD: Structured, efficient
_logger.LogInformation("Processing order {OrderId}", orderId);
```

### ❌ Logging Sensitive Data

```csharp
// ❌ DANGER: Logging password!
_logger.LogDebug("Login attempt: {Username}, {Password}", username, password);

// ✅ SAFE: Don't log sensitive data
_logger.LogDebug("Login attempt: {Username}", username);
```

### ❌ Over-Logging

```csharp
// ❌ BAD: Logs in loop (millions of logs)
foreach (var item in items)
{
    _logger.LogDebug("Processing item {ItemId}", item.Id);
}

// ✅ GOOD: Log summary
_logger.LogInformation("Processing {ItemCount} items", items.Count);
```

### ❌ Wrong Log Level

```csharp
// ❌ BAD: Using Error for normal flow
if (user == null)
{
    _logger.LogError("User not found");  // ❌ Not an error!
    return NotFound();
}

// ✅ GOOD: Use Warning or Information
if (user == null)
{
    _logger.LogWarning("User {UserId} not found", userId);
    return NotFound();
}
```

---

## Detailed Guidance

Common Pitfalls guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Common Pitfalls before implementation work begins.
- Keep boundaries explicit so Common Pitfalls decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Common Pitfalls in production-facing code.
- When performance, correctness, or maintainability depends on consistent Common Pitfalls decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Common Pitfalls as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Common Pitfalls is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Common Pitfalls are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Common Pitfalls is about telemetry design for diagnostics and operations. It matters because good observability shortens detection and recovery times.
- Use it when correlating logs, traces, and metrics across service boundaries.

2-minute answer:
- Start with the problem Common Pitfalls solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: high-cardinality detail vs telemetry cost/noise.
- Close with one failure mode and mitigation: missing correlation context during incident response.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Common Pitfalls but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Common Pitfalls, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Common Pitfalls and map it to one concrete implementation in this module.
- 3 minutes: compare Common Pitfalls with an alternative, then walk through one failure mode and mitigation.