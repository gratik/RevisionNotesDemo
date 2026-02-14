// ==============================================================================
// XUNIT BEST PRACTICES - Comprehensive Testing Patterns
// ==============================================================================
// PURPOSE:
//   Demonstrate xUnit testing framework best practices with real-world examples.
//   xUnit is the modern, minimal testing framework used by Microsoft for .NET Core,
//   ASP.NET Core, and Entity Framework Core. It emphasizes simplicity and follows
//   modern software development principles.
//
// WHY XUNIT:
//   - Modern design (created 2007, redesigned for .NET Core)
//   - Minimal attributes ([Fact], [Theory] - that's it!)
//   - Test isolation by default (new instance per test)
//   - Parallel execution out of the box
//   - Excellent extensibility
//   - Used by Microsoft for their own frameworks
//
// WHAT YOU'LL LEARN:
//   1. Test naming conventions (Method_Scenario_ExpectedResult)
//   2. AAA pattern (Arrange, Act, Assert)
//   3. [Fact] vs [Theory] for parameterized tests
//   4. Assertions (Assert.Equal, Assert.True, Assert.Throws, etc.)
//   5. Setup/Teardown via Constructor/IDisposable
//   6. Shared context with IClassFixture and ICollectionFixture
//   7. Async testing patterns
//   8. Common mistakes and how to avoid them
//
// INSTALLATION:
//   dotnet add package xunit
//   dotnet add package xunit.runner.visualstudio
//   dotnet add package Microsoft.NET.Test.Sdk
//
// REAL-WORLD IMPACT:
//   - Faster test execution (20-30% faster than NUnit/MSTest due to parallelization)
//   - Better test isolation (fewer flaky tests from shared state)
//   - Clear test intent with minimal ceremony
//   - Industry standard for modern .NET development
//
// ==============================================================================

using Xunit;

namespace RevisionNotesDemo.Testing.xUnit;

/// <summary>
/// EXAMPLE 1: TEST NAMING CONVENTIONS
/// 
/// THE PROBLEM:
/// Poor test names like Test1(), TestMethod2() don't explain what's being tested.
/// When a test fails, you spend time figuring out what it was supposed to do.
/// 
/// THE SOLUTION:
/// Use descriptive names that follow: MethodName_Scenario_ExpectedResult
/// Or: Given_When_Then format
/// 
/// WHY IT MATTERS:
/// - Self-documenting tests are easier to maintain
/// - Failed tests immediately show what broke
/// - New team members understand test intent without reading code
/// 
/// PERFORMANCE IMPACT: None (naming is compile-time only)
/// </summary>
public class TestNamingExamples
{
    // ❌ BAD: Unclear what this tests
    // [Fact]
    public void Test1()
    {
        var result = new Calculator().Add(2, 3);
        // Assert.Equal(5, result);
    }

    // ✅ GOOD: Method_Scenario_ExpectedResult pattern
    // [Fact]
    public void Add_PositiveNumbers_ReturnsSum()
    {
        // Clear what method, scenario, and expected outcome
        var calculator = new Calculator();
        var result = calculator.Add(2, 3);
        // Assert.Equal(5, result);
    }

    // ✅ GOOD: Given_When_Then pattern (BDD style)
    // [Fact]
    public void GivenTwoPositiveNumbers_WhenAdded_ThenReturnSum()
    {
        var result = new Calculator().Add(2, 3);
        // Assert.Equal(5, result);
    }

    // ✅ GOOD: For edge cases, be specific
    // [Fact]
    public void Divide_ByZero_ThrowsDivideByZeroException()
    {
        var calculator = new Calculator();
        // Assert.Throws<DivideByZeroException>(() => calculator.Divide(10, 0));
    }
}

/// <summary>
/// EXAMPLE 2: AAA PATTERN (Arrange, Act, Assert)
/// 
/// THE PROBLEM:
/// Tests with mixed setup, execution, and verification are hard to read.
/// Unclear separation makes it difficult to identify what's being tested.
/// 
/// THE SOLUTION:
/// Structure every test in three clear phases:
/// - Arrange: Set up test data and dependencies
/// - Act: Execute the method being tested
/// - Assert: Verify the outcome
/// 
/// WHY IT MATTERS:
/// - Consistent structure across all tests
/// - Easy to spot what's being tested (the Act section)
/// - Simple to review and maintain
/// - Industry standard pattern
/// 
/// BEST PRACTICE: Use blank lines or comments to separate the three sections
/// </summary>
public class AAAPatternExamples
{
    // ❌ BAD: Everything mixed together
    // [Fact]
    public void BadStructure()
    {
        var service = new OrderService();
        var order = new Order { Id = 1, Total = 100 };
        var result = service.ProcessOrder(order);
        // Assert.True(result);
        // var status = service.GetOrderStatus(1);
        // Assert.Equal(OrderStatus.Processed, status);
    }

    // ✅ GOOD: Clear AAA structure with blank lines
    // [Fact]
    public void ProcessOrder_ValidOrder_ReturnsSuccess()
    {
        // Arrange
        var service = new OrderService();
        var order = new Order { Id = 1, Total = 100, Items = new List<OrderItem>() };

        // Act
        var result = service.ProcessOrder(order);

        // Assert
        // Assert.True(result);
    }

    // ✅ GOOD: Complex setup still follows AAA
    // [Fact]
    public void ProcessOrder_WithDiscount_AppliesCorrectAmount()
    {
        // Arrange
        var service = new OrderService();
        var discount = new Discount { Percentage = 10 };
        var order = new Order
        {
            Id = 1,
            Total = 100,
            Discount = discount,
            Items = new List<OrderItem>()
        };

        // Act
        var result = service.ProcessOrder(order);

        // Assert
        // Assert.Equal(90, order.FinalTotal);
    }
}

/// <summary>
/// EXAMPLE 3: FACT VS THEORY - Parameterized Tests
/// 
/// THE PROBLEM:
/// Writing separate test methods for each input combination causes duplication:
/// - More code to maintain
/// - Easy to miss edge cases
/// - Hard to add new test cases
/// 
/// THE SOLUTION:
/// - [Fact]: Single test case, no parameters
/// - [Theory]: Multiple test cases with [InlineData], [MemberData], or [ClassData]
/// 
/// WHY IT MATTERS:
/// - Reduce test code by 60-80% for similar scenarios
/// - Add new test cases in seconds (just add [InlineData])
/// - See all test cases at a glance
/// - xUnit runs each as separate test (better failure isolation)
/// 
/// GOTCHA: Each [InlineData] creates a separate test result in test explorer
/// </summary>
public class FactVsTheoryExamples
{
    // ❌ BAD: Duplicate test methods for similar scenarios
    // [Fact]
    public void IsEven_Two_ReturnsTrue()
    {
        // Assert.True(new Calculator().IsEven(2));
    }

    // [Fact]
    public void IsEven_Four_ReturnsTrue()
    {
        // Assert.True(new Calculator().IsEven(4));
    }

    // [Fact]
    public void IsEven_Three_ReturnsFalse()
    {
        // Assert.False(new Calculator().IsEven(3));
    }

    // ✅ GOOD: Single theory with multiple data points
    // [Theory]
    // [InlineData(2, true)]
    // [InlineData(4, true)]
    // [InlineData(6, true)]
    // [InlineData(1, false)]
    // [InlineData(3, false)]
    // [InlineData(5, false)]
    public void IsEven_VariousNumbers_ReturnsExpected(int number, bool expected)
    {
        var calculator = new Calculator();
        var result = calculator.IsEven(number);
        // Assert.Equal(expected, result);
    }

    // ✅ GOOD: Complex data with MemberData
    // [Theory]
    // [MemberData(nameof(GetOrderTestData))]
    public void ProcessOrder_VariousOrders_ProcessesCorrectly(Order order, bool expectedResult)
    {
        var service = new OrderService();
        var result = service.ProcessOrder(order);
        // Assert.Equal(expectedResult, result);
    }

    public static IEnumerable<object[]> GetOrderTestData()
    {
        yield return new object[] { new Order { Id = 1, Total = 100, Items = new List<OrderItem>() }, true };
        yield return new object[] { new Order { Id = 2, Total = 0, Items = new List<OrderItem>() }, false };
        yield return new object[] { new Order { Id = 3, Total = -10, Items = new List<OrderItem>() }, false };
    }
}

/// <summary>
/// EXAMPLE 4: ASSERTIONS - Comprehensive Guide
/// 
/// THE PROBLEM:
/// Using wrong assertion types makes failures unclear.
/// Generic Assert.True with complex conditions is hard to debug.
/// 
/// THE SOLUTION:
/// Use specific assertion methods for better failure messages:
/// - Assert.Equal for value comparison
/// - Assert.Same for reference equality
/// - Assert.Contains for collections
/// - Assert.Throws for exceptions
/// - Assert.Null, Assert.NotNull for null checks
/// 
/// WHY IT MATTERS:
/// - Better failure messages (shows expected vs actual)
/// - Faster debugging when tests fail
/// - More readable test code
/// 
/// TIP: Always put expected value first: Assert.Equal(expected, actual)
/// </summary>
public class AssertionExamples
{
    // ❌ BAD: Generic boolean assertion with unclear failure message
    // [Fact]
    public void BadAssertion()
    {
        var result = new Calculator().Add(2, 3);
        // Assert.True(result == 5); // Failure message: "Expected True but was False"
    }

    // ✅ GOOD: Specific assertion with clear failure message
    // [Fact]
    public void GoodAssertion()
    {
        var result = new Calculator().Add(2, 3);
        // Assert.Equal(5, result); // Failure message: "Expected 5 but was 8"
    }

    // ✅ GOOD: String assertions
    // [Fact]
    public void StringAssertions()
    {
        string result = "hello world";

        // Assert.Equal("hello world", result);        // Exact match
        // Assert.StartsWith("hello", result);         // Starts with
        // Assert.EndsWith("world", result);           // Ends with
        // Assert.Contains("lo wo", result);           // Contains substring
        // Assert.Matches(@"^hello.*world\$", result); // Regex match
    }

    // ✅ GOOD: Collection assertions
    // [Fact]
    public void CollectionAssertions()
    {
        var numbers = new List<int> { 1, 2, 3, 4, 5 };

        // Assert.Contains(3, numbers);           // Contains item
        // Assert.DoesNotContain(10, numbers);    // Doesn't contain
        // Assert.Empty(new List<int>());         // Empty collection
        // Assert.NotEmpty(numbers);              // Not empty
        // Assert.Equal(5, numbers.Count);        // Count check
        // Assert.Collection(numbers,             // Ordered assertions
        //     n => Assert.Equal(1, n),
        //     n => Assert.Equal(2, n),
        //     n => Assert.Equal(3, n));
    }

    // ✅ GOOD: Exception assertions
    // [Fact]
    public void ExceptionAssertions()
    {
        var calculator = new Calculator();

        // Assert.Throws<DivideByZeroException>(() => calculator.Divide(10, 0));

        // With message verification
        var ex = new Exception(); // Assert.Throws<ArgumentException>(() => calculator.DoSomething(""));
        // Assert.Contains("cannot be empty", ex.Message);
    }

    // ✅ GOOD: Null and type assertions
    // [Fact]
    public void NullAndTypeAssertions()
    {
        object? obj = GetObject();

        // Assert.NotNull(obj);
        // Assert.Null(GetNullObject());
        // Assert.IsType<string>(obj);          // Exact type
        // Assert.IsAssignableFrom<object>(obj); // Type or derived
    }

    private object GetObject() => "test";
    private object? GetNullObject() => null;
}

/// <summary>
/// EXAMPLE 5: SETUP AND TEARDOWN - xUnit Way
/// 
/// THE PROBLEM:
/// NUnit/MSTest use [SetUp]/[TearDown] attributes, but xUnit doesn't.
/// New xUnit users don't know how to initialize tests properly.
/// 
/// THE SOLUTION:
/// xUnit prefers constructor for setup and IDisposable for teardown.
/// This enforces test isolation - each test gets fresh instance.
/// 
/// WHY IT MATTERS:
/// - True test isolation (no shared state between tests)
/// - Familiar C# patterns (constructor/Dispose)
/// - Impossible to forget cleanup (compiler enforces IDisposable)
/// - Tests can run in parallel safely
/// 
/// EXECUTION ORDER:
/// 1. Constructor runs
/// 2. Test method executes
/// 3. Dispose runs (if IDisposable implemented)
/// 
/// GOTCHA: Constructor runs for EVERY test (by design for isolation)
/// </summary>
public class SetupTeardownExamples : IDisposable
{
    private readonly Calculator _calculator;
    private readonly DatabaseConnection _connection;

    // ✅ SETUP: Constructor runs before each test
    public SetupTeardownExamples()
    {
        _calculator = new Calculator();
        _connection = new DatabaseConnection();
        _connection.Open();
        Console.WriteLine("Setup: Test initialized");
    }

    // [Fact]
    public void Test1_UsesSetupResources()
    {
        var result = _calculator.Add(2, 3);
        // Assert.Equal(5, result);
    }

    // [Fact]
    public void Test2_GetsFreshInstance()
    {
        // New instance created, constructor ran again
        var result = _calculator.Add(5, 5);
        // Assert.Equal(10, result);
    }

    // ✅ TEARDOWN: Dispose runs after each test
    public void Dispose()
    {
        _connection?.Close();
        Console.WriteLine("Teardown: Resources cleaned up");
    }
}

/// <summary>
/// EXAMPLE 6: SHARED CONTEXT - IClassFixture
/// 
/// THE PROBLEM:
/// Some setup is expensive (database connection, file I/O, HTTP client).
/// Running setup for every test is slow (constructor runs per test).
/// 
/// THE SOLUTION:
/// Use IClassFixture<T> to share context across all tests in a class.
/// Fixture created once, shared by all tests, disposed at end.
/// 
/// WHY IT MATTERS:
/// - 50-80% faster tests when setup is expensive
/// - Still maintains isolation within test class
/// - Perfect for integration tests with databases
/// 
/// WHEN TO USE:
/// - Database connections (expensive to create)
/// - HTTP clients (connection pooling)
/// - File system operations
/// - Container/dependency setup
/// 
/// GOTCHA: Tests share state! Ensure tests don't modify shared fixtures.
/// </summary>
public class DatabaseFixture : IDisposable
{
    public DatabaseConnection Connection { get; }

    public DatabaseFixture()
    {
        Connection = new DatabaseConnection();
        Connection.Open();
        Connection.SeedTestData();
        Console.WriteLine("Fixture: Database initialized (once)");
    }

    public void Dispose()
    {
        Connection.Close();
        Console.WriteLine("Fixture: Database connection closed");
    }
}

public class SharedContextExamples : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public SharedContextExamples(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    // [Fact]
    public void Test1_UsesSharedDatabase()
    {
        var result = _fixture.Connection.ExecuteQuery("SELECT COUNT(*) FROM Users");
        // Assert.True(result > 0);
    }

    // [Fact]
    public void Test2_AlsoUsesSharedDatabase()
    {
        // Same database connection as Test1
        var result = _fixture.Connection.ExecuteQuery("SELECT * FROM Orders");
        // Assert.NotNull(result);
    }
}

/// <summary>
/// EXAMPLE 7: ASYNC TESTING
/// 
/// THE PROBLEM:
/// Testing async code incorrectly leads to false positives (test passes but code is wrong).
/// Using .Result or .Wait() can cause deadlocks.
/// 
/// THE SOLUTION:
/// - Test methods can be async Task
/// - Use await for async operations
/// - Assert on the awaited result
/// 
/// WHY IT MATTERS:
/// - Tests actually verify async behavior
/// - No deadlocks from blocking calls
/// - Better test accuracy
/// 
/// BEST PRACTICE: Always await in test methods, never use .Result
/// </summary>
public class AsyncTestingExamples
{
    // ❌ BAD: Blocking async call with .Result
    // [Fact]
    public void BadAsyncTest()
    {
        var service = new OrderService();
        var result = service.ProcessOrderAsync(new Order()).Result; // Potential deadlock!
        // Assert.True(result);
    }

    // ✅ GOOD: Proper async test
    // [Fact]
    public async Task GoodAsyncTest()
    {
        var service = new OrderService();
        var order = new Order { Id = 1, Total = 100, Items = new List<OrderItem>() };

        var result = await service.ProcessOrderAsync(order);

        // Assert.True(result);
    }

    // ✅ GOOD: Testing async with exception
    // [Fact]
    public async Task ProcessOrderAsync_InvalidOrder_ThrowsException()
    {
        var service = new OrderService();
        var invalidOrder = new Order { Id = -1, Total = 0, Items = new List<OrderItem>() };

        // await Assert.ThrowsAsync<ArgumentException>(async () =>
        //     await service.ProcessOrderAsync(invalidOrder));
    }
}

/// <summary>
/// EXAMPLE 8: COLLECTION TESTING - Arrays, Lists, and Sequences
/// 
/// THE PROBLEM:
/// Testing collections incorrectly: comparing references instead of contents,
/// not checking item order, or writing loops to manually verify each item.
/// 
/// THE SOLUTION:
/// - Assert.Equal for exact match (order and content)
/// - Assert.Contains for single item presence
/// - Assert.All for verifying predicate on all items
/// - Assert.Collection for ordered verification with predicates
/// 
/// WHY IT MATTERS:
/// - Clear, readable collection assertions
/// - Better failure messages showing what differs
/// - No manual loops or complex logic in tests
/// 
/// PERFORMANCE IMPACT: Minimal, but Assert.Collection is more thorough
/// 
/// GOTCHA: Assert.Equal on collections checks BOTH order AND content!
/// If order doesn't matter, use Assert.Equivalent (xUnit 2.4+) or sort first.
/// </summary>
public class CollectionTestingExamples
{
    // ❌ BAD: Manual verification with loops
    // [Fact]
    public void GetUsers_ReturnsAllUsers_BadApproach()
    {
        var userService = new UserService();
        var users = userService.GetUsers();

        // Manual verification - verbose and unclear
        bool found = false;
        foreach (var user in users)
        {
            if (user.Name == "Alice") found = true;
        }
        // Assert.True(found);
    }

    // ✅ GOOD: Assert.Contains for single item
    // [Fact]
    public void GetUsers_ContainsAlice()
    {
        var userService = new UserService();
        var users = userService.GetUsers();

        // Assert.Contains(users, u => u.Name == "Alice");
    }

    // ✅ GOOD: Assert.Equal for exact collection match
    // [Fact]
    public void GetNumbers_ReturnsExpectedSequence()
    {
        var service = new MathService();
        var expected = new[] { 1, 2, 3, 4, 5 };

        var actual = service.GetFirstFiveNumbers();

        // Assert.Equal(expected, actual); // Checks order AND content
    }

    // ✅ GOOD: Assert.All for predicate on all items
    // [Fact]
    public void GetActiveUsers_AllHaveActiveStatus()
    {
        var userService = new UserService();
        var activeUsers = userService.GetActiveUsers();

        // Verify ALL items match the predicate
        // Assert.All(activeUsers, user => Assert.True(user.IsActive));
    }

    // ✅ GOOD: Assert.Collection for ordered verification
    // [Fact]
    public void GetUsers_ReturnsInExpectedOrder()
    {
        var userService = new UserService();
        var users = userService.GetUsers();

        // Verify exact count and each item in order
        // Assert.Collection(users,
        //     user => Assert.Equal("Alice", user.Name),
        //     user => Assert.Equal("Bob", user.Name),
        //     user => Assert.Equal("Charlie", user.Name)
        // );
    }

    // ✅ GOOD: Assert.Empty and Assert.NotEmpty
    // [Fact]
    public void GetDeletedUsers_ReturnsEmptyList()
    {
        var userService = new UserService();
        var deletedUsers = userService.GetDeletedUsers();

        // Assert.Empty(deletedUsers);
    }
}

/// <summary>
/// EXAMPLE 9: STRING TESTING AND COMPARISON
/// 
/// THE PROBLEM:
/// Using Assert.Equal for strings doesn't show WHERE they differ.
/// Not handling case sensitivity or whitespace correctly.
/// Forgetting about null vs empty string differences.
/// 
/// THE SOLUTION:
/// - Assert.Equal for exact match
/// - Assert.StartsWith / Assert.EndsWith for partial match
/// - Assert.Contains for substring
/// - Assert.Matches for regex patterns
/// - Assert.Equal with case-insensitive comparer when needed
/// 
/// WHY IT MATTERS:
/// - String comparison is extremely common in tests
/// - Proper assertions give better failure messages
/// - Case sensitivity bugs are common in production
/// 
/// TIP: For multi-line strings, xUnit shows a diff. For JSON, consider
/// using a JSON assertion library like FluentAssertions.
/// </summary>
public class StringTestingExamples
{
    // ❌ BAD: Using Assert.True for string comparison
    // [Fact]
    public void GenerateGreeting_ReturnsCorrectMessage_BadApproach()
    {
        var service = new MessageService();
        var message = service.GenerateGreeting("John");

        // Assert.True(message == "Hello, John!"); // No diff on failure!
    }

    // ✅ GOOD: Assert.Equal shows diff on failure
    // [Fact]
    public void GenerateGreeting_ReturnsCorrectMessage()
    {
        var service = new MessageService();

        var message = service.GenerateGreeting("John");

        // Assert.Equal("Hello, John!", message); // Shows diff: "Hello, John!" vs actual
    }

    // ✅ GOOD: Case-insensitive comparison
    // [Fact]
    public void GenerateGreeting_IsCaseInsensitive()
    {
        var service = new MessageService();

        var message = service.GenerateGreeting("john");

        // Assert.Equal("Hello, John!", message, ignoreCase: true);
    }

    // ✅ GOOD: Partial string matching
    // [Fact]
    public void GenerateReport_ContainsUsername()
    {
        var service = new ReportService();

        var report = service.GenerateReport("Alice", 42);

        // Assert.Contains("Alice", report);
        // Assert.StartsWith("Report for", report);
        // Assert.EndsWith("records found.", report);
    }

    // ✅ GOOD: Regex matching for patterns
    // [Fact]
    public void GenerateId_MatchesExpectedPattern()
    {
        var service = new IdGeneratorService();

        var id = service.GenerateId();

        // Should match pattern: USER-12345
        // Assert.Matches(@"USER-\d{5}", id);
    }

    // ✅ GOOD: Null vs Empty distinction
    // [Theory]
    // [InlineData(null, true)]
    // [InlineData("", false)]
    // [InlineData("  ", false)]
    public void IsNullOrEmpty_HandlesAllCases(string? input, bool expectedNull)
    {
        var service = new ValidationService();

        var isNull = service.IsNull(input);
        var isEmpty = service.IsEmpty(input);

        // Assert.Equal(expectedNull, isNull);
        // Different behavior for null vs empty!
    }
}

/// <summary>
/// EXAMPLE 10: TESTING NULL, EMPTY, AND DEFAULT VALUES
/// 
/// THE PROBLEM:
/// Not testing edge cases: null inputs, empty collections, default values.
/// These are where many production bugs hide!
/// 
/// THE SOLUTION:
/// - Assert.Null / Assert.NotNull for reference types
/// - Assert.Empty / Assert.NotEmpty for collections
/// - Test with null/empty in [Theory] parameters
/// - Use [MemberData] for complex test cases
/// 
/// WHY IT MATTERS:
/// - Null reference exceptions are #1 runtime error in C#
/// - Empty collections behave differently than null
/// - Default values can cause subtle bugs
/// 
/// BEST PRACTICE: Always test the "unhappy path" - null, empty, invalid data
/// </summary>
public class NullAndEmptyTestingExamples
{
    // ❌ BAD: Only testing happy path
    // [Fact]
    public void ProcessUser_OnlyTestsValidUser()
    {
        var service = new UserProcessor();
        var user = new User { Name = "John", Email = "john@example.com" };

        var result = service.ProcessUser(user);

        // Assert.NotNull(result);
        // What about null user? Empty name? Invalid email?
    }

    // ✅ GOOD: Test null input explicitly
    // [Fact]
    public void ProcessUser_NullUser_ThrowsArgumentNullException()
    {
        var service = new UserProcessor();

        // Assert.Throws<ArgumentNullException>(() => service.ProcessUser(null!));
    }

    // ✅ GOOD: Test empty/invalid with Theory
    // [Theory]
    // [InlineData(null, "john@example.com")] // Null name
    // [InlineData("", "john@example.com")]   // Empty name
    // [InlineData("John", null)]             // Null email
    // [InlineData("John", "")]               // Empty email
    // [InlineData("John", "invalid")]        // Invalid email format
    public void ProcessUser_InvalidData_ThrowsException(string? name, string? email)
    {
        var service = new UserProcessor();
        var user = new User { Name = name!, Email = email! };

        // Assert.Throws<ArgumentException>(() => service.ProcessUser(user));
    }

    // ✅ GOOD: Test with empty collections
    // [Fact]
    public void CalculateTotal_EmptyOrders_ReturnsZero()
    {
        var service = new OrderCalculator();
        var emptyOrders = new List<Order>();

        var total = service.CalculateTotal(emptyOrders);

        // Assert.Equal(0, total);
    }

    // ✅ GOOD: Default value testing
    // [Fact]
    public void CreateOrder_DefaultValues_AreValid()
    {
        var order = new Order(); // Uses default values

        // Assert.Equal(0, order.Id);
        // Assert.Equal(0, order.Total);
        // Assert.NotNull(order.Items); // Should be empty list, not null!
        // Assert.Empty(order.Items);
    }
}

/// <summary>
/// EXAMPLE 11: MEMBER DATA - Complex Test Data from Methods
/// 
/// THE PROBLEM:
/// [InlineData] only supports simple types (int, string, etc.).
/// Complex test scenarios need objects, collections, or computed data.
/// 
/// THE SOLUTION:
/// - [MemberData] points to a static property or method
/// - Returns IEnumerable<object[]> where each object[] is one test case
/// - Allows complex objects, dependency-injected data, or computed scenarios
/// 
/// WHY IT MATTERS:
/// - Test complex scenarios without code duplication
/// - Single source of truth for test data
/// - Easier to add/modify test cases
/// - Can load data from files, databases, etc.
/// 
/// PERFORMANCE IMPACT: MemberData is evaluated once when tests are discovered
/// </summary>
public class MemberDataExamples
{
    // ❌ BAD: Duplicated test methods for complex scenarios
    // [Fact]
    public void ValidateOrder_Scenario1() { /* ... */ }

    // [Fact]
    public void ValidateOrder_Scenario2() { /* ... */ }

    // [Fact]
    public void ValidateOrder_Scenario3() { /* ... */ }
    // This is tedious and error-prone!

    // ✅ GOOD: MemberData with complex objects
    // [Theory]
    // [MemberData(nameof(GetOrderTestCases))]
    public void ValidateOrder_VariousScenarios_ReturnsExpectedResult(
        Order order, bool expectedValid, string expectedError)
    {
        var validator = new OrderValidator();

        var (isValid, error) = validator.Validate(order);

        // Assert.Equal(expectedValid, isValid);
        // Assert.Equal(expectedError, error);
    }

    // Test data source - can be property or method
    public static IEnumerable<object[]> GetOrderTestCases()
    {
        // Each yield is one test case
        yield return new object[]
        {
            new Order { Id = 1, Total = 100, Items = new List<OrderItem> { new() } },
            true,
            ""
        };

        yield return new object[]
        {
            new Order { Id = 0, Total = 100, Items = new List<OrderItem>() }, // Invalid ID
            false,
            "Order ID must be positive"
        };

        yield return new object[]
        {
            new Order { Id = 1, Total = -50, Items = new List<OrderItem>() }, // Negative total
            false,
            "Order total cannot be negative"
        };

        yield return new object[]
        {
            new Order { Id = 1, Total = 100, Items = new List<OrderItem>() }, // No items
            false,
            "Order must contain at least one item"
        };
    }

    // ✅ GOOD: MemberData with TheoryData<T> (type-safe)
    // [Theory]
    // [MemberData(nameof(GetCalculationTestData))]
    public void Calculate_VariousInputs_ReturnsExpectedResult(int a, int b, int expected)
    {
        var calc = new Calculator();

        var result = calc.Add(a, b);

        // Assert.Equal(expected, result);
    }

    public static TheoryData<int, int, int> GetCalculationTestData()
    {
        return new TheoryData<int, int, int>
        {
            { 1, 2, 3 },
            { -1, 1, 0 },
            { 0, 0, 0 },
            { int.MaxValue, 0, int.MaxValue }
        };
    }
}

/// <summary>
/// EXAMPLE 12: TESTING OBJECT EQUALITY AND COMPARISON
/// 
/// THE PROBLEM:
/// Assert.Equal on objects checks reference equality by default, not value.
/// Two objects with same properties may fail equality check.
/// 
/// THE SOLUTION:
/// - Override Equals and GetHashCode in your classes (recommended)
/// - Use Assert.Equivalent for value-based comparison (xUnit 2.4+)
/// - Check individual properties when appropriate
/// - Use custom comparers for complex scenarios
/// 
/// WHY IT MATTERS:
/// - Testing DTOs, ViewModels, and domain objects requires value comparison
/// - Reference equality leads to false test failures
/// - Proper equality implementation is important for production code too
/// 
/// TIP: FluentAssertions library makes this even easier with .Should().BeEquivalentTo()
/// </summary>
public class ObjectEqualityTestingExamples
{
    // ❌ BAD: Comparing references (will fail even if values match!)
    // [Fact]
    public void CreateUser_ReturnsUserWithCorrectValues_BadApproach()
    {
        var service = new UserFactory();
        var expected = new User { Name = "John", Email = "john@example.com", IsActive = true };

        var actual = service.CreateUser("John", "john@example.com");

        // Assert.Equal(expected, actual); // FAILS! Different references!
    }

    // ✅ GOOD: Check individual properties
    // [Fact]
    public void CreateUser_ReturnsUserWithCorrectValues_PropertyCheck()
    {
        var service = new UserFactory();

        var actual = service.CreateUser("John", "john@example.com");

        // Assert.Equal("John", actual.Name);
        // Assert.Equal("john@example.com", actual.Email);
        // Assert.True(actual.IsActive);
    }

    // ✅ BETTER: Use Assert.Equivalent (xUnit 2.4+) for deep value comparison
    // [Fact]
    public void CreateUser_ReturnsUserWithCorrectValues_Equivalent()
    {
        var service = new UserFactory();
        var expected = new User { Name = "John", Email = "john@example.com", IsActive = true };

        var actual = service.CreateUser("John", "john@example.com");

        // Assert.Equivalent(expected, actual); // Compares all property values!
    }

    // ✅ BEST: Implement Equals in your classes (production-ready)
    // If User has proper Equals/GetHashCode, Assert.Equal works!
    // [Fact]
    public void CreateUser_WithProperEquals_WorksAsExpected()
    {
        // Assuming User implements IEquatable<User>
        var service = new UserFactory();
        var expected = new User { Name = "John", Email = "john@example.com", IsActive = true };

        var actual = service.CreateUser("John", "john@example.com");

        // Assert.Equal(expected, actual); // Now works because of proper Equals!
    }

    // ✅ GOOD: Testing collections of objects
    // [Fact]
    public void GetAllUsers_ReturnsExpectedUsers()
    {
        var service = new UserService();

        var users = service.GetUsers();

        // Option 1: Check count and individual properties
        // Assert.Equal(3, users.Count);
        // Assert.Equal("Alice", users[0].Name);

        // Option 2: Use Assert.Collection with predicates
        // Assert.Collection(users,
        //     user => Assert.Equal("Alice", user.Name),
        //     user => Assert.Equal("Bob", user.Name),
        //     user => Assert.Equal("Charlie", user.Name)
        // );
    }
}

// Supporting classes (demonstration purposes)
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Divide(int a, int b) => b == 0 ? throw new DivideByZeroException() : a / b;
    public bool IsEven(int number) => number % 2 == 0;
}

public class Order
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public decimal FinalTotal { get; set; }
    public Discount? Discount { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem { }
public class Discount { public decimal Percentage { get; set; } }

public class OrderService
{
    public bool ProcessOrder(Order order) => order.Total > 0;
    public async Task<bool> ProcessOrderAsync(Order order)
    {
        if (order.Id < 0) throw new ArgumentException("Invalid order ID");
        await Task.Delay(10);
        return order.Total > 0;
    }
}

public class DatabaseConnection
{
    public void Open() { }
    public void Close() { }
    public void SeedTestData() { }
    public int ExecuteQuery(string query) => 5;
}

// Additional supporting classes for Examples 8-12
public class User
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class UserService
{
    public List<User> GetUsers() => new()
    {
        new User { Name = "Alice", IsActive = true },
        new User { Name = "Bob", IsActive = true },
        new User { Name = "Charlie", IsActive = false }
    };

    public List<User> GetActiveUsers() =>
        GetUsers().Where(u => u.IsActive).ToList();

    public List<User> GetDeletedUsers() => new();
}

public class MathService
{
    public int[] GetFirstFiveNumbers() => new[] { 1, 2, 3, 4, 5 };
}

public class MessageService
{
    public string GenerateGreeting(string name) => $"Hello, {name}!";
}

public class ReportService
{
    public string GenerateReport(string username, int recordCount) =>
        $"Report for {username}: {recordCount} records found.";
}

public class IdGeneratorService
{
    public string GenerateId() => $"USER-{new Random().Next(10000, 99999)}";
}

public class ValidationService
{
    public bool IsNull(string? input) => input == null;
    public bool IsEmpty(string? input) => string.IsNullOrEmpty(input);
}

public class UserProcessor
{
    public User ProcessUser(User? user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrEmpty(user.Name)) throw new ArgumentException("Name is required");
        if (string.IsNullOrEmpty(user.Email)) throw new ArgumentException("Email is required");
        if (!user.Email.Contains("@")) throw new ArgumentException("Invalid email format");

        user.IsActive = true;
        return user;
    }
}

public class OrderCalculator
{
    public decimal CalculateTotal(List<Order> orders) => orders.Sum(o => o.Total);
}

public class OrderValidator
{
    public (bool IsValid, string Error) Validate(Order order)
    {
        if (order.Id <= 0) return (false, "Order ID must be positive");
        if (order.Total < 0) return (false, "Order total cannot be negative");
        if (order.Items.Count == 0) return (false, "Order must contain at least one item");
        return (true, "");
    }
}

public class UserFactory
{
    public User CreateUser(string name, string email) =>
        new User { Name = name, Email = email, IsActive = true };
}
