// ============================================================================
// SINGLETON PATTERN
// Reference: Revision Notes - Design Patterns (Creational) - Page 3
// ============================================================================
// PURPOSE: "Ensures only one instance of a class exists and provides a global access point."
// EXAMPLE: Logging service, configuration manager.
// NOTE: From Revision Notes - "Singleton - replaced by DI lifetimes (AddSingleton)" in modern .NET
// ============================================================================

namespace RevisionNotesDemo.DesignPatterns.Creational;

// ❌ NOT THREAD-SAFE - Don't use in production!
public class SingletonBad
{
    private static SingletonBad? _instance;

    private SingletonBad() { }

    public static SingletonBad Instance
    {
        get
        {
            // Race condition - multiple threads could create multiple instances!
            if (_instance == null)
            {
                _instance = new SingletonBad();
            }
            return _instance;
        }
    }
}

// ✅ THREAD-SAFE SINGLETON - Traditional approach
// Reference: Revision Notes - "Thread-safe Singleton in C#" - Page 13
public sealed class ThreadSafeSingleton
{
    // Using Lazy<T> for thread-safe lazy initialization
    private static readonly Lazy<ThreadSafeSingleton> _instance =
        new Lazy<ThreadSafeSingleton>(() => new ThreadSafeSingleton());

    public static ThreadSafeSingleton Instance => _instance.Value;

    private ThreadSafeSingleton()
    {
        Console.WriteLine("[SINGLETON] Thread-safe Singleton instance created");
    }

    public void DoSomething()
    {
        Console.WriteLine("[SINGLETON] Singleton instance method called");
    }
}

// ✅ PRACTICAL EXAMPLE: Configuration Manager
public sealed class ConfigurationManager
{
    private static readonly Lazy<ConfigurationManager> _instance =
        new Lazy<ConfigurationManager>(() => new ConfigurationManager());

    public static ConfigurationManager Instance => _instance.Value;

    private readonly Dictionary<string, string> _settings;

    private ConfigurationManager()
    {
        Console.WriteLine("[SINGLETON] Loading configuration...");
        _settings = new Dictionary<string, string>
        {
            { "AppName", "RevisionNotesDemo" },
            { "Version", "1.0.0" },
            { "Environment", "Development" },
            { "MaxConnections", "100" }
        };
    }

    public string GetSetting(string key)
    {
        return _settings.TryGetValue(key, out var value)
            ? value
            : throw new KeyNotFoundException($"Setting '{key}' not found");
    }

    public void SetSetting(string key, string value)
    {
        _settings[key] = value;
        Console.WriteLine($"[SINGLETON] Setting updated: {key} = {value}");
    }

    public void PrintAllSettings()
    {
        Console.WriteLine("[SINGLETON] Current Configuration:");
        foreach (var setting in _settings)
        {
            Console.WriteLine($"[SINGLETON]   {setting.Key}: {setting.Value}");
        }
    }
}

// ✅ PRACTICAL EXAMPLE: Logger (thread-safe singleton)
public sealed class Logger
{
    private static readonly Lazy<Logger> _instance =
        new Lazy<Logger>(() => new Logger());

    public static Logger Instance => _instance.Value;

    private readonly object _lock = new object();
    private readonly List<string> _logs = new List<string>();

    private Logger()
    {
        Console.WriteLine("[SINGLETON] Logger initialized");
    }

    public void Log(string message, string level = "INFO")
    {
        lock (_lock) // Thread-safe logging
        {
            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            _logs.Add(logEntry);
            Console.WriteLine($"[SINGLETON] {logEntry}");
        }
    }

    public void LogError(string message) => Log(message, "ERROR");
    public void LogWarning(string message) => Log(message, "WARN");
    public void LogInfo(string message) => Log(message, "INFO");

    public void PrintLogs()
    {
        lock (_lock)
        {
            Console.WriteLine("\n[SINGLETON] === Log History ===");
            foreach (var log in _logs)
            {
                Console.WriteLine($"[SINGLETON] {log}");
            }
            Console.WriteLine($"[SINGLETON] Total logs: {_logs.Count}\n");
        }
    }
}

// Modern .NET alternative - Dependency Injection
// From Revision Notes: "Singleton - replaced by DI lifetimes (AddSingleton)"
public interface IModernConfigService
{
    string GetConfig(string key);
}

public class ModernConfigService : IModernConfigService
{
    public ModernConfigService()
    {
        Console.WriteLine("[SINGLETON] ModernConfigService created via DI");
    }

    public string GetConfig(string key)
    {
        Console.WriteLine($"[SINGLETON] Getting config from DI Singleton: {key}");
        return $"Config value for {key}";
    }
}

// Usage: In Program.cs, use: builder.Services.AddSingleton<IModernConfigService, ModernConfigService>();
// This is the preferred modern approach over traditional Singleton pattern

// Usage demonstration
public class SingletonDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== SINGLETON PATTERN DEMO ===\n");

        Console.WriteLine("--- Example 1: Thread-Safe Singleton ---");
        var singleton1 = ThreadSafeSingleton.Instance;
        var singleton2 = ThreadSafeSingleton.Instance;
        Console.WriteLine($"[SINGLETON] Same instance? {ReferenceEquals(singleton1, singleton2)}");
        singleton1.DoSomething();

        Console.WriteLine("\n--- Example 2: Configuration Manager ---");
        var config = ConfigurationManager.Instance;
        config.PrintAllSettings();
        Console.WriteLine($"[SINGLETON] App Name: {config.GetSetting("AppName")}");
        config.SetSetting("NewSetting", "NewValue");

        Console.WriteLine("\n--- Example 3: Logger ---");
        var logger = Logger.Instance;
        logger.LogInfo("Application started");
        logger.LogWarning("This is a warning");
        logger.LogError("This is an error");
        logger.PrintLogs();

        Console.WriteLine("--- Example 4: Proving Singleton works across calls ---");
        var anotherConfig = ConfigurationManager.Instance;
        Console.WriteLine($"[SINGLETON] Same config instance? {ReferenceEquals(config, anotherConfig)}");
        anotherConfig.PrintAllSettings(); // Should show the NewSetting we added earlier

        Console.WriteLine("\n💡 From Revision Notes:");
        Console.WriteLine("   Traditional Singleton is now mostly replaced by DI lifetimes (AddSingleton)");
        Console.WriteLine("   Modern .NET: builder.Services.AddSingleton<IService, Service>()");
    }
}
