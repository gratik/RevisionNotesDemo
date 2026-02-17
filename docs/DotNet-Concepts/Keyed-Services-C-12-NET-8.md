# Keyed Services (C# 12 / .NET 8+)

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Basic ASP.NET Core app structure and service registration syntax.
- Related examples: docs/DotNet-Concepts/README.md
> Subject: [DotNet-Concepts](../README.md)

## Keyed Services (C# 12 / .NET 8+)

### Multiple Implementations

`csharp
// ✅ Register with keys
builder.Services.AddKeyedScoped<IPaymentService, StripePaymentService>("stripe");
builder.Services.AddKeyedScoped<IPaymentService, PayPalPaymentService>("paypal");

// ✅ Inject specific implementation by key
public class CheckoutService
{
    private readonly IPaymentService _paymentService;
    
    public CheckoutService([FromKeyedServices("stripe")] IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
}
`

---

## Detailed Guidance

Keyed Services (C# 12 / .NET 8+) should emphasize compile-time safety, readability, and maintainable abstractions rather than clever type tricks.

### Design Notes
- Use language features to enforce intent at compile time (constraints, nullability, variance).
- Keep APIs narrow and intention-revealing; avoid generic over-engineering.
- Prefer composition and small interfaces over deep inheritance chains.
- Document where performance optimizations justify additional complexity.

### When To Use
- When building reusable libraries or framework-facing APIs.
- When replacing runtime casts/dynamic code with typed contracts.
- When teaching or reviewing core language design tradeoffs.

### Anti-Patterns To Avoid
- Public APIs with too many type parameters and unclear semantics.
- Constraints that do not correspond to required operations.
- Using reflection/dynamic where static typing is sufficient.

## Practical Example

- Start with a concrete implementation and extract generic behavior only when duplication appears.
- Add minimal constraints needed for compile-time guarantees.
- Validate with tests across reference and value type scenarios.

## Validation Checklist

- API signatures are understandable without deep internal context.
- Nullability and constraints match true invariants.
- Type misuse fails at compile time where possible.
- Benchmarks exist for any non-trivial performance optimizations.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Keyed Services (C# 12 / .NET 8+) is about .NET platform and dependency injection fundamentals. It matters because these concepts determine startup wiring and runtime behavior.
- Use it when configuring robust service registration and app composition.

2-minute answer:
- Start with the problem Keyed Services (C# 12 / .NET 8+) solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: centralized container control vs over-reliance on DI magic.
- Close with one failure mode and mitigation: lifetime mismatches causing subtle runtime bugs.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Keyed Services (C# 12 / .NET 8+) but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Keyed Services (C# 12 / .NET 8+), what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Keyed Services (C# 12 / .NET 8+) and map it to one concrete implementation in this module.
- 3 minutes: compare Keyed Services (C# 12 / .NET 8+) with an alternative, then walk through one failure mode and mitigation.