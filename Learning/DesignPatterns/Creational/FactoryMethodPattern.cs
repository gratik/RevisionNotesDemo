// ============================================================================
// FACTORY METHOD PATTERN - Delegate Object Creation to Subclasses
// Reference: Revision Notes - Design Patterns (Creational) - Page 3
// ============================================================================
//
// WHAT IS THE FACTORY METHOD PATTERN?
// ------------------------------------
// Defines an interface for creating objects but lets subclasses decide which
// concrete class to instantiate. The pattern delegates the responsibility of
// object creation to subclasses, allowing the parent class to work with the
// interface without knowing the exact class being created.
//
// Think of it as: "A restaurant menu (interface) where each franchise (subclass)
// decides which chef prepares the dish"
//
// Core Concepts:
//   • Product: Interface/abstract class for objects created by factory
//   • Concrete Products: Specific implementations of the product
//   • Creator: Abstract class with factory method (returns Product)
//   • Concrete Creators: Subclasses that override factory method to return specific product
//   • Deferred Creation: Parent class doesn't know concrete type being created
//
// WHY IT MATTERS
// --------------
// ✅ OPEN/CLOSED PRINCIPLE: Add new product types without modifying existing code
// ✅ SINGLE RESPONSIBILITY: Object creation logic separated from business logic
// ✅ LOOSE COUPLING: Code depends on abstractions, not concrete classes
// ✅ FLEXIBILITY: Easy to introduce new variants without breaking existing code
// ✅ TESTABILITY: Mock different product types easily
// ✅ INITIALIZATION LOGIC: Complex object creation encapsulated in one place
//
// WHEN TO USE IT
// --------------
// ✅ Don't know exact types and dependencies of objects beforehand
// ✅ Need to provide library/framework users extensibility
// ✅ Want to save system resources by reusing existing objects
// ✅ Object creation requires complex initialization or configuration
// ✅ Need different implementations based on runtime conditions
// ✅ Want to delegate creation responsibility to subclasses
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Only one product type (unnecessary abstraction)
// ❌ Simple object creation with 'new' keyword suffices
// ❌ Adding complexity where it's not needed
// ❌ Every new product requires new creator subclass (maintenance overhead)
// ❌ Modern .NET with DI can handle this more elegantly
//
// REAL-WORLD EXAMPLE
// ------------------
// Imagine a cross-platform game development framework (like Unity):
//   • Game needs to render graphics differently per platform
//   • iOS uses Metal API, Android uses Vulkan, Windows uses DirectX
//   • Game logic shouldn't know about platform-specific rendering
//   • Each platform "factory" creates appropriate renderer
//
// Without Factory Method:
//   → Game code littered with if (iOS) / if (Android) / if (Windows)
//   → Tight coupling to specific platforms
//   → Adding new platform requires modifying game logic everywhere
//   → Testing difficult (can't easily mock renderers)
//
// With Factory Method:
//   → Game uses IRenderer interface
//   → IOSPlatform.CreateRenderer() returns MetalRenderer
//   → AndroidPlatform.CreateRenderer() returns VulkanRenderer
//   → WindowsPlatform.CreateRenderer() returns DirectXRenderer
//   → Game logic unchanged when adding PlayStation (new factory)
//   → Can inject mock renderer for testing
//
// CODE STRUCTURE:
//   abstract class Platform { public abstract IRenderer CreateRenderer(); }
//   class IOSPlatform : Platform { public override IRenderer CreateRenderer() => new MetalRenderer(); }
//   class AndroidPlatform : Platform { public override IRenderer CreateRenderer() => new VulkanRenderer(); }
//
// COMPARISON WITH SIMILAR PATTERNS
// ---------------------------------
// Factory Method vs Abstract Factory:
//   • Factory Method: Single product creation method
//   • Abstract Factory: Family of related products
//
// Factory Method vs Simple Factory:
//   • Factory Method: Uses inheritance (subclasses)
//   • Simple Factory: Uses parameters to decide type
//
// Factory Method vs Builder:
//   • Factory Method: Creates objects (which type)
//   • Builder: Constructs complex objects step-by-step (how to build)
//
// MODERN .NET ALTERNATIVE
// -----------------------
// Factory Method is still relevant, but modern DI can simplify:
//   // Traditional Factory Method
//   IDocument doc = creator.CreateDocument();
//
//   // Modern DI with Factory Pattern
//   public class DocumentFactory
//   {
//       private readonly IServiceProvider _provider;
//       public IDocument Create(string type) => type switch
//       {
//           "word" => _provider.GetService<WordDocument>(),
//           "pdf" => _provider.GetService<PdfDocument>(),
//           _ => throw new ArgumentException("Unknown type")
//       };
//   }
//
// ============================================================================

namespace RevisionNotesDemo.DesignPatterns.Creational;

// Product interface
public interface IDocument
{
    void Open();
    void Save();
    void Close();
    string GetDocumentType();
}

// Concrete Products
public class WordDocument : IDocument
{
    public void Open()
    {
        Console.WriteLine("[FACTORY] Opening Word document...");
    }

    public void Save()
    {
        Console.WriteLine("[FACTORY] Saving Word document...");
    }

    public void Close()
    {
        Console.WriteLine("[FACTORY] Closing Word document...");
    }

    public string GetDocumentType() => "Word Document (.docx)";
}

public class PdfDocument : IDocument
{
    public void Open()
    {
        Console.WriteLine("[FACTORY] Opening PDF document...");
    }

    public void Save()
    {
        Console.WriteLine("[FACTORY] Saving PDF document...");
    }

    public void Close()
    {
        Console.WriteLine("[FACTORY] Closing PDF document...");
    }

    public string GetDocumentType() => "PDF Document (.pdf)";
}

public class ExcelDocument : IDocument
{
    public void Open()
    {
        Console.WriteLine("[FACTORY] Opening Excel document...");
    }

    public void Save()
    {
        Console.WriteLine("[FACTORY] Saving Excel document...");
    }

    public void Close()
    {
        Console.WriteLine("[FACTORY] Closing Excel document...");
    }

    public string GetDocumentType() => "Excel Document (.xlsx)";
}

// Creator (abstract class with factory method)
public abstract class DocumentCreator
{
    // Factory method - subclasses decide which class to instantiate
    public abstract IDocument CreateDocument();

    // Template method that uses the factory method
    public void ProcessDocument(string content)
    {
        // The subclass decides which document type to create
        IDocument doc = CreateDocument();

        Console.WriteLine($"\n[FACTORY] Processing {doc.GetDocumentType()}");
        doc.Open();
        Console.WriteLine($"[FACTORY] Adding content: {content}");
        doc.Save();
        doc.Close();
    }
}

// Concrete Creators
public class WordDocumentCreator : DocumentCreator
{
    public override IDocument CreateDocument()
    {
        Console.WriteLine("[FACTORY] Factory creating Word document");
        return new WordDocument();
    }
}

public class PdfDocumentCreator : DocumentCreator
{
    public override IDocument CreateDocument()
    {
        Console.WriteLine("[FACTORY] Factory creating PDF document");
        return new PdfDocument();
    }
}

public class ExcelDocumentCreator : DocumentCreator
{
    public override IDocument CreateDocument()
    {
        Console.WriteLine("[FACTORY] Factory creating Excel document");
        return new ExcelDocument();
    }
}

// Practical example: Payment processing
public interface IPaymentProcessor
{
    bool ProcessPayment(decimal amount);
    string GetPaymentMethod();
}

public class CreditCardProcessor : IPaymentProcessor
{
    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"[FACTORY] Processing credit card payment: ${amount:F2}");
        return true;
    }

    public string GetPaymentMethod() => "Credit Card";
}

public class PayPalProcessor : IPaymentProcessor
{
    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"[FACTORY] Processing PayPal payment: ${amount:F2}");
        return true;
    }

    public string GetPaymentMethod() => "PayPal";
}

public class CryptoProcessor : IPaymentProcessor
{
    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"[FACTORY] Processing cryptocurrency payment: ${amount:F2}");
        return true;
    }

    public string GetPaymentMethod() => "Cryptocurrency";
}

// Factory for payment processors
public abstract class PaymentProcessorFactory
{
    public abstract IPaymentProcessor CreateProcessor();

    public void ProcessTransaction(decimal amount)
    {
        var processor = CreateProcessor();
        Console.WriteLine($"\n[FACTORY] Using {processor.GetPaymentMethod()} processor");
        processor.ProcessPayment(amount);
    }
}

public class CreditCardFactory : PaymentProcessorFactory
{
    public override IPaymentProcessor CreateProcessor() => new CreditCardProcessor();
}

public class PayPalFactory : PaymentProcessorFactory
{
    public override IPaymentProcessor CreateProcessor() => new PayPalProcessor();
}

public class CryptoFactory : PaymentProcessorFactory
{
    public override IPaymentProcessor CreateProcessor() => new CryptoProcessor();
}

// Usage demonstration
public class FactoryMethodDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== FACTORY METHOD PATTERN DEMO ===\n");

        Console.WriteLine("--- Example 1: Document Creation ---");

        // Client code works with creators through the base class
        DocumentCreator wordCreator = new WordDocumentCreator();
        DocumentCreator pdfCreator = new PdfDocumentCreator();
        DocumentCreator excelCreator = new ExcelDocumentCreator();

        wordCreator.ProcessDocument("Meeting notes");
        pdfCreator.ProcessDocument("Annual report");
        excelCreator.ProcessDocument("Budget spreadsheet");

        Console.WriteLine("\n--- Example 2: Payment Processing ---");

        var paymentFactories = new List<PaymentProcessorFactory>
        {
            new CreditCardFactory(),
            new PayPalFactory(),
            new CryptoFactory()
        };

        foreach (var factory in paymentFactories)
        {
            factory.ProcessTransaction(99.99m);
        }

        Console.WriteLine("\n💡 Benefit: Subclasses decide which concrete class to instantiate");
        Console.WriteLine("💡 Benefit: Follows Open-Closed Principle - easy to add new document types");
    }
}
