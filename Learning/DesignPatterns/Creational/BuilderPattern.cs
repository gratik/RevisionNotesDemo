// ============================================================================
// BUILDER PATTERN
// Reference: Revision Notes - Design Patterns (Creational) - Page 3
// ============================================================================
// PURPOSE: "Separates the construction of a complex object from its representation."
// EXAMPLE: Building complex reports or meal orders.
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
