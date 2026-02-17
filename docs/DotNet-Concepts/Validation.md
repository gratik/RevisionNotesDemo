# Validation

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Basic ASP.NET Core app structure and service registration syntax.
- Related examples: docs/DotNet-Concepts/README.md
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

## Interview Answer Block
30-second answer:
- Validation is about .NET platform and dependency injection fundamentals. It matters because these concepts determine startup wiring and runtime behavior.
- Use it when configuring robust service registration and app composition.

2-minute answer:
- Start with the problem Validation solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: centralized container control vs over-reliance on DI magic.
- Close with one failure mode and mitigation: lifetime mismatches causing subtle runtime bugs.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Validation but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Validation, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Validation and map it to one concrete implementation in this module.
- 3 minutes: compare Validation with an alternative, then walk through one failure mode and mitigation.