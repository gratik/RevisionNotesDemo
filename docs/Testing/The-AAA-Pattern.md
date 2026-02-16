# The AAA Pattern

> Subject: [Testing](../README.md)

## The AAA Pattern

**Arrange, Act, Assert** - the standard test structure.

```csharp
[Fact]
public void Add_TwoPositiveNumbers_ReturnsSum()
{
    // Arrange: Set up test data and dependencies
    var calculator = new Calculator();
    int a = 5;
    int b = 3;
    
    // Act: Execute the method being tested
    var result = calculator.Add(a, b);
    
    // Assert: Verify the result
    Assert.Equal(8, result);
}
```

**Why AAA?**
- Clear test structure
- Easy to read and maintain
- Separates test concerns
- Industry standard

---

## Detailed Guidance

Testing guidance focuses on behavior confidence, failure-path coverage, and maintainable test architecture.

### Design Notes
- Define success criteria for The AAA Pattern before implementation work begins.
- Keep boundaries explicit so The AAA Pattern decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring The AAA Pattern in production-facing code.
- When performance, correctness, or maintainability depends on consistent The AAA Pattern decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying The AAA Pattern as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where The AAA Pattern is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for The AAA Pattern are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

