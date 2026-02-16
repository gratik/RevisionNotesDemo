# Validation

> Subject: [DotNet-Concepts](../README.md)

## Validation

### Validate Services on Startup

`csharp
// ✅ Validate DI configuration on startup (Development only)
if (app.Environment.IsDevelopment())
{
    var serviceProvider = app.Services;
    
    // Throws exception if any service can't be resolved
    using var scope = serviceProvider.CreateScope();
    
    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
}

// ✅ Better: Use ValidateOnBuild()
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IEmailService, EmailService>();
// ... register all services

if (builder.Environment.IsDevelopment())
{
    builder.Host.UseDefaultServiceProvider(options =>
    {
        options.ValidateScopes = true;  // ✅ Detect scope issues
        options.ValidateOnBuild = true;  // ✅ Validate on startup
    });
}
`

---

## Detailed Guidance

Validation guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Validation before implementation work begins.
- Keep boundaries explicit so Validation decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Validation in production-facing code.
- When performance, correctness, or maintainability depends on consistent Validation decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Validation as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Validation is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Validation are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

