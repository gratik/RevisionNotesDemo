# Common Pitfalls

> Subject: [Core-CSharp](../README.md)

## Common Pitfalls

### ❌ Not Unsubscribing from Events

```csharp
// ❌ BAD: Memory leak!
public class Subscriber
{
    public Subscriber(Publisher publisher)
    {
        publisher.SomeEvent += OnSomeEvent;  // ❌ Never unsubscribes
    }
    
    private void OnSomeEvent(object? sender, EventArgs e) { }
}

// ✅ GOOD: Unsubscribe
public class Subscriber : IDisposable
{
    private readonly Publisher _publisher;
    
    public Subscriber(Publisher publisher)
    {
        _publisher = publisher;
        _publisher.SomeEvent += OnSomeEvent;
    }
    
    public void Dispose()
    {
        _publisher.SomeEvent -= OnSomeEvent;  // ✅ Unsubscribe
    }
    
    private void OnSomeEvent(object? sender, EventArgs e) { }
}
```

### ❌ Overusing Extension Methods

```csharp
// ❌ BAD: Should be instance method
public static class UserExtensions
{
    public static void SetPassword(this User user, string password)
    {
        user.PasswordHash = HashPassword(password);  // ❌ Should be in User class
    }
}

// ✅ GOOD: Instance method
public class User
{
    public void SetPassword(string password)
    {
        PasswordHash = HashPassword(password);  // ✅ Encapsulated
    }
}
```

---


