// ============================================================================
// DECORATOR PATTERN
// Reference: Revision Notes - Design Patterns (Structural) - Page 3
// ============================================================================
// PURPOSE: "Adds new functionality to an object dynamically without altering its structure."
// EXAMPLE: Adding encryption to a data stream.
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
