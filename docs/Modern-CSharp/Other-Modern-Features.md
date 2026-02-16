# Other Modern Features

> Subject: [Modern-CSharp](../README.md)

## Other Modern Features

### Top-Level Statements (C# 9)

```csharp
// ✅ No need for Program class and Main method
using System;

Console.WriteLine("Hello, World!");
var result = AddNumbers(3, 5);
Console.WriteLine(result);

int AddNumbers(int a, int b) => a + b;
```

### Global Usings (C# 10)

```csharp
// GlobalUsings.cs
global using System;
global using System.Collections.Generic;
global using System.Linq;

// Now available in all files without explicit using
```

### File-Scoped Namespaces (C# 10)

```csharp
// ❌ Old style: extra indentation
namespace MyApp.Services
{
    public class UserService
    {
        // ...
    }
}

// ✅ New style: less indentation
namespace MyApp.Services;

public class UserService
{
    // ...
}
```

### Required Members (C# 11)

```csharp
// ✅ Compiler ensures property is set
public class User
{
    public required string Name { get; init; }
    public string? Email { get; init; }
}

// Usage
var user1 = new User { Name = "Alice" };  // ✅ OK
var user2 = new User { };  // ❌ Compile error: Name is required
```

### Raw String Literals (C# 11)

```csharp
// ✅ No need to escape quotes
var json = """
    {
        "name": "Alice",
        "email": "alice@example.com"
    }
    """;

var query = """
    SELECT * FROM Users
    WHERE Name = 'Alice'
    """;
```

---


