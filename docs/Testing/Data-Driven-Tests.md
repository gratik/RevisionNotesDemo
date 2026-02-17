# Data-Driven Tests

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: xUnit basics, mocking concepts, and API behavior expectations.
- Related examples: docs/Testing/README.md
> Subject: [Testing](../README.md)

## Data-Driven Tests

### MemberData (Complex Data)

```csharp
public class CalculatorTests
{
    public static IEnumerable<object[]> AddTestData =>
        new List<object[]>
        {
            new object[] { 2, 3, 5 },
            new object[] { -1, 1, 0 },
            new object[] { 0, 0, 0 },
            new object[] { int.MaxValue, 0, int.MaxValue }
        };
    
    [Theory]
    [MemberData(nameof(AddTestData))]
    public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
    {
        var calculator = new Calculator();
        Assert.Equal(expected, calculator.Add(a, b));
    }
}
```

### ClassData (Reusable Data)

```csharp
public class AddTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 2, 3, 5 };
        yield return new object[] { -1, 1, 0 };
        yield return new object[] { 0, 0, 0 };
    }
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

[Theory]
[ClassData(typeof(AddTestData))]
public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
{
    var calculator = new Calculator();
    Assert.Equal(expected, calculator.Add(a, b));
}
```

---

## Detailed Guidance

Testing guidance focuses on behavior confidence, failure-path coverage, and maintainable test architecture.

### Design Notes
- Define success criteria for Data-Driven Tests before implementation work begins.
- Keep boundaries explicit so Data-Driven Tests decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Data-Driven Tests in production-facing code.
- When performance, correctness, or maintainability depends on consistent Data-Driven Tests decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Data-Driven Tests as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Data-Driven Tests is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Data-Driven Tests are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Data-Driven Tests is about verification strategies across unit, integration, and system levels. It matters because testing quality determines confidence in safe refactoring and releases.
- Use it when building fast feedback loops and meaningful regression safety nets.

2-minute answer:
- Start with the problem Data-Driven Tests solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: broader coverage vs build time and maintenance overhead.
- Close with one failure mode and mitigation: brittle tests that validate implementation details instead of behavior.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Data-Driven Tests but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Data-Driven Tests, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Data-Driven Tests and map it to one concrete implementation in this module.
- 3 minutes: compare Data-Driven Tests with an alternative, then walk through one failure mode and mitigation.