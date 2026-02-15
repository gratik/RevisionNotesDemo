// ==============================================================================
// OPTIONS PATTERN DEEP DIVE - Type-Safe Configuration
// ==============================================================================
// WHAT IS THIS?
// -------------
// Strongly typed configuration via IOptions, snapshots, and monitors.
//
// WHY IT MATTERS
// --------------
// ✅ Safer settings with validation
// ✅ Supports reload scenarios with IOptionsMonitor
//
// WHEN TO USE
// -----------
// ✅ ASP.NET Core config bound to POCOs
// ✅ Settings that need validation or reload
//
// WHEN NOT TO USE
// ---------------
// ❌ One-off config access where binding is unnecessary
// ❌ Hardcoding settings that should be configurable
//
// REAL-WORLD EXAMPLE
// ------------------
// Bind EmailSettings and validate on startup.
// ==============================================================================

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace RevisionNotesDemo.Configuration;

/// <summary>
/// EXAMPLE 1: IOPTIONS<T> - Basic Singleton Options
/// 
/// THE PROBLEM:
/// IConfiguration requires string keys, error-prone.
/// 
/// THE SOLUTION:
/// Bind configuration section to strongly-typed class.
/// 
/// WHY IT MATTERS:
/// - IntelliSense support
/// - Compile-time checking
/// - Refactoring safe
/// </summary>
public class IOptionsBasicsExamples
{
    // appsettings.json:
    // {
    //   "Email": {
    //     "SmtpHost": "smtp.gmail.com",
    //     "SmtpPort": 587,
    //     "FromAddress": "noreply@myapp.com",
    //     "EnableSsl": true
    //   }
    // }

    // ✅ GOOD: Settings class
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string FromAddress { get; set; } = string.Empty;
        public bool EnableSsl { get; set; }
    }

    // ✅ Configure in Program.cs
    public static void ConfigureOptions(WebApplicationBuilder builder)
    {
        // ✅ Bind "Email" section to EmailSettings
        builder.Services.Configure<EmailSettings>(
            builder.Configuration.GetSection("Email"));
    }

    // ✅ GOOD: Inject IOptions<T>
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;  // ✅ Get settings value
        }

        public void SendEmail(string to, string subject, string body)
        {
            // ✅ Type-safe access
            var host = _settings.SmtpHost;
            var port = _settings.SmtpPort;
            var from = _settings.FromAddress;

            // Send email...
        }
    }

    // ❌ BAD: Using IConfiguration directly
    public class EmailServiceBad
    {
        private readonly IConfiguration _configuration;

        public EmailServiceBad(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail()
        {
            // ❌ String keys, no type safety
            var host = _configuration["Email:SmtpHost"];  // Typo? Runtime error!
            var port = _configuration.GetValue<int>("Email:SmtpPort");
        }
    }

    // IOptions<T> CHARACTERISTICS:
    // - ✅ Singleton lifetime (one instance for app)
    // - ✅ Fast (cached after first access)
    // - ❌ Doesn't reload on configuration change
    // - ✅ Use for settings that don't need hot reload
}

/// <summary>
/// EXAMPLE 2: IOPTIONSSNAPSHOT<T> - Per-Request Reloading
/// 
/// THE PROBLEM:
/// IOptions<T> doesn't reload. Need fresh config per request.
/// 
/// THE SOLUTION:
/// IOptionsSnapshot<T> reloads configuration per HTTP request.
/// 
/// WHY IT MATTERS:
/// - Hot reload without restart
/// - Per-request isolation
/// - Perfect for feature flags
/// </summary>
public class IOptionsSnapshotExamples
{
    public class FeatureSettings
    {
        public bool EnableNewFeature { get; set; }
        public int MaxItems { get; set; }
        public string Theme { get; set; } = "Light";
    }

    // ✅ Configure
    public static void Configure(WebApplicationBuilder builder)
    {
        builder.Services.Configure<FeatureSettings>(
            builder.Configuration.GetSection("Features"));
    }

    // ✅ GOOD: Inject IOptionsSnapshot<T> in scoped services
    public class ProductService
    {
        private readonly FeatureSettings _settings;

        public ProductService(IOptionsSnapshot<FeatureSettings> options)
        {
            _settings = options.Value;  // ✅ Reloaded per request
        }

        public List<Product> GetProducts()
        {
            // ✅ Settings reflect latest appsettings.json
            var maxItems = _settings.MaxItems;

            if (_settings.EnableNewFeature)
            {
                return GetProductsV2(maxItems);
            }

            return GetProductsV1(maxItems);
        }

        private List<Product> GetProductsV1(int count) => new();
        private List<Product> GetProductsV2(int count) => new();
    }

    public class Product { public int Id { get; set; } }

    // IOptionsSnapshot<T> CHARACTERISTICS:
    // - ✅ Scoped lifetime (reloaded per request)
    // - ✅ Reloads when config file changes
    // - ✅ Use for feature flags, per-request settings
    // - ❌ Cannot inject into singleton services
    // - ❌ Not available in background services (no scope)
}

/// <summary>
/// EXAMPLE 3: IOPTIONSMONITOR<T> - Real-Time Change Notifications
/// 
/// THE PROBLEM:
/// Need hot reload in singleton services or background workers.
/// 
/// THE SOLUTION:
/// IOptionsMonitor<T> provides change notifications and always-current value.
/// </summary>
public class IOptionsMonitorExamples
{
    public class CacheSettings
    {
        public int ExpirationMinutes { get; set; }
        public int MaxSize { get; set; }
        public bool EnableCompression { get; set; }
    }

    // ✅ Configure
    public static void Configure(WebApplicationBuilder builder)
    {
        builder.Services.Configure<CacheSettings>(
            builder.Configuration.GetSection("Cache"));
    }

    // ✅ GOOD: Inject IOptionsMonitor<T> in singleton
    public class CacheService
    {
        private readonly IOptionsMonitor<CacheSettings> _optionsMonitor;
        private CacheSettings _currentSettings;

        public CacheService(IOptionsMonitor<CacheSettings> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
            _currentSettings = optionsMonitor.CurrentValue;

            // ✅ Subscribe to changes
            optionsMonitor.OnChange(settings =>
            {
                _currentSettings = settings;
                Console.WriteLine($"Cache settings changed! New expiration: {settings.ExpirationMinutes}min");

                // ✅ React to change (e.g., clear cache, adjust limits)
                ReconfigureCache();
            });
        }

        public void CacheItem(string key, object value)
        {
            // ✅ Always use latest settings
            var expiration = TimeSpan.FromMinutes(_optionsMonitor.CurrentValue.ExpirationMinutes);

            // Cache with current expiration...
        }

        private void ReconfigureCache()
        {
            // Adjust cache based on new settings
        }
    }

    // ✅ GOOD: Background service with hot reload
    public class MetricsCollector : BackgroundService
    {
        private readonly IOptionsMonitor<MetricsSettings> _optionsMonitor;

        public MetricsCollector(IOptionsMonitor<MetricsSettings> optionsMonitor)
        {
            _optionsMonitor = optionsMonitor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // ✅ Get latest settings each iteration
                var settings = _optionsMonitor.CurrentValue;

                await Task.Delay(TimeSpan.FromSeconds(settings.IntervalSeconds), stoppingToken);

                // Collect metrics...
            }
        }
    }

    public class MetricsSettings
    {
        public int IntervalSeconds { get; set; }
    }

    // IOptionsMonitor<T> CHARACTERISTICS:
    // - ✅ Singleton lifetime
    // - ✅ Always returns latest configuration
    // - ✅ OnChange notifications
    // - ✅ Works in singleton and background services
    // - ✅ Use for long-running services needing hot reload
}

// DECISION TREE - Which Options Interface?
//
// +------------------------+------------------+--------------------+---------------------+
// | Scenario               | IOptions<T>      | IOptionsSnapshot<T>| IOptionsMonitor<T>  |
// +------------------------+------------------+--------------------+---------------------+
// | Service Lifetime       | Any              | Scoped only        | Any                 |
// | Reloads Config         | ❌ No            | ✅ Per request     | ✅ Real-time        |
// | Change Notifications   | ❌ No            | ❌ No              | ✅ Yes (OnChange)   |
// | Performance            | ✅ Fastest       | ⚠️ Moderate        | ⚠️ Moderate         |
// | Use Case               | Static settings  | Feature flags      | Background services |
// +------------------------+------------------+--------------------+---------------------+
//
// RECOMMENDATION:
// - Default: IOptions<T> (simplest, fastest)
// - Per-request reload: IOptionsSnapshot<T> (feature flags)
// - Singleton with hot reload: IOptionsMonitor<T> (background workers)

/// <summary>
/// EXAMPLE 4: VALIDATION - Fail Fast on Invalid Configuration
/// 
/// THE PROBLEM:
/// Invalid configuration discovered at runtime (after deploy).
/// 
/// THE SOLUTION:
/// Validate settings at startup with Data Annotations.
/// </summary>
public class ValidationExamples
{
    // ✅ GOOD: Settings with validation attributes
    public class DatabaseSettings
    {
        [Required]
        [MinLength(10)]
        public string ConnectionString { get; set; } = string.Empty;

        [Range(1, 300)]
        public int CommandTimeout { get; set; } = 30;

        [Range(1, 1000)]
        public int MaxPoolSize { get; set; } = 100;
    }

    public class EmailSettings
    {
        [Required]
        [EmailAddress]
        public string FromAddress { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^smtp\..+$", ErrorMessage = "Must be SMTP host")]
        public string SmtpHost { get; set; } = string.Empty;

        [Range(1, 65535)]
        public int SmtpPort { get; set; }
    }

    // ✅ Configure with validation
    public static void ConfigureWithValidation(WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<DatabaseSettings>()
            .Bind(builder.Configuration.GetSection("Database"))
            .ValidateDataAnnotations()  // ✅ Validate attributes
            .ValidateOnStart();  // ✅ Fail at startup, not first use

        builder.Services.AddOptions<EmailSettings>()
            .Bind(builder.Configuration.GetSection("Email"))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    // ✅ GOOD: Custom validation
    public class ApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; }
        public string ApiKey { get; set; } = string.Empty;
    }

    public static void ConfigureWithCustomValidation(WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<ApiSettings>()
            .Bind(builder.Configuration.GetSection("Api"))
            .Validate(settings =>
            {
                // ✅ Custom validation logic
                if (string.IsNullOrEmpty(settings.ApiKey))
                    return false;

                if (settings.TimeoutSeconds < 1 || settings.TimeoutSeconds > 300)
                    return false;

                if (!Uri.TryCreate(settings.BaseUrl, UriKind.Absolute, out _))
                    return false;

                return true;
            }, "Api settings are invalid")
            .ValidateOnStart();
    }

    // ✅ BEST: Implement IValidateOptions<T> for complex validation
    public class ApiSettingsValidator : IValidateOptions<ApiSettings>
    {
        public ValidateOptionsResult Validate(string? name, ApiSettings options)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(options.ApiKey))
                errors.Add("ApiKey is required");

            if (options.ApiKey?.Length < 20)
                errors.Add("ApiKey must be at least 20 characters");

            if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri))
                errors.Add("BaseUrl must be a valid absolute URL");
            else if (uri.Scheme != "https" && uri.Host != "localhost")
                errors.Add("BaseUrl must use HTTPS (except localhost)");

            if (options.TimeoutSeconds < 1 || options.TimeoutSeconds > 300)
                errors.Add("TimeoutSeconds must be between 1 and 300");

            return errors.Count > 0
                ? ValidateOptionsResult.Fail(errors)
                : ValidateOptionsResult.Success;
        }
    }

    public static void RegisterValidator(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IValidateOptions<ApiSettings>, ApiSettingsValidator>();

        builder.Services.AddOptions<ApiSettings>()
            .Bind(builder.Configuration.GetSection("Api"))
            .ValidateOnStart();  // ✅ Uses registered validator
    }
}

/// <summary>
/// EXAMPLE 5: NAMED OPTIONS - Multiple Configurations of Same Type
/// 
/// THE PROBLEM:
/// Need multiple database connections with same settings structure.
/// 
/// THE SOLUTION:
/// Named options - multiple instances of same settings type.
/// </summary>
public class NamedOptionsExamples
{
    // appsettings.json:
    // {
    //   "Databases": {
    //     "Primary": {
    //       "ConnectionString": "Server=primary;Database=MyDb",
    //       "Timeout": 30
    //     },
    //     "ReadOnly": {
    //       "ConnectionString": "Server=replica;Database=MyDb",
    //       "Timeout": 60
    //     }
    //   }
    // }

    public class DatabaseOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int Timeout { get; set; }
    }

    // ✅ Configure named options
    public static void ConfigureNamedOptions(WebApplicationBuilder builder)
    {
        // Primary database
        builder.Services.Configure<DatabaseOptions>("Primary",
            builder.Configuration.GetSection("Databases:Primary"));

        // Read-only replica
        builder.Services.Configure<DatabaseOptions>("ReadOnly",
            builder.Configuration.GetSection("Databases:ReadOnly"));
    }

    // ✅ GOOD: Use IOptionsSnapshot with names
    public class DataService
    {
        private readonly DatabaseOptions _primaryOptions;
        private readonly DatabaseOptions _readOnlyOptions;

        public DataService(IOptionsSnapshot<DatabaseOptions> optionsSnapshot)
        {
            _primaryOptions = optionsSnapshot.Get("Primary");
            _readOnlyOptions = optionsSnapshot.Get("ReadOnly");
        }

        public void SaveData()
        {
            var connectionString = _primaryOptions.ConnectionString;
            // Write to primary database...
        }

        public List<Data> ReadData()
        {
            var connectionString = _readOnlyOptions.ConnectionString;
            // Read from replica...
            return new List<Data>();
        }
    }

    public class Data { }
}

// SUMMARY - Options Pattern Best Practices:
//
// ✅ DO:
// - Use IOptions<T> for simple, static settings
// - Use IOptionsSnapshot<T> for per-request reload
// - Use IOptionsMonitor<T> for singleton hot reload
// - Validate with [Required], [Range], etc.
// - Call ValidateOnStart() to fail fast
// - Use OptionsBuilder for advanced configuration
// - Use named options for multiple configs of same type
//
// ❌ DON'T:
// - Inject IOptionsSnapshot into singleton services
// - Forget ValidateOnStart (fails on first use, not startup)
// - Cache options.Value in constructor (use CurrentValue for Monitor)
// - Use IConfiguration when IOptions is better
//
// VALIDATION LEVELS:
// 1. Data Annotations: [Required], [Range], [EmailAddress]
// 2. Validate lambda: Simple custom checks
// 3. IValidateOptions<T>: Complex, reusable validation
//
// CONFIGURATION:
// builder.Services.AddOptions<MySettings>()
//     .Bind(configuration.GetSection("MySection"))  // ✅ Bind config
//     .ValidateDataAnnotations()                    // ✅ Validate attributes
//     .Validate(s => s.Value > 0, "Must be positive")  // ✅ Custom check
//     .ValidateOnStart();                           // ✅ Fail at startup
