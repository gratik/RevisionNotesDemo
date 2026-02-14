// ============================================================================
// FACTORY METHOD PATTERN
// Reference: Revision Notes - Design Patterns (Creational) - Page 3
// ============================================================================
// PURPOSE: "Defines an interface for creating objects but lets subclasses decide which class to instantiate."
// EXAMPLE: Creating different types of documents in an editor.
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
