// ============================================================================
// ABSTRACT CLASSES VS INTERFACES
// Reference: Revision Notes - Advanced OOP & Language Features - Page 6
// ============================================================================
// Abstract Class: Can have both abstract and non-abstract members, fields, constructors
// Interface: Only signatures (until C# 8 default implementations), no fields
// When to use: Abstract class for shared base functionality; interface for contracts
// ============================================================================

namespace RevisionNotesDemo.CoreCSharpFeatures;

// From Revision Notes - Page 6: Example of Abstract class vs Interface
public abstract class Shape
{
    public abstract double Area();
    public virtual string Describe() => "Generic shape";
}

public interface IColored
{
    string Color { get; }
}

public sealed class Rectangle : Shape, IColored
{
    public double Width { get; }
    public double Height { get; }
    public string Color { get; }

    public Rectangle(double w, double h, string color)
    {
        Width = w;
        Height = h;
        Color = color;
    }

    public override double Area() => Width * Height;
    public override string Describe() => $"Rectangle {Width}x{Height}, {Color}";
}

public class AbstractVsInterfaceDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== ABSTRACT CLASS VS INTERFACE DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Page 6\n");

        var rect = new Rectangle(5, 3, "Blue");

        Console.WriteLine($"[ABSTRACT] Shape: {rect.Describe()}");
        Console.WriteLine($"[ABSTRACT] Area: {rect.Area()}");
        Console.WriteLine($"[INTERFACE] Color: {rect.Color}");

        Console.WriteLine("\n💡 From Revision Notes:");
        Console.WriteLine("   - Abstract Class: shared base functionality, fields, constructors");
        Console.WriteLine("   - Interface: contracts across unrelated classes");
        Console.WriteLine("   - Use abstract when you need common implementation");
        Console.WriteLine("   - Use interface for pure contracts");
    }
}
