# Extension Methods

> Subject: [Core-CSharp](../README.md)

## Extension Methods

### Basic Extension Methods

```csharp
// ✅ Extend string without modifying the class
public static class StringExtensions
{
    public static bool IsBlank(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }
    
    public static string Truncate(this string value, int maxLength)
    {
        if (value.Length <= maxLength) return value;
        return value.Substring(0, maxLength) + "...";
    }
}

// Usage
string name = "  ";
if (name.IsBlank())  // ✅ Reads like an instance method
{
    Console.WriteLine("Name is blank");
}

string longText = "This is a very long text";
string short = longText.Truncate(10);  // "This is a ..."
```

### Extension Methods on Generics

```csharp
// ✅ Extend IEnumerable<T>
public static class EnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> source)
    {
        return !source.Any();
    }
    
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        where T : class
    {
        return source.Where(x => x != null)!;
    }
}

// Usage
var numbers = new List<int>();
if (numbers.IsEmpty())  // ✅ More readable than !numbers.Any()
{
    Console.WriteLine("List is empty");
}
```

---

## Detailed Guidance

Extension method guidance focuses on improving discoverability and reuse without obscuring core domain behavior.

### Design Notes
- Define success criteria for Extension Methods before implementation work begins.
- Keep boundaries explicit so Extension Methods decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Extension Methods in production-facing code.
- When performance, correctness, or maintainability depends on consistent Extension Methods decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Extension Methods as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Extension Methods is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Extension Methods are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

