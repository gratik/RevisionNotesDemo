# Other Modern Features

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Core C# concepts, nullable awareness, and common refactoring patterns.
- Related examples: docs/Modern-CSharp/README.md
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


## Interview Answer Block
30-second answer:
- Other Modern Features is about newer C# language capabilities. It matters because modern syntax reduces boilerplate and improves intent clarity.
- Use it when updating legacy code to safer and more expressive patterns.

2-minute answer:
- Start with the problem Other Modern Features solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: new language features vs team familiarity and consistency.
- Close with one failure mode and mitigation: mixing old and new idioms inconsistently across the same codebase.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Other Modern Features but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Other Modern Features, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Other Modern Features and map it to one concrete implementation in this module.
- 3 minutes: compare Other Modern Features with an alternative, then walk through one failure mode and mitigation.