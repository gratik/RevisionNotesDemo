# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Distributed call patterns, timeout semantics, and transient fault basics.
- Related examples: docs/Resilience/README.md
> Subject: [Resilience](../README.md)

## Common Pitfalls

### ❌ Retrying Non-Transient Errors

```csharp
// ❌ BAD: Retry 404 Not Found
var policy = Policy
    .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
    .RetryAsync(3);

// ✅ GOOD: Only retry transient errors
var policy = Policy
    .HandleResult<HttpResponseMessage>(r =>
        r.StatusCode == HttpStatusCode.RequestTimeout ||
        r.StatusCode == HttpStatusCode.ServiceUnavailable)
    .RetryAsync(3);
```

### ❌ No Exponential Backoff

```csharp
// ❌ BAD: Fixed delay hammers failing service
var policy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(10, _ => TimeSpan.FromSeconds(1));

// ✅ GOOD: Exponential backoff gives service time
var policy = Policy
    .Handle<HttpRequestException>()
    .WaitAndRetryAsync(5, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
```

---

## Detailed Guidance

Resilience guidance focuses on bounded degradation, dependency isolation, and measurable recovery behavior.

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
- Common Pitfalls is about fault handling and recovery design. It matters because resilience patterns preserve service quality during failures.
- Use it when protecting dependencies with retries, circuit breakers, and timeouts.

2-minute answer:
- Start with the problem Common Pitfalls solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: improved availability vs added control-flow complexity.
- Close with one failure mode and mitigation: stacking policies without understanding interaction effects.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Common Pitfalls but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Common Pitfalls, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Common Pitfalls and map it to one concrete implementation in this module.
- 3 minutes: compare Common Pitfalls with an alternative, then walk through one failure mode and mitigation.