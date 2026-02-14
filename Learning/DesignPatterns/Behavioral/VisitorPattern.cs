// ==============================================================================
// VISITOR PATTERN
// Reference: Revision Notes - Design Patterns
// ==============================================================================
// PURPOSE: Separates algorithm from object structure, adds operations without modifying classes
// BENEFIT: Add new operations easily, keeps related operations together, violates Open/Closed carefully
// USE WHEN: Many distinct operations on object structure, classes rarely change but operations do
// ==============================================================================

namespace RevisionNotesDemo.DesignPatterns.Behavioral;

// ========================================================================
// VISITOR INTERFACE
// ========================================================================

public interface IShapeVisitor
{
    void Visit(Circle circle);
    void Visit(Rectangle rectangle);
    void Visit(Triangle triangle);
}

// ========================================================================
// ELEMENT INTERFACE (Visitable)
// ========================================================================

public interface IShape
{
    void Accept(IShapeVisitor visitor);
}

// ========================================================================
// CONCRETE ELEMENTS
// ========================================================================

public class Circle : IShape
{
    public double Radius { get; set; }

    public Circle(double radius)
    {
        Radius = radius;
    }

    public void Accept(IShapeVisitor visitor)
    {
        visitor.Visit(this);  // Double dispatch - calls correct overload
    }
}

public class Rectangle : IShape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public Rectangle(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public void Accept(IShapeVisitor visitor)
    {
        visitor.Visit(this);
    }
}

public class Triangle : IShape
{
    public double Base { get; set; }
    public double Height { get; set; }

    public Triangle(double baseLength, double height)
    {
        Base = baseLength;
        Height = height;
    }

    public void Accept(IShapeVisitor visitor)
    {
        visitor.Visit(this);
    }
}

// ========================================================================
// CONCRETE VISITORS (Different Operations)
// ========================================================================

/// <summary>
/// Visitor that calculates area of shapes
/// </summary>
public class AreaCalculator : IShapeVisitor
{
    public double TotalArea { get; private set; }

    public void Visit(Circle circle)
    {
        double area = Math.PI * circle.Radius * circle.Radius;
        Console.WriteLine($"  [AREA] Circle (r={circle.Radius}): {area:F2}");
        TotalArea += area;
    }

    public void Visit(Rectangle rectangle)
    {
        double area = rectangle.Width * rectangle.Height;
        Console.WriteLine($"  [AREA] Rectangle ({rectangle.Width}x{rectangle.Height}): {area:F2}");
        TotalArea += area;
    }

    public void Visit(Triangle triangle)
    {
        double area = 0.5 * triangle.Base * triangle.Height;
        Console.WriteLine($"  [AREA] Triangle (b={triangle.Base}, h={triangle.Height}): {area:F2}");
        TotalArea += area;
    }
}

/// <summary>
/// Visitor that calculates perimeter of shapes
/// </summary>
public class PerimeterCalculator : IShapeVisitor
{
    public double TotalPerimeter { get; private set; }

    public void Visit(Circle circle)
    {
        double perimeter = 2 * Math.PI * circle.Radius;
        Console.WriteLine($"  [PERIMETER] Circle (r={circle.Radius}): {perimeter:F2}");
        TotalPerimeter += perimeter;
    }

    public void Visit(Rectangle rectangle)
    {
        double perimeter = 2 * (rectangle.Width + rectangle.Height);
        Console.WriteLine($"  [PERIMETER] Rectangle ({rectangle.Width}x{rectangle.Height}): {perimeter:F2}");
        TotalPerimeter += perimeter;
    }

    public void Visit(Triangle triangle)
    {
        // Assume equilateral for simplicity (in real app, store all sides)
        double side = triangle.Base;
        double perimeter = 3 * side;
        Console.WriteLine($"  [PERIMETER] Triangle (equilateral, side={side}): {perimeter:F2}");
        TotalPerimeter += perimeter;
    }
}

/// <summary>
/// Visitor that exports shapes to different formats
/// </summary>
public class ShapeExporter : IShapeVisitor
{
    private readonly string _format;
    public List<string> ExportedData { get; } = new();

    public ShapeExporter(string format)
    {
        _format = format;
    }

    public void Visit(Circle circle)
    {
        var export = _format switch
        {
            "JSON" => $"{{\"type\":\"circle\",\"radius\":{circle.Radius}}}",
            "XML" => $"<circle radius=\"{circle.Radius}\" />",
            "SVG" => $"<circle r=\"{circle.Radius}\" />",
            _ => $"Circle(r={circle.Radius})"
        };
        Console.WriteLine($"  [EXPORT-{_format}] {export}");
        ExportedData.Add(export);
    }

    public void Visit(Rectangle rectangle)
    {
        var export = _format switch
        {
            "JSON" => $"{{\"type\":\"rectangle\",\"width\":{rectangle.Width},\"height\":{rectangle.Height}}}",
            "XML" => $"<rectangle width=\"{rectangle.Width}\" height=\"{rectangle.Height}\" />",
            "SVG" => $"<rect width=\"{rectangle.Width}\" height=\"{rectangle.Height}\" />",
            _ => $"Rectangle({rectangle.Width}x{rectangle.Height})"
        };
        Console.WriteLine($"  [EXPORT-{_format}] {export}");
        ExportedData.Add(export);
    }

    public void Visit(Triangle triangle)
    {
        var export = _format switch
        {
            "JSON" => $"{{\"type\":\"triangle\",\"base\":{triangle.Base},\"height\":{triangle.Height}}}",
            "XML" => $"<triangle base=\"{triangle.Base}\" height=\"{triangle.Height}\" />",
            "SVG" => $"<polygon points=\"0,0 {triangle.Base},0 {triangle.Base / 2},{triangle.Height}\" />",
            _ => $"Triangle(b={triangle.Base},h={triangle.Height})"
        };
        Console.WriteLine($"  [EXPORT-{_format}] {export}");
        ExportedData.Add(export);
    }
}

// ========================================================================
// DEMONSTRATION
// ========================================================================

public class VisitorDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== VISITOR PATTERN DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Design Patterns\n");

        // Create shape collection
        var shapes = new List<IShape>
        {
            new Circle(5),
            new Rectangle(10, 6),
            new Triangle(8, 4),
            new Circle(3),
            new Rectangle(7, 7)
        };

        Console.WriteLine("Shape Collection:");
        Console.WriteLine($"  {shapes.Count} shapes created\n");

        // Operation 1: Calculate areas
        Console.WriteLine("--- Operation 1: Calculate Area ---");
        var areaCalculator = new AreaCalculator();
        foreach (var shape in shapes)
        {
            shape.Accept(areaCalculator);  // Visitor processes each shape
        }
        Console.WriteLine($"  ✅ Total Area: {areaCalculator.TotalArea:F2}\n");

        // Operation 2: Calculate perimeters
        Console.WriteLine("--- Operation 2: Calculate Perimeter ---");
        var perimeterCalculator = new PerimeterCalculator();
        foreach (var shape in shapes)
        {
            shape.Accept(perimeterCalculator);
        }
        Console.WriteLine($"  ✅ Total Perimeter: {perimeterCalculator.TotalPerimeter:F2}\n");

        // Operation 3: Export to JSON
        Console.WriteLine("--- Operation 3: Export to JSON ---");
        var jsonExporter = new ShapeExporter("JSON");
        foreach (var shape in shapes)
        {
            shape.Accept(jsonExporter);
        }
        Console.WriteLine($"  ✅ Exported {jsonExporter.ExportedData.Count} shapes\n");

        // Operation 4: Export to XML
        Console.WriteLine("--- Operation 4: Export to XML ---");
        var xmlExporter = new ShapeExporter("XML");
        foreach (var shape in shapes)
        {
            shape.Accept(xmlExporter);
        }
        Console.WriteLine($"  ✅ Exported {xmlExporter.ExportedData.Count} shapes\n");

        // Operation 5: Export to SVG
        Console.WriteLine("--- Operation 5: Export to SVG ---");
        var svgExporter = new ShapeExporter("SVG");
        foreach (var shape in shapes)
        {
            shape.Accept(svgExporter);
        }
        Console.WriteLine($"  ✅ Exported {svgExporter.ExportedData.Count} shapes\n");

        Console.WriteLine("💡 Visitor Pattern Benefits:");
        Console.WriteLine("   ✅ Add new operations without modifying shape classes");
        Console.WriteLine("   ✅ Related operations in one visitor class");
        Console.WriteLine("   ✅ Double dispatch - correct method called based on shape type");
        Console.WriteLine("   ✅ Easy to add new visitors (operations)");
        Console.WriteLine("   ✅ Separates algorithm from data structure");

        Console.WriteLine("\n⚠️  Visitor Pattern Drawbacks:");
        Console.WriteLine("   ❌ Hard to add new element types (must update all visitors)");
        Console.WriteLine("   ❌ Breaks encapsulation - visitor needs access to element internals");
        Console.WriteLine("   ❌ Violates Open/Closed Principle for elements");

        Console.WriteLine("\n💡 When to Use:");
        Console.WriteLine("   ✅ Object structure rarely changes");
        Console.WriteLine("   ✅ Need to perform many distinct operations");
        Console.WriteLine("   ✅ Operations don't fit naturally in element classes");
        Console.WriteLine("   ✅ Want to keep related operations together");

        Console.WriteLine("\n💡 Real-World Examples:");
        Console.WriteLine("   • Compiler AST traversal (type checking, code generation)");
        Console.WriteLine("   • Document object model operations (rendering, exporting)");
        Console.WriteLine("   • Shopping cart calculations (taxes, shipping, discounts)");
    }
}