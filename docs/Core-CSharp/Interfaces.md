# Interfaces

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Core-CSharp](../README.md)

## Interfaces

### Interface vs Abstract Class

| Feature | Interface | Abstract Class |
|---------|-----------|----------------|
| **Multiple inheritance** | ✅ Yes | ❌ No (single) |
| **Implementation** | Default methods (C# 8+) | Yes |
| **Fields** | ❌ No | ✅ Yes |
| **Constructors** | ❌ No | ✅ Yes |
| **Access modifiers** | Public only | Any |
| **Use case** | Contract | Shared behavior |

### When to Use Each

```csharp
// ✅ Interface: Multiple implementations, no shared state
public interface IPaymentProcessor
{
    Task<PaymentResult> ProcessAsync(decimal amount);
}

public class StripeProcessor : IPaymentProcessor
{
    public Task<PaymentResult> ProcessAsync(decimal amount) { /* ... */ }
}

public class PayPalProcessor : IPaymentProcessor
{
    public Task<PaymentResult> ProcessAsync(decimal amount) { /* ... */ }
}

// ✅ Abstract class: Shared behavior and state
public abstract class PaymentProcessorBase
{
    protected readonly ILogger _logger;
    
    protected PaymentProcessorBase(ILogger logger)
    {
        _logger = logger;
    }
    
    public async Task<PaymentResult> ProcessAsync(decimal amount)
    {
        _logger.LogInformation($"Processing payment of {amount}");
        return await ProcessPaymentAsync(amount);
    }
    
    protected abstract Task<PaymentResult> ProcessPaymentAsync(decimal amount);
}
```

---

## Detailed Guidance

Interfaces should emphasize compile-time safety, readability, and maintainable abstractions rather than clever type tricks.

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
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

