// ==============================================================================
// OPTIONS PATTERN
// Reference: Revision Notes - Practical Scenarios
// ==============================================================================
// PURPOSE: Strongly-typed configuration with validation and change tracking
// BENEFIT: Type-safe config, validation, IOptionsSnapshot for scoped changes, IOptionsMonitor for hot reload
// USE WHEN: Need configuration in ASP.NET Core, want validated strongly-typed settings
// ==============================================================================

namespace RevisionNotesDemo.PracticalPatterns;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

// ========================================================================
// CONFIGURATION CLASSES (POCO)
// ========================================================================

/// <summary>
/// Database configuration options
/// </summary>
public class DatabaseOptions
{
    public const string SectionName = "Database";

    public string ConnectionString { get; set; } = string.Empty;
    public int MaxRetries { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 30;
    public bool EnableLogging { get; set; } = false;
}

/// <summary>
/// Email service configuration
/// </summary>
public class EmailOptions
{
    public const string SectionName = "Email";

    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public bool UseSsl { get; set; } = true;
}

/// <summary>
/// Feature flags configuration
/// </summary>
public class FeatureOptions
{
    public const string SectionName = "Features";

    public bool EnableNewUI { get; set; } = false;
    public bool EnableBetaFeatures { get; set; } = false;
    public int MaxUploadSizeMB { get; set; } = 10;
}

// ========================================================================
// VALIDATION (FluentValidation style validation)
// ========================================================================

/// <summary>
/// Validates DatabaseOptions on startup
/// </summary>
public class DatabaseOptionsValidator : IValidateOptions<DatabaseOptions>
{
    public ValidateOptionsResult Validate(string? name, DatabaseOptions options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
            errors.Add("ConnectionString is required");

        if (options.MaxRetries < 0 || options.MaxRetries > 10)
            errors.Add("MaxRetries must be between 0 and 10");

        if (options.TimeoutSeconds < 1 || options.TimeoutSeconds > 300)
            errors.Add("TimeoutSeconds must be between 1 and 300");

        if (errors.Any())
            return ValidateOptionsResult.Fail(errors);

        return ValidateOptionsResult.Success;
    }
}

// ========================================================================
// SERVICES USING OPTIONS
// ========================================================================

/// <summary>
/// Database service using IOptions (singleton - config at startup)
/// </summary>
public class DatabaseService
{
    private readonly DatabaseOptions _options;

    public DatabaseService(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;  // Read once at construction
    }

    public void Connect()
    {
        Console.WriteLine($"[DB SERVICE] 🔌 Connecting to database...");
        Console.WriteLine($"  ConnectionString: {_options.ConnectionString}");
        Console.WriteLine($"  MaxRetries: {_options.MaxRetries}");
        Console.WriteLine($"  TimeoutSeconds: {_options.TimeoutSeconds}");
        Console.WriteLine($"  EnableLogging: {_options.EnableLogging}");
        Console.WriteLine($"[DB SERVICE] ✅ Connected!\n");
    }
}

/// <summary>
/// Email service using IOptionsSnapshot (scoped - reloads per request)
/// </summary>
public class EmailService
{
    private readonly IOptionsSnapshot<EmailOptions> _optionsSnapshot;

    public EmailService(IOptionsSnapshot<EmailOptions> optionsSnapshot)
    {
        _optionsSnapshot = optionsSnapshot;
    }

    public void SendEmail(string to, string subject)
    {
        var options = _optionsSnapshot.Value;  // Read current snapshot

        Console.WriteLine($"[EMAIL SERVICE] 📧 Sending email...");
        Console.WriteLine($"  From: {options.FromName} <{options.FromAddress}>");
        Console.WriteLine($"  To: {to}");
        Console.WriteLine($"  Subject: {subject}");
        Console.WriteLine($"  SMTP: {options.SmtpServer}:{options.SmtpPort} (SSL: {options.UseSsl})");
        Console.WriteLine($"[EMAIL SERVICE] ✅ Email sent!\n");
    }
}

/// <summary>
/// Feature flag service using IOptionsMonitor (singleton - hot reload)
/// </summary>
public class FeatureService
{
    private readonly IOptionsMonitor<FeatureOptions> _optionsMonitor;

    public FeatureService(IOptionsMonitor<FeatureOptions> optionsMonitor)
    {
        _optionsMonitor = optionsMonitor;

        // Subscribe to changes
        _optionsMonitor.OnChange(options =>
        {
            Console.WriteLine($"[FEATURE SERVICE] 🔄 Configuration changed!");
            Console.WriteLine($"  EnableNewUI: {options.EnableNewUI}");
            Console.WriteLine($"  EnableBetaFeatures: {options.EnableBetaFeatures}\n");
        });
    }

    public bool IsFeatureEnabled(string featureName)
    {
        var options = _optionsMonitor.CurrentValue;  // Always current

        return featureName switch
        {
            "NewUI" => options.EnableNewUI,
            "BetaFeatures" => options.EnableBetaFeatures,
            _ => false
        };
    }

    public void ShowCurrentFeatures()
    {
        var options = _optionsMonitor.CurrentValue;

        Console.WriteLine($"[FEATURE SERVICE] 🚩 Current Feature Flags:");
        Console.WriteLine($"  EnableNewUI: {options.EnableNewUI}");
        Console.WriteLine($"  EnableBetaFeatures: {options.EnableBetaFeatures}");
        Console.WriteLine($"  MaxUploadSizeMB: {options.MaxUploadSizeMB}\n");
    }
}

// ========================================================================
// DEMONSTRATION
// ========================================================================

public class OptionsPatternDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== OPTIONS PATTERN DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Practical Scenarios\n");

        // Setup dependency injection container
        var services = new ServiceCollection();

        // Configure options from in-memory configuration
        services.Configure<DatabaseOptions>(options =>
        {
            options.ConnectionString = "Server=localhost;Database=MyApp;";
            options.MaxRetries = 5;
            options.TimeoutSeconds = 60;
            options.EnableLogging = true;
        });

        services.Configure<EmailOptions>(options =>
        {
            options.SmtpServer = "smtp.gmail.com";
            options.SmtpPort = 587;
            options.FromAddress = "noreply@example.com";
            options.FromName = "MyApp Notifications";
            options.UseSsl = true;
        });

        services.Configure<FeatureOptions>(options =>
        {
            options.EnableNewUI = true;
            options.EnableBetaFeatures = false;
            options.MaxUploadSizeMB = 25;
        });

        // Register validation
        services.AddSingleton<IValidateOptions<DatabaseOptions>, DatabaseOptionsValidator>();

        // Register services
        services.AddSingleton<DatabaseService>();
        services.AddScoped<EmailService>();
        services.AddSingleton<FeatureService>();

        // Build service provider
        var serviceProvider = services.BuildServiceProvider();

        // Example 1: IOptions<T> - Singleton, read once
        Console.WriteLine("=== EXAMPLE 1: IOptions<T> (Singleton) ===\n");
        Console.WriteLine("IOptions<T>: Configuration read once at construction time\n");

        var dbService = serviceProvider.GetRequiredService<DatabaseService>();
        dbService.Connect();

        // Example 2: IOptionsSnapshot<T> - Scoped, reloads per scope
        Console.WriteLine("=== EXAMPLE 2: IOptionsSnapshot<T> (Scoped) ===\n");
        Console.WriteLine("IOptionsSnapshot<T>: Configuration reloaded per scope/request\n");

        using (var scope1 = serviceProvider.CreateScope())
        {
            Console.WriteLine("--- Scope 1 ---");
            var emailService = scope1.ServiceProvider.GetRequiredService<EmailService>();
            emailService.SendEmail("user@example.com", "Welcome!");
        }

        using (var scope2 = serviceProvider.CreateScope())
        {
            Console.WriteLine("--- Scope 2 ---");
            var emailService = scope2.ServiceProvider.GetRequiredService<EmailService>();
            emailService.SendEmail("admin@example.com", "Monthly Report");
        }

        // Example 3: IOptionsMonitor<T> - Singleton, hot reload
        Console.WriteLine("=== EXAMPLE 3: IOptionsMonitor<T> (Hot Reload) ===\n");
        Console.WriteLine("IOptionsMonitor<T>: Always reads current configuration, supports change notifications\n");

        var featureService = serviceProvider.GetRequiredService<FeatureService>();
        featureService.ShowCurrentFeatures();

        Console.WriteLine("--- Checking Feature Flags ---");
        Console.WriteLine($"  Is NewUI enabled? {featureService.IsFeatureEnabled("NewUI")}");
        Console.WriteLine($"  Is BetaFeatures enabled? {featureService.IsFeatureEnabled("BetaFeatures")}\n");

        // Example 4: Named Options (multiple configurations)
        Console.WriteLine("=== EXAMPLE 4: Named Options ===\n");
        Console.WriteLine("Named options allow multiple configurations of the same type\n");

        var services2 = new ServiceCollection();

        // Configure "Primary" database
        services2.Configure<DatabaseOptions>("Primary", options =>
        {
            options.ConnectionString = "Server=primary.db;Database=MainDB;";
            options.MaxRetries = 5;
        });

        // Configure "Reporting" database
        services2.Configure<DatabaseOptions>("Reporting", options =>
        {
            options.ConnectionString = "Server=reporting.db;Database=AnalyticsDB;";
            options.MaxRetries = 3;
        });

        var provider2 = services2.BuildServiceProvider();
        var optionsSnapshot = provider2.GetRequiredService<IOptionsSnapshot<DatabaseOptions>>();

        var primaryDb = optionsSnapshot.Get("Primary");
        var reportingDb = optionsSnapshot.Get("Reporting");

        Console.WriteLine("[NAMED OPTIONS] Primary database:");
        Console.WriteLine($"  ConnectionString: {primaryDb.ConnectionString}");
        Console.WriteLine($"  MaxRetries: {primaryDb.MaxRetries}\n");

        Console.WriteLine("[NAMED OPTIONS] Reporting database:");
        Console.WriteLine($"  ConnectionString: {reportingDb.ConnectionString}");
        Console.WriteLine($"  MaxRetries: {reportingDb.MaxRetries}\n");

        Console.WriteLine("💡 Options Pattern Benefits:");
        Console.WriteLine("   ✅ Strongly-typed configuration - compile-time safety");
        Console.WriteLine("   ✅ Validation - validate on startup or per request");
        Console.WriteLine("   ✅ IOptions<T> - singleton, read once (best performance)");
        Console.WriteLine("   ✅ IOptionsSnapshot<T> - scoped, reloads per request");
        Console.WriteLine("   ✅ IOptionsMonitor<T> - hot reload, change notifications");
        Console.WriteLine("   ✅ Named options - multiple configurations");
        Console.WriteLine("   ✅ Testable - easy to mock configuration");

        Console.WriteLine("\n💡 When to Use Each:");
        Console.WriteLine("   🔹 IOptions<T>: Singleton services, config doesn't change");
        Console.WriteLine("   🔹 IOptionsSnapshot<T>: Scoped services, per-request config");
        Console.WriteLine("   🔹 IOptionsMonitor<T>: Hot reload, change notifications");

        Console.WriteLine("\n💡 Real-World Usage:");
        Console.WriteLine("   • ASP.NET Core configuration (appsettings.json)");
        Console.WriteLine("   • Feature flags");
        Console.WriteLine("   • Database connection settings");
        Console.WriteLine("   • External service configurations");
        Console.WriteLine("   • Application behavior settings");
    }
}