# Test Data Builders

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


