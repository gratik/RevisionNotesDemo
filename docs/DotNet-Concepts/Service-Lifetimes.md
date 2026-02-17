# Service Lifetimes

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [DotNet-Concepts](../README.md)

## Service Lifetimes

### Three Lifetimes

| Lifetime | Created | Disposed | Use Case |
|----------|---------|----------|----------|
| **Transient** | Every time requested | After use | Stateless, lightweight services |
| **Scoped** | Once per request/scope | End of request/scope | DbContext, per-request state |
| **Singleton** | Once per application | Application shutdown | Stateless, thread-safe, expensive to create |

### Transient

`csharp
// ✅ Register as Transient
builder.Services.AddTransient<IEmailService, EmailService>();

// New instance EVERY TIME
public class Controller1
{
    public Controller1(IEmailService emailService) { }  // Instance A
}

public class Controller2
{
    public Controller2(IEmailService emailService) { }  // Instance B (different)
}

// Even in same class
public class MyService
{
    public MyService(IEmailService email1, IEmailService email2)
    {
        // email1 and email2 are DIFFERENT instances
    }
}
`

**When to Use**: Lightweight, stateless services, no shared state

### Scoped

`csharp
// ✅ Register as Scoped
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddDbContext<AppDbContext>();  // Scoped by default

// One instance PER REQUEST
public class Controller1
{
    public Controller1(IOrderService orderService) { }  // Instance A for this request
}

public class Controller2
{
    public Controller2(IOrderService orderService) { }  // Same instance A (same request)
}

// Next request gets new instance
// Request 1: Instance A
// Request 2: Instance B
`

**When to Use**: DbContext, per-request state, database transactions

### Singleton

`csharp
// ✅ Register as Singleton
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

// One instance FOR ENTIRE APPLICATION
public class Controller1
{
    public Controller1(ICacheService cache) { }  // Instance A
}

public class Controller2
{
    public Controller2(ICacheService cache) { }  // Same instance A
}

// All requests, all classes = same instance
`

**When to Use**: Configuration, caching, logging, stateless utilities

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

