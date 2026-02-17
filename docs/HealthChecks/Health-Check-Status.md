# Health Check Status

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: ASP.NET middleware basics and service dependency mapping.
- Related examples: docs/HealthChecks/README.md
> Subject: [HealthChecks](../README.md)

## Health Check Status

### Three States

| Status | HTTP Code | Meaning |
|--------|-----------|---------|
| **Healthy** | 200 OK | All systems operational |
| **Degraded** | 200 OK | Operational but with issues |
| **Unhealthy** | 503 Service Unavailable | Critical failure |

`csharp
// ✅ Return appropriate status
public async Task<HealthCheckResult> CheckHealthAsync(
    HealthCheckContext context,
    CancellationToken cancellationToken = default)
{
    var responseTime = await MeasureResponseTimeAsync();
    
    if (responseTime < 100)
        return HealthCheckResult.Healthy("Fast response");
    
    if (responseTime < 500)
        return HealthCheckResult.Degraded("Slow response");
    
    return HealthCheckResult.Unhealthy("Very slow response");
}
`

---

## Detailed Guidance

Health Check Status guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Health Check Status before implementation work begins.
- Keep boundaries explicit so Health Check Status decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Health Check Status in production-facing code.
- When performance, correctness, or maintainability depends on consistent Health Check Status decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Health Check Status as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Health Check Status is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Health Check Status are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Health Check Status is about service health signaling and readiness strategy. It matters because accurate health endpoints prevent bad routing and noisy incidents.
- Use it when separating liveness/readiness/dependency checks by purpose.

2-minute answer:
- Start with the problem Health Check Status solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: signal depth vs probe execution overhead.
- Close with one failure mode and mitigation: health checks that are either too shallow or too expensive.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Health Check Status but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Health Check Status, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Health Check Status and map it to one concrete implementation in this module.
- 3 minutes: compare Health Check Status with an alternative, then walk through one failure mode and mitigation.