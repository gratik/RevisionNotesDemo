# xUnit Examples

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Testing](../README.md)

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

