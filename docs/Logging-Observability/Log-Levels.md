# Log Levels

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Logging-Observability](../README.md)

## Log Levels

| Level | Value | When to Use | Examples |
|-------|-------|-------------|----------|
| **Trace** | 0 | Detailed diagnostic info | Method entry/exit, variable values |
| **Debug** | 1 | Developer debugging | Query parameters, cache hits |
| **Information** | 2 | General flow | Request started, order processed |
| **Warning** | 3 | Unexpected but handled | Retry attempted, fallback used |
| **Error** | 4 | Failure in operation | Exception caught, operation failed |
| **Critical** | 5 | Application crash | Unrecoverable error, data corruption |

### Choosing the Right Level

```csharp
// ✅ Trace: Very detailed (disabled in production)
_logger.LogTrace("Entering GetUser method with userId={UserId}", userId);

// ✅ Debug: Helpful for debugging (disabled in production)
_logger.LogDebug("Cache hit for key {CacheKey}", key);

// ✅ Information: General flow (enabled in production)
_logger.LogInformation("User {UserId} logged in successfully", userId);

// ✅ Warning: Something unexpected but handled
_logger.LogWarning("API rate limit reached, using fallback data");

// ✅ Error: Operation failed
_logger.LogError(ex, "Failed to save order {OrderId}", orderId);

// ✅ Critical: System is unusable
_logger.LogCritical(ex, "Database connection pool exhausted");
```

---

## Detailed Guidance

Log Levels guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Log Levels before implementation work begins.
- Keep boundaries explicit so Log Levels decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Log Levels in production-facing code.
- When performance, correctness, or maintainability depends on consistent Log Levels decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Log Levels as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Log Levels is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Log Levels are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

