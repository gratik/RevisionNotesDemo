# Best Practices

> Subject: [Advanced-CSharp](../README.md)

## Best Practices

### ✅ Reflection
- Cache Type and MemberInfo objects (expensive to retrieve)
- Use compiled expressions for hot paths
- Prefer generic constraints over reflection when possible
- Use reflection for frameworks/tools, not business logic
- Consider source generators (compile-time alternative)

### ✅ Attributes
- Keep attributes simple and data-focused
- Use AttributeUsage to control where applied
- Make attribute properties immutable when possible
- Document attribute behavior clearly
- Don't put complex logic in attributes

### ✅ Performance
- Measure impact with BenchmarkDotNet
- Cache reflection results
- Use expression trees for repeated calls
- Consider source generators for compile-time alternatives

---

## Detailed Guidance

Best Practices guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

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

