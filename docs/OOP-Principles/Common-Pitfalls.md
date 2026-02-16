# Common Pitfalls

> Subject: [OOP-Principles](../README.md)

## Common Pitfalls

### ❌ Over-Abstraction

```csharp
// ❌ BAD: Too many layers
public interface IUserFactory { }
public interface IBaseUserFactory : IUserFactory { }
public interface IAdvancedUserFactory : IBaseUserFactory { }
// ... 5 more interfaces

// ✅ GOOD: Simple abstraction
public interface IUserFactory
{
    User Create(string name, string email);
}
```

### ❌ Violating SRP with "God Objects"

```csharp
// ❌ BAD: Does everything
public class OrderManager
{
    public void CreateOrder() { }
    public void ProcessPayment() { }
    public void SendEmail() { }
    public void UpdateInventory() { }
    public void GenerateInvoice() { }
    public void CreateShipment() { }
}

// ✅ GOOD: Separate concerns
public class OrderService { }
public class PaymentService { }
public class EmailService { }
public class InventoryService { }
```

### ❌ Breaking LSP with Exceptions

```csharp
// ❌ BAD: Subclass changes behavior
public class FileReader
{
    public virtual string Read(string path)
    {
        return File.ReadAllText(path);
    }
}

public class SecureFileReader : FileReader
{
    public override string Read(string path)
    {
        throw new UnauthorizedException();  // ❌ Violates LSP!
    }
}
```

---


