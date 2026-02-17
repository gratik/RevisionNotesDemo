# Service Location (Anti-Pattern)

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Basic ASP.NET Core app structure and service registration syntax.
- Related examples: docs/DotNet-Concepts/README.md
> Subject: [DotNet-Concepts](../README.md)

## Service Location (Anti-Pattern)

### Don't Use Service Locator

`csharp
// ❌ BAD: Service locator pattern (anti-pattern)
public class OrderService
{
    private readonly IServiceProvider _serviceProvider;
    
    public OrderService(IServiceProvider serviceProvider)  // ❌ Don't do this
    {
        _serviceProvider = serviceProvider;
    }
    
    public void ProcessOrder()
    {
        var emailService = _serviceProvider.GetService<IEmailService>();  // ❌ Hidden dependency
    }
}

// ✅ GOOD: Explicit constructor injection
public class OrderService
{
    private readonly IEmailService _emailService;
    
    public OrderService(IEmailService emailService)  // ✅ Clear dependency
    {
        _emailService = emailService;
    }
}
`

**Exception**: Acceptable in factories or when dynamically resolving services

---

## Detailed Guidance

Service Location (Anti-Pattern) guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Service Location (Anti-Pattern) before implementation work begins.
- Keep boundaries explicit so Service Location (Anti-Pattern) decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Service Location (Anti-Pattern) in production-facing code.
- When performance, correctness, or maintainability depends on consistent Service Location (Anti-Pattern) decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Service Location (Anti-Pattern) as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Service Location (Anti-Pattern) is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Service Location (Anti-Pattern) are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Service Location (Anti-Pattern) is about .NET platform and dependency injection fundamentals. It matters because these concepts determine startup wiring and runtime behavior.
- Use it when configuring robust service registration and app composition.

2-minute answer:
- Start with the problem Service Location (Anti-Pattern) solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: centralized container control vs over-reliance on DI magic.
- Close with one failure mode and mitigation: lifetime mismatches causing subtle runtime bugs.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Service Location (Anti-Pattern) but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Service Location (Anti-Pattern), what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Service Location (Anti-Pattern) and map it to one concrete implementation in this module.
- 3 minutes: compare Service Location (Anti-Pattern) with an alternative, then walk through one failure mode and mitigation.