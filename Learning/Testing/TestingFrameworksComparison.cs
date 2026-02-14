// ==============================================================================
// TESTING FRAMEWORKS COMPARISON - xUnit vs NUnit vs MSTest
// Reference: Revision Notes - Unit Testing Best Practices
// ==============================================================================
// PURPOSE: Compare testing frameworks with examples
// KEY CONCEPTS: xUnit (modern), NUnit (traditional), MSTest (VS integrated)
// ==============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace RevisionNotesDemo.Testing;

// Sample class to test
public class Calculator
{
    public int Add(int a, int b) => a + b;
    public int Divide(int a, int b)
    {
        if (b == 0) throw new DivideByZeroException();
        return a / b;
    }
    public bool IsEven(int number) => number % 2 == 0;
}

public class TestingFrameworksComparison
{
    /// <summary>
    /// XUNIT EXAMPLES - Modern, recommended for new projects
    /// Installation: dotnet add package xUnit
    /// </summary>
    public class XUnitExamples
    {
        // Basic test with [Fact]
        // [Fact]
        public void Add_TwoNumbers_ReturnsSum()
        {
            // Arrange
            var calculator = new Calculator();
            
            // Act
            var result = calculator.Add(2, 3);
            
            // Assert
            // Assert.Equal(5, result);
        }
        
        // Parameterized test with [Theory]
        // [Theory]
        // [InlineData(2, true)]
        // [InlineData(3, false)]
        // [InlineData(10, true)]
        public void IsEven_VariousNumbers_ReturnsExpected(int number, bool expected)
        {
            var calculator = new Calculator();
            var result = calculator.IsEven(number);
            // Assert.Equal(expected, result);
        }
        
        // Setup/Teardown with constructor/IDisposable
        public XUnitExamples()
        {
            // Constructor = Setup
            Console.WriteLine("XUnit: Test setup (constructor)");
        }
        
        // IDisposable for teardown (if needed)
        public void Dispose()
        {
            Console.WriteLine("XUnit: Test teardown (Dispose)");
        }
    }
    
    /// <summary>
    /// NUNIT EXAMPLES - Traditional, feature-rich
    /// Installation: dotnet add package NUnit
    /// </summary>
    public class NUnitExamples
    {
        private Calculator _calculator = null!;
        
        // Setup before each test
        // [SetUp]
        public void Setup()
        {
            _calculator = new Calculator();
            Console.WriteLine("NUnit: Test setup ([SetUp])");
        }
        
        // Teardown after each test
        // [TearDown]
        public void Teardown()
        {
            Console.WriteLine("NUnit: Test teardown ([TearDown])");
        }
        
        // Basic test with [Test]
        // [Test]
        public void Add_TwoNumbers_ReturnsSum()
        {
            var result = _calculator.Add(2, 3);
            // Assert.That(result, Is.EqualTo(5));
        }
        
        // Parameterized test with [TestCase]
        // [TestCase(2, true)]
        // [TestCase(3, false)]
        // [TestCase(10, true)]
        public void IsEven_VariousNumbers_ReturnsExpected(int number, bool expected)
        {
            var result = _calculator.IsEven(number);
            // Assert.That(result, Is.EqualTo(expected));
        }
    }
    
    /// <summary>
    /// MSTEST EXAMPLES - Visual Studio integrated
    /// Installation: dotnet add package MSTest.TestFramework
    /// </summary>
    public class MSTestExamples
    {
        private Calculator _calculator = null!;
        
        // Setup before each test
        // [TestInitialize]
        public void Initialize()
        {
            _calculator = new Calculator();
            Console.WriteLine("MSTest: Test setup ([TestInitialize])");
        }
        
        // Teardown after each test
        // [TestCleanup]
        public void Cleanup()
        {
            Console.WriteLine("MSTest: Test teardown ([TestCleanup])");
        }
        
        // Basic test with [TestMethod]
        // [TestMethod]
        public void Add_TwoNumbers_ReturnsSum()
        {
            var result = _calculator.Add(2, 3);
            // Assert.AreEqual(5, result);
        }
        
        // Parameterized test with [DataRow]
        // [DataTestMethod]
        // [DataRow(2, true)]
        // [DataRow(3, false)]
        // [DataRow(10, true)]
        public void IsEven_VariousNumbers_ReturnsExpected(int number, bool expected)
        {
            var result = _calculator.IsEven(number);
            // Assert.AreEqual(expected, result);
        }
    }
    
    /// <summary>
    /// COMPARISON TABLE
    /// </summary>
    public static void ShowComparison()
    {
        Console.WriteLine("\n=== TESTING FRAMEWORKS COMPARISON ===\n");
        
        Console.WriteLine("╔════════════════╦═══════════╦═══════════╦═══════════╗");
        Console.WriteLine("║ Feature        ║ xUnit     ║ NUnit     ║ MSTest    ║");
        Console.WriteLine("╠════════════════╬═══════════╬═══════════╬═══════════╣");
        Console.WriteLine("║ Modern Design  ║ ✅ Yes    ║ ❌ No     ║ ❌ No     ║");
        Console.WriteLine("║ Parallel       ║ ✅ Default║ ⚠️ Opt-in ║ ⚠️ Opt-in ║");
        Console.WriteLine("║ Setup/Teardown ║ Constructor/IDisposable ║ [SetUp]/[TearDown] ║ [TestInitialize]/[TestCleanup] ║");
        Console.WriteLine("║ Parameterized  ║ [Theory]  ║ [TestCase]║ [DataRow] ║");
        Console.WriteLine("║ Assertion      ║ Assert.Equal ║ Assert.That ║ Assert.AreEqual ║");
        Console.WriteLine("║ Community      ║ ✅ Popular║ ✅ Popular║ ⚠️ VS-Only║");
        Console.WriteLine("║ VS Integration ║ ✅ Good   ║ ✅ Good   ║ ✅ Excel  ║");
        Console.WriteLine("╚════════════════╩═══════════╩═══════════╩═══════════╝");
        
        Console.WriteLine("\n📌 RECOMMENDATIONS:");
        Console.WriteLine("   ✅ xUnit - Best for NEW projects (modern, parallel by default)");
        Console.WriteLine("   ✅ NUnit - Great for EXISTING projects (mature, feature-rich)");
        Console.WriteLine("   ✅ MSTest - Good for VS-CENTRIC teams (tight integration)");
    }
    
    public static void RunAllExamples()
    {
        Console.WriteLine("\n=== TESTING FRAMEWORKS COMPARISON ===\n");
        ShowComparison();
        Console.WriteLine("\n💡 All frameworks are valid choices - pick based on team preference!");
        Console.WriteLine("Testing Frameworks Comparison examples completed!\n");
    }
}
