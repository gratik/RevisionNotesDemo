# Other Important Principles

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Classes/interfaces, dependency inversion basics, and unit testing fundamentals.
- Related examples: docs/OOP-Principles/README.md
> Subject: [OOP-Principles](../README.md)

## Other Important Principles

### KISS (Keep It Simple, Stupid)

```csharp
// ❌ BAD: Overengineered
public class StringManipulator
{
    public string ReverseString(string input)
    {
        return new string(input.ToCharArray().Reverse().ToArray());
    }
}

// ✅ GOOD: Simple and clear
public static string Reverse(string input)
{
    char[] chars = input.ToCharArray();
    Array.Reverse(chars);
    return new string(chars);
}
```

### DRY (Don't Repeat Yourself)

```csharp
// ❌ BAD: Duplication
public void SaveUser(User user)
{
    if (string.IsNullOrEmpty(user.Name))
        throw new ArgumentException("Name required");
    // Save...
}

public void UpdateUser(User user)
{
    if (string.IsNullOrEmpty(user.Name))  // ❌ Duplicated validation
        throw new ArgumentException("Name required");
    // Update...
}

// ✅ GOOD: Extract common logic
private void ValidateUser(User user)
{
    if (string.IsNullOrEmpty(user.Name))
        throw new ArgumentException("Name required");
}

public void SaveUser(User user)
{
    ValidateUser(user);
    // Save...
}

public void UpdateUser(User user)
{
    ValidateUser(user);
    // Update...
}
```

### YAGNI (You Aren't Gonna Need It)

```csharp
// ❌ BAD: Speculative features
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }  // ❌ Not needed yet
    public Address ShippingAddress { get; set; }  // ❌ Not needed yet
    public Address BillingAddress { get; set; }  // ❌ Not needed yet
}

// ✅ GOOD: Only what's needed now
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    // Add other properties when actually needed
}
```

### Tell-Don't-Ask

```csharp
// ❌ BAD: Asking for data and making decisions
public void ProcessOrder(Order order)
{
    if (order.Status == OrderStatus.Pending)
    {
        order.Status = OrderStatus.Confirmed;
        order.ConfirmedAt = DateTime.UtcNow;
    }
}

// ✅ GOOD: Tell the object what to do
public class Order
{
    public OrderStatus Status { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    
    public void Confirm()
    {
        if (Status == OrderStatus.Pending)
        {
            Status = OrderStatus.Confirmed;
            ConfirmedAt = DateTime.UtcNow;
        }
    }
}

// Usage
order.Confirm();  // ✅ Encapsulated behavior
```

---


## Interview Answer Block
30-second answer:
- Other Important Principles is about object-oriented design boundaries and responsibilities. It matters because good boundaries reduce coupling and improve testability.
- Use it when designing services and entities with clear responsibilities.

2-minute answer:
- Start with the problem Other Important Principles solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: extensibility vs added abstraction layers.
- Close with one failure mode and mitigation: applying principles mechanically without considering domain context.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Other Important Principles but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Other Important Principles, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Other Important Principles and map it to one concrete implementation in this module.
- 3 minutes: compare Other Important Principles with an alternative, then walk through one failure mode and mitigation.