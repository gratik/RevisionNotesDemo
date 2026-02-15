// ============================================================================
// OPEN-CLOSED PRINCIPLE (OCP)
// Reference: Revision Notes - OOP (Object Oriented Principals) - Page 2
// ============================================================================
//
// WHAT IS OCP?
// ------------
// Software entities should be open for extension but closed for modification.
// You should add new behavior without changing existing, tested code.
//
// WHY IT MATTERS
// --------------
// - Reduces risk of breaking existing functionality
// - Encourages stable APIs and extensibility
// - Enables plugin-style architectures
// - Improves maintainability and test confidence
//
// WHEN TO USE
// -----------
// - YES: When new behaviors are expected (payments, shipping, pricing rules)
// - YES: When existing code is stable and widely used
// - YES: When large if/else or switch chains are forming
//
// WHEN NOT TO USE
// ---------------
// - NO: If the domain is stable and extensions are unlikely
// - NO: If the abstraction is premature and unclear
// - NO: If changes always require modifying existing logic anyway
//
// REAL-WORLD EXAMPLE
// ------------------
// Payment processing:
// - Add Apple Pay by creating ApplePayPaymentStrategy
// - No changes needed to CheckoutService
// - Existing payment methods remain untouched
// ============================================================================

namespace RevisionNotesDemo.OOPPrinciples;

// ❌ BAD EXAMPLE - Violates OCP
// Every time we need to add a new shape, we must modify this class
public class AreaCalculatorBad
{
    public double CalculateArea(object shape)
    {
        if (shape is Circle circle)
        {
            return Math.PI * circle.Radius * circle.Radius;
        }
        else if (shape is Rectangle rectangle)
        {
            return rectangle.Width * rectangle.Height;
        }
        // Adding a new shape requires modifying this method!
        else if (shape is Triangle triangle)
        {
            return 0.5 * triangle.Base * triangle.Height;
        }

        throw new ArgumentException("Unknown shape");
    }
}

public class Circle { public double Radius { get; set; } }
public class Rectangle { public double Width { get; set; } public double Height { get; set; } }
public class Triangle { public double Base { get; set; } public double Height { get; set; } }

// ✅ GOOD EXAMPLE - Follows OCP
// We can add new shapes without modifying existing code

// Base abstraction - closed for modification
public abstract class Shape
{
    public abstract double CalculateArea();
    public abstract string GetShapeName();
}

// Extended via inheritance - open for extension
public class CircleShape : Shape
{
    public double Radius { get; }

    public CircleShape(double radius)
    {
        Radius = radius;
    }

    public override double CalculateArea()
    {
        return Math.PI * Radius * Radius;
    }

    public override string GetShapeName() => "Circle";
}

public class RectangleShape : Shape
{
    public double Width { get; }
    public double Height { get; }

    public RectangleShape(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public override double CalculateArea()
    {
        return Width * Height;
    }

    public override string GetShapeName() => "Rectangle";
}

// New shape added without modifying existing code!
public class TriangleShape : Shape
{
    public double Base { get; }
    public double Height { get; }

    public TriangleShape(double baseLength, double height)
    {
        Base = baseLength;
        Height = height;
    }

    public override double CalculateArea()
    {
        return 0.5 * Base * Height;
    }

    public override string GetShapeName() => "Triangle";
}

// Another extension - Hexagon (added without changing Shape or other shapes)
public class HexagonShape : Shape
{
    public double SideLength { get; }

    public HexagonShape(double sideLength)
    {
        SideLength = sideLength;
    }

    public override double CalculateArea()
    {
        return (3 * Math.Sqrt(3) / 2) * SideLength * SideLength;
    }

    public override string GetShapeName() => "Hexagon";
}

// Calculator doesn't need to change when new shapes are added
public class AreaCalculator
{
    public double CalculateTotalArea(IEnumerable<Shape> shapes)
    {
        return shapes.Sum(shape => shape.CalculateArea());
    }

    public void PrintAreas(IEnumerable<Shape> shapes)
    {
        foreach (var shape in shapes)
        {
            Console.WriteLine($"[OCP] {shape.GetShapeName()} area: {shape.CalculateArea():F2}");
        }
    }
}

// Usage demonstration
public class OCPDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== OPEN-CLOSED PRINCIPLE DEMO ===\n");

        var shapes = new List<Shape>
        {
            new CircleShape(5),
            new RectangleShape(4, 6),
            new TriangleShape(3, 4),
            new HexagonShape(2) // New shape added without modifying existing code!
        };

        var calculator = new AreaCalculator();
        calculator.PrintAreas(shapes);

        double total = calculator.CalculateTotalArea(shapes);
        Console.WriteLine($"\n[OCP] Total area: {total:F2}");
        Console.WriteLine("\nBenefit: New shapes can be added without changing existing, tested code!");
    }
}
