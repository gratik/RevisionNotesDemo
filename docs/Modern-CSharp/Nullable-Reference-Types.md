# Nullable Reference Types

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Modern-CSharp](../README.md)

## Nullable Reference Types

### Enabling Nullable Context

```xml
<!-- In .csproj -->
<PropertyGroup>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

### Non-Nullable by Default

```csharp
// ✅ Compiler helps prevent NullReferenceException
public class User
{
    public string Name { get; set; }  // ⚠️ Warning: Non-nullable property must contain non-null value
    public string? NickName { get; set; }  // ✅ Explicitly nullable
}

// ✅ Constructor ensures non-null
public class User
{
    public string Name { get; set; }
    
    public User(string name)
    {
        Name = name;  // ✅ Initialized in constructor
    }
}
```

### Null-Forgiving Operator (!)

```csharp
// ✅ Tell compiler: "I know this won't be null"
public void ProcessUser(string? name)
{
    if (name == null)
        throw new ArgumentNullException(nameof(name));
    
    // After null check, but compiler doesn't know
    DoSomething(name!);  // ✅ Suppress warning
}
```

### Nullable Annotations

```csharp
// ✅ Method returns null if not found
public User? FindUserById(int id)
{
    return _users.FirstOrDefault(u => u.Id == id);  // ✅ Returns null if not found
}

// ✅ Parameter can be null
public void Log(string? message)
{
    Console.WriteLine(message ?? "No message");
}
```

---

## Detailed Guidance

Nullable Reference Types should emphasize compile-time safety, readability, and maintainable abstractions rather than clever type tricks.

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

