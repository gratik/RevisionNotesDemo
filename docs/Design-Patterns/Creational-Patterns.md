# Creational Patterns

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Object-oriented design fundamentals and refactoring familiarity.
- Related examples: docs/Design-Patterns/README.md
> Subject: [Design-Patterns](../README.md)

## Creational Patterns

**Purpose**: Control object creation mechanisms

### Singleton

**Problem**: Need exactly one instance of a class

```csharp
// ✅ Thread-safe singleton
public sealed class ConfigurationManager
{
    private static readonly Lazy<ConfigurationManager> _instance =
        new Lazy<ConfigurationManager>(() => new ConfigurationManager());
    
    public static ConfigurationManager Instance => _instance.Value;
    
    private ConfigurationManager()
    {
        // Load configuration
    }
    
    public string GetSetting(string key) => /* ... */;
}

// Usage
var config = ConfigurationManager.Instance;
```

**When to Use**: Logging, configuration, caching (but prefer DI)

### Factory Method

**Problem**: Create objects without specifying exact class

```csharp
// ✅ Factory method pattern
public abstract class PaymentProcessor
{
    public abstract IPaymentGateway CreateGateway();
    
    public PaymentResult ProcessPayment(decimal amount)
    {
        var gateway = CreateGateway();
        return gateway.Process(amount);
    }
}

public class StripeProcessor : PaymentProcessor
{
    public override IPaymentGateway CreateGateway() => new StripeGateway();
}

public class PayPalProcessor : PaymentProcessor
{
    public override IPaymentGateway CreateGateway() => new PayPalGateway();
}
```

### Builder

**Problem**: Construct complex objects step by step

```csharp
// ✅ Fluent builder
public class QueryBuilder
{
    private string _select = "*";
    private string _from = "";
    private string _where = "";
    private string _orderBy = "";
    
    public QueryBuilder Select(string columns)
    {
        _select = columns;
        return this;
    }
    
    public QueryBuilder From(string table)
    {
        _from = table;
        return this;
    }
    
    public QueryBuilder Where(string condition)
    {
        _where = condition;
        return this;
    }
    
    public QueryBuilder OrderBy(string column)
    {
        _orderBy = column;
        return this;
    }
    
    public string Build()
    {
        return $"SELECT {_select} FROM {_from} WHERE {_where} ORDER BY {_orderBy}";
    }
}

// Usage
var query = new QueryBuilder()
    .Select("Name, Email")
    .From("Users")
    .Where("IsActive = 1")
    .OrderBy("Name")
    .Build();
```

---


## Interview Answer Block
30-second answer:
- Creational Patterns is about reusable design solutions for recurring software problems. It matters because pattern choice shapes long-term extensibility and readability.
- Use it when selecting pattern structure to simplify complex behavior.

2-minute answer:
- Start with the problem Creational Patterns solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: architectural consistency vs accidental overengineering.
- Close with one failure mode and mitigation: forcing patterns where straightforward code is enough.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Creational Patterns but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Creational Patterns, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Creational Patterns and map it to one concrete implementation in this module.
- 3 minutes: compare Creational Patterns with an alternative, then walk through one failure mode and mitigation.