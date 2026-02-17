# Single Responsibility Principle (SRP)

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [OOP-Principles](../README.md)

## Single Responsibility Principle (SRP)

**"A class should have one, and only one, reason to change."**

### ❌ Violation

```csharp
// ❌ BAD: Multiple responsibilities (data, validation, persistence, email)
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Name) && Email.Contains("@");
    }
    
    public void SaveToDatabase()
    {
        // Database logic here...
    }
    
    public void SendWelcomeEmail()
    {
        // Email logic here...
    }
}
// Changes to database, validation, or email logic all affect this class
```

### ✅ Solution

```csharp
// ✅ GOOD: Separate responsibilities
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class UserValidator
{
    public bool IsValid(User user)
    {
        return !string.IsNullOrEmpty(user.Name) && user.Email.Contains("@");
    }
}

public class UserRepository
{
    public void Save(User user)
    {
        // Database logic
    }
}

public class EmailService
{
    public void SendWelcomeEmail(User user)
    {
        // Email logic
    }
}
// Each class has one reason to change
```

---

## Detailed Guidance

Single Responsibility Principle (SRP) should emphasize compile-time safety, readability, and maintainable abstractions rather than clever type tricks.

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

