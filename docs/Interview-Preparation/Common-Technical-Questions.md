# Common Technical Questions

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Comfort with core module topics and deliberate timed practice.
- Related examples: docs/Interview-Preparation/README.md
> Subject: [Interview-Preparation](../README.md)

## Common Technical Questions

### C# and .NET Fundamentals

**Q1: What is the difference between IEnumerable and IQueryable?**

**Answer**:

- **IEnumerable**: In-memory collection, LINQ to Objects
  - Executes queries locally in C# code
  - Fetches all data first, then filters
  - Use for: In-memory collections, List<T>, arrays
- **IQueryable**: Expression trees, deferred execution
  - Converts LINQ to SQL/other query languages
  - Filters data at the source (database)
  - Use for: Database queries with EF Core

**Example**:

```csharp
// ❌ IEnumerable - bad for large datasets
IEnumerable<User> users = dbContext.Users;  // Fetches ALL users
var adults = users.Where(u => u.Age >= 18); // Filters in memory

// ✅ IQueryable - filters in database
IQueryable<User> users = dbContext.Users;
var adults = users.Where(u => u.Age >= 18).ToList(); // SQL: WHERE Age >= 18
```

---

**Q2: When would you use async/await?**

**Answer**: Use async/await for **I/O-bound operations** to avoid blocking threads:

- Database queries
- HTTP requests
- File I/O
- Network operations

**Don't use** for CPU-bound work (use Task.Run instead).

**Example**:

```csharp
// ✅ Good: I/O-bound
public async Task<User> GetUserAsync(int id)
{
    return await dbContext.Users.FindAsync(id);  // Don't block thread while waiting
}

// ❌ Bad: CPU-bound
public async Task<int> CalculateFactorial(int n)
{
    return await Task.Run(() => Factorial(n));  // ✅ This is actually correct for CPU work
}
```

---

**Q3: Explain SOLID principles in one sentence each.**

**Answer**:

- **S** - Single Responsibility: One class, one reason to change
- **O** - Open/Closed: Open for extension, closed for modification
- **L** - Liskov Substitution: Subtypes must be substitutable for base types
- **I** - Interface Segregation: Many specific interfaces > one general interface
- **D** - Dependency Inversion: Depend on abstractions, not concrete implementations

---

**Q4: What is dependency injection and why use it?**

**Answer**:
DI inverts control by **providing dependencies** to a class rather than having it create them.

**Benefits**:

- ✅ Testability (inject mocks)
- ✅ Loose coupling
- ✅ Single source of configuration
- ✅ Easier to swap implementations

**Example**:

```csharp
// ❌ Bad: Hard to test, tightly coupled
public class OrderService
{
    private readonly EmailService _emailService = new EmailService();  // ❌ Creates dependency
}

// ✅ Good: Dependency injected
public class OrderService
{
    private readonly IEmailService _emailService;

    public OrderService(IEmailService emailService)  // ✅ Injected
    {
        _emailService = emailService;
    }
}
```

---

**Q5: What are the DI service lifetimes?**

**Answer**:

- **Singleton**: One instance for entire application lifetime
  - Use for: Stateless services, configurations
  - Example: Logging, caching
- **Scoped**: One instance per request/scope
  - Use for: DbContext, request-specific services
  - Example: Database context, unit of work
- **Transient**: New instance every time
  - Use for: Lightweight, stateless services
  - Example: Validators, utilities

**Pitfall**: Don't inject scoped service into singleton (captive dependency).

---

**Q6: How do you prevent memory leaks in .NET?**

**Answer**:

1. **Dispose unmanaged resources** (IDisposable, using statements)
2. **Unsubscribe from events** (or use weak references)
3. **Avoid long-lived references** to short-lived objects
4. **Clear collections** when no longer needed
5. **Don't hold references in static fields** unnecessarily

**Example**:

```csharp
// ❌ Memory leak: Event subscription
public class Subscriber
{
    public Subscriber(Publisher publisher)
    {
        publisher.SomeEvent += OnEvent;  // ❌ Publisher holds reference forever
    }

    private void OnEvent(object sender, EventArgs e) { }
}

// ✅ Fixed: Unsubscribe
public class Subscriber : IDisposable
{
    private readonly Publisher _publisher;

    public Subscriber(Publisher publisher)
    {
        _publisher = publisher;
        _publisher.SomeEvent += OnEvent;
    }

    public void Dispose()
    {
        _publisher.SomeEvent -= OnEvent;  // ✅ Clean up
    }

    private void OnEvent(object sender, EventArgs e) { }
}
```

---

**Q7: What is a circuit breaker?**

**Answer**:
Resilience pattern that **stops calling a failing service** to:

- Prevent cascading failures
- Allow service time to recover
- Fail fast instead of waiting for timeout

**States**:

- **Closed**: Normal operation
- **Open**: Stop calling service (fail immediately)
- **Half-Open**: Test if service recovered

**Use Polly library** for implementation.

---

**Q8: When should you use records?**

**Answer**:
Use records for **immutable, value-based data**:

- DTOs (Data Transfer Objects)
- API requests/responses
- Value objects in domain models
- Messages in event-driven systems

**Benefits**:

- Immutability by default
- Value-based equality
- Concise syntax (no boilerplate)
- Deconstruction support

**Example**:

```csharp
// ✅ Perfect for record
public record UserDto(int Id, string Name, string Email);

// ❌ Don't use record for
public record UserEntity  // ❌ Entity with behavior and identity
{
    public int Id { get; set; }
    public void UpdateEmail(string email) { }  // ❌ Mutable behavior
}
```

---

**Q9: How do you handle exceptions in Web APIs?**

**Answer**:
Use **centralized exception handling middleware** with **consistent ProblemDetails** responses.

**Example**:

```csharp
// Global exception handler
app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Title = "An error occurred",
            Status = 500,
            Detail = "Contact support if issue persists"
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});
```

**Don't**:

- ❌ Expose stack traces in production
- ❌ Return generic "error" strings
- ❌ Use different error formats per endpoint

---

**Q10: Why is logging with templates preferred?**

**Answer**:
Structured logging with **templates preserves field names** for:

- Filtering by specific values
- Aggregation and analytics
- Better performance (pre-parsed template)

**Example**:

```csharp
// ❌ Bad: String interpolation loses structure
_logger.LogInformation($"User {userId} logged in at {DateTime.Now}");

// ✅ Good: Template preserves fields
_logger.LogInformation("User {UserId} logged in at {LoginTime}", userId, DateTime.Now);
// Result: Can query all logins for specific UserId
```

---

### Advanced Topics

**Q11: Explain async/await internals**

**Answer**:

- C# compiler transforms async method into **state machine**
- **await** captures current context (SynchronizationContext)
- Task represents **eventual completion** of async operation
- **ConfigureAwait(false)** avoids capturing context (library code)

**Key Points**:

- Async != parallel (no new threads for I/O)
- await is suspension point (thread returns to pool)
- ValueTask for hot path optimization

---

**Q12: How does garbage collection work?**

**Answer**:
**.NET uses generational GC**:

- **Gen 0**: Short-lived objects (collected frequently)
- **Gen 1**: Mid-term objects (buffer between 0 and 2)
- **Gen 2**: Long-lived objects (collected rarely)

**Process**:

1. Mark: Find live objects (reachable from roots)
2. Sweep: Remove dead objects
3. Compact: Move objects together (reduce fragmentation)

**Types**:

- Workstation GC (client apps)
- Server GC (high-throughput, multi-core)

---

**Q13: What are the differences between ref, out, and in?**

**Answer**:

- **ref**: Pass by reference, must be initialized before call
- **out**: Pass by reference, must be assigned before method returns
- **in**: Read-only reference (performance optimization for large structs)

```csharp
void Method1(ref int value) { value++; }  // Requires initialization
void Method2(out int value) { value = 42; }  // Must assign in method
void Method3(in LargeStruct value) { }  // Read-only, no copy
```

---

**Q14: Explain middleware pipeline in ASP.NET Core**

**Answer**:
Middleware = **components** that process HTTP requests and responses in a **pipeline**.

**Key Concepts**:

- **Order matters** (authentication before authorization)
- **next()** calls next middleware
- **Run()** terminates pipeline (no next)
- **Use()** can call next or short-circuit

**Example Order**:

```csharp
app.UseExceptionHandler();    // 1. Catch errors
app.UseHttpsRedirection();    // 2. Redirect to HTTPS
app.UseRouting();             // 3. Route matching
app.UseAuthentication();      // 4. WHO are you?
app.UseAuthorization();       // 5. WHAT can you do?
app.MapControllers();         // 6. Execute endpoint
```

---

**Q15: What is the difference between Task and ValueTask?**

**Answer**:

- **Task**: Reference type, heap allocation
  - Use for: Most async methods
  - Overhead: Small allocation per task
- **ValueTask**: Value type, can avoid allocation
  - Use for: Hot path, frequently called methods that may complete synchronously
  - Caveat: Can only await once, more complex to use

**Example**:

```csharp
// ✅ Task - general use
public async Task<User> GetUserAsync(int id)
{
    return await _repository.GetAsync(id);
}

// ✅ ValueTask - cached result, hot path
private readonly Dictionary<int, User> _cache = new();

public async ValueTask<User> GetUserAsync(int id)
{
    if (_cache.TryGetValue(id, out var user))
        return user;  // ✅ Returns synchronously, no allocation

    user = await _repository.GetAsync(id);
    _cache[id] = user;
    return user;
}
```

---


## Interview Answer Block
30-second answer:
- Common Technical Questions is about communication structure for technical interviews. It matters because clear articulation of tradeoffs improves interview signal quality.
- Use it when translating implementation knowledge into concise answers.

2-minute answer:
- Start with the problem Common Technical Questions solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: brevity vs sufficient technical depth.
- Close with one failure mode and mitigation: memorized answers that ignore problem context.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Common Technical Questions but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Common Technical Questions, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Common Technical Questions and map it to one concrete implementation in this module.
- 3 minutes: compare Common Technical Questions with an alternative, then walk through one failure mode and mitigation.