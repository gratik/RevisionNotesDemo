// ==============================================================================
// CONFIGURATION PATTERNS - Managing Application Settings
// ==============================================================================
// PURPOSE:
//   Master ASP.NET Core configuration system.
//   appsettings.json, environment variables, user secrets, reloading.
//
// WHY CONFIGURATION:
//   - Environment-specific settings (dev/staging/prod)
//   - Secure secrets management
//   - Hierarchical and composable
//   - Reload without restart
//
// WHAT YOU'LL LEARN:
//   1. IConfiguration basics
//   2. Configuration providers (JSON, Environment, User Secrets)
//   3. Hierarchical configuration
//   4. Connection strings
//   5. Configuration reloading
//   6. Best practices
//
// KEY PRINCIPLE:
//   Config providers stack - later providers override earlier ones.
// ==============================================================================

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Microsoft.EntityFrameworkCore;
// Note: Azure Key Vault requires: Azure.Extensions.AspNetCore.Configuration.Secrets, Azure.Identity
// using Azure.Identity;  // For DefaultAzureCredential

namespace RevisionNotesDemo.Configuration;

/// <summary>
/// EXAMPLE 1: ICONFIGURATION BASICS - Reading Settings
/// 
/// THE PROBLEM:
/// Need to access application settings from appsettings.json.
/// 
/// THE SOLUTION:
/// Inject IConfiguration, use indexer or GetSection/GetValue.
/// 
/// WHY IT MATTERS:
/// - Centralized configuration
/// - Environment-aware
/// - Type-safe access
/// </summary>
public class IConfigurationBasicsExamples
{
    // appsettings.json:
    // {
    //   "AppName": "MyApp",
    //   "MaxItems": 100,
    //   "Database": {
    //     "ConnectionString": "Server=.;Database=MyDb",
    //     "Timeout": 30
    //   },
    //   "Features": {
    //     "EnableCache": true,
    //     "EnableLogging": true
    //   }
    // }
    
    // ✅ GOOD: Inject IConfiguration
    public class MyService
    {
        private readonly IConfiguration _configuration;
        
        public MyService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void ReadSettings()
        {
            // ✅ Method 1: Indexer (returns string or null)
            var appName = _configuration["AppName"];  // "MyApp"
            
            // ✅ Method 2: GetValue<T> with type conversion
            var maxItems = _configuration.GetValue<int>("MaxItems");  // 100
            var enableCache = _configuration.GetValue<bool>("Features:EnableCache");  // true
            
            // ✅ Method 3: GetSection for nested config
            var databaseSection = _configuration.GetSection("Database");
            var connectionString = databaseSection["ConnectionString"];
            var timeout = databaseSection.GetValue<int>("Timeout");
            
            // ✅ Method 4: Colon notation for nested values
            var connString = _configuration["Database:ConnectionString"];
        }
        
        // ✅ GOOD: Default values
        public void ReadWithDefaults()
        {
            // If key doesn't exist, use default
            var pageSize = _configuration.GetValue<int>("PageSize", defaultValue: 20);
            var theme = _configuration.GetValue<string>("Theme", defaultValue: "Light");
        }
    }
    
    // ❌ BAD: Hardcoded settings
    public class BadService
    {
        public void DoWork()
        {
            var connectionString = "Server=.;Database=MyDb";  // ❌ Hardcoded
            var timeout = 30;  // ❌ Magic number
        }
    }
    
    // ✅ GOOD: Environment-specific appsettings
    // appsettings.json - Base settings
    // appsettings.Development.json - Dev overrides
    // appsettings.Production.json - Prod overrides
    //
    // ASP.NET Core automatically loads:
    // 1. appsettings.json
    // 2. appsettings.{Environment}.json (overrides)
    // 3. Environment variables (overrides)
    // 4. Command-line arguments (overrides)
}

/// <summary>
/// EXAMPLE 2: CONFIGURATION PROVIDERS - Multiple Sources
/// 
/// THE PROBLEM:
/// Need settings from JSON, environment variables, user secrets.
/// 
/// THE SOLUTION:
/// Configuration providers stack - last one wins.
/// 
/// WHY IT MATTERS:
/// - Dev: User Secrets (not in git)
/// - Staging/Prod: Environment Variables (Azure Key Vault)
/// - Base: appsettings.json
/// </summary>
public class ConfigurationProvidersExamples
{
    // ✅ GOOD: Default ASP.NET Core configuration
    public static void ConfigureDefault(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // ✅ Automatically configured in this order:
        // 1. appsettings.json
        // 2. appsettings.{Environment}.json
        // 3. User Secrets (Development only)
        // 4. Environment Variables
        // 5. Command-line arguments
        
        var app = builder.Build();
        
        // Later providers override earlier ones
        var config = app.Services.GetRequiredService<IConfiguration>();
    }
    
    // ✅ GOOD: Custom configuration sources
    public static IConfiguration BuildCustomConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            
            // 1. JSON files
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                optional: true, reloadOnChange: true)
            
            // 2. User Secrets (Development only)
            .AddUserSecrets<Program>(optional: true)
            
            // 3. Environment Variables
            .AddEnvironmentVariables()
            
            // 4. Command-line
            .AddCommandLine(Environment.GetCommandLineArgs());
        
        return builder.Build();
    }
    
    // ✅ USER SECRETS SETUP:
    // 1. Right-click project → Manage User Secrets
    // 2. Adds to secrets.json (not in git):
    //    {
    //      "Database:ConnectionString": "Server=dev;Database=DevDb",
    //      "ApiKeys:OpenAI": "sk-..."
    //    }
    // 3. Only loaded in Development environment
    
    // ✅ ENVIRONMENT VARIABLES:
    // Windows: set Database__ConnectionString=Server=prod;Database=ProdDb
    // Linux: export Database__ConnectionString="Server=prod;Database=ProdDb"
    // Azure: Configure in App Service Configuration
    // Note: Double underscore (__) = colon (:) in config keys
    
    /*
    // ✅ AZURE KEY VAULT:
    // Requires: dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
    //           dotnet add package Azure.Identity
    public static void AddKeyVault(WebApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["KeyVault:Uri"];
        if (!string.IsNullOrEmpty(keyVaultUri))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());  // ✅ Managed Identity
        }
    }
    */
}

/// <summary>
/// EXAMPLE 3: HIERARCHICAL CONFIGURATION - Nested Settings
/// 
/// THE PROBLEM:
/// Settings have natural hierarchy (Database.ConnectionString, Email.SmtpHost).
/// 
/// THE SOLUTION:
/// Use colon notation or GetSection for nested access.
/// </summary>
public class HierarchicalConfigurationExamples
{
    // appsettings.json:
    // {
    //   "Email": {
    //     "SmtpHost": "smtp.gmail.com",
    //     "SmtpPort": 587,
    //     "FromAddress": "noreply@myapp.com",
    //     "Credentials": {
    //       "Username": "user@gmail.com",
    //       "Password": "stored-in-secrets"
    //     }
    //   },
    //   "Logging": {
    //     "LogLevel": {
    //       "Default": "Information",
    //       "Microsoft": "Warning",
    //       "System": "Warning"
    //     }
    //   }
    // }
    
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void SendEmail()
        {
            // ✅ Method 1: Colon notation
            var host = _configuration["Email:SmtpHost"];
            var port = _configuration.GetValue<int>("Email:SmtpPort");
            var username = _configuration["Email:Credentials:Username"];
            
            // ✅ Method 2: GetSection (better for multiple reads)
            var emailSection = _configuration.GetSection("Email");
            var fromAddress = emailSection["FromAddress"];
            
            var credentialsSection = emailSection.GetSection("Credentials");
            var password = credentialsSection["Password"];
        }
    }
    
    // ✅ GOOD: Bind to strongly-typed class
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string FromAddress { get; set; } = string.Empty;
        public EmailCredentials Credentials { get; set; } = new();
    }
    
    public class EmailCredentials
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    
    public class EmailServiceTyped
    {
        private readonly EmailSettings _settings;
        
        public EmailServiceTyped(IConfiguration configuration)
        {
            // ✅ Bind section to class
            _settings = configuration.GetSection("Email").Get<EmailSettings>() ?? new();
            
            // Now use strongly-typed properties
            var host = _settings.SmtpHost;
            var port = _settings.SmtpPort;
            var username = _settings.Credentials.Username;
        }
    }
}

/// <summary>
/// EXAMPLE 4: CONNECTION STRINGS - Special Configuration Section
/// 
/// THE PROBLEM:
/// Connection strings are common - need easy access.
/// 
/// THE SOLUTION:
/// Special "ConnectionStrings" section with helper method.
/// </summary>
public class ConnectionStringsExamples
{
    // appsettings.json:
    // {
    //   "ConnectionStrings": {
    //     "DefaultConnection": "Server=.;Database=MyDb;Trusted_Connection=true",
    //     "ReadOnlyConnection": "Server=replica;Database=MyDb;Trusted_Connection=true",
    //     "RedisCache": "localhost:6379"
    //   }
    // }
    
    public class DatabaseService
    {
        private readonly string _connectionString;
        
        public DatabaseService(IConfiguration configuration)
        {
            // ✅ GetConnectionString helper method
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
            
            // Equivalent to:
            // _connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }
    }
    
    /*
    // ✅ GOOD: Register with DI
    // Requires DbContext definition: public class MyDbContext : DbContext { }
    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        
        builder.Services.AddDbContext<MyDbContext>(options =>
            options.UseSqlServer(connectionString));
    }
    */
    
    // ✅ PRODUCTION: Override in environment variables
    // Azure App Service: Add app setting "ConnectionStrings__DefaultConnection"
    // Docker: -e ConnectionStrings__DefaultConnection="..."
}

/// <summary>
/// EXAMPLE 5: CONFIGURATION RELOADING - Hot Reload Settings
/// 
/// THE PROBLEM:
/// Change settings without restarting application.
/// 
/// THE SOLUTION:
/// reloadOnChange: true + IOptionsMonitor or IConfiguration directly.
/// </summary>
public class ConfigurationReloadingExamples
{
    // ✅ GOOD: Enable reloading
    public static void EnableReloading()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json",
                optional: false,
                reloadOnChange: true)  // ✅ Watch file for changes
            .Build();
    }
    
    // ✅ GOOD: React to changes with IChangeToken
    public class FeatureService
    {
        private readonly IConfiguration _configuration;
        
        public FeatureService(IConfiguration configuration)
        {
            _configuration = configuration;
            
            // ✅ Register callback for changes
            ChangeToken.OnChange(
                () => _configuration.GetReloadToken(),
                () =>
                {
                    // Configuration changed!
                    var newValue = _configuration["FeatureFlag"];
                    Console.WriteLine($"Feature flag changed to: {newValue}");
                });
        }
    }
    
    // ⚠️ GOTCHA: IConfiguration always reads latest value
    // But IOptions<T> caches value - use IOptionsMonitor<T> for reloading
    // (See OptionsPatternDeepDive.cs for details)
}

// SUMMARY - Configuration Best Practices:
//
// ✅ DO:
// - Use IConfiguration or IOptions<T> for settings
// - Store secrets in User Secrets (dev) or Key Vault (prod)
// - Use environment-specific appsettings.{Environment}.json
// - Enable reloadOnChange for appsettings.json
// - Use ConnectionStrings section for connection strings
// - Use colon (:) for nested config keys
// - Use double underscore (__) in environment variables
//
// ❌ DON'T:
// - Hardcode settings in code
// - Commit secrets to git (use .gitignore for appsettings.Local.json)
// - Use appsettings.json for production secrets
// - Cache IConfiguration in static fields
// - Duplicate settings across files (use base + overrides)
//
// CONFIGURATION PRECEDENCE (last wins):
// 1. appsettings.json (base)
// 2. appsettings.{Environment}.json (environment override)
// 3. User Secrets (dev only, not in git)
// 4. Environment Variables (Azure/Docker)
// 5. Azure Key Vault (production secrets)
// 6. Command-line arguments (highest priority)
//
// ENVIRONMENT VARIABLES FORMAT:
// - Nested key: Database:ConnectionString
// - Env var name: Database__ConnectionString (double underscore)
// - Array: MyArray__0, MyArray__1, MyArray__2
