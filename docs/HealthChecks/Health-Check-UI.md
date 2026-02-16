# Health Check UI

> Subject: [HealthChecks](../README.md)

## Health Check UI

### Adding UI Dashboard

`csharp
// ✅ Install: AspNetCore.HealthChecks.UI

builder.Services.AddHealthChecksUI(settings =>
{
    settings.AddHealthCheckEndpoint("API Health", "/health");
    settings.SetEvaluationTimeInSeconds(10);  // Check every 10 seconds
    settings.MaximumHistoryEntriesPerEndpoint(50);
})
.AddInMemoryStorage();

// ✅ Add UI middleware
app.UseHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";  // Dashboard at /health-ui
});

// Access dashboard: https://localhost:5001/health-ui
`

---

## Detailed Guidance

UI integration guidance focuses on boundary contracts, predictable state flow, and release-safe cross-layer changes.

### Design Notes
- Define success criteria for Health Check UI before implementation work begins.
- Keep boundaries explicit so Health Check UI decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Health Check UI in production-facing code.
- When performance, correctness, or maintainability depends on consistent Health Check UI decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Health Check UI as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Health Check UI is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Health Check UI are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

