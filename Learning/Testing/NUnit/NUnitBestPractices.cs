// ==============================================================================
// NUNIT BEST PRACTICES - Comprehensive Testing Patterns
// ==============================================================================
// PURPOSE:
//   Demonstrate NUnit testing framework best practices with real-world examples.
//   NUnit is the mature, feature-rich testing framework with 17+ years of development.
//   It's the enterprise standard with the most extensive assertion library.
//
// WHY NUNIT:
//   - Battle-tested (first released 2002, continuously updated)
//   - Rich constraint-based assertion model (Assert.That)
//   - Extensive attribute support ([SetUp], [TearDown], [Category], etc.)
//   - Strong enterprise adoption (Unity3D, Xamarin, legacy codebases)
//   - Most flexible test organization
//
// WHAT YOU'LL LEARN:
//   1. [TestFixture] and [Test] attributes
//   2. [SetUp]/[TearDown] vs [OneTimeSetUp]/[OneTimeTearDown]
//   3. Constraint-based assertions (Assert.That with Is, Has, Contains)
//   4. [TestCase] for parameterized tests
//   5. [Category] for test organization
//   6. [Explicit], [Ignore], [MaxTime] attributes
//   7. String, collection, and exception assertions
//
// INSTALLATION:
//   dotnet add package NUnit
//   dotnet add package NUnit3TestAdapter
//   dotnet add package Microsoft.NET.Test.Sdk
//
// COMPARISON TO XUNIT:
//   - More attributes (xUnit: 2, NUnit: 20+)
//   - Shared state by default (xUnit: isolation by default)
//   - Constraint model vs direct assertions
//   - Better for migration from MSTest/older frameworks
//
// ==============================================================================

using NUnit.Framework;

namespace RevisionNotesDemo.Testing.NUnit;

/// <summary>
/// EXAMPLE 1: TEST FIXTURE AND BASIC TESTS
/// 
/// THE PROBLEM:
/// NUnit requires [TestFixture] attribute on test classes (xUnit doesn't need this).
/// Without it, tests won't be discovered.
/// 
/// THE SOLUTION:
/// Always mark test classes with [TestFixture] and test methods with [Test].
/// 
/// WHY IT MATTERS:
/// - Tests must be discoverable by test runner
/// - [TestFixture] can have parameters for data-driven class-level testing
/// - Explicit marking prevents accidental test discovery
/// 
/// GOTCHA: Forgetting [TestFixture] means tests won't run (no error, just silently skipped)
/// </summary>
// [TestFixture]
public class BasicTestExamples
{
    // [Test]
    public void Add_TwoPositiveNumbers_ReturnsSum()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var result = calculator.Add(2, 3);

        // Assert
        // Assert.AreEqual(5, result);
    }

    // ✅ GOOD: NUnit's constraint-based assertion (preferred)
    // [Test]
    public void Add_TwoPositiveNumbers_ReturnsSumConstraint()
    {
        var calculator = new Calculator();
        var result = calculator.Add(2, 3);

        // Assert.That(result, Is.EqualTo(5));  // More readable, better error messages
    }

    // ❌ BAD: Using classic Assert without TestFixture
    public void ThisTestWontRun()
    {
        // Missing [Test] attribute - won't be discovered!
        // Assert.That(true, Is.True);
    }
}

/// <summary>
/// EXAMPLE 2: SETUP AND TEARDOWN - Four Levels
/// 
/// THE PROBLEM:
/// NUnit has multiple setup/teardown attributes that confuse developers.
/// Using wrong one leads to performance issues or incorrect test state.
/// 
/// THE SOLUTION:
/// Four levels of setup/teardown (from most to least frequent):
/// 1. [SetUp]/[TearDown] - Before/after EACH test
/// 2. [OneTimeSetUp]/[OneTimeTearDown] - Before/after ALL tests in fixture
/// 3. [OneTimeSetUp]/[OneTimeTearDown] at assembly level
/// 
/// WHY IT MATTERS:
/// - Expensive setup runs once with OneTimeSetUp (database connections)
/// - Test-specific setup runs per test with SetUp
/// - Choosing wrong one: 10x slower tests or shared state bugs
/// 
/// EXECUTION ORDER:
/// OneTimeSetUp → SetUp → Test → TearDown → (repeat for each test) → OneTimeTearDown
/// 
/// BEST PRACTICE: Use OneTimeSetUp for expensive operations, SetUp for test data
/// </summary>
// [TestFixture]
public class SetupTeardownExamples
{
    private Calculator? _calculator;
    private static DatabaseConnection? _sharedDb;

    // ✅ Runs ONCE before all tests in this fixture
    // [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _sharedDb = new DatabaseConnection();
        _sharedDb.Open();
        _sharedDb.SeedTestData();
        Console.WriteLine("OneTimeSetUp: Database initialized (expensive, runs once)");
    }

    // ✅ Runs BEFORE EACH test
    // [SetUp]
    public void Setup()
    {
        _calculator = new Calculator();
        Console.WriteLine("SetUp: Fresh calculator created for this test");
    }

    // [Test]
    public void Test1_HasFreshCalculator()
    {
        // Assert.That(_calculator, Is.Not.Null);
        // Assert.That(_sharedDb, Is.Not.Null);
    }

    // [Test]
    public void Test2_AlsoGetsFreshCalculator()
    {
        // SetUp ran again, new calculator instance
        // Assert.That(_calculator, Is.Not.Null);
    }

    // ✅ Runs AFTER EACH test
    // [TearDown]
    public void Teardown()
    {
        _calculator = null;
        Console.WriteLine("TearDown: Cleaned up after test");
    }

    // ✅ Runs ONCE after all tests
    // [OneTimeTearDown]
    public void OneTimeTeardown()
    {
        _sharedDb?.Close();
        _sharedDb = null;
        Console.WriteLine("OneTimeTearDown: Database connection closed");
    }
}

/// <summary>
/// EXAMPLE 3: CONSTRAINT-BASED ASSERTIONS - NUnit's Strength
/// 
/// THE PROBLEM:
/// Classic assertions (Assert.AreEqual, Assert.IsTrue) give poor error messages.
/// Complex conditions are hard to express.
/// 
/// THE SOLUTION:
/// Use Assert.That with constraints for:
/// - Better readability (reads like English)
/// - Superior error messages (shows context)
/// - Composable constraints (Is.Not, Is.GreaterThan.And.LessThan(...))
/// 
/// WHY IT MATTERS:
/// - Failed test messages show: Expected, Actual, and Constraint
/// - Complex assertions in one line
/// - easier to understand test intent
/// 
/// PERFORMANCE: Negligible overhead, but massively better debugging
/// </summary>
// [TestFixture]
public class ConstraintAssertionExamples
{
    // ❌ BAD: Classic assertions (less readable)
    // [Test]
    public void ClassicAssertions()
    {
        var result = 5;
        // Assert.AreEqual(5, result);
        // Assert.IsTrue(result > 0);
        // Assert.Greater(result, 3);
    }

    // ✅ GOOD: Constraint-based assertions (NUnit's strength)
    // [Test]
    public void ConstraintAssertions()
    {
        var result = 5;

        // Assert.That(result, Is.EqualTo(5));
        // Assert.That(result, Is.Positive);
        // Assert.That(result, Is.GreaterThan(3));

        // Composite constraints
        // Assert.That(result, Is.GreaterThan(0).And.LessThan(10));
        // Assert.That(result, Is.Not.EqualTo(10));
    }

    // ✅ GOOD: String constraints (very powerful)
    // [Test]
    public void StringAssertions()
    {
        var text = "Hello World";

        // Assert.That(text, Is.EqualTo("Hello World"));
        // Assert.That(text, Does.StartWith("Hello"));
        // Assert.That(text, Does.EndWith("World"));
        // Assert.That(text, Does.Contain("lo Wo"));
        // Assert.That(text, Does.Match(@"^Hello.*World\$"));
        // Assert.That(text, Is.Not.Empty);

        // Case-insensitive
        // Assert.That(text, Is.EqualTo("hello world").IgnoreCase);
    }

    // ✅ GOOD: Collection constraints (extensive)
    // [Test]
    public void CollectionAssertions()
    {
        var numbers = new List<int> { 1, 2, 3, 4, 5 };

        // Assert.That(numbers, Has.Count.EqualTo(5));
        // Assert.That(numbers, Has.Member(3));
        // Assert.That(numbers, Does.Contain(3));
        // Assert.That(numbers, Does.Not.Contain(10));
        // Assert.That(numbers, Is.Ordered);
        // Assert.That(numbers, Is.All.GreaterThan(0));
        // Assert.That(numbers, Is.Unique);

        // Range checks
        // Assert.That(numbers, Has.Some.GreaterThan(3));
        // Assert.That(numbers, Has.None.Negative);
    }

    // ✅ GOOD: Exception assertions
    // [Test]
    public void ExceptionAssertions()
    {
        var calculator = new Calculator();

        // Assert.That(() => calculator.Divide(10, 0), 
        //     Throws.TypeOf<DivideByZeroException>());

        // With message check
        // Assert.That(() => calculator.ValidatePositive(-1),
        //     Throws.ArgumentException.With.Message.Contains("must be positive"));
    }

    // ✅ GOOD: Null and type assertions
    // [Test]
    public void NullAndTypeAssertions()
    {
        object? obj = "test";

        // Assert.That(obj, Is.Not.Null);
        // Assert.That(GetNull(), Is.Null);
        // Assert.That(obj, Is.TypeOf<string>());
        // Assert.That(obj, Is.InstanceOf<object>());
    }

    private object? GetNull() => null;
}

/// <summary>
/// EXAMPLE 4: TESTCASE - Parameterized Tests
/// 
/// THE PROBLEM:
/// Multiple test methods for similar scenarios cause duplication.
/// 
/// THE SOLUTION:
/// [TestCase] attribute provides inline test data.
/// Similar to xUnit's [InlineData] but with more features:
/// - Named parameters (ExpectedResult)
/// - Test name customization
/// - Category, Description, Explicit per case
/// 
/// WHY IT MATTERS:
/// - Reduce test code by 70%
/// - Add test cases without new methods
/// - Better test coverage with less effort
/// 
/// ADVANTAGE OVER XUNIT: ExpectedResult keeps assertion implicit
/// </summary>
// [TestFixture]
public class TestCaseExamples
{
    // ❌ BAD: Separate methods for each case
    // [Test]
    public void Add_2_And_3_Returns_5()
    {
        // Assert.That(new Calculator().Add(2, 3), Is.EqualTo(5));
    }

    // [Test]
    public void Add_10_And_20_Returns_30()
    {
        // Assert.That(new Calculator().Add(10, 20), Is.EqualTo(30));
    }

    // ✅ GOOD: Single method with multiple test cases
    // [TestCase(2, 3, 5)]
    // [TestCase(10, 20, 30)]
    // [TestCase(-5, 5, 0)]
    // [TestCase(0, 0, 0)]
    public void Add_VariousInputs_ReturnsExpectedSum(int a, int b, int expected)
    {
        var calculator = new Calculator();
        var result = calculator.Add(a, b);
        // Assert.That(result, Is.EqualTo(expected));
    }

    // ✅ GOOD: Using ExpectedResult (implicit assertion)
    // [TestCase(2, 3, ExpectedResult = 5)]
    // [TestCase(10, 20, ExpectedResult = 30)]
    // [TestCase(-5, 5, ExpectedResult = 0)]
    public int Add_WithExpectedResult_ReturnsSum(int a, int b)
    {
        return new Calculator().Add(a, b);
        // No explicit Assert needed - NUnit compares return value to ExpectedResult
    }

    // ✅ GOOD: TestCase with descriptive names
    // [TestCase(2, true, TestName = "IsEven_EvenNumber_ReturnsTrue")]
    // [TestCase(3, false, TestName = "IsEven_OddNumber_ReturnsFalse")]
    public void IsEven_TestCaseWithCustomName(int number, bool expected)
    {
        var result = new Calculator().IsEven(number);
        // Assert.That(result, Is.EqualTo(expected));
    }

    // ✅ GOOD: Complex data with TestCaseSource
    // [TestCaseSource(nameof(GetOrderTestCases))]
    public void ProcessOrder_VariousOrders_ProcessesCorrectly(Order order, bool expected)
    {
        var service = new OrderService();
        var result = service.ProcessOrder(order);
        // Assert.That(result, Is.EqualTo(expected));
    }

    private static IEnumerable<TestCaseData> GetOrderTestCases()
    {
        yield return new TestCaseData(new Order { Total = 100 }, true)
            .SetName("ProcessOrder_ValidOrder");
        yield return new TestCaseData(new Order { Total = 0 }, false)
            .SetName("ProcessOrder_ZeroTotal");
    }
}

/// <summary>
/// EXAMPLE 5: TEST ORGANIZATION - Categories and Attributes
/// 
/// THE PROBLEM:
/// Large test suites need organization for:
/// - Running specific subsets (unit vs integration)
/// - CI pipeline stages (fast vs slow tests)
/// - Feature-specific test groups
/// 
/// THE SOLUTION:
/// Use [Category] to tag tests, then run by category:
/// - dotnet test --filter TestCategory=Unit
/// - [Explicit] for tests that need manual triggering
/// - [Ignore] for temporarily disabled tests
/// 
/// WHY IT MATTERS:
/// - CI runs fast unit tests first (feedback in 30s vs 5min)
/// - Integration tests run separately (require infrastructure)
/// - Team can focus on specific areas
/// 
/// BEST PRACTICE: Use consistent category names across team
/// </summary>
// [TestFixture]
// [Category("Unit")]
public class CategoryExamples
{
    // [Test]
    // [Category("Fast")]
    public void UnitTest_RunsInCI()
    {
        // Fast, no dependencies
        // Assert.That(new Calculator().Add(1, 1), Is.EqualTo(2));
    }

    // [Test]
    // [Category("Integration")]
    // [Category("Slow")]
    public void IntegrationTest_RequiresDatabase()
    {
        // Slow, needs infrastructure
        // Run separately: dotnet test --filter TestCategory=Integration
        var db = new DatabaseConnection();
        // Assert.That(db, Is.Not.Null);
    }

    // [Test]
    // [Explicit("Requires manual setup")]
    // [Category("Manual")]
    public void ManualTest_RequiresHumanAction()
    {
        // Won't run automatically - must explicitly request
        Console.WriteLine("This test requires manual verification");
    }

    // [Test]
    // [Ignore("Known issue - ticket #1234")]
    public void TemporarilyDisabled()
    {
        // Skipped in all runs - use for known failures
        // Assert.Fail("This would fail");
    }

    // [Test]
    // [MaxTime(100)] // Test must complete within 100ms
    // [Category("Performance")]
    public void PerformanceTest_MustBeFast()
    {
        var calculator = new Calculator();
        for (int i = 0; i < 1000; i++)
        {
            calculator.Add(i, i);
        }
        // Test fails if takes > 100ms
    }
}

/// <summary>
/// EXAMPLE 6: ASYNC TESTING
/// 
/// THE PROBLEM:
/// Testing async code requires proper async/await patterns.
/// Blocking calls (.Result) cause deadlocks.
/// 
/// THE SOLUTION:
/// - Test methods return Task
/// - Use async/await properly
/// - Use Throws.TypeOf for async exceptions
/// 
/// WHY IT MATTERS:
/// - Correct async behavior testing
/// - No deadlocks
/// - Real-world async patterns verified
/// </summary>
// [TestFixture]
public class AsyncTestExamples
{
    // ❌ BAD: Blocking async with .Result
    // [Test]
    public void BadAsyncTest()
    {
        var service = new OrderService();
        var result = service.ProcessOrderAsync(new Order { Total = 100 }).Result;
        // Assert.That(result, Is.True);
    }

    // ✅ GOOD: Proper async test
    // [Test]
    public async Task GoodAsyncTest()
    {
        var service = new OrderService();
        var order = new Order { Total = 100 };

        var result = await service.ProcessOrderAsync(order);

        // Assert.That(result, Is.True);
    }

    // ✅ GOOD: Async exception testing
    // [Test]
    public void AsyncException_UsesThrowsAsync()
    {
        var service = new OrderService();
        var invalidOrder = new Order { Id = -1 };

        // Assert.That(async () => await service.ProcessOrderAsync(invalidOrder),
        //     Throws.TypeOf<ArgumentException>());
    }

    // ✅ GOOD: Testing timeout behavior
    // [Test]
    // [Timeout(5000)] // Test must complete within 5 seconds
    public async Task LongRunningTest_MustCompleteInTime()
    {
        await Task.Delay(100);
        // Assert.Pass();
    }
}

/// <summary>
/// EXAMPLE 8: COLLECTION TESTING - NUnit Constraint Model
/// 
/// THE PROBLEM:
/// Testing collections with basic assert methods is verbose and unclear.
/// NUnit's power comes from its constraint-based collection assertions.
/// 
/// THE SOLUTION:
/// - Assert.That with Is.EquivalentTo (ignores order)
/// - Assert.That with Is.EqualTo (checks order)
/// - Assert.That with Has.Member / Contains.Item
/// - Assert.That with Has.Count / Is.Empty
/// - Assert.That with All.GreaterThan() for predicates
/// 
/// WHY IT MATTERS:
/// - NUnit's constraint model is the most expressive for collections
/// - Better error messages showing expected vs actual
/// - More readable test intent
/// 
/// BEST PRACTICE: Use constraint-based assertions over classic Assert methods
/// </summary>
// [TestFixture]
public class CollectionTestingExamples
{
    // ❌ BAD: Manual loop verification
    // [Test]
    public void GetUsers_ContainsAlice_ManualLoop()
    {
        var userService = new UserService();
        var users = userService.GetUsers();

        bool found = false;
        foreach (var user in users)
        {
            if (user.Name == "Alice") found = true;
        }
        // Assert.That(found, Is.True); // Unclear what we're checking
    }

    // ✅ GOOD: Has.Member constraint
    // [Test]
    public void GetUsers_ContainsUserWithNameAlice()
    {
        var userService = new UserService();
        var users = userService.GetUsers();

        // Assert.That(users, Has.Some.Property("Name").EqualTo("Alice"));
    }

    // ✅ GOOD: Collection equality (order matters)
    // [Test]
    public void GetNumbers_ReturnsExpectedSequence()
    {
        var service = new MathService();
        var expected = new[] { 1, 2, 3, 4, 5 };

        var actual = service.GetFirstFiveNumbers();

        // Assert.That(actual, Is.EqualTo(expected)); // Order AND content must match
    }

    // ✅ GOOD: Collection equivalence (order doesn't matter)
    // [Test]
    public void GetUsers_ContainsExpectedUsers_AnyOrder()
    {
        var userService = new UserService();

        var users = userService.GetUsers();

        // Assert.That(users.Select(u => u.Name), 
        //     Is.EquivalentTo(new[] { "Alice", "Bob", "Charlie" }));
    }

    // ✅ GOOD: All items match predicate
    // [Test]
    public void GetActiveUsers_AllHaveActiveStatus()
    {
        var userService = new UserService();
        var activeUsers = userService.GetActiveUsers();

        // Assert.That(activeUsers, Has.All.Property("IsActive").True);
    }

    // ✅ GOOD: Count and Empty assertions
    // [Test]
    public void GetDeletedUsers_ReturnsEmptyList()
    {
        var userService = new UserService();
        var deletedUsers = userService.GetDeletedUsers();

        // Assert.That(deletedUsers, Is.Empty);
        // Assert.That(deletedUsers, Has.Count.EqualTo(0));
    }

    // ✅ GOOD: Contains.Item for single value
    // [Test]
    public void GetNumbers_ContainsThree()
    {
        var service = new MathService();
        var numbers = service.GetFirstFiveNumbers();

        // Assert.That(numbers, Contains.Item(3));
        // Assert.That(numbers, Does.Contain(3)); // Alternative syntax
    }
}

/// <summary>
/// EXAMPLE 9: STRING TESTING - NUnit String Constraints
/// 
/// THE PROBLEM:
/// String comparison needs case sensitivity control, substring matching,
/// regex patterns, and clear failure messages.
/// 
/// THE SOLUTION:
/// NUnit has specialized string constraints:
/// - Is.EqualTo with .IgnoreCase
/// - Does.Contain / Does.StartWith / Does.EndWith
/// - RegexConstraint for pattern matching
/// - EqualIgnoringCase for direct comparison
/// 
/// WHY IT MATTERS:
/// - String bugs are common (case, whitespace, null vs empty)
/// - NUnit's constraints give clear, readable string tests
/// - Better error messages show string differences
/// 
/// TIP: For JSON strings, consider using JToken.DeepEquals instead
/// </summary>
// [TestFixture]
public class StringTestingExamples
{
    // ❌ BAD: Using boolean equality
    // [Test]
    public void GenerateGreeting_BadApproach()
    {
        var service = new MessageService();
        var message = service.GenerateGreeting("John");

        // Assert.That(message == "Hello, John!", Is.True); // No diff on failure
    }

    // ✅ GOOD: Direct equality with Is.EqualTo
    // [Test]
    public void GenerateGreeting_ReturnsCorrectMessage()
    {
        var service = new MessageService();

        var message = service.GenerateGreeting("John");

        // Assert.That(message, Is.EqualTo("Hello, John!"));
    }

    // ✅ GOOD: Case-insensitive comparison
    // [Test]
    public void GenerateGreeting_IsCaseInsensitive()
    {
        var service = new MessageService();

        var message = service.GenerateGreeting("john");

        // Assert.That(message, Is.EqualTo("Hello, John!").IgnoreCase);
    }

    // ✅ GOOD: Substring matching with Does
    // [Test]
    public void GenerateReport_ContainsExpectedParts()
    {
        var service = new ReportService();

        var report = service.GenerateReport("Alice", 42);

        // Assert.That(report, Does.Contain("Alice"));
        // Assert.That(report, Does.StartWith("Report for"));
        // Assert.That(report, Does.EndWith("records found."));
    }

    // ✅ GOOD: Regex pattern matching
    // [Test]
    public void GenerateId_MatchesExpectedPattern()
    {
        var service = new IdGeneratorService();

        var id = service.GenerateId();

        // Assert.That(id, Does.Match(@"USER-\d{5}"));
    }

    // ✅ GOOD: Null vs Empty distinction
    // [TestCase(null, ExpectedResult = true)]
    // [TestCase("", ExpectedResult = false)]
    // [TestCase("  ", ExpectedResult = false)]
    public bool IsNull_HandlesAllCases(string? input)
    {
        var service = new ValidationService();
        return service.IsNull(input);
    }
}

/// <summary>
/// EXAMPLE 10: NULL AND EMPTY VALUE TESTING
/// 
/// THE PROBLEM:
/// Not testing null, empty, or default values leads to NullReferenceException
/// and other runtime errors in production.
/// 
/// THE SOLUTION:
/// - Assert.That with Is.Null / Is.Not.Null
/// - Assert.That with Is.Empty
/// - [TestCase] with null parameters
/// - Test all edge cases explicitly
/// 
/// WHY IT MATTERS:
/// - Null reference exceptions are the #1 C# runtime error
/// - Empty collections behave differently than null
/// - Default values can cause subtle bugs
/// 
/// GOTCHA: [TestCase(null)] may need explicit type casting for method resolution
/// </summary>
// [TestFixture]
public class NullAndEmptyTestingExamples
{
    // ✅ GOOD: Test null input explicitly
    // [Test]
    public void ProcessUser_NullUser_ThrowsException()
    {
        var service = new UserProcessor();

        // Assert.That(() => service.ProcessUser(null!), 
        //     Throws.TypeOf<ArgumentNullException>());
    }

    // ✅ GOOD: TestCase with multiple null/empty scenarios
    // [TestCase(null, "john@example.com")]
    // [TestCase("", "john@example.com")]
    // [TestCase("John", null)]
    // [TestCase("John", "")]
    // [TestCase("John", "invalid")]
    public void ProcessUser_InvalidData_ThrowsException(string? name, string? email)
    {
        var service = new UserProcessor();
        var user = new User { Name = name!, Email = email! };

        // Assert.That(() => service.ProcessUser(user), 
        //     Throws.InstanceOf<ArgumentException>());
    }

    // ✅ GOOD: Test empty collections
    // [Test]
    public void CalculateTotal_EmptyOrders_ReturnsZero()
    {
        var service = new OrderCalculator();
        var emptyOrders = new List<Order>();

        var total = service.CalculateTotal(emptyOrders);

        // Assert.That(total, Is.EqualTo(0));
    }

    // ✅ GOOD: Null vs Empty assertions
    // [Test]
    public void GetDeletedUsers_ReturnsEmptyNotNull()
    {
        var userService = new UserService();
        var users = userService.GetDeletedUsers();

        // Assert.That(users, Is.Not.Null); // Must not be null
        // Assert.That(users, Is.Empty);     // But should be empty
    }
}

/// <summary>
/// EXAMPLE 11: TEST CASE SOURCE - Complex Test Data
/// 
/// THE PROBLEM:
/// [TestCase] only allows simple inline values. Complex scenarios need
/// objects, collections, or computed data.
/// 
/// THE SOLUTION:
/// - [TestCaseSource] points to a static method/property
/// - Returns IEnumerable<TestCaseData> or IEnumerable<object[]>
/// - TestCaseData allows naming, categories, and expected results
/// 
/// WHY IT MATTERS:
/// - Test complex scenarios without duplication
/// - Better test organization with descriptive names
/// - Can load data from files or external sources
/// 
/// COMPARISON TO XUNIT: Similar to [MemberData] but with more metadata options
/// </summary>
// [TestFixture]
public class TestCaseSourceExamples
{
    // ✅ GOOD: TestCaseSource with complex objects
    // [Test, TestCaseSource(nameof(GetOrderTestCases))]
    public void ValidateOrder_VariousScenarios(Order order, bool expectedValid, string expectedError)
    {
        var validator = new OrderValidator();

        var (isValid, error) = validator.Validate(order);

        // Assert.That(isValid, Is.EqualTo(expectedValid));
        // Assert.That(error, Is.EqualTo(expectedError));
    }

    private static IEnumerable<TestCaseData> GetOrderTestCases()
    {
        yield return new TestCaseData(
            new Order { Id = 1, Total = 100 },
            true,
            ""
        ).SetName("ValidOrder");

        yield return new TestCaseData(
            new Order { Id = 0, Total = 100 },
            false,
            "Order ID must be positive"
        ).SetName("InvalidId");

        yield return new TestCaseData(
            new Order { Id = 1, Total = -50 },
            false,
            "Order total cannot be negative"
        ).SetName("NegativeTotal");
    }

    // ✅ GOOD: Simple TestCaseSource with values
    // [Test, TestCaseSource(nameof(AdditionTestCases))]
    public void Add_VariousInputs_ReturnsExpectedResult(int a, int b, int expected)
    {
        var calc = new Calculator();

        var result = calc.Add(a, b);

        // Assert.That(result, Is.EqualTo(expected));
    }

    private static object[] AdditionTestCases =
    {
        new object[] { 1, 2, 3 },
        new object[] { -1, 1, 0 },
        new object[] { 0, 0, 0 },
        new object[] { int.MaxValue, 0, int.MaxValue }
    };
}

/// <summary>
/// EXAMPLE 12: OBJECT EQUALITY AND COMPARISON
/// 
/// THE PROBLEM:
/// Comparing objects by reference instead of by value leads to false failures.
/// Need to compare DTOs, domain objects, and complex types.
/// 
/// THE SOLUTION:
/// NUnit offers multiple approaches:
/// - Check individual properties  
/// - Use Is.EqualTo with proper Equals implementation
/// - Use property-based assertions
/// - Custom comparers for complex scenarios
/// 
/// WHY IT MATTERS:
/// - DTOs, ViewModels, and domain objects need value comparison
/// - Reference equality fails even when values match
/// - Proper comparison makes tests more maintainable
/// 
/// TIP: NUnit 4.0+ has Object.EquivalentTo for deep comparison
/// </summary>
// [TestFixture]
public class ObjectEqualityTestingExamples
{
    // ❌ BAD: Comparing by reference
    // [Test]
    public void CreateUser_BadReferenceComparison()
    {
        var service = new UserFactory();
        var expected = new User { Name = "John", Email = "john@example.com" };

        var actual = service.CreateUser("John", "john@example.com");

        // Assert.That(actual, Is.EqualTo(expected)); // FAILS - different references!
    }

    // ✅ GOOD: Check individual properties
    // [Test]
    public void CreateUser_ChecksAllProperties()
    {
        var service = new UserFactory();

        var actual = service.CreateUser("John", "john@example.com");

        // Assert.That(actual.Name, Is.EqualTo("John"));
        // Assert.That(actual.Email, Is.EqualTo("john@example.com"));
        // Assert.That(actual.IsActive, Is.True);
    }

    // ✅ GOOD: Property-based assertions (NUnit style)
    // [Test]
    public void CreateUser_UsesPropertyAssertions()
    {
        var service = new UserFactory();

        var actual = service.CreateUser("John", "john@example.com");

        // Assert.That(actual, Has.Property("Name").EqualTo("John")
        //                        .And.Property("Email").EqualTo("john@example.com")
        //                        .And.Property("IsActive").True);
    }

    // ✅ GOOD: Multiple assertions with Assert.Multiple (NUnit 3.13+)
    // [Test]
    public void CreateUser_WithMultipleAssertions()
    {
        var service = new UserFactory();
        var actual = service.CreateUser("John", "john@example.com");

        // Assert.Multiple(() =>
        // {
        //     Assert.That(actual.Name, Is.EqualTo("John"));
        //     Assert.That(actual.Email, Is.EqualTo("john@example.com"));
        //     Assert.That(actual.IsActive, Is.True);
        // });
        // All assertions run even if one fails - better test output!
    }
}

// Supporting classes
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Divide(int a, int b) => b == 0 ? throw new DivideByZeroException() : a / b;
    public bool IsEven(int number) => number % 2 == 0;
    public void ValidatePositive(int value)
    {
        if (value < 0) throw new ArgumentException("Value must be positive");
    }
}

public class Order
{
    public int Id { get; set; }
    public decimal Total { get; set; }
}

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
}

public class TestCaseData
{
    private readonly object[] _args;

    public TestCaseData(params object[] args)
    {
        _args = args;
    }

    public TestCaseData SetName(string name) => this;
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
        return (true, "");
    }
}

public class UserFactory
{
    public User CreateUser(string name, string email) =>
        new User { Name = name, Email = email, IsActive = true };
}
