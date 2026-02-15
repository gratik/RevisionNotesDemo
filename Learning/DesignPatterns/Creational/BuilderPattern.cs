// ============================================================================
// BUILDER PATTERN - Construct Complex Objects Step-by-Step
// Reference: Revision Notes - Design Patterns (Creational) - Page 3
// ============================================================================
//
// WHAT IS THE BUILDER PATTERN?
// -----------------------------
// Separates the construction of a complex object from its representation so
// that the same construction process can create different representations.
// Allows you to construct complex objects step-by-step using a fluent interface.
//
// Think of it as: "Ordering a custom burger - choose bun, patty, toppings, sauce
// one at a time, then get final burger"
//
// Core Concepts:
//   • Builder: Interface defining steps to build product
//   • Concrete Builder: Implements building steps
//   • Product: Complex object being built
//   • Director: Optional - orchestrates building steps in specific order
//   • Fluent Interface: Method chaining (builder.AddX().AddY().Build())
//
// WHY IT MATTERS
// --------------
// ✅ READABLE CODE: .AddBun().AddCheese().AddLettuce() vs new Burger(bun, cheese, lettuce, ...)
// ✅ IMMUTABLE PRODUCTS: Build object completely before returning
// ✅ STEP-BY-STEP: Construct complex objects in manageable steps
// ✅ DIFFERENT REPRESENTATIONS: Same building process, different results
// ✅ ENCAPSULATION: Construction logic hidden from client
// ✅ VALIDATION: Validate each step or final product before returning
//
// WHEN TO USE IT
// --------------
// ✅ Object has many optional parameters (avoid telescoping constructors)
// ✅ Need to create different representations of same object
// ✅ Construction process must be independent of parts being created
// ✅ Object creation involves multiple steps
// ✅ Want immutable objects (all fields set before object returned)
// ✅ Constructor has 4+ parameters (especially if many are optional)
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Simple objects with few parameters (use constructor or object initializer)
// ❌ Object doesn't have optional parameters
// ❌ Construction is straightforward (new MyClass() suffices)
// ❌ Overhead of builder class not justified
// ❌ C# record types with 'with' expressions handle it better
//
// REAL-WORLD EXAMPLE
// ------------------
// Imagine a SQL query builder (like Entity Framework's LINQ or Dapper):
//   • Complex SQL queries with SELECT, FROM, WHERE, JOIN, ORDER BY, etc.
//   • Not all clauses needed for every query
//   • Order matters but can be specified flexibly
//   • Easy to make mistakes with raw SQL strings
//
// Without Builder:
//   → string sql = "SELECT * FROM Users WHERE Age > 18";
//   → Hard to read, easy to make SQL injection mistakes
//   → Difficult to conditionally add WHERE/ORDER BY clauses
//   → No compile-time safety
//
// With Builder:
//   → var query = new QueryBuilder()
//         .Select("FirstName", "LastName", "Email")
//         .From("Users")
//         .Where("Age > @age")
//         .OrderBy("LastName")
//         .Build();
//   → Fluent, readable, type-safe
//   → Can conditionally add clauses: if (needFilter) builder.Where(...);
//   → Validates query structure before building
//   → Prevents SQL injection with parameterization
//
// COMPARISON WITH SIMILAR PATTERNS
// ---------------------------------
// Builder vs Constructor with Parameters:
//   • Builder: Fluent, step-by-step, optional parameters clear
//   • Constructor: new MyClass(p1, p2, p3, p4, p5...) - parameter hell
//
// Builder vs Factory Method:
//   • Builder: Constructs complex objects step-by-step (HOW to build)
//   • Factory: Decides which class to instantiate (WHAT to build)
//
// MODERN C# ALTERNATIVES
// ----------------------
// C# 9.0+ Record Types with 'with' expressions:
//   record Person(string First, string Last, int Age);
//   var person = new Person("John", "Doe", 30);
//   var updated = person with { Age = 31 };  // Create modified copy
//
// Object Initializers (simple cases):
//   var person = new Person { First = "John", Last = "Doe", Age = 30 };
//
// But Builder still wins for:
//   • Complex validation logic
//   • Multi-step construction process
//   • Immutable objects requiring complete state
//
// ============================================================================

namespace RevisionNotesDemo.DesignPatterns.Creational;

// Complex product
public class MealOrder
{
    public string? MainCourse { get; set; }
    public string? SideDish { get; set; }
    public string? Drink { get; set; }
    public string? Dessert { get; set; }
    public bool ExtraSauce { get; set; }
    public bool ExtraCheese { get; set; }
    public List<string> SpecialInstructions { get; set; } = new();

    public void Display()
    {
        Console.WriteLine("\n[BUILDER] === Meal Order ===");
        Console.WriteLine($"[BUILDER] Main Course: {MainCourse ?? "None"}");
        Console.WriteLine($"[BUILDER] Side Dish: {SideDish ?? "None"}");
        Console.WriteLine($"[BUILDER] Drink: {Drink ?? "None"}");
        Console.WriteLine($"[BUILDER] Dessert: {Dessert ?? "None"}");
        Console.WriteLine($"[BUILDER] Extra Sauce: {ExtraSauce}");
        Console.WriteLine($"[BUILDER] Extra Cheese: {ExtraCheese}");
        if (SpecialInstructions.Any())
        {
            Console.WriteLine($"[BUILDER] Special Instructions: {string.Join(", ", SpecialInstructions)}");
        }
        Console.WriteLine();
    }
}

// Builder interface
public interface IMealBuilder
{
    IMealBuilder AddMainCourse(string mainCourse);
    IMealBuilder AddSideDish(string sideDish);
    IMealBuilder AddDrink(string drink);
    IMealBuilder AddDessert(string dessert);
    IMealBuilder AddExtraSauce();
    IMealBuilder AddExtraCheese();
    IMealBuilder AddSpecialInstruction(string instruction);
    MealOrder Build();
}

// Concrete builder
public class MealBuilder : IMealBuilder
{
    private readonly MealOrder _meal = new();

    public IMealBuilder AddMainCourse(string mainCourse)
    {
        _meal.MainCourse = mainCourse;
        Console.WriteLine($"[BUILDER] Added main course: {mainCourse}");
        return this;
    }

    public IMealBuilder AddSideDish(string sideDish)
    {
        _meal.SideDish = sideDish;
        Console.WriteLine($"[BUILDER] Added side dish: {sideDish}");
        return this;
    }

    public IMealBuilder AddDrink(string drink)
    {
        _meal.Drink = drink;
        Console.WriteLine($"[BUILDER] Added drink: {drink}");
        return this;
    }

    public IMealBuilder AddDessert(string dessert)
    {
        _meal.Dessert = dessert;
        Console.WriteLine($"[BUILDER] Added dessert: {dessert}");
        return this;
    }

    public IMealBuilder AddExtraSauce()
    {
        _meal.ExtraSauce = true;
        Console.WriteLine("[BUILDER] Added extra sauce");
        return this;
    }

    public IMealBuilder AddExtraCheese()
    {
        _meal.ExtraCheese = true;
        Console.WriteLine("[BUILDER] Added extra cheese");
        return this;
    }

    public IMealBuilder AddSpecialInstruction(string instruction)
    {
        _meal.SpecialInstructions.Add(instruction);
        Console.WriteLine($"[BUILDER] Added special instruction: {instruction}");
        return this;
    }

    public MealOrder Build()
    {
        return _meal;
    }
}

// Director (optional) - provides predefined building sequences
public class MealDirector
{
    public static MealOrder CreateValueMeal(IMealBuilder builder)
    {
        Console.WriteLine("[BUILDER] Creating Value Meal...");
        return builder
            .AddMainCourse("Burger")
            .AddSideDish("Fries")
            .AddDrink("Cola")
            .Build();
    }

    public static MealOrder CreateHealthyMeal(IMealBuilder builder)
    {
        Console.WriteLine("[BUILDER] Creating Healthy Meal...");
        return builder
            .AddMainCourse("Grilled Chicken Salad")
            .AddSideDish("Mixed Vegetables")
            .AddDrink("Water")
            .AddSpecialInstruction("Low sodium")
            .Build();
    }

    public static MealOrder CreateKidsMeal(IMealBuilder builder)
    {
        Console.WriteLine("[BUILDER] Creating Kids Meal...");
        return builder
            .AddMainCourse("Chicken Nuggets")
            .AddSideDish("Apple Slices")
            .AddDrink("Juice Box")
            .AddDessert("Ice Cream")
            .Build();
    }
}

// Another example: Report Builder
public class Report
{
    public string? Title { get; set; }
    public string? Header { get; set; }
    public List<string> Sections { get; set; } = new();
    public string? Footer { get; set; }
    public string? Format { get; set; }

    public void Print()
    {
        Console.WriteLine("\n[BUILDER] === Report ===");
        Console.WriteLine($"[BUILDER] Title: {Title}");
        Console.WriteLine($"[BUILDER] Header: {Header}");
        Console.WriteLine($"[BUILDER] Sections: {Sections.Count}");
        foreach (var section in Sections)
        {
            Console.WriteLine($"[BUILDER]   - {section}");
        }
        Console.WriteLine($"[BUILDER] Footer: {Footer}");
        Console.WriteLine($"[BUILDER] Format: {Format}\n");
    }
}

public class ReportBuilder
{
    private readonly Report _report = new();

    public ReportBuilder SetTitle(string title)
    {
        _report.Title = title;
        return this;
    }

    public ReportBuilder SetHeader(string header)
    {
        _report.Header = header;
        return this;
    }

    public ReportBuilder AddSection(string section)
    {
        _report.Sections.Add(section);
        return this;
    }

    public ReportBuilder SetFooter(string footer)
    {
        _report.Footer = footer;
        return this;
    }

    public ReportBuilder SetFormat(string format)
    {
        _report.Format = format;
        return this;
    }

    public Report Build() => _report;
}

// Usage demonstration
public class BuilderDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== BUILDER PATTERN DEMO ===\n");

        Console.WriteLine("--- Example 1: Custom Meal Order ---");
        var customMeal = new MealBuilder()
            .AddMainCourse("Steak")
            .AddSideDish("Mashed Potatoes")
            .AddDrink("Red Wine")
            .AddDessert("Chocolate Cake")
            .AddExtraSauce()
            .AddExtraCheese()
            .AddSpecialInstruction("Medium rare")
            .AddSpecialInstruction("No onions")
            .Build();
        customMeal.Display();

        Console.WriteLine("--- Example 2: Using Director for Predefined Meals ---");
        var valueMeal = MealDirector.CreateValueMeal(new MealBuilder());
        valueMeal.Display();

        var healthyMeal = MealDirector.CreateHealthyMeal(new MealBuilder());
        healthyMeal.Display();

        var kidsMeal = MealDirector.CreateKidsMeal(new MealBuilder());
        kidsMeal.Display();

        Console.WriteLine("--- Example 3: Report Building ---");
        var report = new ReportBuilder()
            .SetTitle("Q4 Sales Report")
            .SetHeader("Company XYZ - Quarterly Report")
            .AddSection("Executive Summary")
            .AddSection("Sales Analysis")
            .AddSection("Market Trends")
            .AddSection("Recommendations")
            .SetFooter("Confidential - Internal Use Only")
            .SetFormat("PDF")
            .Build();
        report.Print();

        Console.WriteLine("💡 Benefit: Separates construction logic from representation");
        Console.WriteLine("💡 Benefit: Same building process can create different representations");
        Console.WriteLine("💡 Benefit: Fluent interface makes code readable");
    }
}
