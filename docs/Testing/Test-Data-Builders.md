# Test Data Builders

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: xUnit basics, mocking concepts, and API behavior expectations.
- Related examples: docs/Testing/README.md
> Subject: [Testing](../README.md)

## Test Data Builders

### The Problem

```csharp
// ❌ BAD: Repetitive test setup
[Fact]
public void Test1()
{
    var user = new User 
    { 
        Id = 1, 
        Name = "Alice", 
        Email = "alice@example.com",
        CreatedDate = DateTime.UtcNow,
        IsActive = true 
    };
}

[Fact]
public void Test2()
{
    var user = new User  // ❌ Same setup again
    { 
        Id = 2, 
        Name = "Bob", 
        Email = "bob@example.com",
        CreatedDate = DateTime.UtcNow,
        IsActive = true 
    };
}
```

### The Solution: Builder Pattern

```csharp
public class UserBuilder
{
    private int _id = 1;
    private string _name = "Test User";
    private string _email = "test@example.com";
    private bool _isActive = true;
    
    public UserBuilder WithId(int id)
    {
        _id = id;
        return this;
    }
    
    public UserBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public UserBuilder Inactive()
    {
        _isActive = false;
        return this;
    }
    
    public User Build() => new User
    {
        Id = _id,
        Name = _name,
        Email = _email,
        IsActive = _isActive
    };
}

// ✅ GOOD: Clean test setup
[Fact]
public void Test_WithBuilder()
{
    var user = new UserBuilder()
        .WithName("Alice")
        .Build();
    
    // Test with user
}

[Fact]
public void Test_InactiveUser()
{
    var user = new UserBuilder()
        .Inactive()
        .Build();
    
    // Test with inactive user
}
```

---


## Interview Answer Block
30-second answer:
- Test Data Builders is about verification strategies across unit, integration, and system levels. It matters because testing quality determines confidence in safe refactoring and releases.
- Use it when building fast feedback loops and meaningful regression safety nets.

2-minute answer:
- Start with the problem Test Data Builders solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: broader coverage vs build time and maintenance overhead.
- Close with one failure mode and mitigation: brittle tests that validate implementation details instead of behavior.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Test Data Builders but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Test Data Builders, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Test Data Builders and map it to one concrete implementation in this module.
- 3 minutes: compare Test Data Builders with an alternative, then walk through one failure mode and mitigation.