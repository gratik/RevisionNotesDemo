// ==============================================================================
// RECORDS & RECORD STRUCTS - Modern C# Data Types
// ==============================================================================
// WHAT IS THIS?
// -------------
// Record types that provide immutability and value-based equality.
//
// WHY IT MATTERS
// --------------
// ✅ Reduces boilerplate for data models
// ✅ Encourages safe, immutable design
//
// WHEN TO USE
// -----------
// ✅ DTOs, value objects, and domain events
// ✅ Small value types that benefit from value equality
//
// WHEN NOT TO USE
// ---------------
// ❌ Entities with mutable identity or behavior
// ❌ Large mutable objects with frequent state changes
//
// REAL-WORLD EXAMPLE
// ------------------
// API DTOs using records with with-expressions.
// ==============================================================================

namespace RevisionNotesDemo.ModernCSharp;

/// <summary>
/// EXAMPLE 1: RECORD CLASSES - Immutable Reference Types
/// 
/// THE PROBLEM:
/// Creating immutable classes requires lots of boilerplate:
/// - Properties with init-only setters
/// - Custom Equals/GetHashCode
/// - Custom ToString()
/// - Deconstruction
/// 
/// THE SOLUTION:
/// Use record classes - compiler generates all of the above automatically.
/// 
/// WHY IT MATTERS:
/// - Less code (10 lines → 1 line)
/// - Immutability enforced
/// - Value equality out of the box
/// - Thread-safe
/// - Clear intent
/// 
/// BEST FOR: DTOs, API models, domain events, value objects
/// </summary>

// ❌ BAD: Traditional class with manual equals, boilerplate
public class PersonClass
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public int Age { get; init; }

    public PersonClass(string firstName, string lastName, int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }

    // Manual Equals
    public override bool Equals(object? obj) =>
        obj is PersonClass other &&
        FirstName == other.FirstName &&
        LastName == other.LastName &&
        Age == other.Age;

    // Manual GetHashCode
    public override int GetHashCode() =>
        HashCode.Combine(FirstName, LastName, Age);

    // Manual ToString
    public override string ToString() =>
        $"Person {{ FirstName = {FirstName}, LastName = {LastName}, Age = {Age} }}";
}

// ✅ GOOD: Record with all of the above generated automatically!
public record Person(string FirstName, string LastName, int Age);

// That's it! Compiler generates:
// - Properties (public, init-only)
// - Constructor
// - Deconstruct method
// - Equals/GetHashCode (value-based)
// - ToString() (structured)
// - Copy and With expressions

public static class RecordExamples
{
    public static void DemonstrateRecords()
    {
        // ✅ GOOD: Create record (concise)
        var person1 = new Person("John", "Doe", 30);
        var person2 = new Person("John", "Doe", 30);

        // ✅ Value equality (compares contents, not reference)
        Console.WriteLine(person1 == person2); // True! (different instances, same values)

        // ✅ Built-in ToString()
        Console.WriteLine(person1); // Person { FirstName = John, LastName = Doe, Age = 30 }

        // ✅ Built-in deconstruction
        var (firstName, lastName, age) = person1;
        Console.WriteLine($"{firstName} {lastName}, {age}");

        // ✅ Non-destructive mutation with 'with' expression
        var person3 = person1 with { Age = 31 }; // Creates new record, only Age changed
        Console.WriteLine(person3); // Person { FirstName = John, LastName = Doe, Age = 31 }
        Console.WriteLine(person1); // Person { FirstName = John, LastName = Doe, Age = 30 } (unchanged!)
    }
}

/// <summary>
/// EXAMPLE 2: RECORD STRUCTS - Immutable Value Types
/// 
/// THE PROBLEM:
/// Want record benefits (value equality, with expressions) but on the stack (value type).
/// Struct doesn't have value equality or with expressions.
/// 
/// THE SOLUTION:
/// Use record struct (C# 10+) - value type with record features.
/// 
/// WHY IT MATTERS:
/// - Stack allocation (better performance for small data)
/// - Value semantics (copied by value)
/// - Value equality built-in
/// - No heap allocations
/// 
/// BEST FOR: Small, frequently-created value types (Point, Color, Money)
/// SIZE GUIDELINE: < 16 bytes recommended
/// </summary>

// ✅ GOOD: Readonly record struct (immutable, value type)
public readonly record struct Point(int X, int Y);

// ✅ GOOD: Mutable record struct (if we need mutability)
public record struct MutablePoint(int X, int Y);

public static class RecordStructExamples
{
    public static void DemonstrateRecordStructs()
    {
        // ✅ Stack allocation
        var p1 = new Point(10, 20);
        var p2 = new Point(10, 20);

        // ✅ Value equality (built-in)
        Console.WriteLine(p1 == p2); // True

        // ✅ With expressions work!
        var p3 = p1 with { X = 15 };
        Console.WriteLine(p3); // Point { X = 15, Y = 20 }

        // ✅ ToString() generated
        Console.WriteLine(p1); // Point { X = 10, Y = 20 }

        // Performance: No heap allocation, faster for small types
    }
}

/// <summary>
/// EXAMPLE 3: RECORDS WITH ADDITIONAL MEMBERS
/// 
/// THE PROBLEM:
/// Need calculated properties, methods, or validation in records.
/// 
/// THE SOLUTION:
/// Records can have additional members just like classes.
/// 
/// WHY IT MATTERS:
/// - Not limited to simple data containers
/// - Can add business logic
/// - Can validate in constructor
/// - Can have computed properties
/// </summary>

// ✅ GOOD: Record with additional members
public record Product(string Name, decimal Price, int Stock, string Category = "General")
{
    // Computed property
    public decimal TotalValue => Price * Stock;

    // Validation
    public bool IsInStock => Stock > 0;

    // Method
    public Product ReduceStock(int quantity) =>
        this with { Stock = Math.Max(0, Stock - quantity) };
}

// ✅ GOOD: Record with private fields (encapsulation)
public record BankAccount
{
    public string AccountNumber { get; init; }
    public decimal Balance { get; private set; } // Private setter!

    public BankAccount(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    // Methods can modify private state
    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive");
        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive");
        if (amount > Balance) throw new InvalidOperationException("Insufficient funds");
        Balance -= amount;
    }
}

/// <summary>
/// EXAMPLE 4: INHERITANCE WITH RECORDS
/// 
/// THE PROBLEM:
/// Need inheritance for domain models.
/// Want record benefits with polymorphism.
/// 
/// THE SOLUTION:
/// Records support inheritance (unlike structs).
/// Base record can be inherited, sealed records cannot.
/// 
/// WHY IT MATTERS:
/// - Model "IS-A" relationships
/// - Polymorphic behavior
/// - Still get value equality
/// - Type-safe hierarchies
/// 
/// GOTCHA: Records can only inherit from records (not classes)
/// </summary>

// ✅ GOOD: Base record
public abstract record Shape(string Color)
{
    public abstract double Area { get; }
}

// ✅ GOOD: Derived records
public record Circle(string Color, double Radius) : Shape(Color)
{
    public override double Area => Math.PI * Radius * Radius;
}

public record Rectangle(string Color, double Width, double Height) : Shape(Color)
{
    public override double Area => Width * Height;
}

// ✅ GOOD: Sealed record (cannot be inherited)
public sealed record Point3D(int X, int Y, int Z);

public static class RecordInheritanceExamples
{
    public static void DemonstrateInheritance()
    {
        Shape circle = new Circle("Red", 5.0);
        Shape rectangle = new Rectangle("Blue", 10.0, 20.0);

        Console.WriteLine($"Circle area: {circle.Area}"); // Polymorphism works!
        Console.WriteLine($"Rectangle area: {rectangle.Area}");

        // ✅ Value equality considers full hierarchy
        var circle1 = new Circle("Red", 5.0);
        var circle2 = new Circle("Red", 5.0);
        Console.WriteLine(circle1 == circle2); // True

        // ✅ With expressions work with inheritance
        var blueCircle = circle1 with { Color = "Blue" };
        Console.WriteLine(blueCircle); // Circle { Color = Blue, Radius = 5.0 }
    }
}

/// <summary>
/// EXAMPLE 5: WHEN TO USE RECORDS VS CLASSES VS STRUCTS
/// 
/// Decision guide for choosing the right type
/// </summary>
public static class RecordVsClassVsStructGuide
{
    // ✅ USE RECORD CLASS WHEN:
    // - Immutable data (DTOs, API models, events)
    // - Value equality desired
    // - Data container with little behavior
    // - Thread safety important
    // Examples: User, Order, ProductDto, DomainEvent

    public record UserDto(int Id, string Email, string Name);
    public record OrderCreatedEvent(int OrderId, DateTime Timestamp);

    // ✅ USE CLASS WHEN:
    // - Mutable state required
    // - Complex behavior/logic
    // - Reference equality desired
    // - Large object (> 1KB)
    // - Entity with identity
    // Examples: Repository, Service, Controller, Entity

    public class UserService
    {
        private readonly IUserRepository _repository;
        public UserService(IUserRepository repository) => _repository = repository;
        // ... methods
    }

    // ✅ USE RECORD STRUCT WHEN:
    // - Small, immutable value type (< 16 bytes)
    // - Value equality desired
    // - High performance needed (stack allocation)
    // - Frequently created/destroyed
    // Examples: Point, Color, Money, Coordinate

    public readonly record struct Money(decimal Amount, string Currency);
    public readonly record struct Coordinate(double Latitude, double Longitude);

    // ✅ USE STRUCT WHEN:
    // - Small value type with mutable state
    // - Legacy/interop requirements
    // Examples: DateTime, TimeSpan, Guid (framework types)

    // ❌ AVOID:
    // - Large structs (causes performance issues)
    // - Mutable structs (confusing behavior)
    // - Records when complex mutable behavior needed
}

/// <summary>
/// EXAMPLE 6: POSITIONAL VS NOMINAL RECORDS
/// 
/// Two syntaxes for defining records
/// </summary>
public static class PositionalVsNominalRecords
{
    // ✅ GOOD: Positional record (concise, immutable)
    public record PersonPositional(string FirstName, string LastName, int Age);

    // ✅ GOOD: Nominal record (traditional property syntax)
    public record PersonNominal
    {
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public int Age { get; init; }
    }

    // ✅ GOOD: Mixing positional with additional properties
    public record Employee(string FirstName, string LastName)
    {
        public int EmployeeId { get; init; }
        public string Department { get; init; } = string.Empty;
        public decimal Salary { get; init; }
    }

    // ✅ GOOD: Required members (C# 11+)
    public record Configuration
    {
        public required string ConnectionString { get; init; }
        public required string ApiKey { get; init; }
        public int Timeout { get; init; } = 30; // Optional with default
    }
}
