// ==============================================================================
// TEMPLATE METHOD PATTERN - Define Algorithm Skeleton, Let Subclasses Override Steps
// Reference: Revision Notes - Design Patterns
// ==============================================================================
//
// WHAT IS THE TEMPLATE METHOD PATTERN?
// -------------------------------------
// Defines the skeleton of an algorithm in a base class method, deferring some steps
// to subclasses. Subclasses can override specific steps without changing the algorithm's
// structure. Uses inheritance to share common behavior while allowing customization.
//
// Think of it as: "Recipe template - all recipes follow same steps (prep, cook, serve)
// but each recipe customizes the details (what to prep, how to cook, how to serve)"
//
// Core Concepts:
//   • Template Method: Defines algorithm skeleton (final/sealed)
//   • Abstract Methods: Steps subclasses must implement
//   • Hook Methods: Optional steps with default (empty) implementation
//   • Concrete Methods: Fixed steps implemented in base class
//   • Inversion of Control: "Hollywood Principle" - Don't call us, we'll call you
//
// WHY IT MATTERS
// --------------
// ✅ CODE REUSE: Common algorithm in base class, variations in subclasses
// ✅ ENFORCES STRUCTURE: Algorithm steps can't be skipped or reordered
// ✅ OPEN/CLOSED: Add new variations without modifying base algorithm
// ✅ ELIMINATES DUPLICATION: Shared code in one place
// ✅ INVERSION OF CONTROL: Framework calls your code (not vice versa)
// ✅ HOOK METHODS: Optional extension points without forcing override
//
// WHEN TO USE IT
// --------------
// ✅ Multiple classes have similar algorithms with minor differences
// ✅ Want to control which steps can be overridden
// ✅ Algorithm structure must remain consistent across variations
// ✅ Common behavior should be factored out to avoid duplication
// ✅ Need extension points (hooks) for optional customization
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Algorithm rarely varies (no need for inheritance)
// ❌ Steps need to be reordered or skipped (use Strategy)
// ❌ Prefer composition over inheritance (use Strategy pattern)
// ❌ Only 1-2 methods differ (inheritance overkill)
//
// REAL-WORLD EXAMPLE - Data Import Pipeline
// -----------------------------------------
// ETL import system for CSV, JSON, XML:
//   • All formats follow same pipeline:
//     1. Open file
//     2. Parse data (varies by format)
//     3. Validate records (varies by format)
//     4. Transform data (shared logic)
//     5. Load to database (shared logic)
//     6. Close file
//     7. Cleanup (optional hook)
//
// WITHOUT TEMPLATE METHOD:
//   ❌ class CsvImporter {
//         void Import() {
//             OpenFile();
//             ParseCsv();      // CSV-specific
//             ValidateCsv();   // CSV-specific
//             Transform();     // Duplicated!
//             LoadToDb();      // Duplicated!
//             CloseFile();     // Duplicated!
//             Cleanup();       // Duplicated!
//         }
//     }
//   ❌ class JsonImporter { /* 80% same code duplicated */ }
//   ❌ class XmlImporter { /* 80% same code duplicated */ }
//   ❌ Change shared logic = update 3 classes
//
// WITH TEMPLATE METHOD:
//   ✅ abstract class DataImporter {
//         // Template method (sealed/final)
//         public sealed void Import() {
//             OpenFile();           // Concrete (shared)
//             var data = ParseData();     // Abstract (varies)
//             ValidateData(data);   // Abstract (varies)
//             Transform(data);      // Concrete (shared)
//             LoadToDb(data);       // Concrete (shared)
//             CloseFile();          // Concrete (shared)
//             Cleanup();            // Hook (optional)
//         }
//         
//         protected abstract object ParseData();   // Must override
//         protected abstract void ValidateData(object data); // Must override
//         protected virtual void Cleanup() { }  // Hook (optional)
//         
//         private void OpenFile() { /* shared */ }
//         private void Transform(object data) { /* shared */ }
//         private void LoadToDb(object data) { /* shared */ }
//         private void CloseFile() { /* shared */ }
//     }
//   
//   ✅ class CsvImporter : DataImporter {
//         protected override object ParseData() { /* CSV logic */ }
//         protected override void ValidateData(object data) { /* CSV validation */ }
//         // Cleanup() optional, not overridden
//     }
//   
//   ✅ class JsonImporter : DataImporter {
//         protected override object ParseData() { /* JSON logic */ }
//         protected override void ValidateData(object data) { /* JSON validation */ }
//         protected override void Cleanup() { /* Custom cleanup */ }
//     }
//   
//   ✅ csvImporter.Import(); // Algorithm structure enforced
//   ✅ Change shared logic in one place (base class)
//
// ANOTHER EXAMPLE - Game AI
// -------------------------
// NPC behavior in video game:
//   • All NPCs follow: Update() → Think() → Move() → Act()
//   • Variations: Guard, Merchant, Enemy
//
// Code:
//   abstract class NpcAI {
//       public void Update() {
//           ScanEnvironment();  // Shared
//           Think();            // Abstract (varies)
//           Move();             // Abstract (varies)
//           Act();              // Abstract (varies)
//           UpdateAnimation();  // Shared
//       }
//       protected abstract void Think();
//       protected abstract void Move();
//       protected abstract void Act();
//   }
//   
//   class GuardAI : NpcAI {
//       protected override void Think() { /* Patrol path logic */ }
//       protected override void Move() { /* Walk patrol route */ }
//       protected override void Act() { /* Attack if see enemy */ }
//   }
//   
//   class MerchantAI : NpcAI {
//       protected override void Think() { /* Check for customers */ }
//       protected override void Move() { /* Stay at shop */ }
//       protected override void Act() { /* Sell items */ }
//   }
//
// ANOTHER EXAMPLE - Unit Testing Frameworks
// -----------------------------------------
// xUnit/NUnit test lifecycle:
//   • Template: Setup → RunTest → Teardown
//   • Framework controls flow:
//     1. [SetUp] or constructor
//     2. [Test] method
//     3. [TearDown] or Dispose
//
// Internal implementation:
//   abstract class TestCase {
//       public void Run() {
//           SetUp();       // Hook (optional)
//           try {
//               RunTest(); // Abstract
//           } finally {
//               TearDown(); // Hook (optional)
//           }
//       }
//       protected abstract void RunTest();
//       protected virtual void SetUp() { }
//       protected virtual void TearDown() { }
//   }
//
// HOOK METHODS
// ------------
// Hook = Optional extension point with default (often empty) implementation
//   • protected virtual void BeforeProcess() { } // Empty default
//   • protected virtual bool ShouldValidate() => true; // Default behavior
//   • Subclass can override if needed
//   • If not overridden, default is used
//
// vs Abstract Methods:
//   • Abstract: Must override (no default)
//   • Hook: Can override (has default)
//
// .NET FRAMEWORK EXAMPLES
// -----------------------
// Template Method in .NET:
//   • ASP.NET Page lifecycle: Init → Load → PreRender → Render
//   • Stream classes: Read() template calls ReadByte()
//   • DbConnection: Open() template with provider-specific OpenConnection()
//   • Test frameworks: xUnit, NUnit test execution
//
// HOLLYWOOD PRINCIPLE
// -------------------
// "Don't call us, we'll call you"
//   • Base class calls subclass methods (inversion of control)
//   • Application code doesn't control flow, framework does
//   • Similar to: Dependency Injection, Event-driven systems
//
// TEMPLATE METHOD VS SIMILAR PATTERNS
// -----------------------------------
// Template Method vs Strategy:
//   • Template Method: Inheritance, fixed structure, can't swap at runtime
//   • Strategy: Composition, flexible structure, swap at runtime
//   • TM: "This is how you must do it" (enforcement)
//   • Strategy: "Here are options" (flexibility)
//
// Template Method vs Factory Method:
//   • Template Method: Defines algorithm steps
//   • Factory Method: Special case (one step is object creation)
//
// BEST PRACTICES
// --------------
// ✅ Make template method final/sealed (prevent override)
// ✅ Use abstract for required steps
// ✅ Use virtual (hooks) for optional steps
// ✅ Keep template method at appropriate abstraction level
// ✅ Document the algorithm flow clearly
// ✅ Minimize number of abstract methods (3-5 is good)
// ✅ Consider whether Strategy pattern is better (composition > inheritance)
//
// WHEN TO PREFER STRATEGY OVER TEMPLATE METHOD
// --------------------------------------------
// Choose Strategy if:
//   • Need to swap algorithm at runtime
//   • Want to use composition instead of inheritance
//   • Algorithm structure varies significantly
//   • Have many variations (inheritance explosion)
//
// Choose Template Method if:
//   • Variations share significant common code
//   • Algorithm structure must be enforced
//   • Extension points are well-defined
//   • Inheritance makes sense for domain
//
// ==============================================================================

namespace RevisionNotesDemo.DesignPatterns.Behavioral;

// ========================================================================
// EXAMPLE 1: DATA PARSER (Common Use Case)
// ========================================================================

/// <summary>
/// Abstract class defining the template method
/// </summary>
public abstract class DataParser
{
    // Template Method - defines the algorithm structure
    public void ParseData(string filePath)
    {
        Console.WriteLine($"\n[PARSER] Starting parse for: {filePath}");

        OpenFile(filePath);
        ExtractData();
        ParseContent();

        if (ShouldValidate())  // Hook method
            ValidateData();

        CloseFile();

        Console.WriteLine("[PARSER] ✅ Parsing complete\n");
    }

    // Common steps (implemented in base class)
    private void OpenFile(string filePath)
    {
        Console.WriteLine($"  📂 Opening file: {filePath}");
    }

    private void CloseFile()
    {
        Console.WriteLine("  📂 Closing file");
    }

    // Abstract methods (must be implemented by subclasses)
    protected abstract void ExtractData();
    protected abstract void ParseContent();

    // Hook method (optional override - provides default behavior)
    protected virtual bool ShouldValidate() => true;

    protected virtual void ValidateData()
    {
        Console.WriteLine("  ✅ Validating data (default)");
    }
}

public class CSVParser : DataParser
{
    protected override void ExtractData()
    {
        Console.WriteLine("  📊 Extracting CSV data (commaseparated)");
    }

    protected override void ParseContent()
    {
        Console.WriteLine("  📊 Parsing CSV rows and columns");
    }

    protected override void ValidateData()
    {
        Console.WriteLine("  ✅ Validating CSV format (headers, delimiters)");
    }
}

public class JSONParser : DataParser
{
    protected override void ExtractData()
    {
        Console.WriteLine("  📋 Extracting JSON data");
    }

    protected override void ParseContent()
    {
        Console.WriteLine("  📋 Parsing JSON objects and arrays");
    }

    protected override void ValidateData()
    {
        Console.WriteLine("  ✅ Validating JSON syntax (braces, quotes)");
    }
}

public class XMLParser : DataParser
{
    protected override void ExtractData()
    {
        Console.WriteLine("  📄 Extracting XML data");
    }

    protected override void ParseContent()
    {
        Console.WriteLine("  📄 Parsing XML nodes and attributes");
    }

    protected override bool ShouldValidate() => false;  // Override hook - skip validation
}

// ========================================================================
// EXAMPLE 2: BEVERAGE PREPARATION
// ========================================================================

public abstract class Beverage
{
    // Template Method
    public void PrepareBeverage()
    {
        Console.WriteLine($"\n[BEVERAGE] Preparing {GetName()}...");

        BoilWater();
        Brew();
        PourInCup();

        if (CustomerWantsCondiments())  // Hook
            AddCondiments();

        Console.WriteLine($"[BEVERAGE] ✅ {GetName()} ready!\n");
    }

    // Common methods
    private void BoilWater()
    {
        Console.WriteLine("  💧 Boiling water...");
    }

    private void PourInCup()
    {
        Console.WriteLine("  ☕ Pouring into cup");
    }

    // Abstract methods (vary by subclass)
    protected abstract void Brew();
    protected abstract void AddCondiments();
    protected abstract string GetName();

    // Hook method (optional customization)
    protected virtual bool CustomerWantsCondiments() => true;
}

public class Coffee : Beverage
{
    protected override void Brew()
    {
        Console.WriteLine("  ☕ Dripping coffee through filter");
    }

    protected override void AddCondiments()
    {
        Console.WriteLine("  🥛 Adding sugar and milk");
    }

    protected override string GetName() => "Coffee";
}

public class Tea : Beverage
{
    private readonly bool _withLemon;

    public Tea(bool withLemon = true)
    {
        _withLemon = withLemon;
    }

    protected override void Brew()
    {
        Console.WriteLine("  🍵 Steeping tea bag");
    }

    protected override void AddCondiments()
    {
        Console.WriteLine("  🍋 Adding lemon");
    }

    protected override string GetName() => "Tea";

    protected override bool CustomerWantsCondiments() => _withLemon;
}

// ========================================================================
// EXAMPLE 3: REPORT GENERATION
// ========================================================================

public abstract class ReportGenerator
{
    // Template Method
    public void GenerateReport(string title)
    {
        Console.WriteLine($"\n[REPORT] Generating {GetReportType()} report: '{title}'");

        StartReport();
        AddHeader(title);
        AddBody();
        AddFooter();
        EndReport();

        Console.WriteLine($"[REPORT] ✅ {GetReportType()} report generated\n");
    }

    // Template steps
    private void StartReport()
    {
        Console.WriteLine($"  📄 Starting {GetReportType()} report...");
    }

    private void EndReport()
    {
        Console.WriteLine($"  📄 Finalizing {GetReportType()} report");
    }

    // Abstract methods
    protected abstract void AddHeader(string title);
    protected abstract void AddBody();
    protected abstract void AddFooter();
    protected abstract string GetReportType();
}

public class PDFReport : ReportGenerator
{
    protected override void AddHeader(string title)
    {
        Console.WriteLine($"  📕 PDF Header: {title}");
    }

    protected override void AddBody()
    {
        Console.WriteLine("  📕 PDF Body: Adding formatted content with styles");
    }

    protected override void AddFooter()
    {
        Console.WriteLine("  📕 PDF Footer: Page numbers and copyright");
    }

    protected override string GetReportType() => "PDF";
}

public class HTMLReport : ReportGenerator
{
    protected override void AddHeader(string title)
    {
        Console.WriteLine($"  🌐 HTML Header: <h1>{title}</h1>");
    }

    protected override void AddBody()
    {
        Console.WriteLine("  🌐 HTML Body: <div> with CSS styles </div>");
    }

    protected override void AddFooter()
    {
        Console.WriteLine("  🌐 HTML Footer: <footer> with links </footer>");
    }

    protected override string GetReportType() => "HTML";
}

// ========================================================================
// DEMONSTRATION
// ========================================================================

public class TemplateMethodDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== TEMPLATE METHOD PATTERN DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Design Patterns\n");

        // Example 1: Data Parsers
        Console.WriteLine("=== EXAMPLE 1: Data Parsers ===");

        DataParser csvParser = new CSVParser();
        csvParser.ParseData("data.csv");

        DataParser jsonParser = new JSONParser();
        jsonParser.ParseData("config.json");

        DataParser xmlParser = new XMLParser();
        xmlParser.ParseData("settings.xml");  // Note: skips validation (hook override)

        // Example 2: Beverage Preparation
        Console.WriteLine("=== EXAMPLE 2: Beverage Preparation ===");

        Beverage coffee = new Coffee();
        coffee.PrepareBeverage();

        Beverage teaWithLemon = new Tea(withLemon: true);
        teaWithLemon.PrepareBeverage();

        Beverage teaPlain = new Tea(withLemon: false);  // Hook prevents condiments
        teaPlain.PrepareBeverage();

        // Example 3: Report Generation
        Console.WriteLine("=== EXAMPLE 3: Report Generation ===");

        ReportGenerator pdfReport = new PDFReport();
        pdfReport.GenerateReport("Q4 Sales Report");

        ReportGenerator htmlReport = new HTMLReport();
        htmlReport.GenerateReport("Annual Summary");

        Console.WriteLine("💡 Template Method Pattern Benefits:");
        Console.WriteLine("   ✅ Code reuse - common algorithm in base class");
        Console.WriteLine("   ✅ Control structure - enforce algorithm steps");
        Console.WriteLine("   ✅ Extension points - subclasses customize specific steps");
        Console.WriteLine("   ✅ Inversion of Control - \"Hollywood Principle\" (don't call us, we'll call you)");
        Console.WriteLine("   ✅ Hook methods - optional customization points");

        Console.WriteLine("\n💡 Key Concepts:");
        Console.WriteLine("   🔹 Template Method: Final method defining algorithm skeleton");
        Console.WriteLine("   🔹 Abstract Methods: Must be implemented by subclasses");
        Console.WriteLine("   🔹 Hook Methods: Optional override with default behavior");
        Console.WriteLine("   🔹 Concrete Methods: Implement in base class, cannot override");
    }
}