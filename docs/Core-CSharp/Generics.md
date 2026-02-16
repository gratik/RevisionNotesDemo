# Generics

> Subject: [Core-CSharp](../README.md)

## Generics

### Why Generics?

**Without Generics (Boxing/Unboxing)**:
```csharp
// ❌ BAD: Boxing, no type safety
ArrayList list = new ArrayList();
list.Add(1);       // Boxes int to object
list.Add("two");   // No compile-time error!
int value = (int)list[0];  // Unboxing, runtime cast
```

**With Generics**:
```csharp
// ✅ GOOD: Type-safe, no boxing
List<int> list = new List<int>();
list.Add(1);
list.Add("two");  // ❌ Compile-time error!
int value = list[0];  // No cast needed
```

### Generic Classes

```csharp
// ✅ Generic repository pattern
public class Repository<T> where T : class
{
    private readonly List<T> _items = new();
    
    public void Add(T item) => _items.Add(item);
    
    public T? GetById(int id)
    {
        // Implementation
        return default;
    }
    
    public IEnumerable<T> GetAll() => _items;
}

// Usage
var userRepo = new Repository<User>();
var productRepo = new Repository<Product>();
```

### Generic Constraints

```csharp
// ✅ where T : class (reference type)
public class ReferenceTypeRepo<T> where T : class
{
    public T? GetOrDefault() => default;  // Can be null
}

// ✅ where T : struct (value type)
public class ValueTypeStorage<T> where T : struct
{
    public T GetOrDefault() => default;  // Never null
}

// ✅ where T : new() (parameterless constructor)
public class Factory<T> where T : new()
{
    public T Create() => new T();
}

// ✅ where T : IComparable (interface)
public class Sorter<T> where T : IComparable<T>
{
    public T Max(T a, T b) => a.CompareTo(b) > 0 ? a : b;
}

// ✅ Multiple constraints
public class EntityRepository<T> 
    where T : Entity, IValidatable, new()
{
    public T Create()
    {
        var entity = new T();
        if (!entity.IsValid())
            throw new InvalidOperationException();
        return entity;
    }
}
```

---

## Detailed Guidance

Generics are a type-system tool for building reusable code without giving up compile-time safety. The core value is not just performance; it is expressing intent in APIs so invalid usage fails at compile time instead of at runtime.

### Design Notes
- Prefer generics when behavior is truly type-agnostic and repeated across multiple types.
- Add constraints only when required by behavior. A good rule: every constraint should justify one operation in the body.
- Return interface abstractions (`IReadOnlyList<T>`, `IEnumerable<T>`) when callers do not need mutation.
- Keep generic public APIs simple; complexity compounds quickly with multi-parameter generic types.

### Generic Methods vs Generic Types

Use a generic method when only one operation needs type flexibility:

```csharp
public static T FirstOrDefaultSafe<T>(IEnumerable<T> items) =>
    items.FirstOrDefault();
```

Use a generic type when the object stores or coordinates typed state:

```csharp
public sealed class InMemoryCache<TKey, TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _store = new();
    public void Set(TKey key, TValue value) => _store[key] = value;
    public bool TryGet(TKey key, out TValue value) => _store.TryGetValue(key, out value!);
}
```

### Variance (Why `IEnumerable<string>` Can Be `IEnumerable<object>`)

- `out` (covariant): safe for producers.
- `in` (contravariant): safe for consumers.

```csharp
IEnumerable<string> names = new[] { "A", "B" };
IEnumerable<object> objects = names; // Covariance (out T)

Action<object> logObject = o => Console.WriteLine(o);
Action<string> logString = logObject; // Contravariance (in T)
```

Variance works only on interfaces/delegates that are designed for it. Concrete classes like `List<T>` are invariant.

### Runtime Behavior (Important)

- For value types, generics avoid boxing in typed collections (`List<int>`, `Dictionary<int, ...>`).
- The JIT generates type-specific code paths, which can improve performance but may increase code size for many closed generic instantiations.
- Generic constraints do not replace business validation; they enforce type capabilities, not domain rules.

### When To Use
- Reusable algorithms/utilities that must work for many types.
- Strongly typed collections, cache/repository abstractions, and pipelines.
- APIs where compile-time misuse should be impossible or difficult.

### Anti-Patterns To Avoid
- Over-generic APIs (`Processor<T1, T2, T3...>`) with unclear intent.
- Using `where T : class` only because “that’s what we always do”.
- Replacing clear domain types with unconstrained generic objects.
- Building reflection-heavy generic frameworks when simple typed code is sufficient.

## Practical Example

Bad:
- A method accepts `object`, uses casts, and fails at runtime for invalid inputs.

Better:
- Convert method to `TInput`/`TOutput` generic form, add minimal constraints, and fail early at compile time.

Best:
- Keep generic boundary narrow, expose simple typed contracts, and test representative type combinations.

## Validation Checklist

- Constraints are minimal and justified by code behavior.
- Public API is readable without deep generic type gymnastics.
- Tests include reference/value type inputs and nullability edge cases.
- No unnecessary casting/reflection is required by normal callers.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

