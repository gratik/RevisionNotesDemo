// ==============================================================================
// TEMPLATE METHOD PATTERN
// Reference: Revision Notes - Design Patterns
// ==============================================================================
// PURPOSE: Defines skeleton of algorithm, allowing subclasses to override specific steps
// BENEFIT: Code reuse, enforces algorithm structure, hook methods for customization
// USE WHEN: Common algorithm with varying steps, want to control extension points
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