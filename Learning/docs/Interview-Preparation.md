# Interview Preparation Guide

**Last Updated**: 2026-02-15

Comprehensive interview preparation covering common questions, coding challenges, system design,
behavioral questions, and strategic advice for .NET developer positions.

---

## How to Use This Guide

**Preparation Timeline**:

- **1 week before**: Review all common questions
- **3 days before**: Practice coding challenges
- **1 day before**: Review behavioral answers and company research
- **Day of**: Review quick reference tables

**Study Strategy**:

1. Read question
2. Try to answer (write it down)
3. Compare with provided answer
4. Identify gaps in knowledge
5. Study related documentation

---

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

## Quick Reference Tables

### Do's and Don'ts

| Topic            | ✅ Do                               | ❌ Avoid                          |
| ---------------- | ----------------------------------- | --------------------------------- |
| **Async I/O**    | Use async/await end-to-end          | Blocking with .Result or .Wait()  |
| **CPU Work**     | Use Task.Run for expensive work     | Marking CPU methods as async      |
| **Logging**      | Use structured templates            | String interpolation in hot paths |
| **EF Core**      | Use AsNoTracking for reads          | Tracking every query by default   |
| **Caching**      | Set expirations and size limits     | Unbounded cache growth            |
| **DI Lifetimes** | Scoped for request services         | Singleton for per-request state   |
| **Exceptions**   | Use specific exception types        | Catching Exception (too broad)    |
| **Strings**      | Use StringBuilder for concatenation | += in loops                       |
| **Collections**  | Use appropriate collection type     | List<T> for everything            |
| **Nullability**  | Enable nullable reference types     | Ignoring null warnings            |

---

## Patterns: When to Use vs Overused

### Singleton Pattern

**Overused because**: Hidden global state, hard to test, lifetime coupling

**Prefer instead**: Use DI with `AddSingleton` and pass dependencies explicitly

**Still valid when**: Truly global infrastructure with stable lifecycle (logging, configuration)

---

### Abstract Factory Pattern

**Overused because**: Too many layers for simple object creation

**Prefer instead**: Use DI, configuration, or simple factory methods

**Still valid when**: Multiple product families must stay consistent

---

### Service Locator Pattern

**Overused because**: Hidden dependencies, runtime failures

**Prefer instead**: Constructor injection with explicit dependencies

**Still valid when**: Legacy frameworks where DI is not possible

---

### Repository Pattern (for every entity)

**Overused because**: Extra abstraction when EF Core DbSet already acts as repository

**Prefer instead**: Use DbContext directly with query-focused services

**Still valid when**: Complex domain rules or multiple data sources

---

## Coding Challenges

### Challenge 1: Reverse a String

```csharp
// Multiple approaches

// 1. Using Array.Reverse
public string ReverseString1(string input)
{
    char[] chars = input.ToCharArray();
    Array.Reverse(chars);
    return new string(chars);
}

// 2. Using LINQ
public string ReverseString2(string input)
{
    return new string(input.Reverse().ToArray());
}

// 3. Manual (best for interviews - shows logic)
public string ReverseString3(string input)
{
    char[] chars = input.ToCharArray();
    int left = 0, right = chars.Length - 1;

    while (left < right)
    {
        // Swap
        (chars[left], chars[right]) = (chars[right], chars[left]);
        left++;
        right--;
    }

    return new string(chars);
}

// Time: O(n), Space: O(n)
```

---

### Challenge 2: Find Duplicates in Array

```csharp
// Return all duplicate values

public List<int> FindDuplicates(int[] nums)
{
    var seen = new HashSet<int>();
    var duplicates = new HashSet<int>();

    foreach (var num in nums)
    {
        if (!seen.Add(num))  // ✅ Add returns false if already exists
        {
            duplicates.Add(num);
        }
    }

    return duplicates.ToList();
}

// Time: O(n), Space: O(n)

// Alternative: Using LINQ
public List<int> FindDuplicatesLinq(int[] nums)
{
    return nums.GroupBy(x => x)
               .Where(g => g.Count() > 1)
               .Select(g => g.Key)
               .ToList();
}
```

---

### Challenge 3: FizzBuzz

```csharp
public List<string> FizzBuzz(int n)
{
    var result = new List<string>();

    for (int i = 1; i <= n; i++)
    {
        if (i % 15 == 0)           // ✅ Divisible by both 3 and 5
            result.Add("FizzBuzz");
        else if (i % 3 == 0)
            result.Add("Fizz");
        else if (i % 5 == 0)
            result.Add("Buzz");
        else
            result.Add(i.ToString());
    }

    return result;
}

// Time: O(n), Space: O(n)
```

---

### Challenge 4: Two Sum

```csharp
// Find indices of two numbers that add up to target

public int[] TwoSum(int[] nums, int target)
{
    var map = new Dictionary<int, int>();  // Value -> Index

    for (int i = 0; i < nums.Length; i++)
    {
        int complement = target - nums[i];

        if (map.ContainsKey(complement))
        {
            return new[] { map[complement], i };
        }

        map[nums[i]] = i;
    }

    return Array.Empty<int>();
}

// Time: O(n), Space: O(n)

// Example:
// Input: nums = [2, 7, 11, 15], target = 9
// Output: [0, 1]  (nums[0] + nums[1] = 2 + 7 = 9)
```

---

### Challenge 5: Validate Balanced Parentheses

```csharp
public bool IsValid(string s)
{
    var stack = new Stack<char>();
    var pairs = new Dictionary<char, char>
    {
        { ')', '(' },
        { '}', '{' },
        { ']', '[' }
    };

    foreach (char c in s)
    {
        if (pairs.ContainsValue(c))  // Opening bracket
        {
            stack.Push(c);
        }
        else if (pairs.ContainsKey(c))  // Closing bracket
        {
            if (stack.Count == 0 || stack.Pop() != pairs[c])
                return false;
        }
    }

    return stack.Count == 0;
}

// Time: O(n), Space: O(n)

// Examples:
// "()" → true
// "()[]{}" → true
// "(]" → false
// "([)]" → false
```

---

## System Design Questions

### Design a URL Shortener

**Requirements**:

- Shorten long URLs to short codes
- Redirect short → long URL
- Analytics (click count)
- Expiration

**High-Level Design**:

```
User → API Gateway → URL Service → Database (Redis + SQL)
                          ↓
                     Cache Layer (Redis)
```

**Key Components**:

1. **URL Generation Service**
   - Base62 encoding (a-z, A-Z, 0-9)
   - 7 characters = 62^7 = 3.5 trillion URLs
2. **Database Schema**:

   ```sql
   CREATE TABLE Urls (
       Id BIGINT PRIMARY KEY,
       ShortCode VARCHAR(10) UNIQUE,
       LongUrl VARCHAR(2048),
       CreatedAt DATETIME,
       ExpiresAt DATETIME,
       ClickCount INT
   );

   INDEX on ShortCode for fast lookup
   ```

3. **Redis Cache**:
   - Key: short code
   - Value: long URL
   - TTL: 24 hours

4. **API Endpoints**:
   ```
   POST /api/shorten → Create short URL
   GET /{shortCode} → Redirect to long URL
   GET /api/stats/{shortCode} → Get analytics
   ```

**Scaling**:

- Read-heavy → Cache with Redis
- Write-heavy → Database sharding by hash(shortCode)
- CDN for static content
- Load balancer for multiple API instances

---

### Design a Rate Limiter

**Requirements**:

- Limit API calls per user
- Different limits per tier (free/premium)
- Track requests across distributed servers

**Algorithms**:

1. **Token Bucket** (Recommended):
   - Bucket with fixed capacity
   - Tokens added at fixed rate
   - Request consumes token
   - If no tokens → reject

2. **Sliding Window Log**:
   - Track timestamp of each request
   - Count requests in last N seconds

**Implementation with Redis**:

```csharp
public class RateLimiter
{
    private readonly IDatabase _redis;

    public async Task<bool> IsAllowedAsync(string userId, int maxRequests, TimeSpan window)
    {
        string key = $"rate_limit:{userId}";
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long windowStart = now - (long)window.TotalSeconds;

        // Remove old entries
        await _redis.SortedSetRemoveRangeByScoreAsync(key, 0, windowStart);

        // Count requests in window
        long count = await _redis.SortedSetLengthAsync(key);

        if (count < maxRequests)
        {
            // Add current request
            await _redis.SortedSetAddAsync(key, now, now);
            await _redis.KeyExpireAsync(key, window);
            return true;
        }

        return false;  // Rate limit exceeded
    }
}
```

---

## Behavioral Questions

### STAR Method Framework

**S**ituation: Context and background
**T**ask: Challenge or responsibility
**A**ction: Steps you took
**R**esult: Outcome and learnings

---

### Common Behavioral Questions

**Q: Tell me about a time you faced a difficult bug**

**Example Answer**:

- **Situation**: Production issue - API response time increased from 200ms to 5s
- **Task**: Identify root cause and fix without downtime
- **Action**:
  1. Checked monitoring dashboards (APM)
  2. Found N+1 query problem in EF Core
  3. Added `.Include()` for eager loading
  4. Deployed fix with feature flag
- **Result**: Response time back to 200ms, learned importance of query analysis

---

**Q: Describe a time you had to learn something quickly**

**Example Answer**:

- **Situation**: Project required SignalR for real-time features, had 1 week
- **Task**: Learn SignalR and implement chat feature
- **Action**:
  1. Read official docs
  2. Built prototype
  3. Code review with senior dev
  4. Implemented production feature
- **Result**: Delivered on time, became team's SignalR expert

---

**Q: Tell me about a time you disagreed with a team member**

**Example Answer**:

- **Situation**: Teammate wanted to use Repository pattern for all entities
- **Task**: Discuss trade-offs and reach consensus
- **Action**:
  1. Presented research on when Repository adds value
  2. Showed EF Core DbSet already provides repository functionality
  3. Proposed using it only for complex domain logic
- **Result**: Team agreed, reduced unnecessary abstraction

---

## Interview Day Strategy

### Before the Interview

- [ ] Review company's tech stack
- [ ] Prepare 3-5 questions to ask interviewer
- [ ] Review your own projects/resume
- [ ] Practice coding on whiteboard/paper
- [ ] Get good sleep

### During Technical Questions

1. **Clarify requirements**: Ask about edge cases, constraints
2. **Think out loud**: Share your reasoning
3. **Start simple**: Get basic solution working first
4. **Optimize**: Discuss time/space complexity improvements
5. **Test**: Walk through example inputs

### Red Flags to Avoid

- ❌ Saying "I don't know" without attempting
- ❌ Jumping to code without clarifying
- ❌ Getting defensive about feedback
- ❌ Not asking questions
- ❌ Bad-mouthing previous employers

### Green Flags to Show

- ✅ Asking clarifying questions
- ✅ Discussing trade-offs
- ✅ Mentioning testing
- ✅ Considering scalability
- ✅ Being open to feedback

---

## Questions to Ask Interviewer

### About the Role

1. What does a typical day look like?
2. What's the team structure?
3. How do you measure success in this role?
4. What are the biggest challenges the team is facing?

### About Technology

1. What's the current tech stack?
2. What's the deployment process?
3. How do you handle technical debt?
4. What's the code review process?

### About Culture

1. How does the team handle disagreements?
2. What's the work-life balance like?
3. How are learning and growth supported?
4. What's the team's approach to remote work?

### About Growth

1. What are the career paths from this role?
2. How often are performance reviews?
3. What does success look like in the first 90 days?
4. Are there mentorship opportunities?

---

## Salary Negotiation Tips

### Research First

- Check Glassdoor, Levels.fyi, Payscale
- Know your market value
- Consider total compensation (benefits, equity, bonus)

### When Asked "What's your expected salary?"

**Strategy 1**: "I'm flexible and open to discussing the entire compensation package. What's the budgeted range for this role?"

**Strategy 2**: "Based on my research and experience, I'm targeting $X to $Y. Is that within the range for this position?"

### Negotiation Tips

- ✅ Ask for time to review offer
- ✅ Negotiate total package (not just salary)
- ✅ Be prepared to justify your ask
- ✅ Get it in writing
- ❌ Don't accept first offer immediately
- ❌ Don't make ultimatums

---

## Study Resources

### Official Documentation

- Microsoft Learn: https://learn.microsoft.com/training/
- .NET Documentation: https://learn.microsoft.com/dotnet/
- C# Programming Guide: https://learn.microsoft.com/dotnet/csharp/

### Practice Platforms

- LeetCode: Algorithm practice
- HackerRank: Coding challenges
- Codewars: Kata exercises
- Exercism: Mentored learning

### Books

- _C# in Depth_ by Jon Skeet
- _CLR via C#_ by Jeffrey Richter
- _Designing Data-Intensive Applications_ by Martin Kleppmann

---

## Final Checklist

### Week Before Interview

- [ ] Review common questions in this guide
- [ ] Practice 5-10 coding challenges
- [ ] Review your projects and be ready to discuss
- [ ] Research the company thoroughly
- [ ] Prepare questions to ask

### Day Before Interview

- [ ] Review quick reference tables
- [ ] Practice introducing yourself
- [ ] Review STAR method examples
- [ ] Prepare your workspace (if remote)
- [ ] Test camera/mic (if remote)

### Interview Day

- [ ] Arrive/login 10 minutes early
- [ ] Have water nearby
- [ ] Have pen and paper for notes
- [ ] Turn off notifications
- [ ] Relax and be yourself!

---

## Related Documentation

- [Core C#](Core-CSharp.md) - Fundamentals review
- [Modern C#](Modern-CSharp.md) - Latest C# features
- [Design Patterns](Design-Patterns.md) - Common interview patterns
- [SOLID Principles](OOP-Principles.md) - OOP interview topics
- [Data Access](Data-Access.md) - EF Core and SQL questions
- [Async Multithreading](Async-Multithreading.md) - Async/await deep dive
- [Performance](Performance.md) - Optimization questions
- [Security](Security.md) - Security best practices
- [Testing](Testing.md) - Testing strategies

---

## Remember

**You're not expected to know everything!**

Key qualities interviewers look for:

- Problem-solving approach
- Communication skills
- Willingness to learn
- Collaboration mindset
- Passion for technology

**Good luck! You've got this!** 🚀

---

Generated: 2026-02-14
