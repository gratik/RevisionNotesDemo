# Timeout Pattern

> Subject: [Resilience](../README.md)

## Timeout Pattern

```csharp
// ✅ Operation must complete within 5 seconds
var timeoutPolicy = Policy
    .TimeoutAsync(TimeSpan.FromSeconds(5));

var result = await timeoutPolicy.ExecuteAsync(async () =>
{
    return await _httpClient.GetAsync("https://slow-api.com/data");
});
```

---

## Detailed Guidance

Resilience guidance focuses on bounded degradation, dependency isolation, and measurable recovery behavior.

### Design Notes
- Define success criteria for Timeout Pattern before implementation work begins.
- Keep boundaries explicit so Timeout Pattern decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Timeout Pattern in production-facing code.
- When performance, correctness, or maintainability depends on consistent Timeout Pattern decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Timeout Pattern as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Timeout Pattern is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Timeout Pattern are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

