# Basic Health Checks

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

