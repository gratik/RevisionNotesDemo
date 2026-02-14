# Unit Testing Examples - Complete Guide

## 📚 Overview

This folder contains **comprehensive, production-ready examples** of unit testing in .NET using all three major frameworks: **xUnit**, **NUnit**, and **MSTest**. Each example demonstrates **good vs bad practices** with extensive inline documentation following the same quality standards as the Entity Framework examples.

## 🎯 What You'll Learn

- ✅ **Test naming conventions** - Write self-documenting test names
- ✅ **AAA pattern** - Arrange, Act, Assert structure
- ✅ **Assertions** - Right way to verify outcomes
- ✅ **Parameterized tests** - Test multiple cases efficiently
- ✅ **Setup/Teardown** - Proper test lifecycle management
- ✅ **Exception testing** - Assert.Throws patterns
- ✅ **Async testing** - Proper async/await in tests
- ✅ **Test isolation** - Avoid shared state antipatterns
- ✅ **Test organization** - Class structure and categories
- ✅ **Framework comparison** - Choose the right tool

## 📂 Folder Structure

✅ **All Framework Examples Now Complete!**

```
Testing/
├── xUnit/                               # Modern, minimal (Microsoft's choice)
│   └── XUnitBestPractices.cs (17.6 KB)  # ✅ COMPLETE - 7 comprehensive examples
│       - Test naming conventions
│       - AAA Pattern
│       - [Fact] vs [Theory]
│       - Assertions (Equal, Throws, Contains, etc.)
│       - Setup/Teardown (Constructor/IDisposable)
│       - IClassFixture for shared context
│       - Async testing patterns
│
├── NUnit/                               # Mature, feature-rich (Enterprise)
│   └── NUnitBestPractices.cs (16.7 KB)  # ✅ COMPLETE - 7 comprehensive examples
│       - [TestFixture] and [Test]
│       - 4-level setup/teardown
│       - Constraint-based assertions (Assert.That)
│       - [TestCase] parameterized tests
│       - [Category] organization
│       - Async testing with Task
│       - [Explicit], [Ignore], [MaxTime]
│
├── MSTest/                              # VS native (Azure DevOps optimized)
│   └── MSTestBestPractices.cs (17.6 KB) # ✅ COMPLETE - 7 comprehensive examples
│       - [TestClass] and [TestMethod]
│       - [TestInitialize]/[ClassInitialize]
│       - MSTest assertion methods
│       - [DataTestMethod] + [DataRow]
│       - [TestCategory] and [Priority]
│       - TestContext unique features
│       - Async testing
│
├── TestingFrameworksComparison.cs (6.9 KB)   # Cross-framework comparison
├── MockingInDepthExamples.cs (11.9 KB)       # Moq patterns
├── TestingAsyncCodeExamples.cs (12.2 KB)     # Async/await testing
├── TestDataBuildersExamples.cs (12.1 KB)     # Test data patterns
├── IntegrationTestingExamples.cs (11.5 KB)   # Integration test patterns
├── README.md (8.2 KB)                        # This file
└── SETUP_GUIDE.md (5.8 KB)                   # Implementation completed!
```

**Total: 10 files, ~120 KB of comprehensive testing examples**

## 🚀 Quick Start

### Choose Your Framework

**New .NET 5+ project?** → **xUnit** (Modern, Microsoft uses it)  
**Enterprise with NUnit history?** → **NUnit** (Mature, stable)  
**Visual Studio + Azure DevOps?** → **MSTest** (Best integration)

### Installation

```bash
# xUnit
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Microsoft.NET.Test.Sdk

# NUnit
dotnet add package NUnit
dotnet add package NUnit3TestAdapter
dotnet add package Microsoft.NET.Test.Sdk

# MSTest
dotnet add package MSTest.TestFramework
dotnet add package MSTest.TestAdapter
dotnet add package Microsoft.NET.Test.Sdk
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific framework tests
dotnet test --filter FullyQualifiedName~xUnit
dotnet test --filter FullyQualifiedName~NUnit
dotnet test --filter FullyQualifiedName~MSTest

# Run by category
dotnet test --filter "TestCategory=Unit"
dotnet test --filter "Category=Integration"

# Verbose output
dotnet test --logger "console;verbosity=detailed"
```

## 📖 Framework Highlights

### xUnit - The Modern Choice

```csharp
public class CalculatorTests
{
    [Fact]  // Simple test
    public void Add_TwoNumbers_ReturnsSum()
    {
        var calc = new Calculator();
        var result = calc.Add(2, 3);
        Assert.Equal(5, result);
    }

    [Theory]  // Parameterized test
    [InlineData(2, 3, 5)]
    [InlineData(10, 5, 15)]
    public void Add_VariousInputs(int a, int b, int expected)
    {
        var calc = new Calculator();
        Assert.Equal(expected, calc.Add(a, b));
    }
}
```

**Why xUnit:**

- ✅ Used by Microsoft for .NET Core
- ✅ Parallel execution by default
- ✅ No static state (new instance per test)
- ✅ Clean, minimal syntax

### NUnit - The Enterprise Standard

```csharp
[TestFixture]
public class CalculatorTests
{
    [Test]  // Simple test
    public void Add_TwoNumbers_ReturnsSum()
    {
        var calc = new Calculator();
        var result = calc.Add(2, 3);
        Assert.That(result, Is.EqualTo(5));  // Fluent assertions
    }

    [TestCase(2, 3, 5)]  // Parameterized test
    [TestCase(10, 5, 15)]
    public void Add_VariousInputs(int a, int b, int expected)
    {
        var calc = new Calculator();
        Assert.That(calc.Add(a, b), Is.EqualTo(expected));
    }
}
```

**Why NUnit:**

- ✅ 17+ years battle-tested
- ✅ Rich fluent assertions
- ✅ Advanced TestCase features
- ✅ Wide enterprise adoption

### MSTest - The VS Native

```csharp
[TestClass]  // REQUIRED
public class CalculatorTests
{
    [TestMethod]  // Simple test
    public void Add_TwoNumbers_ReturnsSum()
    {
        var calc = new Calculator();
        var result = calc.Add(2, 3);
        Assert.AreEqual(5, result);
    }

    [DataTestMethod]  // Parameterized test
    [DataRow(2, 3, 5)]
    [DataRow(10, 5, 15)]
    public void Add_VariousInputs(int a, int b, int expected)
    {
        var calc = new Calculator();
        Assert.AreEqual(expected, calc.Add(a, b));
    }
}
```

**Why MSTest:**

- ✅ Native Visual Studio integration
- ✅ Best Azure DevOps support
- ✅ Explicit, clear syntax
- ✅ Microsoft maintained

## 🎓 Best Practices (All Frameworks)

### ✅ DO:

1. **Name tests descriptively**: `Add_TwoNumbers_ReturnsSum`
2. **Follow AAA pattern**: Arrange, Act, Assert
3. **Keep tests independent**: No shared state
4. **One logical assert per test**: Test one thing
5. **Use parameterized tests**: Avoid duplication
6. **Categorize tests**: Unit, Integration, Slow
7. **Mock external dependencies**: Keep tests fast
8. **Test behavior, not implementation**: Public API only

### ❌ DON'T:

1. **Generic names**: Test1, Test2, TestMethod
2. **Share mutable state**: Static fields between tests
3. **Test multiple concerns**: One test, one purpose
4. **Ignore failing tests**: Fix or document why
5. **Thread.Sleep**: Use async properly
6. **Test private methods**: Test through public API
7. **Depend on test order**: Tests must be independent

## 📊 Framework Comparison Matrix

| Feature               | xUnit       | NUnit        | MSTest             |
| --------------------- | ----------- | ------------ | ------------------ |
| **Simple Test**       | `[Fact]`    | `[Test]`     | `[TestMethod]`     |
| **Parameterized**     | `[Theory]`  | `[TestCase]` | `[DataTestMethod]` |
| **Setup**             | Constructor | `[SetUp]`    | `[TestInitialize]` |
| **Teardown**          | IDisposable | `[TearDown]` | `[TestCleanup]`    |
| **Parallel Default**  | ✅ Yes      | ❌ No        | ❌ No              |
| **Fluent Assertions** | ❌          | ✅           | ❌                 |
| **VS Integration**    | ✅          | ✅           | ✅✅ Native        |
| **Learning Curve**    | Low         | Medium       | Low                |
| **Feature Richness**  | Minimal     | Rich         | Moderate           |

## 🔗 Related Topics

- **Mocking**: See `MockingInDepthExamples.cs` for Moq patterns
- **Async Testing**: See `TestingAsyncCodeExamples.cs`
- **Test Data**: See `TestDataBuildersExamples.cs`
- **Integration Tests**: See `IntegrationTestingExamples.cs`

## 📚 Additional Resources

### Official Documentation

- [xUnit Documentation](https://xunit.net/)
- [NUnit Documentation](https://nunit.org/)
- [MSTest Documentation](https://docs.microsoft.com/visualstudio/test/using-microsoft-visualstudio-testtools-unittesting-members-in-unit-tests)

### Best Practices

- Martin Fowler - Test Pyramid
- Roy Osherove - The Art of Unit Testing
- Vladimir Khorikov - Unit Testing Principles

### Complementary Tools

- **Moq** - Most popular mocking library
- **Fluent Assertions** - Better assertion syntax
- **Coverlet** - Code coverage for .NET
- **Bogus** - Test data generation

## 💡 Pro Tips

1. **Start with xUnit** if unsure - it's the modern standard
2. **Don't mix frameworks** in a single project (pick one)
3. **Use categories** to separate fast/slow tests
4. **Run fast tests** in PR builds, all tests nightly
5. **Aim for 80%+ coverage** on business logic
6. **Mock external dependencies** (DB, HTTP, file system)
7. **Keep tests fast** (< 100ms unit tests, < 1s integration)

## 🎯 Next Steps

1. **Read the framework-specific best practices** files
2. **Try the examples** - uncomment attributes and run
3. **Compare frameworks** - see FrameworkComparison.cs
4. **Apply patterns** to your own code
5. **Share with your team** - align on conventions

---

**Remember**: The best testing framework is the one your team will actually use consistently. All three are production-ready. Just write tests! 🎯

For questions or improvements, refer to the comprehensive inline documentation in each file.
