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


