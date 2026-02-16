# Unit Testing Frameworks and Patterns

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Unit testing fundamentals
- Related examples: Learning/Testing/TestingFrameworksComparison.cs, Learning/Testing/MockingInDepthExamples.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../README.md)

## Module Metadata

- **Prerequisites**: Core C#, OOP Principles
- **When to Study**: Start early and revisit continuously during implementation.
- **Related Files**: `../Learning/Testing/**/*.cs`
- **Estimated Time**: 120-150 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](Learning-Path.md) | [Track Start](Configuration.md)
- **Next Step**: [Practical-Patterns.md](Practical-Patterns.md)
<!-- STUDY-NAV-END -->


## Overview

This guide covers unit testing best practices using xUnit, NUnit, and MSTest. It includes mocking patterns,
async testing, data-driven tests, integration testing, and test data builders. Good tests are fast, isolated,
repeatable, and easy to understand.

---

## Testing Frameworks Comparison

| Feature | xUnit | NUnit | MSTest |
|---------|-------|-------|--------|
| **Philosophy** | Modern, opinionated | Feature-rich, flexible | VS integrated |
| **Test Method** | `[Fact]` | `[Test]` | `[TestMethod]` |
| **Parameterized** | `[Theory]` + `[InlineData]` | `[TestCase]` | `[DataTestMethod]` + `[DataRow]` |
| **Setup** | Constructor | `[SetUp]` | `[TestInitialize]` |
| **Teardown** | `IDisposable` | `[TearDown]` | `[TestCleanup]` |
| **Assertions** | `Assert.Equal()` | `Assert.That()` | `Assert.AreEqual()` |
| **Parallel** | ✅ Default | ⚠️ Opt-in | ⚠️ Limited |
| **Popularity** | High (modern projects) | High (legacy) | Medium (VS users) |
| **ASP.NET Core** | ✅ Recommended | ✅ Supported | ✅ Supported |

### Recommendation

**Use xUnit** for new projects:
- Modern, clean API
- Parallel by default (faster)
- No global state
- Used by ASP.NET Core team

**Use NUnit** if:
- Existing projects already use it
- Need richer assertion API
- Prefer `[SetUp]`/`[TearDown]` pattern

**Use MSTest** if:
- Tight VS integration required
- Enterprise standardization

---

## The AAA Pattern

**Arrange, Act, Assert** - the standard test structure.

```csharp
[Fact]
public void Add_TwoPositiveNumbers_ReturnsSum()
{
    // Arrange: Set up test data and dependencies
    var calculator = new Calculator();
    int a = 5;
    int b = 3;
    
    // Act: Execute the method being tested
    var result = calculator.Add(a, b);
    
    // Assert: Verify the result
    Assert.Equal(8, result);
}
```

**Why AAA?**
- Clear test structure
- Easy to read and maintain
- Separates test concerns
- Industry standard

---

## xUnit Examples

### Basic Tests

```csharp
// Simple test with [Fact]
[Fact]
public void Divide_ByZero_ThrowsException()
{
    var calculator = new Calculator();
    
    Assert.Throws<DivideByZeroException>(() => 
        calculator.Divide(10, 0));
}

// Parameterized test with [Theory]
[Theory]
[InlineData(2, 4, 6)]   // Test case 1
[InlineData(0, 5, 5)]   // Test case 2
[InlineData(-1, 1, 0)]  // Test case 3
public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
{
    var calculator = new Calculator();
    
    var result = calculator.Add(a, b);
    
    Assert.Equal(expected, result);
}
```

### Setup and Teardown

```csharp
public class DatabaseTests : IDisposable
{
    private readonly DatabaseContext _context;
    
    // ✅ Constructor = Setup (runs before each test)
    public DatabaseTests()
    {
        _context = new DatabaseContext();
        _context.Database.EnsureCreated();
    }
    
    [Fact]
    public void SaveUser_ValidData_Success()
    {
        var user = new User { Name = "Alice" };
        
        _context.Users.Add(user);
        _context.SaveChanges();
        
        Assert.NotEqual(0, user.Id);
    }
    
    // ✅ Dispose = Teardown (runs after each test)
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
```

### Test Class Fixtures (Shared Setup)

```csharp
// ✅ Share expensive setup across multiple tests
public class DatabaseFixture : IDisposable
{
    public DatabaseContext Context { get; }
    
    public DatabaseFixture()
    {
        Context = new DatabaseContext();
        Context.Database.EnsureCreated();
    }
    
    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}

// Use fixture in test class
public class UserRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseContext _context;
    
    public UserRepositoryTests(DatabaseFixture fixture)
    {
        _context = fixture.Context;
    }
    
    [Fact]
    public void GetUsers_ReturnsAllUsers()
    {
        // Context is shared across all tests in this class
    }
}
```

---

## Mocking with Moq

### Why Mock?

**Isolation**: Test one class without its dependencies
**Speed**: No database, network, or file I/O
**Control**: Simulate success, failure, edge cases

### Basic Mocking

```csharp
[Fact]
public void GetUser_UserExists_ReturnsUser()
{
    // Arrange: Create mock
    var mockRepo = new Mock<IUserRepository>();
    mockRepo.Setup(r => r.GetById(1))
        .Returns(new User { Id = 1, Name = "Alice" });
    
    var service = new UserService(mockRepo.Object);
    
    // Act
    var user = service.GetUser(1);
    
    // Assert
    Assert.NotNull(user);
    Assert.Equal("Alice", user.Name);
}
```

### Verifying Interactions

```csharp
[Fact]
public void CreateUser_ValidUser_SavesCalled()
{
    var mockRepo = new Mock<IUserRepository>();
    var service = new UserService(mockRepo.Object);
    
    service.CreateUser(new User { Name = "Bob" });
    
    // ✅ Verify method was called once
    mockRepo.Verify(r => r.Save(It.IsAny<User>()), Times.Once);
}

[Fact]
public void DeleteUser_InvalidId_DoesNotCallDatabase()
{
    var mockRepo = new Mock<IUserRepository>();
    var service = new UserService(mockRepo.Object);
    
    service.DeleteUser(-1);  // Invalid ID
    
    // ✅ Verify method was never called
    mockRepo.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
}
```

### Async Mocking

```csharp
[Fact]
public async Task GetUserAsync_UserExists_ReturnsUser()
{
    var mockRepo = new Mock<IUserRepository>();
    mockRepo.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(new User { Id = 1, Name = "Alice" });
    
    var service = new UserService(mockRepo.Object);
    
    var user = await service.GetUserAsync(1);
    
    Assert.NotNull(user);
}
```

### Exception Mocking

```csharp
[Fact]
public async Task GetUserAsync_DatabaseError_ThrowsException()
{
    var mockRepo = new Mock<IUserRepository>();
    mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
        .ThrowsAsync(new DatabaseException("Connection failed"));
    
    var service = new UserService(mockRepo.Object);
    
    await Assert.ThrowsAsync<DatabaseException>(() => 
        service.GetUserAsync(1));
}
```

---

## Testing Async Code

### Common Mistakes

```csharp
// ❌ BAD: Not awaiting async method
[Fact]
public void Test_Bad()  // ❌ Not async
{
    var result = service.GetDataAsync();  // ❌ Not awaited
    Assert.NotNull(result);  // ❌ Testing Task, not result!
}

// ✅ GOOD: Properly awaiting
[Fact]
public async Task Test_Good()  // ✅ async Task
{
    var result = await service.GetDataAsync();  // ✅ Awaited
    Assert.NotNull(result);  // ✅ Testing actual result
}
```

### Testing Timeouts

```csharp
[Fact(Timeout = 5000)]  // ✅ Fail if takes > 5 seconds
public async Task GetData_RespondsQuickly()
{
    var result = await service.GetDataAsync();
    Assert.NotNull(result);
}
```

### Testing Cancellation

```csharp
[Fact]
public async Task GetData_Cancelled_ThrowsOperationCanceledException()
{
    using var cts = new CancellationTokenSource();
    cts.Cancel();  // Cancel immediately
    
    await Assert.ThrowsAsync<OperationCanceledException>(() =>
        service.GetDataAsync(cts.Token));
}
```

---

## Data-Driven Tests

### MemberData (Complex Data)

```csharp
public class CalculatorTests
{
    public static IEnumerable<object[]> AddTestData =>
        new List<object[]>
        {
            new object[] { 2, 3, 5 },
            new object[] { -1, 1, 0 },
            new object[] { 0, 0, 0 },
            new object[] { int.MaxValue, 0, int.MaxValue }
        };
    
    [Theory]
    [MemberData(nameof(AddTestData))]
    public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
    {
        var calculator = new Calculator();
        Assert.Equal(expected, calculator.Add(a, b));
    }
}
```

### ClassData (Reusable Data)

```csharp
public class AddTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 2, 3, 5 };
        yield return new object[] { -1, 1, 0 };
        yield return new object[] { 0, 0, 0 };
    }
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

[Theory]
[ClassData(typeof(AddTestData))]
public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
{
    var calculator = new Calculator();
    Assert.Equal(expected, calculator.Add(a, b));
}
```

---

## Test Data Builders

### The Problem

```csharp
// ❌ BAD: Repetitive test setup
[Fact]
public void Test1()
{
    var user = new User 
    { 
        Id = 1, 
        Name = "Alice", 
        Email = "alice@example.com",
        CreatedDate = DateTime.UtcNow,
        IsActive = true 
    };
}

[Fact]
public void Test2()
{
    var user = new User  // ❌ Same setup again
    { 
        Id = 2, 
        Name = "Bob", 
        Email = "bob@example.com",
        CreatedDate = DateTime.UtcNow,
        IsActive = true 
    };
}
```

### The Solution: Builder Pattern

```csharp
public class UserBuilder
{
    private int _id = 1;
    private string _name = "Test User";
    private string _email = "test@example.com";
    private bool _isActive = true;
    
    public UserBuilder WithId(int id)
    {
        _id = id;
        return this;
    }
    
    public UserBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public UserBuilder Inactive()
    {
        _isActive = false;
        return this;
    }
    
    public User Build() => new User
    {
        Id = _id,
        Name = _name,
        Email = _email,
        IsActive = _isActive
    };
}

// ✅ GOOD: Clean test setup
[Fact]
public void Test_WithBuilder()
{
    var user = new UserBuilder()
        .WithName("Alice")
        .Build();
    
    // Test with user
}

[Fact]
public void Test_InactiveUser()
{
    var user = new UserBuilder()
        .Inactive()
        .Build();
    
    // Test with inactive user
}
```

---

## Integration Testing

### WebApplicationFactory (ASP.NET Core)

```csharp
public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task GetUsers_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/users");
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }
}
```

### In-Memory Database

```csharp
public class RepositoryIntegrationTests
{
    [Fact]
    public async Task SaveUser_ValidUser_Success()
    {
        // Arrange: In-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        
        using var context = new AppDbContext(options);
        var repository = new UserRepository(context);
        
        // Act
        var user = new User { Name = "Alice" };
        await repository.SaveAsync(user);
        
        // Assert
        var saved = await context.Users.FindAsync(user.Id);
        Assert.NotNull(saved);
        Assert.Equal("Alice", saved.Name);
    }
}
```

---

## Best Practices

### ✅ Test Naming
```
MethodName_Scenario_ExpectedBehavior

Examples:
- Add_TwoPositiveNumbers_ReturnsSum
- GetUser_UserNotFound_ReturnsNull
- CreateOrder_InvalidData_ThrowsValidationException
```

### ✅ One Assert Per Test
```csharp
// ❌ BAD: Multiple asserts (which failed?)
[Fact]
public void Test_Bad()
{
    Assert.Equal(5, result.Count);
    Assert.True(result.All(x => x.IsActive));
    Assert.Equal("Alice", result.First().Name);
}

// ✅ GOOD: One logical assertion
[Fact]
public void Test_Good()
{
    Assert.Equal(5, result.Count);
}

[Fact]
public void Test_AllActive()
{
    Assert.All(result, x => Assert.True(x.IsActive));
}
```

### ✅ Test Independence
```csharp
// ❌ BAD: Tests depend on each other
private static int _counter = 0;

[Fact]
public void Test1() { _counter++; }  // ❌ Shared state

[Fact]
public void Test2() { Assert.Equal(1, _counter); }  // ❌ Depends on Test1

// ✅ GOOD: Each test is independent
[Fact]
public void Test1() 
{ 
    int counter = 0;
    counter++;
    Assert.Equal(1, counter);
}
```

### ✅ Fast Tests
- Avoid real databases (use in-memory)
- Avoid real HTTP calls (use mocks)
- Avoid Thread.Sleep (use Task.CompletedTask)
- Keep tests under 100ms

---

## Common Pitfalls

### ❌ Not Testing Edge Cases
```csharp
// ❌ Only tests happy path
[Fact]
public void Divide_ValidNumbers_ReturnsQuotient()
{
    Assert.Equal(5, calculator.Divide(10, 2));
}

// ✅ Tests edge cases
[Theory]
[InlineData(10, 0)]  // Division by zero
[InlineData(int.MaxValue, 1)]  // Large numbers
[InlineData(-10, 2)]  // Negative numbers
public void Divide_EdgeCases_HandlesCorrectly(int a, int b)
{
    // Test appropriate behavior
}
```

### ❌ Testing Implementation Details
```csharp
// ❌ BAD: Testing private methods
[Fact]
public void TestPrivateMethod()
{
    var result = (int)typeof(Calculator)
        .GetMethod("PrivateAdd", BindingFlags.NonPublic)
        .Invoke(calculator, new object[] { 2, 3 });
}

// ✅ GOOD: Test public behavior
[Fact]
public void Add_TwoNumbers_ReturnsSum()
{
    Assert.Equal(5, calculator.Add(2, 3));
}
```

---

## Related Files

- [Testing/xUnit/XUnitBestPractices.cs](../Learning/Testing/xUnit/XUnitBestPractices.cs)
- [Testing/NUnit/NUnitBestPractices.cs](../Learning/Testing/NUnit/NUnitBestPractices.cs)
- [Testing/MSTest/MSTestBestPractices.cs](../Learning/Testing/MSTest/MSTestBestPractices.cs)
- [Testing/TestingFrameworksComparison.cs](../Learning/Testing/TestingFrameworksComparison.cs)
- [Testing/MockingInDepthExamples.cs](../Learning/Testing/MockingInDepthExamples.cs)
- [Testing/TestingAsyncCodeExamples.cs](../Learning/Testing/TestingAsyncCodeExamples.cs)
- [Testing/TestDataBuildersExamples.cs](../Learning/Testing/TestDataBuildersExamples.cs)
- [Testing/IntegrationTestingExamples.cs](../Learning/Testing/IntegrationTestingExamples.cs)

---

## See Also

- [Practical Patterns](Practical-Patterns.md) - Testing patterns in real applications
- [Web API and MVC](Web-API-MVC.md) - Integration testing APIs
- [Async/Await](Async-Multithreading.md) - Testing async code
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [Practical-Patterns.md](Practical-Patterns.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: This topic covers Testing and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know Testing and I would just follow best practices."
- Strong answer: "For Testing, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Testing in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
