# Combining Policies

> Subject: [Resilience](../README.md)

## Combining Policies

### Wrap Policies Together

```csharp
// ✅ Retry + Circuit Breaker + Timeout
var timeout = Policy.TimeoutAsync(TimeSpan.FromSeconds(5));
var retry = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(attempt));
var circuitBreaker = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

// Combine: innermost policy executes first
var combined = Policy.WrapAsync(retry, circuitBreaker, timeout);

var result = await combined.ExecuteAsync(async () =>
{
    return await _httpClient.GetAsync(url);
});
```

### PolicyWrap Order Matters

```
Order: Retry → Circuit Breaker → Timeout

1. Timeout: Ensures operation completes within time
2. Circuit Breaker: Stops calling if service is down
3. Retry: Retries transient failures
```

---

## Detailed Guidance

Resilience guidance focuses on bounded degradation, dependency isolation, and measurable recovery behavior.

### Design Notes
- Define success criteria for Combining Policies before implementation work begins.
- Keep boundaries explicit so Combining Policies decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Combining Policies in production-facing code.
- When performance, correctness, or maintainability depends on consistent Combining Policies decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Combining Policies as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Combining Policies is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Combining Policies are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

