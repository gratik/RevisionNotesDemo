# Circuit Breaker Pattern

> Subject: [Resilience](../README.md)

## Circuit Breaker Pattern

### The Three States

**CLOSED** (Normal): Requests flow through, failures counted
**OPEN** (Tripped): Requests fail immediately, no calls made  
**HALF-OPEN** (Testing): Allow one request to test recovery

### Basic Circuit Breaker

```csharp
var circuitBreakerPolicy = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(
        exceptionsAllowedBeforeBreaking: 3,  // Break after 3 failures
        durationOfBreak: TimeSpan.FromSeconds(30));  // Stay open 30s

// Flow:
// Attempt 1: Fails → Count = 1 (CLOSED)
// Attempt 2: Fails → Count = 2 (CLOSED)
// Attempt 3: Fails → Circuit OPENS
// Attempt 4+: BrokenCircuitException (immediate)
// After 30s: Circuit moves to HALF-OPEN
// Next: Success → CLOSED | Failure → OPEN again
```

### Advanced Circuit Breaker

```csharp
// ✅ Based on failure rate, not consecutive failures
var circuitBreakerPolicy = Policy
    .Handle<HttpRequestException>()
    .AdvancedCircuitBreakerAsync(
        failureThreshold: 0.5,  // 50% failure rate
        samplingDuration: TimeSpan.FromSeconds(10),  // Over 10s window
        minimumThroughput: 10,  // Minrequests before evaluating
        durationOfBreak: TimeSpan.FromSeconds(30));
```

---

## Detailed Guidance

Resilience guidance focuses on bounded degradation, dependency isolation, and measurable recovery behavior.

### Design Notes
- Define success criteria for Circuit Breaker Pattern before implementation work begins.
- Keep boundaries explicit so Circuit Breaker Pattern decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Circuit Breaker Pattern in production-facing code.
- When performance, correctness, or maintainability depends on consistent Circuit Breaker Pattern decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Circuit Breaker Pattern as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Circuit Breaker Pattern is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Circuit Breaker Pattern are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

