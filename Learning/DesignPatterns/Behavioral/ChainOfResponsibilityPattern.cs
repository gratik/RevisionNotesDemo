// ============================================================================
// CHAIN OF RESPONSIBILITY PATTERN
// Reference: Revision Notes - Design Patterns (Behavioral) - Page 3
// ============================================================================
// PURPOSE: "Passes a request along a chain of handlers until one handles it."
// EXAMPLE: Logging with multiple output targets.
// ============================================================================

namespace RevisionNotesDemo.DesignPatterns.Behavioral;

// Handler interface
public abstract class LogHandler
{
    protected LogHandler? NextHandler;

    public void SetNext(LogHandler handler)
    {
        NextHandler = handler;
    }

    public abstract void Handle(string message, LogLevel level);
}

public enum LogLevel
{
    Info,
    Warning,
    Error
}

// Concrete Handlers
public class ConsoleLogHandler : LogHandler
{
    public override void Handle(string message, LogLevel level)
    {
        if (level == LogLevel.Info)
        {
            Console.WriteLine($"[CHAIN] 💻 Console: [INFO] {message}");
        }
        else
        {
            NextHandler?.Handle(message, level);
        }
    }
}

public class FileLogHandler : LogHandler
{
    public override void Handle(string message, LogLevel level)
    {
        if (level == LogLevel.Warning)
        {
            Console.WriteLine($"[CHAIN] 📄 File: [WARNING] {message}");
        }
        else
        {
            NextHandler?.Handle(message, level);
        }
    }
}

public class EmailLogHandler : LogHandler
{
    public override void Handle(string message, LogLevel level)
    {
        if (level == LogLevel.Error)
        {
            Console.WriteLine($"[CHAIN] 📧 Email: [ERROR] {message}");
        }
        else
        {
            NextHandler?.Handle(message, level);
        }
    }
}

// Usage demonstration
public class ChainOfResponsibilityDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== CHAIN OF RESPONSIBILITY PATTERN DEMO ===\n");

        // Build the chain
        var consoleHandler = new ConsoleLogHandler();
        var fileHandler = new FileLogHandler();
        var emailHandler = new EmailLogHandler();

        consoleHandler.SetNext(fileHandler);
        fileHandler.SetNext(emailHandler);

        Console.WriteLine("[CHAIN] Logging various messages:\n");

        consoleHandler.Handle("Application started", LogLevel.Info);
        consoleHandler.Handle("Low disk space", LogLevel.Warning);
        consoleHandler.Handle("Database connection failed", LogLevel.Error);

        Console.WriteLine("\n💡 Benefit: Request passes through chain until handled");
        Console.WriteLine("💡 Benefit: Decouples sender from receivers");
    }
}
