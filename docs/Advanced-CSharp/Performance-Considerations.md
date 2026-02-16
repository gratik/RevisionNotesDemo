# Performance Considerations

> Subject: [Advanced-CSharp](../README.md)

## Performance Considerations

### Reflection is Slow

`csharp
// ❌ BAD: Reflection in hot path
for (int i = 0; i < 1000000; i++)
{
    PropertyInfo? prop = typeof(User).GetProperty("Name");
    prop?.SetValue(user, "Alice");  // ❌ Very slow!
}

// ✅ GOOD: Cache PropertyInfo
PropertyInfo? prop = typeof(User).GetProperty("Name");
for (int i = 0; i < 1000000; i++)
{
    prop?.SetValue(user, "Alice");  // ✅ Better
}

// ✅ BETTER: Use compiled expressions
var setter = CreateSetter<User, string>("Name");
for (int i = 0; i < 1000000; i++)
{
    setter(user, "Alice");  // ✅ Fast (near-native)
}
`

### Compiled Expressions for Performance

`csharp
// ✅ Create fast property setter using expressions
public static Action<T, TValue> CreateSetter<T, TValue>(string propertyName)
{
    var param1 = Expression.Parameter(typeof(T));
    var param2 = Expression.Parameter(typeof(TValue));
    var property = Expression.Property(param1, propertyName);
    var assign = Expression.Assign(property, param2);
    
    return Expression.Lambda<Action<T, TValue>>(assign, param1, param2).Compile();
}

// ✅ Create fast property getter
public static Func<T, TValue> CreateGetter<T, TValue>(string propertyName)
{
    var param = Expression.Parameter(typeof(T));
    var property = Expression.Property(param, propertyName);
    
    return Expression.Lambda<Func<T, TValue>>(property, param).Compile();
}
`

---

## Detailed Guidance

Performance guidance focuses on bottleneck-first optimization supported by representative measurements and guardrails.

### Design Notes
- Define success criteria for Performance Considerations before implementation work begins.
- Keep boundaries explicit so Performance Considerations decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Performance Considerations in production-facing code.
- When performance, correctness, or maintainability depends on consistent Performance Considerations decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Performance Considerations as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Performance Considerations is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Performance Considerations are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

