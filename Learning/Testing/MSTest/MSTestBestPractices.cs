// ==============================================================================
// MSTEST BEST PRACTICES - Comprehensive Testing Patterns
// ==============================================================================
// WHAT IS THIS?
// -------------
// MSTest is Microsoft's native test framework with deep Visual Studio and
// Azure DevOps integration, offering structured attributes and TestContext.
//
// WHY IT MATTERS
// --------------
// ✅ TOOLING INTEGRATION: Best-in-class VS and Azure DevOps reporting
// ✅ ENTERPRISE FIT: Common choice in Microsoft-first environments
// ✅ TEST METADATA: TestContext enables richer output and attachments
// ✅ LOW FRICTION: Default templates and built-in test runner support
//
// WHEN TO USE
// -----------
// ✅ VS/Azure DevOps-centric teams
// ✅ Organizations standardizing on Microsoft tooling
// ✅ Projects needing TestContext metadata and attachments
//
// WHEN NOT TO USE
// ---------------
// ❌ Minimal-ceremony test suites (xUnit may be simpler)
// ❌ Teams preferring constraint-based assertions (NUnit)
//
// REAL-WORLD EXAMPLE
// ------------------
// Enterprise line-of-business app:
// - Runs in Azure DevOps with TRX reporting
// - Relies on TestContext for diagnostics and attachments
// - Uses MSTest for consistent developer onboarding
//
// WHY MSTEST:
//   - Built into Visual Studio (zero setup for basic tests)
//   - Seamless Azure DevOps integration
//   - TestContext for rich test metadata
//   - Best Live Unit Testing support in VS Enterprise
//   - Familiar if coming from MSTest v1
//
// WHAT YOU'LL LEARN:
//   1. [TestClass] and [TestMethod] (both required!)
//   2. [TestInitialize]/[TestCleanup] for setup/teardown
//   3. [ClassInitialize]/[ClassCleanup] for expensive setup
//   4. [DataTestMethod] and [DataRow] for parameterized tests
//   5. Assert methods (Assert.AreEqual, Assert.IsTrue, etc.)
//   6. [TestCategory] for test organization
//   7. TestContext for test metadata and output
//
// INSTALLATION:
//   dotnet add package MSTest
//   dotnet add package MSTest.TestAdapter
//   dotnet add package Microsoft.NET.Test.Sdk
//
// COMPARISON TO XUNIT/NUNIT:
//   - More ceremony ([TestClass] required, xUnit doesn't need)
//   - Static ClassInitialize (xUnit/NUnit use instance methods)
//   - TestContext unique feature for test metadata
//   - Best Azure DevOps reporting
//
// ==============================================================================

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RevisionNotesDemo.Testing.MSTest;

/// <summary>
/// EXAMPLE 1: TEST CLASS AND BASIC TESTS
/// 
/// THE PROBLEM:
/// MSTest requires BOTH [TestClass] AND [TestMethod] attributes.
/// Forgetting either means tests won't be discovered.
/// This is stricter than xUnit (no attributes needed) or NUnit (just [TestFixture]).
/// 
/// THE SOLUTION:
/// Always mark:
/// - Test classes with [TestClass]
/// - Test methods with [TestMethod]
/// 
/// WHY IT MATTERS:
/// - Tests must be discoverable
/// - Without [TestClass], entire class is skipped
/// - Without [TestMethod], individual tests are skipped
/// - No error, just silent failure
/// 
/// GOTCHA: Public classes without [TestClass] are ignored (intentional in MSTest)
/// </summary>
// [TestClass]
public class BasicTestExamples
{
    // ❌ BAD: Missing [TestMethod] - won't run
    public void ThisWontRun()
    {
        // Assert.AreEqual(5, 2 + 3);
    }

    // ✅ GOOD: Proper MSTest attributes
    // [TestMethod]
    public void Add_TwoPositiveNumbers_ReturnsSum()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var result = calculator.Add(2, 3);

        // Assert
        // Assert.AreEqual(5, result);
    }

    // ✅ GOOD: Test with descriptive name
    // [TestMethod]
    public void Divide_ByZero_ThrowsDivideByZeroException()
    {
        var calculator = new Calculator();

        // Assert.ThrowsException<DivideByZeroException>(() => calculator.Divide(10, 0));
    }
}

/// <summary>
/// EXAMPLE 2: SETUP AND CLEANUP - Four Levels
/// 
/// THE PROBLEM:
/// MSTest has four lifecycle methods, each for different scenarios.
/// Using wrong one causes performance problems or test isolation issues.
/// 
/// THE SOLUTION:
/// Four levels (most to least frequent):
/// 1. [TestInitialize]/[TestCleanup] - Before/after EACH test
/// 2. [ClassInitialize]/[ClassCleanup] - Before/after ALL tests in class
/// 3. [AssemblyInitialize]/[AssemblyCleanup] - Before/after ALL tests in assembly
/// 
/// WHY IT MATTERS:
/// - Expensive setup (database) goes in ClassInitialize (runs once)
/// - Test-specific setup goes in TestInitialize (runs per test)
/// - Wrong choice = 10x slower tests or shared state bugs
/// 
/// EXECUTION ORDER:
/// ClassInitialize → TestInitialize → Test → TestCleanup → (repeat) → ClassCleanup
/// 
/// GOTCHA: ClassInitialize must be static and take TestContext parameter!
/// </summary>
// [TestClass]
public class SetupCleanupExamples
{
    private Calculator? _calculator;
    private static DatabaseConnection? _sharedDb;
    public TestContext? TestContext { get; set; }

    // ✅ Runs ONCE before all tests in class (MUST BE STATIC)
    // [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        _sharedDb = new DatabaseConnection();
        _sharedDb.Open();
        _sharedDb.SeedTestData();
        Console.WriteLine("ClassInitialize: Database setup (expensive, once)");
    }

    // ✅ Runs BEFORE EACH test
    // [TestInitialize]
    public void TestInit()
    {
        _calculator = new Calculator();
        Console.WriteLine($"TestInitialize: Fresh calculator for {TestContext?.TestName}");
    }

    // [TestMethod]
    public void Test1_HasFreshCalculator()
    {
        // Assert.IsNotNull(_calculator);
        // Assert.IsNotNull(_sharedDb);
    }

    // [TestMethod]
    public void Test2_AlsoGetsFreshCalculator()
    {
        // TestInitialize ran again - new instance
        // Assert.IsNotNull(_calculator);
    }

    // ✅ Runs AFTER EACH test
    // [TestCleanup]
    public void TestCleanup()
    {
        _calculator = null;
        Console.WriteLine("TestCleanup: Cleaned up test resources");
    }

    // ✅ Runs ONCE after all tests (MUST BE STATIC)
    // [ClassCleanup]
    public static void ClassCleanup()
    {
        _sharedDb?.Close();
        _sharedDb = null;
        Console.WriteLine("ClassCleanup: Closed database");
    }
}

/// <summary>
/// EXAMPLE 3: ASSERTIONS - MSTest Style
/// 
/// THE PROBLEM:
/// MSTest has different assertion methods than xUnit/NUnit.
/// Using wrong methods or poor messages makes debugging hard.
/// 
/// THE SOLUTION:
/// Use specific assertion methods with custom messages:
/// - Assert.AreEqual for equality
/// - Assert.IsTrue/IsFalse for boolean
/// - Assert.IsNull/IsNotNull for null checks
/// - Assert.ThrowsException for exceptions
/// - Always add message parameter for better failures
/// 
/// WHY IT MATTERS:
/// - Clear assertion type shows test intent
/// - Custom messages help debugging
/// - Better than generic Assert.IsTrue(x == y)
/// 
/// TIP: Message parameter is optional but highly recommended
/// </summary>
// [TestClass]
public class AssertionExamples
{
    // ❌ BAD: Generic assertion without message
    // [TestMethod]
    public void BadAssertion()
    {
        var result = new Calculator().Add(2, 3);
        // Assert.IsTrue(result == 5); // Poor error message
    }

    // ✅ GOOD: Specific assertion with message
    // [TestMethod]
    public void GoodAssertion()
    {
        var result = new Calculator().Add(2, 3);
        // Assert.AreEqual(5, result, "Addition failed for 2 + 3");
    }

    // ✅ GOOD: Value comparisons
    // [TestMethod]
    public void ValueAssertions()
    {
        int result = 5;

        // Assert.AreEqual(5, result, "Values should be equal");
        // Assert.AreNotEqual(10, result);
        // Assert.IsTrue(result > 0, "Result should be positive");
        // Assert.IsFalse(result < 0);
    }

    // ✅ GOOD: String assertions
    // [TestMethod]
    public void StringAssertions()
    {
        string text = "Hello World";

        // Assert.AreEqual("Hello World", text);
        // StringAssert.StartsWith(text, "Hello");
        // StringAssert.EndsWith(text, "World");
        // StringAssert.Contains(text, "lo Wo");
        // StringAssert.Matches(text, new System.Text.RegularExpressions.Regex(@"^Hello.*World\$"));
    }

    // ✅ GOOD: Collection assertions
    // [TestMethod]
    public void CollectionAssertions()
    {
        var numbers = new List<int> { 1, 2, 3, 4, 5 };
        var expected = new List<int> { 1, 2, 3, 4, 5 };

        // CollectionAssert.AreEqual(expected, numbers);
        // CollectionAssert.Contains(numbers, 3);
        // CollectionAssert.DoesNotContain(numbers, 10);
        // CollectionAssert.AllItemsAreNotNull(numbers);
        // CollectionAssert.AllItemsAreUnique(numbers);
    }

    // ✅ GOOD: Exception assertions
    // [TestMethod]
    public void ExceptionAssertions()
    {
        var calculator = new Calculator();

        // var ex = Assert.ThrowsException<DivideByZeroException>(
        //     () => calculator.Divide(10, 0),
        //     "Should throw when dividing by zero");

        // Assert.IsNotNull(ex);
    }

    // ✅ GOOD: Null and type assertions
    // [TestMethod]
    public void NullAndTypeAssertions()
    {
        object obj = "test";

        // Assert.IsNotNull(obj);
        // Assert.IsNull(GetNull());
        // Assert.IsInstanceOfType(obj, typeof(string));
        // Assert.IsNotInstanceOfType(obj, typeof(int));
    }

    private object? GetNull() => null;
}

/// <summary>
/// EXAMPLE 4: DATA-DRIVEN TESTS - MSTest Style
/// 
/// THE PROBLEM:
/// Multiple test methods for similar scenarios cause code duplication.
/// 
/// THE SOLUTION:
/// Use [DataTestMethod] with [DataRow] for inline data:
/// - [DataTestMethod] instead of [TestMethod]
/// - [DataRow(...)] for each test case
/// - Parameters match DataRow values
/// 
/// WHY IT MATTERS:
/// - Reduce test code by 70%
/// - Add test cases quickly
/// - Each DataRow runs as separate test
/// 
/// COMPARISON:
/// - xUnit: [Theory] + [InlineData]
/// - NUnit: [TestCase]
/// - MSTest: [DataTestMethod] + [DataRow]
/// 
/// GOTCHA: Must use [DataTestMethod], not [TestMethod]!
/// </summary>
// [TestClass]
public class DataDrivenTestExamples
{
    // ❌ BAD: Separate methods for each case
    // [TestMethod]
    public void Add_2_And_3_Returns_5()
    {
        // Assert.AreEqual(5, new Calculator().Add(2, 3));
    }

    // [TestMethod]
    public void Add_10_And_20_Returns_30()
    {
        // Assert.AreEqual(30, new Calculator().Add(10, 20));
    }

    // ✅ GOOD: Single method with multiple DataRows
    // [DataTestMethod]
    // [DataRow(2, 3, 5)]
    // [DataRow(10, 20, 30)]
    // [DataRow(-5, 5, 0)]
    // [DataRow(0, 0, 0)]
    public void Add_VariousInputs_ReturnsExpectedSum(int a, int b, int expected)
    {
        var calculator = new Calculator();
        var result = calculator.Add(a, b);
        // Assert.AreEqual(expected, result, $"Failed for {a} + {b}");
    }

    // ✅ GOOD: DataRow with display name
    // [DataTestMethod]
    // [DataRow(2, true, DisplayName = "IsEven_EvenNumber_ReturnsTrue")]
    // [DataRow(3, false, DisplayName = "IsEven_OddNumber_ReturnsFalse")]
    // [DataRow(0, true, DisplayName = "IsEven_Zero_ReturnsTrue")]
    public void IsEven_VariousNumbers_ReturnsExpected(int number, bool expected)
    {
        var result = new Calculator().IsEven(number);
        // Assert.AreEqual(expected, result);
    }

    // ✅ GOOD: Dynamic data with DynamicData
    // [DataTestMethod]
    // [DynamicData(nameof(GetOrderTestData), DynamicDataSourceType.Method)]
    public void ProcessOrder_VariousOrders_ProcessesCorrectly(Order order, bool expected)
    {
        var service = new OrderService();
        var result = service.ProcessOrder(order);
        // Assert.AreEqual(expected, result);
    }

    public static IEnumerable<object[]> GetOrderTestData()
    {
        yield return new object[] { new Order { Total = 100 }, true };
        yield return new object[] { new Order { Total = 0 }, false };
        yield return new object[] { new Order { Total = -10 }, false };
    }
}

/// <summary>
/// EXAMPLE 5: TEST ORGANIZATION - Categories and Properties
/// 
/// THE PROBLEM:
/// Large test suites need organization for selective test runs:
/// - CI runs fast unit tests, slow integration tests separately
/// - Feature teams run their specific tests
/// - Performance tests run on schedule
/// 
/// THE SOLUTION:
/// Use [TestCategory] to tag tests:
/// - dotnet test --filter TestCategory=Unit
/// - [Ignore] for temporarily disabled tests
/// - [Priority] for test execution order
/// 
/// WHY IT MATTERS:
/// - CI feedback in 30s (unit) vs 5min (all tests)
/// - Team focuses on relevant tests
/// - Better Azure DevOps reporting
/// 
/// MSTEST ADVANTAGE: Best Azure DevOps integration for categories
/// </summary>
// [TestClass]
// [TestCategory("Unit")]
public class TestOrganizationExamples
{
    // [TestMethod]
    // [TestCategory("Fast")]
    // [Priority(1)]
    public void UnitTest_RunsFirst()
    {
        // Fast, no dependencies
        // Assert.AreEqual(2, new Calculator().Add(1, 1));
    }

    // [TestMethod]
    // [TestCategory("Integration")]
    // [TestCategory("Slow")]
    // [Priority(2)]
    public void IntegrationTest_RequiresDatabase()
    {
        // Slow, needs infrastructure
        // Run: dotnet test --filter TestCategory=Integration
        var db = new DatabaseConnection();
        // Assert.IsNotNull(db);
    }

    // [TestMethod]
    // [Ignore("Known bug - ticket #1234")]
    public void TemporarilyDisabled()
    {
        // Skipped in all runs
        // Assert.Fail("This would fail");
    }

    // [TestMethod]
    // [TestCategory("Performance")]
    // [Timeout(100)] // Must complete within 100ms
    public void PerformanceTest_MustBeFast()
    {
        var calculator = new Calculator();
        for (int i = 0; i < 1000; i++)
        {
            calculator.Add(i, i);
        }
    }
}

/// <summary>
/// EXAMPLE 6: TESTCONTEXT - MSTest's Unique Feature
/// 
/// THE PROBLEM:
/// Need access to test metadata (test name, properties, results).
/// Need to write output from tests (beyond Console.WriteLine).
/// 
/// THE SOLUTION:
/// TestContext property provides:
/// - TestName - current test name
/// - WriteLine() - output to test results
/// - Properties - custom test properties
/// - Test results and timing
/// 
/// WHY IT MATTERS:
/// - Rich test output in Azure DevOps
/// - Conditional logic based on test name
/// - Attach files to test results
/// 
/// UNIQUE TO MSTEST: xUnit/NUnit don't have equivalent
/// </summary>
// [TestClass]
public class TestContextExamples
{
    public TestContext? TestContext { get; set; }

    // [TestMethod]
    public void TestContext_ProvidesMetadata()
    {
        // Access test information
        Console.WriteLine($"Running: {TestContext?.TestName}");
        Console.WriteLine($"Test directory: {TestContext?.TestRunDirectory}");

        // Write to test results (visible in Azure DevOps)
        TestContext?.WriteLine("Custom test output");
        TestContext?.WriteLine($"Timestamp: {DateTime.Now}");

        // Add properties
        TestContext?.Properties["CustomProperty"] = "CustomValue";

        // Assert.IsNotNull(TestContext);
    }

    // [TestMethod]
    // [TestProperty("Author", "John Doe")]
    // [TestProperty("Feature", "Calculator")]
    public void TestWithProperties()
    {
        // Properties visible in test results
        var author = TestContext?.Properties["Author"];
        TestContext?.WriteLine($"Test author: {author}");

        // Assert.IsNotNull(author);
    }

    // [TestMethod]
    public void TestContext_AttachFiles()
    {
        // Create test artifact
        string filePath = "test-output.txt";
        System.IO.File.WriteAllText(filePath, "Test output data");

        // Attach to test results (Azure DevOps)
        TestContext?.AddResultFile(filePath);

        TestContext?.WriteLine($"Attached file: {filePath}");
    }
}

/// <summary>
/// EXAMPLE 7: ASYNC TESTING
/// 
/// THE PROBLEM:
/// Testing async code requires proper async/await patterns.
/// Blocking calls cause deadlocks.
/// 
/// THE SOLUTION:
/// - Test methods return Task
/// - Use async/await properly
/// - Assert.ThrowsExceptionAsync for async exceptions
/// 
/// WHY IT MATTERS:
/// - Correct async behavior verification
/// - No deadlocks
/// - Real-world patterns tested
/// </summary>
// [TestClass]
public class AsyncTestExamples
{
    // ❌ BAD: Blocking async with .Result
    // [TestMethod]
    public void BadAsyncTest()
    {
        var service = new OrderService();
        var result = service.ProcessOrderAsync(new Order { Total = 100 }).Result;
        // Assert.IsTrue(result);
    }

    // ✅ GOOD: Proper async test
    // [TestMethod]
    public async Task GoodAsyncTest()
    {
        var service = new OrderService();
        var order = new Order { Total = 100 };

        var result = await service.ProcessOrderAsync(order);

        // Assert.IsTrue(result);
    }

    // ✅ GOOD: Async exception testing
    // [TestMethod]
    public async Task AsyncException_ThrowsExpectedException()
    {
        var service = new OrderService();
        var invalidOrder = new Order { Id = -1 };

        // await Assert.ThrowsExceptionAsync<ArgumentException>(
        //     async () => await service.ProcessOrderAsync(invalidOrder));
    }
}

/// <summary>
/// EXAMPLE 8: COLLECTION TESTING - MSTest Collection Assertions
/// 
/// THE PROBLEM:
/// Testing collections with basic asserts is verbose and doesn't show clear failures.
/// MSTest has CollectionAssert for specialized collection operations.
/// 
/// THE SOLUTION:
/// - CollectionAssert.Contains for item presence
/// - CollectionAssert.AreEqual for exact match (order matters)
/// - CollectionAssert.AreEquivalent for same items (order doesn't matter)
/// - CollectionAssert.AllItemsAreNotNull
/// - CollectionAssert.AllItemsAreUnique
/// 
/// WHY IT MATTERS:
/// - Clear test intent with specialized assertions
/// - Better failure messages showing differences
/// - Readable collection tests without loops
/// 
/// TIP: Use CollectionAssert over manual loops - it's faster and clearer
/// </summary>
// [TestClass]
public class CollectionTestingExamples
{
    // ❌ BAD: Manual loop for verification
    // [TestMethod]
    public void GetUsers_ContainsAlice_ManualLoop()
    {
        var userService = new UserService();
        var users = userService.GetUsers();

        bool found = false;
        foreach (var user in users)
        {
            if (user.Name == "Alice") found = true;
        }
        // Assert.IsTrue(found); // Unclear what we're checking
    }

    // ✅ GOOD: CollectionAssert.Contains (note: checks object reference or Equals)
    // [TestMethod]
    public void GetUsers_ContainsExpectedNames()
    {
        var userService = new UserService();
        var users = userService.GetUsers();
        var names = users.Select(u => u.Name).ToList();

        // CollectionAssert.Contains(names, "Alice");
        // CollectionAssert.Contains(names, "Bob");
    }

    // ✅ GOOD: AreEqual for exact match (order AND content)
    // [TestMethod]
    public void GetNumbers_ReturnsExpectedSequence()
    {
        var service = new MathService();
        var expected = new[] { 1, 2, 3, 4, 5 };

        var actual = service.GetFirstFiveNumbers();

        // CollectionAssert.AreEqual(expected, actual);
    }

    // ✅ GOOD: AreEquivalent (order doesn't matter)
    // [TestMethod]
    public void GetUsers_ContainsExpectedUsers_AnyOrder()
    {
        var userService = new UserService();
        var names = userService.GetUsers().Select(u => u.Name).ToList();
        var expected = new[] { "Charlie", "Alice", "Bob" }; // Different order

        // CollectionAssert.AreEquivalent(expected, names);
    }

    // ✅ GOOD: AllItemsAreNotNull
    // [TestMethod]
    public void GetUsers_AllUsersAreNotNull()
    {
        var userService = new UserService();
        var users = userService.GetUsers();

        // CollectionAssert.AllItemsAreNotNull(users);
    }

    // ✅ GOOD: Count check
    // [TestMethod]
    public void GetDeletedUsers_ReturnsEmptyList()
    {
        var userService = new UserService();
        var deletedUsers = userService.GetDeletedUsers();

        // Assert.AreEqual(0, deletedUsers.Count);
    }
}

/// <summary>
/// EXAMPLE 9: STRING TESTING - MSTest String Assertions
/// 
/// THE PROBLEM:
/// String comparison needs case handling, substring matching, and
/// clear failure messages showing differences.
/// 
/// THE SOLUTION:
/// MSTest has StringAssert class with specialized methods:
/// - StringAssert.Contains for substring
/// - StringAssert.StartsWith / EndsWith
/// - StringAssert.Matches for regex
/// - Assert.AreEqual with ignoreCase parameter
/// 
/// WHY IT MATTERS:
/// - String bugs are extremely common (case, whitespace, null)
/// - Specialized assertions give better failure messages
/// - Clear test intent without manual string manipulation
/// 
/// BEST PRACTICE: Use StringAssert over Assert.IsTrue(str.Contains(...))
/// </summary>
// [TestClass]
public class StringTestingExamples
{
    // ❌ BAD: Using Assert.IsTrue for string comparison
    // [TestMethod]
    public void GenerateGreeting_BadApproach()
    {
        var service = new MessageService();
        var message = service.GenerateGreeting("John");

        // Assert.IsTrue(message == "Hello, John!"); // No diff on failure!
    }

    // ✅ GOOD: AreEqual shows diff on failure
    // [TestMethod]
    public void GenerateGreeting_ReturnsCorrectMessage()
    {
        var service = new MessageService();

        var message = service.GenerateGreeting("John");

        // Assert.AreEqual("Hello, John!", message);
    }

    // ✅ GOOD: Case-insensitive comparison
    // [TestMethod]
    public void GenerateGreeting_IsCaseInsensitive()
    {
        var service = new MessageService();

        var message = service.GenerateGreeting("john");

        // Assert.AreEqual("Hello, John!", message, ignoreCase: true);
    }

    // ✅ GOOD: StringAssert for substring matching
    // [TestMethod]
    public void GenerateReport_ContainsExpectedParts()
    {
        var service = new ReportService();

        var report = service.GenerateReport("Alice", 42);

        // StringAssert.Contains(report, "Alice");
        // StringAssert.StartsWith(report, "Report for");
        // StringAssert.EndsWith(report, "records found.");
    }

    // ✅ GOOD: Regex matching
    // [TestMethod]
    public void GenerateId_MatchesExpectedPattern()
    {
        var service = new IdGeneratorService();

        var id = service.GenerateId();

        // StringAssert.Matches(id, new System.Text.RegularExpressions.Regex(@"USER-\d{5}"));
    }

    // ✅ GOOD: Null vs Empty distinction with DataRow
    // [DataTestMethod]
    // [DataRow(null, true)]
    // [DataRow("", false)]
    // [DataRow("  ", false)]
    public void IsNull_HandlesAllCases(string? input, bool expectedNull)
    {
        var service = new ValidationService();
        var isNull = service.IsNull(input);

        // Assert.AreEqual(expectedNull, isNull);
    }
}

/// <summary>
/// EXAMPLE 10: NULL AND EMPTY VALUE TESTING
/// 
/// THE PROBLEM:
/// Not testing null, empty, or default values is where most production
/// bugs hide. NullReferenceException is the #1 C# runtime error.
/// 
/// THE SOLUTION:
/// - Assert.IsNull / Assert.IsNotNull
/// - [DataRow] with null values for multiple scenarios
/// - Test empty collections explicitly
/// - Check default values of structs/classes
/// 
/// WHY IT MATTERS:
/// - Null-related bugs are most common in production
/// - Empty collections behave differently than null
/// - Default values can cause subtle bugs
/// 
/// GOTCHA: [DataRow(null)] might need casting for method overload resolution
/// </summary>
// [TestClass]
public class NullAndEmptyTestingExamples
{
    // ✅ GOOD: Test null input explicitly
    // [TestMethod]
    public void ProcessUser_NullUser_ThrowsArgumentNullException()
    {
        var service = new UserProcessor();

        // Assert.ThrowsException<ArgumentNullException>(() => service.ProcessUser(null!));
    }

    // ✅ GOOD: DataRow with multiple null/empty scenarios
    // [DataTestMethod]
    // [DataRow(null, "john@example.com", DisplayName = "Null name")]
    // [DataRow("", "john@example.com", DisplayName = "Empty name")]
    // [DataRow("John", null, DisplayName = "Null email")]
    // [DataRow("John", "", DisplayName = "Empty email")]
    // [DataRow("John", "invalid", DisplayName = "Invalid email")]
    public void ProcessUser_InvalidData_ThrowsException(string? name, string? email)
    {
        var service = new UserProcessor();
        var user = new User { Name = name!, Email = email! };

        // Assert.ThrowsException<ArgumentException>(() => service.ProcessUser(user));
    }

    // ✅ GOOD: Test empty collections
    // [TestMethod]
    public void CalculateTotal_EmptyOrders_ReturnsZero()
    {
        var service = new OrderCalculator();
        var emptyOrders = new List<Order>();

        var total = service.CalculateTotal(emptyOrders);

        // Assert.AreEqual(0m, total);
    }

    // ✅ GOOD: Null vs Empty distinction
    // [TestMethod]
    public void GetDeletedUsers_ReturnsEmptyNotNull()
    {
        var userService = new UserService();
        var users = userService.GetDeletedUsers();

        // Assert.IsNotNull(users); // Must not be null
        // Assert.AreEqual(0, users.Count); // But should be empty
    }
}

/// <summary>
/// EXAMPLE 11: DYNAMIC DATA - Complex Test Data
/// 
/// THE PROBLEM:
/// [DataRow] only supports simple inline values. Complex scenarios need
/// objects, collections, or computed data.
/// 
/// THE SOLUTION:
/// - [DynamicData] points to a static property or method
/// - Returns IEnumerable<object[]>
/// - Each object[] is one test case with parameters
/// - Can compute data, load from files, etc.
/// 
/// WHY IT MATTERS:
/// - Test complex scenarios without duplication
/// - Single source of truth for test data
/// - Can load from external sources (files, DB)
/// 
/// COMPARISON: Similar to xUnit's [MemberData] and NUnit's [TestCaseSource]
/// </summary>
// [TestClass]
public class DynamicDataExamples
{
    // ✅ GOOD: DynamicData with complex objects
    // [DataTestMethod]
    // [DynamicData(nameof(GetOrderTestCases), DynamicDataSourceType.Method)]
    public void ValidateOrder_VariousScenarios(Order order, bool expectedValid, string expectedError)
    {
        var validator = new OrderValidator();

        var (isValid, error) = validator.Validate(order);

        // Assert.AreEqual(expectedValid, isValid);
        // Assert.AreEqual(expectedError, error);
    }

    public static IEnumerable<object[]> GetOrderTestCases()
    {
        yield return new object[]
        {
            new Order { Id = 1, Total = 100 },
            true,
            ""
        };

        yield return new object[]
        {
            new Order { Id = 0, Total = 100 },
            false,
            "Order ID must be positive"
        };

        yield return new object[]
        {
            new Order { Id = 1, Total = -50 },
            false,
            "Order total cannot be negative"
        };
    }

    // ✅ GOOD: DynamicData from property
    // [DataTestMethod]
    // [DynamicData(nameof(AdditionTestData), DynamicDataSourceType.Property)]
    public void Add_VariousInputs_ReturnsExpectedResult(int a, int b, int expected)
    {
        var calc = new Calculator();

        var result = calc.Add(a, b);

        // Assert.AreEqual(expected, result);
    }

    public static IEnumerable<object[]> AdditionTestData
    {
        get
        {
            yield return new object[] { 1, 2, 3 };
            yield return new object[] { -1, 1, 0 };
            yield return new object[] { 0, 0, 0 };
            yield return new object[] { int.MaxValue, 0, int.MaxValue };
        }
    }
}

/// <summary>
/// EXAMPLE 12: OBJECT EQUALITY AND COMPARISON
/// 
/// THE PROBLEM:
/// Assert.AreEqual on objects checks reference equality by default.
/// Two objects with identical property values will fail the assertion.
/// 
/// THE SOLUTION:
/// Multiple approaches:
/// - Check individual properties (most explicit)
/// - Implement Equals/GetHashCode in your classes
/// - Use serialization comparison for DTOs
/// - Use custom comparers
/// 
/// WHY IT MATTERS:
/// - DTOs, ViewModels, and domain objects need value comparison
/// - Reference equality causes false test failures
/// - Proper equality is important for production code too
/// 
/// TIP: For complex comparisons, consider FluentAssertions library
/// </summary>
// [TestClass]
public class ObjectEqualityTestingExamples
{
    // ❌ BAD: Comparing by reference
    // [TestMethod]
    public void CreateUser_BadReferenceComparison()
    {
        var service = new UserFactory();
        var expected = new User { Name = "John", Email = "john@example.com" };

        var actual = service.CreateUser("John", "john@example.com");

        // Assert.AreEqual(expected, actual); // FAILS - different references!
    }

    // ✅ GOOD: Check individual properties (most explicit)
    // [TestMethod]
    public void CreateUser_ChecksAllProperties()
    {
        var service = new UserFactory();

        var actual = service.CreateUser("John", "john@example.com");

        // Assert.AreEqual("John", actual.Name);
        // Assert.AreEqual("john@example.com", actual.Email);
        // Assert.IsTrue(actual.IsActive);
    }

    // ✅ GOOD: Multiple assertions in one test
    // [TestMethod]
    public void CreateUser_WithMultipleAssertions()
    {
        var service = new UserFactory();
        var actual = service.CreateUser("John", "john@example.com");

        // All these run, but first failure stops execution
        // Assert.AreEqual("John", actual.Name);
        // Assert.AreEqual("john@example.com", actual.Email);
        // Assert.IsTrue(actual.IsActive);
    }

    // ✅ GOOD: If class implements Equals properly
    // [TestMethod]
    public void CreateUser_WithProperEquals_WorksAsExpected()
    {
        // Assuming User implements IEquatable<User>
        var service = new UserFactory();
        var expected = new User { Name = "John", Email = "john@example.com", IsActive = true };

        var actual = service.CreateUser("John", "john@example.com");

        // Would work if User has proper Equals implementation
        // Assert.AreEqual(expected, actual);
    }
}

// Supporting classes
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
