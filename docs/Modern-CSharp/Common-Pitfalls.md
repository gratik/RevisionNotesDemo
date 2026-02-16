# Common Pitfalls

> Subject: [Modern-CSharp](../README.md)

## Common Pitfalls

### ❌ Using Records for Mutable State

```csharp
// ❌ BAD: Fighting record semantics
public record User(string Name)
{
    public string Name { get; set; } = Name;  // ❌ Mutable record
}

// ✅ GOOD: Use class for mutable state
public class User
{
    public string Name { get; set; } = string.Empty;
}
```

### ❌ Over-Using Pattern Matching

```csharp
// ❌ BAD: Simple if is clearer
var result = value switch
{
    null => "null",
    _ => "not null"
};

// ✅ GOOD: Use simple if
var result = value == null ? "null" : "not null";
```

### ❌ Ignoring Nullable Warnings

```csharp
// ❌ BAD: Suppressing all warnings
#nullable disable
public string GetName(User user)
{
    return user.Name;  // ❌ Could be null!
}

// ✅ GOOD: Handle nulls properly
public string GetName(User? user)
{
    if (user == null)
        throw new ArgumentNullException(nameof(user));
    
    return user.Name ?? "Unknown";
}
```

---


