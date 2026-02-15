// ==============================================================================
// MOCKING IN DEPTH - Comprehensive Moq Examples
// Reference: Revision Notes - Unit Testing Best Practices
// ==============================================================================
// WHAT IS IT?
// -----------
// A deep dive into mocking patterns (using Moq) for isolating dependencies,
// verifying interactions, and controlling edge cases in unit tests.
//
// WHY IT MATTERS
// --------------
// ✅ ISOLATION: Test business logic without external systems
// ✅ CONTROL: Simulate failures, latency, and edge cases
// ✅ VERIFICATION: Assert critical interactions (emails sent, logs written)
// ✅ SPEED: Unit tests remain fast and deterministic
//
// WHEN TO USE
// -----------
// ✅ External dependencies: HTTP, email, queues, time, file system
// ✅ Interaction-heavy logic where side effects must be verified
// ✅ Failure path testing (exceptions, timeouts, retries)
//
// WHEN NOT TO USE
// ---------------
// ❌ Pure functions or value objects (no dependencies to mock)
// ❌ Over-mocking simple collaborators (prefer real objects)
// ❌ Integration tests (use real infrastructure or test containers)
//
// REAL-WORLD EXAMPLE
// ------------------
// User registration:
// - Mock email provider and repository
// - Verify welcome email and audit log are emitted
// - Simulate email failure to test fallback behavior
// ==============================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevisionNotesDemo.Testing;

// Interfaces to mock
public interface IEmailService
{
    bool SendEmail(string to, string subject, string body);
    Task<bool> SendEmailAsync(string to, string subject, string body);
    int GetRemainingQuota();
}

public interface ILogger
{
    void Log(string message);
    void LogError(string message, Exception ex);
}

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
}

// Example entity
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

// Service that uses dependencies
public class UserService
{
    private readonly IRepository<User> _repository;
    private readonly IEmailService _emailService;
    private readonly ILogger _logger;

    public UserService(IRepository<User> repository, IEmailService emailService, ILogger logger)
    {
        _repository = repository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<bool> CreateUserAsync(User user)
    {
        try
        {
            await _repository.AddAsync(user);
            _emailService.SendEmail(user.Email, "Welcome", "Welcome to our service!");
            _logger.Log($"User created: {user.Name}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to create user", ex);
            return false;
        }
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }
}

public class MockingInDepthExamples
{
    /// <summary>
    /// EXAMPLE 1: Basic Mocking Setup
    /// Installation: dotnet add package Moq
    /// </summary>
    public static void BasicMockingExample()
    {
        Console.WriteLine("\n=== EXAMPLE 1: Basic Mocking ===");

        // Create mock (commented out - would require Moq package)
        // var mockRepository = new Mock<IRepository<User>>();

        // Setup return value
        // mockRepository.Setup(r => r.GetByIdAsync(1))
        //     .ReturnsAsync(new User { Id = 1, Name = "John", Email = "john@example.com" });

        // Use mock
        // var service = new UserService(mockRepository.Object, null!, null!);
        // var user = await service.GetUserByIdAsync(1);

        Console.WriteLine("Basic mocking: Mock created, setup with Returns, used .Object");
    }

    /// <summary>
    /// EXAMPLE 2: Setup with It.IsAny (Argument Matchers)
    /// </summary>
    public static void ArgumentMatchersExample()
    {
        Console.WriteLine("\n=== EXAMPLE 2: Argument Matchers ===");

        // var mockEmailService = new Mock<IEmailService>();

        // Setup with It.IsAny (matches any argument)
        // mockEmailService.Setup(e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //     .Returns(true);

        // Setup with It.Is (conditional matching)
        // mockEmailService.Setup(e => e.SendEmail(It.Is<string>(email => email.Contains("@")), It.IsAny<string>(), It.IsAny<string>()))
        //     .Returns(true);

        // Setup with It.IsInRange
        // mockService.Setup(s => s.GetItems(It.IsInRange(1, 100, Range.Inclusive)))
        //     .Returns(items);

        Console.WriteLine("Argument matchers: It.IsAny, It.Is, It.IsInRange, It.IsRegex");
    }

    /// <summary>
    /// EXAMPLE 3: Throwing Exceptions
    /// </summary>
    public static void ThrowingExceptionsExample()
    {
        Console.WriteLine("\n=== EXAMPLE 3: Throwing Exceptions ===");

        // var mockRepository = new Mock<IRepository<User>>();

        // Setup to throw exception
        // mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
        //     .ThrowsAsync(new InvalidOperationException("Database error"));

        // Conditional throw
        // mockRepository.Setup(r => r.GetByIdAsync(It.Is<int>(id => id < 0)))
        //     .ThrowsAsync(new ArgumentException("Invalid ID"));

        Console.WriteLine("Throwing exceptions: .Throws(), .ThrowsAsync()");
    }

    /// <summary>
    /// EXAMPLE 4: Callbacks (Capturing Arguments)
    /// </summary>
    public static void CallbacksExample()
    {
        Console.WriteLine("\n=== EXAMPLE 4: Callbacks ===");

        // Capture arguments passed to mock
        // string capturedEmail = string.Empty;
        // string capturedSubject = string.Empty;

        // var mockEmailService = new Mock<IEmailService>();
        // mockEmailService.Setup(e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //     .Callback<string, string, string>((to, subject, body) =>
        //     {
        //         capturedEmail = to;
        //         capturedSubject = subject;
        //     })
        //     .Returns(true);

        // After calling the method, verify captured values
        // Assert.Equal("expected@example.com", capturedEmail);

        Console.WriteLine("Callbacks: Capture and verify arguments passed to mocks");
    }

    /// <summary>
    /// EXAMPLE 5: Verification
    /// </summary>
    public static void VerificationExample()
    {
        Console.WriteLine("\n=== EXAMPLE 5: Verification ===");

        // var mockLogger = new Mock<ILogger>();

        // Verify method was called
        // mockLogger.Verify(l => l.Log(It.IsAny<string>()), Times.Once);
        // mockLogger.Verify(l => l.Log("User created"), Times.Exactly(1));
        // mockLogger.Verify(l => l.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);

        // Verify all setups were called
        // mockLogger.VerifyAll();

        // Verify no other calls
        // mockLogger.VerifyNoOtherCalls();

        Console.WriteLine("Verification: Times.Once, Times.Never, Times.AtLeastOnce, VerifyAll");
    }

    /// <summary>
    /// EXAMPLE 6: SetupSequence (Different Returns)
    /// </summary>
    public static void SetupSequenceExample()
    {
        Console.WriteLine("\n=== EXAMPLE 6: Setup Sequence ===");

        // var mockEmailService = new Mock<IEmailService>();

        // First call returns true, second returns false
        // mockEmailService.SetupSequence(e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
        //     .Returns(true)
        //     .Returns(false)
        //     .Returns(true);

        Console.WriteLine("SetupSequence: Different return values for successive calls");
    }

    /// <summary>
    /// EXAMPLE 7: Strict vs Loose Mocks
    /// </summary>
    public static void StrictVsLooseMocksExample()
    {
        Console.WriteLine("\n=== EXAMPLE 7: Strict vs Loose Mocks ===");

        // LOOSE (default): Returns default values for unmocked methods
        // var looseMock = new Mock<IEmailService>();
        // var result = looseMock.Object.SendEmail("test@test.com", "Subject", "Body");  // Returns false (default)

        // STRICT: Throws exception for unmocked methods
        // var strictMock = new Mock<IEmailService>(MockBehavior.Strict);
        // strictMock.Setup(e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        // var result = strictMock.Object.SendEmail("test@test.com", "Subject", "Body");  // OK
        // var quota = strictMock.Object.GetRemainingQuota();  // Throws exception!

        Console.WriteLine("Strict mocks: Enforce all methods are explicitly mocked");
        Console.WriteLine("Loose mocks: Return defaults for unmocked methods (default behavior)");
    }

    /// <summary>
    /// EXAMPLE 8: Complete Test Example with Multiple Mocks
    /// </summary>
    public static async Task CompleteTestExample()
    {
        Console.WriteLine("\n=== EXAMPLE 8: Complete Test with Multiple Mocks ===");

        // var mockRepository = new Mock<IRepository<User>>();
        // var mockEmailService = new Mock<IEmailService>();
        // var mockLogger = new Mock<ILogger>();

        // Setup mocks
        // mockRepository.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        // mockEmailService.Setup(e => e.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        // Create service with mocked dependencies
        // var service = new UserService(mockRepository.Object, mockEmailService.Object, mockLogger.Object);

        // Act
        // var user = new User { Id = 1, Name = "John", Email = "john@example.com" };
        // var result = await service.CreateUserAsync(user);

        // Assert
        // Assert.True(result);

        // Verify all interactions
        // mockRepository.Verify(r => r.AddAsync(user), Times.Once);
        // mockEmailService.Verify(e => e.SendEmail(user.Email, "Welcome", It.IsAny<string>()), Times.Once);
        // mockLogger.Verify(l => l.Log(It.Is<string>(msg => msg.Contains("User created"))), Times.Once);

        Console.WriteLine("Complete test: Multiple mocks, setup, act, assert, verify");
    }

    /// <summary>
    /// EXAMPLE 9: When NOT to Mock
    /// </summary>
    public static void WhenNotToMockExample()
    {
        Console.WriteLine("\n=== EXAMPLE 9: When NOT to Mock ===");

        Console.WriteLine("❌ Don't mock:");
        Console.WriteLine("   • Value objects (DTOs, records)");
        Console.WriteLine("   • Pure functions (no side effects)");
        Console.WriteLine("   • Framework classes (HttpContext, DbContext)");
        Console.WriteLine("   • Database (use in-memory or TestContainers)");

        Console.WriteLine("\n✅ Do mock:");
        Console.WriteLine("   • External services (email, SMS)");
        Console.WriteLine("   • HTTP clients");
        Console.WriteLine("   • Repositories");
        Console.WriteLine("   • Message queues");
        Console.WriteLine("   • Time providers (for testing time-sensitive code)");
    }

    /// <summary>
    /// Best Practices
    /// </summary>
    public static void BestPractices()
    {
        Console.WriteLine("\n=== MOCKING BEST PRACTICES ===");
        Console.WriteLine("✅ Mock interfaces, not concrete classes");
        Console.WriteLine("✅ Use It.IsAny for non-critical arguments");
        Console.WriteLine("✅ Use It.Is for critical argument validation");
        Console.WriteLine("✅ Verify important interactions");
        Console.WriteLine("✅ Don't over-mock (prefer real objects when simple)");
        Console.WriteLine("✅ Use descriptive mock names");
        Console.WriteLine("✅ Keep mocks simple");
        Console.WriteLine("✅ Consider alternatives (NSubstitute, FakeItEasy) if Moq feels verbose");
    }

    public static async Task RunAllExamples()
    {
        Console.WriteLine("\n=== MOCKING IN DEPTH EXAMPLES ===\n");
        BasicMockingExample();
        ArgumentMatchersExample();
        ThrowingExceptionsExample();
        CallbacksExample();
        VerificationExample();
        SetupSequenceExample();
        StrictVsLooseMocksExample();
        await CompleteTestExample();
        WhenNotToMockExample();
        BestPractices();
        Console.WriteLine("\nMocking In Depth examples completed!\n");
    }
}
