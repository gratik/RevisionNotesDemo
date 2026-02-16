# Init-Only Properties

> Subject: [Modern-CSharp](../README.md)

## Init-Only Properties

```csharp
// ✅ Set during object initialization only
public class User
{
    public int Id { get; init; }  // ✅ Can only set during initialization
    public string Name { get; init; } = string.Empty;
}

// Usage
var user = new User
{
    Id = 1,
    Name = "Alice"
};

user.Id = 2;  // ❌ Compile error: init-only property
```

---


