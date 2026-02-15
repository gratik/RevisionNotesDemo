// ==============================================================================
// VISITOR PATTERN - Add Operations Without Modifying Classes
// Reference: Revision Notes - Design Patterns
// ==============================================================================
//
// WHAT IS THE VISITOR PATTERN?
// ----------------------------
// Separates algorithms from the object structure they operate on. Lets you add
// new operations to existing classes without modifying them. Uses double dispatch
// to route method calls based on both visitor and element types.
//
// Think of it as: "IRS tax inspector visits different entities (person, business, trust)
// - each entity 'accepts' the visitor and provides access to its data. The inspector
// knows how to calculate taxes for each entity type without entities knowing tax rules."
//
// Core Concepts:
//   • Visitor: Interface defining operations for each element type
//   • Concrete Visitor: Implements specific operation (tax calculation, export, etc.)
//   • Element: Interface with Accept(IVisitor) method
//   • Concrete Elements: Classes that accept visitors
//   • Double Dispatch: Runtime determines both visitor and element types
//
// WHY IT MATTERS
// --------------
// ✅ ADD OPERATIONS EASILY: New visitor = new operation (no element changes)
// ✅ SINGLE RESPONSIBILITY: Operations separated from data structures
// ✅ RELATED OPERATIONS TOGETHER: Visitor groups logic by operation
// ✅ ACCESS PRIVATE DATA: Elements can expose internals to visitors
// ✅ MULTIPLE UNRELATED OPERATIONS: Avoid polluting element classes
//
// WHEN TO USE IT
// --------------
// ✅ Need many distinct operations on object structure
// ✅ Object structure classes rarely change
// ✅ Operations change frequently
// ✅ Algorithm needs data from different classes in structure
// ✅ Want to avoid "polluting" classes with unrelated operations
// ✅ Need to accumulate state while traversing structure
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Element classes change frequently (visitor must update for each)
// ❌ Object structure is simple (visitor adds complexity)
// ❌ Few operations needed (just add methods to elements)
// ❌ Elements have simple interfaces (visitor can't access internals)
//
// REAL-WORLD EXAMPLE - Tax Calculation System
// -------------------------------------------
// IRS/CRA tax calculation for different entity types:
//   • Entities: Individual, Corporation, Partnership, Trust
//   • Each has different tax rules and data
//   • Operations needed:
//     1. Calculate federal tax
//     2. Calculate state tax
//     3. Generate tax report
//     4. Calculate penalties
//     5. Export to tax software
//
// WITHOUT VISITOR:
//   ❌ class Individual {
//         decimal CalculateFederalTax() { /* complex logic */ }
//         decimal CalculateStateTax() { /* more logic */ }
//         string GenerateReport() { /* reporting logic */ }
//         decimal CalculatePenalties() { /* penalty logic */ }
//         string ExportToTaxSoftware() { /* export logic */ }
//       } // 5 operations mixed with entity data
//   ❌ class Corporation { /* same 5 operations duplicated */ }
//   ❌ Adding "Calculate AMT" = modify 4 classes
//   ❌ Tax logic scattered across entity classes
//   ❌ Hard to see all tax calculation logic in one place
//
// WITH VISITOR:
//   ✅ interface ITaxEntity {
//         void Accept(ITaxVisitor visitor);
//     }
//   
//   ✅ class Individual : ITaxEntity {
//         public decimal Income { get; set; }
//         public decimal Deductions { get; set; }
//         public void Accept(ITaxVisitor visitor) => visitor.Visit(this);
//     }
//   
//   ✅ class Corporation : ITaxEntity {
//         public decimal Revenue { get; set; }
//         public decimal Expenses { get; set; }
//         public void Accept(ITaxVisitor visitor) => visitor.Visit(this);
//     }
//   
//   ✅ interface ITaxVisitor {
//         void Visit(Individual individual);
//         void Visit(Corporation corporation);
//         void Visit(Partnership partnership);
//         void Visit(Trust trust);
//     }
//   
//   ✅ class FederalTaxCalculator : ITaxVisitor {
//         private decimal _totalTax;
//         public void Visit(Individual ind) {
//             _totalTax = (ind.Income - ind.Deductions) * 0.25m; // Individual rate
//         }
//         public void Visit(Corporation corp) {
//             _totalTax = (corp.Revenue - corp.Expenses) * 0.21m; // Corporate rate
//         }
//     }
//   
//   ✅ class TaxReportGenerator : ITaxVisitor {
//         public void Visit(Individual ind) { /* Generate 1040 form */ }
//         public void Visit(Corporation corp) { /* Generate 1120 form */ }
//     }
//   
//   ✅ Usage:
//     var entities = new List<ITaxEntity> { new Individual(), new Corporation() };
//     var taxCalc = new FederalTaxCalculator();
//     entities.ForEach(e => e.Accept(taxCalc)); // Calculate all taxes
//     
//     var report = new TaxReportGenerator();
//     entities.ForEach(e => e.Accept(report)); // Generate all reports
//   
//   ✅ Adding "AMT Calculator":
//     class AMTCalculator : ITaxVisitor { /* implement for each type */ }
//     No changes to Individual, Corporation, etc.!
//
// ANOTHER EXAMPLE - Compiler AST Operations
// -----------------------------------------
// Abstract Syntax Tree for programming language:
//   • Nodes: NumberNode, AddNode, MultiplyNode, VariableNode
//   • Operations:
//     - Evaluate: Calculate result
//     - PrettyPrint: Format as string
//     - Optimize: Simplify expression
//     - TypeCheck: Verify types
//     - CodeGen: Generate machine code
//
// Without Visitor:
//   ❌ class AddNode {
//         decimal Evaluate() { }
//         string PrettyPrint() { }
//         AddNode Optimize() { }
//         Type TypeCheck() { }
//         Instruction[] CodeGen() { }
//       } // 5 operations, hard to maintain
//
// With Visitor:
//   ✅ class EvaluateVisitor : IAstVisitor { /* only evaluation logic */ }
//   ✅ class PrettyPrintVisitor : IAstVisitor { /* only printing logic */ }
//   ✅ class OptimizeVisitor : IAstVisitor { /* only optimization logic */ }
//   ✅ Each visitor focused on one concern
//
// ANOTHER EXAMPLE - E-commerce Shopping Cart
// ------------------------------------------
// Calculate different things for cart items:
//   • Items: Book, Electronics, Clothing
//   • Operations:
//     - Calculate total price
//     - Calculate shipping cost
//     - Calculate taxes
//     - Generate invoice
//     - Check inventory
//
// Code:
//   interface ICartItemVisitor {
//       void Visit(Book book);
//       void Visit(Electronics electronics);
//       void Visit(Clothing clothing);
//   }
//   
//   class ShippingCostCalculator : ICartItemVisitor {
//       private decimal _totalShipping;
//       public void Visit(Book book) {
//           _totalShipping += 2.99m; // Flat rate
//       }
//       public void Visit(Electronics electronics) {
//           _totalShipping += electronics.Weight * 0.50m + 5.00m; // Weight-based + insurance
//       }
//       public void Visit(Clothing clothing) {
//           _totalShipping += 4.99m; // Medium rate
//       }
//   }
//
// DOUBLE DISPATCH EXPLAINED
// -------------------------
// Double dispatch allows method call resolution based on TWO types at runtime:
//   1. Element type (Individual, Corporation)
//   2. Visitor type (TaxCalculator, ReportGenerator)
//
// Flow:
//   ITaxEntity entity = new Individual();
//   ITaxVisitor visitor = new FederalTaxCalculator();
//   entity.Accept(visitor); 
//   → Calls Individual.Accept(ITaxVisitor)
//   → Calls visitor.Visit(Individual this)
//   → Calls FederalTaxCalculator.Visit(Individual)
//
// Result: Correct overload called based on both types!
//
// PROS & CONS
// -----------
// ✅ PROS:
//   • Easy to add new operations (new visitor)
//   • Related operations grouped in visitor
//   • Can accumulate state during traversal
//   • Visitor can access element internals
//
// ❌ CONS:
//   • Hard to add new element types (must update all visitors)
//   • Breaks encapsulation (visitors need element access)
//   • Circular dependency (elements know visitors)
//   • Complexity (double dispatch is tricky)
//
// VISITOR VARIATIONS
// ------------------
// 1. **Classic Visitor** (shown above)
// 2. **Acyclic Visitor** (no element/visitor coupling)
// 3. **Reflective Visitor** (uses reflection, no Visit overloads)
//
// .NET FRAMEWORK EXAMPLES
// -----------------------
// Visitor-like patterns in .NET:
//   • Expression trees: ExpressionVisitor for LINQ
//   • Roslyn compiler: SyntaxWalker, SyntaxRewriter
//   • JSON.NET: JsonVisitor for custom serialization
//
// VISITOR VS SIMILAR PATTERNS
// ---------------------------
// Visitor vs Strategy:
//   • Visitor: Operates on object structure, double dispatch
//   • Strategy: Algorithm selection, single dispatch
//
// Visitor vs Decorator:
//   • Visitor: Adds operations externally
//   • Decorator: Adds behavior by wrapping
//
// BEST PRACTICES
// --------------
// ✅ Use when object structure stable, operations volatile
// ✅ Keep visitor interface cohesive (related operations)
// ✅ Consider return values from Visit methods
// ✅ Use generic visitors: IVisitor<TResult>
// ✅ Document that adding elements requires updating all visitors
// ✅ Consider alternatives if element classes change frequently
//
// MODERN ALTERNATIVES
// -------------------
// Instead of Visitor, consider:
//   • Pattern matching (C# 9+): switch expressions
//   • Extension methods: Add operations without modifying classes
//   • Dynamic typing: Use 'dynamic' keyword (no compile-time safety)
//
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