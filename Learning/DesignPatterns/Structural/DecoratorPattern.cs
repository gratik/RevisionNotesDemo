// ============================================================================
// DECORATOR PATTERN - Add Functionality Dynamically Without Inheritance
// Reference: Revision Notes - Design Patterns (Structural) - Page 3
// ============================================================================
//
// WHAT IS THE DECORATOR PATTERN?
// -------------------------------
// Attaches additional responsibilities to an object dynamically by wrapping it.
// Decorators provide a flexible alternative to subclassing for extending functionality.
// Multiple decorators can be stacked to combine behaviors without class explosion.
//
// Think of it as: "Dressing up - start with base outfit, add jacket, then scarf,
// then hat - each layer adds functionality"
//
// Core Concepts:
//   • Component: Interface for objects that can have responsibilities added
//   • Concrete Component: Base object to which functionality is added
//   • Decorator: Abstract class implementing Component and containing Component
//   • Concrete Decorators: Add specific responsibilities to the component
//   • Wrapping: Each decorator wraps another component (chain of responsibility)
//
// WHY IT MATTERS
// --------------
// ✅ FLEXIBILITY: Add/remove responsibilities at runtime, not compile-time
// ✅ SINGLE RESPONSIBILITY: Each decorator has one specific enhancement
// ✅ OPEN/CLOSED: Extend behavior without modifying existing code
// ✅ COMPOSABILITY: Mix and match decorators in any combination
// ✅ AVOID CLASS EXPLOSION: No need for every combination as subclass
// ✅ TRANSPARENT: Decorators conform to same interface as base object
//
// WHEN TO USE IT
// --------------
// ✅ Add responsibilities to individual objects dynamically
// ✅ Responsibilities can be withdrawn later
// ✅ Extension by subclassing is impractical (class explosion)
// ✅ Need to add features in various combinations
// ✅ Want to keep classes focused (Single Responsibility Principle)
// ✅ Runtime configuration of object behavior needed
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Simple inheritance suffices
// ❌ Order of decorators matters and is hard to maintain
// ❌ Too many small decorator classes (maintenance burden)
// ❌ Need to change core behavior (use inheritance instead)
// ❌ Debugging becomes difficult (stack of wrappers)
//
// REAL-WORLD EXAMPLE
// ------------------
// Imagine Starbucks coffee ordering system:
//   • Base: Coffee ($2)
//   • Add Milk (+$0.50)
//   • Add Sugar (+$0.25)
//   • Add Whipped Cream (+$0.75)
//   • Add Caramel (+$0.50)
//   • Customers want ANY combination
//
// Without Decorator (Inheritance):
//   → Need classes: Coffee, CoffeeWithMilk, CoffeeWithSugar, CoffeeWithMilkAndSugar,
//     CoffeeWithMilkAndWhippedCream, CoffeeWithMilkSugarAndWhippedCream...
//   → 5 additions = 2^5 = 32 possible classes!
//   → Adding new topping (Vanilla) = double all classes
//   → Impossible to maintain
//
// With Decorator:
//   → Coffee coffee = new Coffee();                    // $2.00
//   → coffee = new MilkDecorator(coffee);             // $2.50
//   → coffee = new SugarDecorator(coffee);            // $2.75
//   → coffee = new WhippedCreamDecorator(coffee);     // $3.50
//   → coffee = new CaramelDecorator(coffee);          // $4.00
//   → Any combination possible with same 6 classes
//   → Adding Vanilla = just 1 new VanillaDecorator class
//   → Total: Cost() method adds up all decorator costs
//
// ANOTHER REAL-WORLD EXAMPLE - Logging/Streaming
// ----------------------------------------------
// Data processing pipeline needs various transformations:
//   • Base: FileStream (reads/writes raw bytes)
//   • Add EncryptionDecorator (encrypts data)
//   • Add CompressionDecorator (compresses data)
//   • Add LoggingDecorator (logs all operations)
//
// Stream pipeline:
//   IDataStream stream = new FileStream();
//   stream = new EncryptionDecorator(stream);    // Encrypt
//   stream = new CompressionDecorator(stream);   // Then compress
//   stream = new LoggingDecorator(stream);       // Then log
//   stream.WriteData("Sensitive data");           // Logged → Compressed → Encrypted → Written
//
// Data flows through decorator chain:
//   WriteData → Log → Compress → Encrypt → File
//   ReadData → File → Decrypt → Decompress → Log
//
// .NET FRAMEWORK EXAMPLES
// -----------------------
// .NET uses Decorator pattern extensively:
//   • Stream classes: BufferedStream wraps FileStream
//   • CryptoStream wraps any stream to add encryption
//   • GZipStream wraps streams to add compression
//
//   Example:
//   using (var fileStream = new FileStream("data.txt", FileMode.Create))
//   using (var gzipStream = new GZipStream(fileStream, CompressionMode.Compress))
//   using (var cryptoStream = new CryptoStream(gzipStream, encryptor, CryptoStreamMode.Write))
//   {
//       // Write to cryptoStream → encrypted → compressed → written to file
//   }
//
// DECORATOR VS SIMILAR PATTERNS
// -----------------------------
// Decorator vs Adapter:
//   • Decorator: Enhances with same interface
//   • Adapter: Converts one interface to another
//
// Decorator vs Proxy:
//   • Decorator: Adds functionality
//   • Proxy: Controls access (same functionality)
//
// Decorator vs Composite:
//   • Decorator: Single wrapped object
//   • Composite: Tree of objects
//
// ============================================================================

namespace RevisionNotesDemo.DesignPatterns.Structural;

// Component interface
public interface IDataStream
{
    void WriteData(string data);
    string ReadData();
}

// Concrete component
public class FileDataStream : IDataStream
{
    private string _data = string.Empty;

    public void WriteData(string data)
    {
        _data = data;
        Console.WriteLine($"[DECORATOR] FileDataStream: Writing data");
    }

    public string ReadData()
    {
        Console.WriteLine($"[DECORATOR] FileDataStream: Reading data");
        return _data;
    }
}

// Base decorator
public abstract class DataStreamDecorator : IDataStream
{
    protected readonly IDataStream _wrappedStream;

    protected DataStreamDecorator(IDataStream stream)
    {
        _wrappedStream = stream;
    }

    public virtual void WriteData(string data)
    {
        _wrappedStream.WriteData(data);
    }

    public virtual string ReadData()
    {
        return _wrappedStream.ReadData();
    }
}

// Concrete decorator - Encryption
public class EncryptionDecorator : DataStreamDecorator
{
    public EncryptionDecorator(IDataStream stream) : base(stream)
    {
        Console.WriteLine("[DECORATOR] Adding encryption layer");
    }

    public override void WriteData(string data)
    {
        var encrypted = Encrypt(data);
        Console.WriteLine($"[DECORATOR] Encrypting data: {data} -> {encrypted}");
        _wrappedStream.WriteData(encrypted);
    }

    public override string ReadData()
    {
        var data = _wrappedStream.ReadData();
        var decrypted = Decrypt(data);
        Console.WriteLine($"[DECORATOR] Decrypting data: {data} -> {decrypted}");
        return decrypted;
    }

    private string Encrypt(string data) => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data));
    private string Decrypt(string data) => System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(data));
}

// Concrete decorator - Compression
public class CompressionDecorator : DataStreamDecorator
{
    public CompressionDecorator(IDataStream stream) : base(stream)
    {
        Console.WriteLine("[DECORATOR] Adding compression layer");
    }

    public override void WriteData(string data)
    {
        var compressed = Compress(data);
        Console.WriteLine($"[DECORATOR] Compressing data: {data.Length} -> {compressed.Length} chars");
        _wrappedStream.WriteData(compressed);
    }

    public override string ReadData()
    {
        var data = _wrappedStream.ReadData();
        var decompressed = Decompress(data);
        Console.WriteLine($"[DECORATOR] Decompressing data: {data.Length} -> {decompressed.Length} chars");
        return decompressed;
    }

    private string Compress(string data) => $"[COMPRESSED:{data}]";
    private string Decompress(string data) => data.Replace("[COMPRESSED:", "").TrimEnd(']');
}

// Concrete decorator - Logging
public class LoggingDecorator : DataStreamDecorator
{
    public LoggingDecorator(IDataStream stream) : base(stream)
    {
        Console.WriteLine("[DECORATOR] Adding logging layer");
    }

    public override void WriteData(string data)
    {
        Console.WriteLine($"[DECORATOR] LOG: Writing {data.Length} characters at {DateTime.Now:T}");
        _wrappedStream.WriteData(data);
        Console.WriteLine("[DECORATOR] LOG: Write operation completed");
    }

    public override string ReadData()
    {
        Console.WriteLine($"[DECORATOR] LOG: Reading data at {DateTime.Now:T}");
        var data = _wrappedStream.ReadData();
        Console.WriteLine($"[DECORATOR] LOG: Read {data.Length} characters");
        return data;
    }
}

// Usage demonstration
public class DecoratorDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== DECORATOR PATTERN DEMO ===\n");

        Console.WriteLine("--- Example 1: Basic File Stream ---");
        IDataStream basicStream = new FileDataStream();
        basicStream.WriteData("Hello World");
        Console.WriteLine($"[DECORATOR] Data read: {basicStream.ReadData()}\n");

        Console.WriteLine("--- Example 2: File Stream + Encryption ---");
        IDataStream encryptedStream = new EncryptionDecorator(new FileDataStream());
        encryptedStream.WriteData("Sensitive Data");
        Console.WriteLine($"[DECORATOR] Data read: {encryptedStream.ReadData()}\n");

        Console.WriteLine("--- Example 3: File Stream + Compression + Encryption ---");
        IDataStream multiStream = new EncryptionDecorator(
            new CompressionDecorator(
                new FileDataStream()
            )
        );
        multiStream.WriteData("This is compressed and encrypted data");
        Console.WriteLine($"[DECORATOR] Data read: {multiStream.ReadData()}\n");

        Console.WriteLine("--- Example 4: All Decorators (File + Log + Compress + Encrypt) ---");
        IDataStream fullyDecoratedStream = new LoggingDecorator(
            new EncryptionDecorator(
                new CompressionDecorator(
                    new FileDataStream()
                )
            )
        );
        fullyDecoratedStream.WriteData("Complex processing pipeline");
        Console.WriteLine($"[DECORATOR] Final data: {fullyDecoratedStream.ReadData()}\n");

        Console.WriteLine("💡 Benefit: Add functionality dynamically without modifying original class");
        Console.WriteLine("💡 Benefit: Stack multiple decorators for complex behavior");
        Console.WriteLine("💡 Benefit: Follows Open-Closed Principle");
    }
}
