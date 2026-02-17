# Scoped Services in Singletons (Captive Dependency)

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Basic ASP.NET Core app structure and service registration syntax.
- Related examples: docs/DotNet-Concepts/README.md
> Subject: [DotNet-Concepts](../README.md)

## Scoped Services in Singletons (Captive Dependency)

### The Problem

`csharp
// ❌ BAD: Scoped service injected into Singleton
public class MySingletonService  // Singleton
{
    private readonly AppDbContext _context;  // ❌ Scoped! Will be held forever
    
    public MySingletonService(AppDbContext context)
    {
        _context = context;  // ❌ DbContext never disposed
    }
}
// Memory leak! DbContext should be disposed after each request
`

### The Solution

`csharp
// ✅ GOOD: Use IServiceScopeFactory
public class MySingletonService
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public MySingletonService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    
    public async Task DoWorkAsync()
    {
        // ✅ Create scope, get scoped service, dispose scope
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        await context.SaveChangesAsync();
    }  // ✅ Scope disposed, DbContext disposed
}
`

---

## Detailed Guidance

Scoped Services in Singletons (Captive Dependency) guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Scoped Services in Singletons (Captive Dependency) before implementation work begins.
- Keep boundaries explicit so Scoped Services in Singletons (Captive Dependency) decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Scoped Services in Singletons (Captive Dependency) in production-facing code.
- When performance, correctness, or maintainability depends on consistent Scoped Services in Singletons (Captive Dependency) decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Scoped Services in Singletons (Captive Dependency) as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Scoped Services in Singletons (Captive Dependency) is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Scoped Services in Singletons (Captive Dependency) are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Scoped Services in Singletons (Captive Dependency) is about .NET platform and dependency injection fundamentals. It matters because these concepts determine startup wiring and runtime behavior.
- Use it when configuring robust service registration and app composition.

2-minute answer:
- Start with the problem Scoped Services in Singletons (Captive Dependency) solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: centralized container control vs over-reliance on DI magic.
- Close with one failure mode and mitigation: lifetime mismatches causing subtle runtime bugs.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Scoped Services in Singletons (Captive Dependency) but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Scoped Services in Singletons (Captive Dependency), what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Scoped Services in Singletons (Captive Dependency) and map it to one concrete implementation in this module.
- 3 minutes: compare Scoped Services in Singletons (Captive Dependency) with an alternative, then walk through one failure mode and mitigation.