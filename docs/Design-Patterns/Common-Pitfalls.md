# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Object-oriented design fundamentals and refactoring familiarity.
- Related examples: docs/Design-Patterns/README.md
> Subject: [Design-Patterns](../README.md)

## Common Pitfalls

### ❌ Pattern Overuse

```csharp
// ❌ BAD: Over-engineered simple logic
public class AdditionStrategyFactory
{
    public ICalculationStrategy CreateStrategy()
    {
        return new AdditionStrategy();
    }
}

// ✅ GOOD: Simple is better
public int Add(int a, int b) => a + b;
```

### ❌ Singleton Abuse

```csharp
// ❌ BAD: Everything is singleton
public class UserService  // Singleton
public class OrderService  // Singleton
public class PaymentService  // Singleton

// ✅ GOOD: Use dependency injection
services.AddScoped<IUserService, UserService>();
services.AddScoped<IOrderService, OrderService>();
```

---

## Detailed Guidance

Common Pitfalls guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

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
- Common Pitfalls is about reusable design solutions for recurring software problems. It matters because pattern choice shapes long-term extensibility and readability.
- Use it when selecting pattern structure to simplify complex behavior.

2-minute answer:
- Start with the problem Common Pitfalls solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: architectural consistency vs accidental overengineering.
- Close with one failure mode and mitigation: forcing patterns where straightforward code is enough.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Common Pitfalls but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Common Pitfalls, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Common Pitfalls and map it to one concrete implementation in this module.
- 3 minutes: compare Common Pitfalls with an alternative, then walk through one failure mode and mitigation.