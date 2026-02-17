# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: xUnit basics, mocking concepts, and API behavior expectations.
- Related examples: docs/Testing/README.md
> Subject: [Testing](../README.md)

## Common Pitfalls

### ❌ Not Testing Edge Cases
```csharp
// ❌ Only tests happy path
[Fact]
public void Divide_ValidNumbers_ReturnsQuotient()
{
    Assert.Equal(5, calculator.Divide(10, 2));
}

// ✅ Tests edge cases
[Theory]
[InlineData(10, 0)]  // Division by zero
[InlineData(int.MaxValue, 1)]  // Large numbers
[InlineData(-10, 2)]  // Negative numbers
public void Divide_EdgeCases_HandlesCorrectly(int a, int b)
{
    // Test appropriate behavior
}
```

### ❌ Testing Implementation Details
```csharp
// ❌ BAD: Testing private methods
[Fact]
public void TestPrivateMethod()
{
    var result = (int)typeof(Calculator)
        .GetMethod("PrivateAdd", BindingFlags.NonPublic)
        .Invoke(calculator, new object[] { 2, 3 });
}

// ✅ GOOD: Test public behavior
[Fact]
public void Add_TwoNumbers_ReturnsSum()
{
    Assert.Equal(5, calculator.Add(2, 3));
}
```

---

## Detailed Guidance

Testing guidance focuses on behavior confidence, failure-path coverage, and maintainable test architecture.

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
- Common Pitfalls is about verification strategies across unit, integration, and system levels. It matters because testing quality determines confidence in safe refactoring and releases.
- Use it when building fast feedback loops and meaningful regression safety nets.

2-minute answer:
- Start with the problem Common Pitfalls solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: broader coverage vs build time and maintenance overhead.
- Close with one failure mode and mitigation: brittle tests that validate implementation details instead of behavior.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Common Pitfalls but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Common Pitfalls, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Common Pitfalls and map it to one concrete implementation in this module.
- 3 minutes: compare Common Pitfalls with an alternative, then walk through one failure mode and mitigation.