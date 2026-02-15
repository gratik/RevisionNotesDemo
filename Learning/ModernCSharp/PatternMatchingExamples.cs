// ==============================================================================
// PATTERN MATCHING - Modern C# 7-11 Features
// ==============================================================================
// WHAT IS THIS?
// -------------
// Pattern matching for expressive, type-safe conditional logic.
//
// WHY IT MATTERS
// --------------
// ✅ Reduces boilerplate in complex branching
// ✅ Improves readability and safety
//
// WHEN TO USE
// -----------
// ✅ Parsing, validation, and multi-shape rules
// ✅ Switch expressions replacing long if/else chains
//
// WHEN NOT TO USE
// ---------------
// ❌ Simple conditions where if/else is clearer
// ❌ Overly clever patterns that reduce readability
//
// REAL-WORLD EXAMPLE
// ------------------
// Switch expressions for order pricing rules.
// ==============================================================================

namespace RevisionNotesDemo.ModernCSharp;

/// <summary>
/// EXAMPLE 1: TYPE PATTERNS - Checking and Casting Types
/// 
/// THE PROBLEM:
/// Traditional type checking and casting is verbose and error-prone.
/// Multiple steps to check type, cast, and use.
/// 
/// THE SOLUTION:
/// Use type patterns with 'is' keyword - check and cast in one step.
/// 
/// WHY IT MATTERS:
/// - Less code (3 lines → 1 line)
/// - Type-safe (no casting errors)
/// - More readable
/// - Variable scoped to where it's valid
/// 
/// BEST FOR: Polymorphic code, parsing, visitor patterns
/// </summary>
public static class TypePatternExamples
{
    // ❌ BAD: Traditional type checking and casting
    public static string BadDescribeShape(object shape)
    {
        if (shape.GetType() == typeof(Circle))
        {
            Circle circle = (Circle)shape; // Cast after checking
            return $"Circle with radius {circle.Radius}";
        }
        else if (shape.GetType() == typeof(Rectangle))
        {
            Rectangle rect = (Rectangle)shape; // Another cast
            return $"Rectangle {rect.Width}x{rect.Height}";
        }
        return "Unknown shape";
    }

    // ✅ GOOD: Pattern matching with type patterns
    public static string GoodDescribeShape(object shape)
    {
        // Check type AND declare variable in one step
        if (shape is Circle circle)
        {
            return $"Circle with radius {circle.Radius}";
        }
        else if (shape is Rectangle rect)
        {
            return $"Rectangle {rect.Width}x{rect.Height}";
        }
        else if (shape is Triangle triangle)
        {
            return $"Triangle with base {triangle.Base}";
        }
        return "Unknown shape";
    }

    // ✅ BEST: Switch expression with type patterns
    public static string BestDescribeShape(object shape) => shape switch
    {
        Circle circle => $"Circle with radius {circle.Radius}",
        Rectangle rect => $"Rectangle {rect.Width}x{rect.Height}",
        Triangle triangle => $"Triangle with base {triangle.Base}",
        null => "Null shape",
        _ => "Unknown shape"
    };

    // ✅ GOOD: Negative type pattern (C# 9+)
    public static bool IsNotCircle(object shape) =>
        shape is not Circle; // More readable than !(shape is Circle)
}

/// <summary>
/// EXAMPLE 2: PROPERTY PATTERNS - Match on Object Properties
/// 
/// THE PROBLEM:
/// Need to check multiple properties of an object.
/// Traditional approach requires nested if statements.
/// 
/// THE SOLUTION:
/// Use property patterns to match on object properties declaratively.
/// 
/// WHY IT MATTERS:
/// - Declarative vs imperative
/// - Handles null safely
/// - Very readable
/// - Perfect for business rules
/// 
/// BEST FOR: Business rule validation, state machines, filtering
/// </summary>
public static class PropertyPatternExamples
{
    // ❌ BAD: Nested if statements
    public static string BadGetDiscount(Order order)
    {
        if (order != null)
        {
            if (order.Customer != null)
            {
                if (order.Customer.IsVIP)
                {
                    if (order.TotalAmount > 1000)
                    {
                        return "30% VIP discount";
                    }
                    return "20% VIP discount";
                }
                else if (order.TotalAmount > 500)
                {
                    return "10% bulk discount";
                }
            }
        }
        return "No discount";
    }

    // ✅ GOOD: Property patterns
    public static string GoodGetDiscount(Order order) => order switch
    {
        { Customer.IsVIP: true, TotalAmount: > 1000 } => "30% VIP discount",
        { Customer.IsVIP: true } => "20% VIP discount",
        { TotalAmount: > 500 } => "10% bulk discount",
        { Customer: not null } => "5% customer discount",
        null => "No discount",
        _ => "No discount"
    };

    // ✅ GOOD: Nested property patterns
    public static string GetShippingMethod(Order order) => order switch
    {
        { Customer: { Address: { Country: "US", State: "CA" } }, TotalAmount: > 100 }
            => "Free express shipping",
        { Customer: { Address.Country: "US" }, TotalAmount: > 50 }
            => "Free standard shipping",
        { Customer: { IsPrime: true } }
            => "Prime 2-day shipping",
        _ => "Standard shipping - $9.99"
    };

    // ✅ GOOD: Combining type and property patterns
    public static decimal CalculateTax(object item) => item switch
    {
        Product { Category: "Food", Price: > 0 } product => product.Price * 0.05m,
        Product { Category: "Electronics" } product => product.Price * 0.15m,
        Service { IsDigital: true } => 0m, // No tax on digital services
        Service service => service.Cost * 0.10m,
        _ => 0m
    };
}

/// <summary>
/// EXAMPLE 3: RELATIONAL AND LOGICAL PATTERNS - Complex Conditions
/// 
/// THE PROBLEM:
/// Complex range checks and combinations are hard to read.
/// 
/// THE SOLUTION:
/// Use relational patterns (>, <, >=, <=) and logical patterns (and, or, not).
/// 
/// WHY IT MATTERS:
/// - Express ranges clearly
/// - Combine conditions readably
/// - No magic numbers scattered  - One place for business rules
/// 
/// C# 9+
/// </summary>
public static class RelationalLogicalPatternExamples
{
    // ❌ BAD: Traditional range checking
    public static string BadGetAgeGroup(int age)
    {
        if (age >= 0 && age < 13)
            return "Child";
        else if (age >= 13 && age < 20)
            return "Teenager";
        else if (age >= 20 && age < 65)
            return "Adult";
        else if (age >= 65)
            return "Senior";
        else
            return "Invalid age";
    }

    // ✅ GOOD: Relational patterns
    public static string GoodGetAgeGroup(int age) => age switch
    {
        < 0 => "Invalid age",
        < 13 => "Child",
        < 20 => "Teenager",
        < 65 => "Adult",
        _ => "Senior"
    };

    // ✅ GOOD: Logical patterns (and, or, not)
    public static string GetTemperatureDescription(double celsius) => celsius switch
    {
        < -40 => "Extremely cold",
        >= -40 and < 0 => "Freezing",
        >= 0 and < 15 => "Cold",
        >= 15 and < 25 => "Mild",
        >= 25 and < 35 => "Warm",
        >= 35 => "Hot",
        double.NaN => "Invalid temperature"
    };

    // ✅ GOOD: Complex logical patterns
    public static bool IsValidScore(int score) => score switch
    {
        >= 0 and <= 100 => true,
        _ => false
    };

    public static string GetGrade(int score) => score switch
    {
        >= 90 and <= 100 => "A",
        >= 80 and < 90 => "B",
        >= 70 and < 80 => "C",
        >= 60 and < 70 => "D",
        >= 0 and < 60 => "F",
        _ => "Invalid score"
    };

    // ✅ GOOD: Not patterns
    public static bool IsWeekday(DayOfWeek day) => day is
        not DayOfWeek.Saturday and
        not DayOfWeek.Sunday;

    // ✅ GOOD: Or patterns
    public static bool IsWeekend(DayOfWeek day) => day is
        DayOfWeek.Saturday or
        DayOfWeek.Sunday;
}

/// <summary>
/// EXAMPLE 4: POSITIONAL PATTERNS - Deconstruction Matching
/// 
/// THE PROBLEM:
/// Need to match on tuple or record values.
/// Traditional extraction is verbose.
/// 
/// THE SOLUTION:
/// Use positional patterns with deconstructors.
/// 
/// WHY IT MATTERS:
/// - Match on tuple/record structure
/// - Extract values inline
/// - Very readable for coordinate/point logic
/// 
/// BEST FOR: Tuples, records, types with deconstructors
/// </summary>
public static class PositionalPatternExamples
{
    // ✅ GOOD: Positional patterns with tuples
    public static string GetQuadrant((int x, int y) point) => point switch
    {
        (0, 0) => "Origin",
        ( > 0, > 0) => "Quadrant I",
        ( < 0, > 0) => "Quadrant II",
        ( < 0, < 0) => "Quadrant III",
        ( > 0, < 0) => "Quadrant IV",
        (0, _) => "On X-axis",
        (_, 0) => "On Y-axis"
    };

    // ✅ GOOD: Positional patterns with records
    public static string Describe(Point point) => point switch
    {
        (0, 0) => "Origin",
        (var x, 0) => $"On X-axis at {x}",
        (0, var y) => $"On Y-axis at {y}",
        (var x, var y) when x == y => $"On diagonal at ({x}, {y})",
        (var x, var y) => $"Point at ({x}, {y})"
    };

    // ✅ GOOD: Nested positional patterns
    public static string DescribeLine(Line line) => line switch
    {
        ((0, 0), (var x2, var y2)) => $"From origin to ({x2}, {y2})",
        ((var x1, var y1), (var x2, var y2)) when x1 == x2 => "Vertical line",
        ((var x1, var y1), (var x2, var y2)) when y1 == y2 => "Horizontal line",
        _ => "Diagonal line"
    };
}

/// <summary>
/// EXAMPLE 5: LIST PATTERNS - Match Array/List Elements (C# 11)
/// 
/// THE PROBLEM:
/// Checking array/list contents requires loops and manual checks.
/// 
/// THE SOLUTION:
/// Use list patterns to match on collection structure and contents.
/// 
/// WHY IT MATTERS:
/// - Match on collection length
/// - Match on specific positions
/// - Match on patterns within collections
/// - Very powerful for parsing
/// 
/// C# 11+
/// BEST FOR: Parsing, validation, pattern recognition
/// </summary>
public static class ListPatternExamples
{
    // ✅ GOOD: List patterns - basic
    public static string DescribeArray(int[] numbers) => numbers switch
    {
        [] => "Empty array",
        [var single] => $"Single element: {single}",
        [var first, var second] => $"Two elements: {first}, {second}",
        [var first, .., var last] => $"Multiple elements, first: {first}, last: {last}",
    };

    // ✅ GOOD: List patterns - specific values
    public static string AnalyzeSequence(int[] seq) => seq switch
    {
        [1, 2, 3] => "Exactly 1, 2, 3",
        [1, ..] => "Starts with 1",
        [.., 9] => "Ends with 9",
        [0, .., 0] => "Starts and ends with 0",
        [_, >= 100, _] => "Second element is >= 100",
        _ => "Other sequence"
    };

    // ✅ GOOD: List patterns - complex
    public static bool IsValidCommand(string[] args) => args switch
    {
        ["help"] => true,
        ["list", "all" or "active" or "completed"] => true,
        ["add", _, ..] => true, // Add with at least one argument
        ["delete", _] => true,   // Delete with exactly one argument
        _ => false
    };

    // ✅ GOOD: Nested list and property patterns
    public static string ProcessCommand(Command cmd) => cmd switch
    {
        { Type: "query", Args: ["select", .., "from", var table] }
            => $"Querying from {table}",
        { Type: "insert", Args: [var table, ..] }
            => $"Inserting into {table}",
        { Type: "update", Args: [var table, "set", ..] }
            => $"Updating {table}",
        _ => "Unknown command"
    };
}

/// <summary>
/// EXAMPLE 6: SWITCH EXPRESSIONS - Modern Alternative to Switch Statements
/// 
/// THE PROBLEM:
/// Traditional switch statements are verbose and error-prone.
/// Fall-through behavior can cause bugs.
/// 
/// THE SOLUTION:
/// Use switch expressions (C# 8+) - more concise, expression-based.
/// 
/// WHY IT MATTERS:
/// - Returns a value (expression, not statement)
/// - No fall-through (safer)
/// - More concise
/// - Exhaustiveness checking
/// - Combines with other patterns
/// 
/// BEST FOR: Mapping, calculation, string generation
/// </summary>
public static class SwitchExpressionExamples
{
    // ❌ BAD: Traditional switch statement
    public static string BadGetDayType(DayOfWeek day)
    {
        string result;
        switch (day)
        {
            case DayOfWeek.Monday:
            case DayOfWeek.Tuesday:
            case DayOfWeek.Wednesday:
            case DayOfWeek.Thursday:
            case DayOfWeek.Friday:
                result = "Weekday";
                break;
            case DayOfWeek.Saturday:
            case DayOfWeek.Sunday:
                result = "Weekend";
                break;
            default:
                result = "Unknown";
                break;
        }
        return result;
    }

    // ✅ GOOD: Switch expression
    public static string GoodGetDayType(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday or
        DayOfWeek.Tuesday or
        DayOfWeek.Wednesday or
        DayOfWeek.Thursday or
        DayOfWeek.Friday => "Weekday",
        DayOfWeek.Saturday or DayOfWeek.Sunday => "Weekend",
        _ => throw new ArgumentException("Invalid day")
    };

    // ✅ GOOD: Complex switch expression with multiple patterns
    public static decimal CalculateShipping(Package package) => package switch
    {
        { Weight: <= 1, Destination.Country: "US" } => 5.00m,
        { Weight: <= 1 } => 15.00m,
        { Weight: > 1 and <= 5, Destination.Country: "US" } => 10.00m,
        { Weight: > 1 and <= 5 } => 25.00m,
        { IsFragile: true, Weight: > 5 } => 100.00m,
        { Weight: > 5 } => 50.00m,
        _ => 0m
    };

    // ✅ GOOD: Recursive pattern matching
    public static int CountNodes(TreeNode? node) => node switch
    {
        null => 0,
        { Left: null, Right: null } => 1,
        { Left: var left, Right: null } => 1 + CountNodes(left),
        { Left: null, Right: var right } => 1 + CountNodes(right),
        { Left: var left, Right: var right } => 1 + CountNodes(left) + CountNodes(right)
    };
}

/// <summary>
/// EXAMPLE 7: WHEN CLAUSES - Additional Guards
/// 
/// THE PROBLEM:
/// Pattern alone isn't enough - need additional conditions.
/// 
/// THE SOLUTION:
/// Use 'when' clauses to add guards to patterns.
/// 
/// WHY IT MATTERS:
/// - Express complex conditions
/// - Keep patterns readable
/// - Business logic alongside pattern
/// </summary>
public static class WhenClauseExamples
{
    // ✅ GOOD: When clauses for additional conditions
    public static string GetPricing(Customer customer, decimal amount) => (customer, amount) switch
    {
        ({ IsVIP: true }, _) when amount > 10000 => "Contact for custom pricing",
        ({ IsVIP: true }, _) => $"VIP price: ${amount * 0.7m}",
        (_, var amt) when amt > 1000 => $"Bulk price: ${amt * 0.9m}",
        (_, var amt) when amt > 100 => $"Standard price: ${amt * 0.95m}",
        _ => $"Regular price: ${amount}"
    };

    // ✅ GOOD: Complex when clauses
    public static string ApprovalRequired(Transaction trans) => trans switch
    {
        { Amount: > 10000 } when trans.Sender.TrustLevel < 5
            => "High-value, low-trust: Manual review required",
        { Amount: > 50000 }
            => "High-value: Manager approval required",
        { Receiver.Address.Country: not "US" } when trans.Amount > 5000
            => "International high-value: Compliance review",
        _ => "Approved"
    };
}

// Supporting types (some types shared from RecordsAndRecordStructs.cs)
public record Line(Point Start, Point End);
public record Triangle(double Base, double Height);
public record Order(Customer Customer, decimal TotalAmount);
public record Customer(string Name, bool IsVIP, bool IsPrime, Address Address, int TrustLevel = 5);
public record Address(string Street, string City, string State, string Country);
public record Service(string Name, bool IsDigital, decimal Cost);
public record Package(double Weight, Address Destination, bool IsFragile);
public record Command(string Type, string[] Args);
public record Transaction(decimal Amount, Customer Sender, Customer Receiver);

public class TreeNode
{
    public int Value { get; set; }
    public TreeNode? Left { get; set; }
    public TreeNode? Right { get; set; }
}
