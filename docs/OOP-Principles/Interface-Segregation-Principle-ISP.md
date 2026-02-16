# Interface Segregation Principle (ISP)

> Subject: [OOP-Principles](../README.md)

## Interface Segregation Principle (ISP)

**"No client should be forced to depend on methods it does not use."**

### ❌ Violation

```csharp
// ❌ BAD: Fat interface forces clients to implement unused methods
public interface IWorker
{
    void Work();
    void Eat();
    void Sleep();
}

public class Robot : IWorker
{
    public void Work() { /* ... */ }
    public void Eat() { throw new NotImplementedException(); }  // ❌ Doesn't eat!
    public void Sleep() { throw new NotImplementedException(); }  // ❌ Doesn't sleep!
}
```

### ✅ Solution

```csharp
// ✅ GOOD: Segregated interfaces
public interface IWorkable
{
    void Work();
}

public interface IFeedable
{
    void Eat();
}

public interface ISleepable
{
    void Sleep();
}

public class Human : IWorkable, IFeedable, ISleepable
{
    public void Work() { /* ... */ }
    public void Eat() { /* ... */ }
    public void Sleep() { /* ... */ }
}

public class Robot : IWorkable
{
    public void Work() { /* ... */ }
    // Only implements what it needs
}
```

---


