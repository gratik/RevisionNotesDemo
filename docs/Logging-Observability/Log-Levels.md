# Log Levels

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

