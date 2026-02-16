# Other Important Principles

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


