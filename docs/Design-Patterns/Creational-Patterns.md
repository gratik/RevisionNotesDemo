# Creational Patterns

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


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
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

