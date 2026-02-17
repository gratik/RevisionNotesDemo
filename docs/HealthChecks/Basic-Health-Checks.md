# Basic Health Checks

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: ASP.NET middleware basics and service dependency mapping.
- Related examples: docs/HealthChecks/README.md
> Subject: [HealthChecks](../README.md)

## Basic Health Checks

### Simple Health Endpoint

`csharp
// ✅ Register health checks
builder.Services.AddHealthChecks();

// ✅ Map endpoint
app.MapHealthChecks("/health");

// Returns:
// 200 OK with "Healthy" if all checks pass
// 503 Service Unavailable with "Unhealthy" if any check fails
`

### Detailed Health Report

`csharp
// ✅ Custom response with detailed information
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            duration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description,
                error = e.Value.Exception?.Message
            })
        });
        
        await context.Response.WriteAsync(result);
    }
});

// Returns JSON:
// {
//   "status": "Healthy",
//   "duration": 123.45,
//   "checks": [
//     {
//       "name": "database",
//       "status": "Healthy",
//       "duration": 45.6,
//       "description": "Database is responsive"
//     }
//   ]
// }
`

---

## Detailed Guidance

Basic Health Checks guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Basic Health Checks before implementation work begins.
- Keep boundaries explicit so Basic Health Checks decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Basic Health Checks in production-facing code.
- When performance, correctness, or maintainability depends on consistent Basic Health Checks decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Basic Health Checks as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Basic Health Checks is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Basic Health Checks are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Basic Health Checks is about service health signaling and readiness strategy. It matters because accurate health endpoints prevent bad routing and noisy incidents.
- Use it when separating liveness/readiness/dependency checks by purpose.

2-minute answer:
- Start with the problem Basic Health Checks solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: signal depth vs probe execution overhead.
- Close with one failure mode and mitigation: health checks that are either too shallow or too expensive.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Basic Health Checks but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Basic Health Checks, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Basic Health Checks and map it to one concrete implementation in this module.
- 3 minutes: compare Basic Health Checks with an alternative, then walk through one failure mode and mitigation.