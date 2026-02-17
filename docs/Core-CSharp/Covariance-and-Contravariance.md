# Covariance and Contravariance

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Core C# syntax, object-oriented fundamentals, and basic collection usage.
- Related examples: docs/Core-CSharp/README.md
> Subject: [Core-CSharp](../README.md)

## Covariance and Contravariance

### Covariance (out)

```csharp
// ✅ Covariant interface (output only)
public interface IProducer<out T>
{
    T Produce();
}

public class Animal { }
public class Dog : Animal { }

public class DogProducer : IProducer<Dog>
{
    public Dog Produce() => new Dog();
}

// ✅ Covariance allows this
IProducer<Dog> dogProducer = new DogProducer();
IProducer<Animal> animalProducer = dogProducer;  // ✅ OK!
```

### Contravariance (in)

```csharp
// ✅ Contravariant interface (input only)
public interface IConsumer<in T>
{
    void Consume(T item);
}

public class AnimalConsumer : IConsumer<Animal>
{
    public void Consume(Animal animal) { /* ... */ }
}

// ✅ Contravariance allows this
IConsumer<Animal> animalConsumer = new AnimalConsumer();
IConsumer<Dog> dogConsumer = animalConsumer;  // ✅ OK!
```

---

## Detailed Guidance

Covariance and Contravariance should emphasize compile-time safety, readability, and maintainable abstractions rather than clever type tricks.

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
- Covariance and Contravariance is about core C# language features and API design. It matters because it directly affects correctness, readability, and maintainability.
- Use it when designing reusable domain and application abstractions.

2-minute answer:
- Start with the problem Covariance and Contravariance solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: flexibility vs API complexity.
- Close with one failure mode and mitigation: over-abstracting simple code paths; keep public contracts intentional.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Covariance and Contravariance but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Covariance and Contravariance, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Covariance and Contravariance and map it to one concrete implementation in this module.
- 3 minutes: compare Covariance and Contravariance with an alternative, then walk through one failure mode and mitigation.