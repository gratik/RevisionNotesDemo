# Nullable Reference Types

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


