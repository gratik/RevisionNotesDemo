# Single Responsibility Principle (SRP)

> Subject: [OOP-Principles](../README.md)

## Single Responsibility Principle (SRP)

**"A class should have one, and only one, reason to change."**

### ❌ Violation

```csharp
// ❌ BAD: Multiple responsibilities (data, validation, persistence, email)
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Name) && Email.Contains("@");
    }
    
    public void SaveToDatabase()
    {
        // Database logic here...
    }
    
    public void SendWelcomeEmail()
    {
        // Email logic here...
    }
}
// Changes to database, validation, or email logic all affect this class
```

### ✅ Solution

```csharp
// ✅ GOOD: Separate responsibilities
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class UserValidator
{
    public bool IsValid(User user)
    {
        return !string.IsNullOrEmpty(user.Name) && user.Email.Contains("@");
    }
}

public class UserRepository
{
    public void Save(User user)
    {
        // Database logic
    }
}

public class EmailService
{
    public void SendWelcomeEmail(User user)
    {
        // Email logic
    }
}
// Each class has one reason to change
```

---


