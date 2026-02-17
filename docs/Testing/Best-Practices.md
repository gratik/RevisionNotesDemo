# Best Practices

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: xUnit basics, mocking concepts, and API behavior expectations.
- Related examples: docs/Testing/README.md
> Subject: [Testing](../README.md)

## Best Practices

### ✅ Test Naming
```
MethodName_Scenario_ExpectedBehavior

Examples:
- Add_TwoPositiveNumbers_ReturnsSum
- GetUser_UserNotFound_ReturnsNull
- CreateOrder_InvalidData_ThrowsValidationException
```

### ✅ One Assert Per Test
```csharp
// ❌ BAD: Multiple asserts (which failed?)
[Fact]
public void Test_Bad()
{
    Assert.Equal(5, result.Count);
    Assert.True(result.All(x => x.IsActive));
    Assert.Equal("Alice", result.First().Name);
}

// ✅ GOOD: One logical assertion
[Fact]
public void Test_Good()
{
    Assert.Equal(5, result.Count);
}

[Fact]
public void Test_AllActive()
{
    Assert.All(result, x => Assert.True(x.IsActive));
}
```

### ✅ Test Independence
```csharp
// ❌ BAD: Tests depend on each other
private static int _counter = 0;

[Fact]
public void Test1() { _counter++; }  // ❌ Shared state

[Fact]
public void Test2() { Assert.Equal(1, _counter); }  // ❌ Depends on Test1

// ✅ GOOD: Each test is independent
[Fact]
public void Test1() 
{ 
    int counter = 0;
    counter++;
    Assert.Equal(1, counter);
}
```

### ✅ Fast Tests
- Avoid real databases (use in-memory)
- Avoid real HTTP calls (use mocks)
- Avoid Thread.Sleep (use Task.CompletedTask)
- Keep tests under 100ms

---

## Detailed Guidance

Testing guidance focuses on behavior confidence, failure-path coverage, and maintainable test architecture.

### Design Notes
- Define success criteria for Best Practices before implementation work begins.
- Keep boundaries explicit so Best Practices decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Best Practices in production-facing code.
- When performance, correctness, or maintainability depends on consistent Best Practices decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Best Practices as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Best Practices is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Best Practices are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Best Practices is about verification strategies across unit, integration, and system levels. It matters because testing quality determines confidence in safe refactoring and releases.
- Use it when building fast feedback loops and meaningful regression safety nets.

2-minute answer:
- Start with the problem Best Practices solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: broader coverage vs build time and maintenance overhead.
- Close with one failure mode and mitigation: brittle tests that validate implementation details instead of behavior.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Best Practices but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Best Practices, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Best Practices and map it to one concrete implementation in this module.
- 3 minutes: compare Best Practices with an alternative, then walk through one failure mode and mitigation.