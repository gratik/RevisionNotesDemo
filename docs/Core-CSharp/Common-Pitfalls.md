# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Core-CSharp](../README.md)

## Common Pitfalls

### ❌ Not Unsubscribing from Events

```csharp
// ❌ BAD: Memory leak!
public class Subscriber
{
    public Subscriber(Publisher publisher)
    {
        publisher.SomeEvent += OnSomeEvent;  // ❌ Never unsubscribes
    }
    
    private void OnSomeEvent(object? sender, EventArgs e) { }
}

// ✅ GOOD: Unsubscribe
public class Subscriber : IDisposable
{
    private readonly Publisher _publisher;
    
    public Subscriber(Publisher publisher)
    {
        _publisher = publisher;
        _publisher.SomeEvent += OnSomeEvent;
    }
    
    public void Dispose()
    {
        _publisher.SomeEvent -= OnSomeEvent;  // ✅ Unsubscribe
    }
    
    private void OnSomeEvent(object? sender, EventArgs e) { }
}
```

### ❌ Overusing Extension Methods

```csharp
// ❌ BAD: Should be instance method
public static class UserExtensions
{
    public static void SetPassword(this User user, string password)
    {
        user.PasswordHash = HashPassword(password);  // ❌ Should be in User class
    }
}

// ✅ GOOD: Instance method
public class User
{
    public void SetPassword(string password)
    {
        PasswordHash = HashPassword(password);  // ✅ Encapsulated
    }
}
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

