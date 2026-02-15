// ============================================================================
// ADAPTER PATTERN - Make Incompatible Interfaces Work Together
// Reference: Revision Notes - Design Patterns (Structural) - Page 3
// ============================================================================
//
// WHAT IS THE ADAPTER PATTERN?
// -----------------------------
// Allows two incompatible interfaces to work together by wrapping one interface
// and translating it to match the other. Acts as a bridge between two incompatible
// interfaces without modifying their source code.
//
// Think of it as: "A power adapter for UK plugs in US sockets - same electricity,
// different plug shapes"
//
// Core Concepts:
//   • Target Interface: What your application expects/needs
//   • Adaptee: Existing class with incompatible interface
//   • Adapter: Wraps Adaptee and implements Target interface
//   • Client: Uses Target interface, unaware of Adaptee
//   • Translation: Converts calls from Target to Adaptee format
//
// WHY IT MATTERS
// --------------
// ✅ LEGACY INTEGRATION: Use old code without rewriting it
// ✅ THIRD-PARTY LIBRARIES: Adapt external APIs to your interfaces
// ✅ OPEN/CLOSED PRINCIPLE: Extend functionality without modifying existing code
// ✅ INTERFACE CONSISTENCY: Provide uniform interface to incompatible systems
// ✅ MIGRATION PATH: Gradually replace old systems with new ones
// ✅ TESTABILITY: Mock adapted interface for testing
//
// WHEN TO USE IT
// --------------
// ✅ Need to use existing class but interface doesn't match
// ✅ Integrating with legacy systems or third-party libraries
// ✅ Want to reuse several existing subclasses lacking common functionality
// ✅ Cannot modify source code of existing class
// ✅ Need consistent interface across multiple incompatible implementations
// ✅ Migrating from old API to new one gradually
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Can modify the incompatible class directly
// ❌ Interfaces are already compatible
// ❌ Too many method translations needed (consider refactoring instead)
// ❌ Modern .NET libraries already provide abstractions (per Revision Notes)
// ❌ Adapter adds more complexity than value
//
// REAL-WORLD EXAMPLE
// ------------------
// Imagine a weather forecasting application using multiple data providers:
//   • Your app expects IWeatherService interface
//   • OpenWeatherMap API has GetCurrentWeather(lat, lon)
//   • WeatherAPI.com has FetchWeatherData(coordinates)
//   • AccuWeather has RetrieveForecast(location)
//   • Each returns data in different JSON format
//
// Without Adapter:
//   → if (provider == "OpenWeather") {
//         var data = openWeatherApi.GetCurrentWeather(lat, lon);
//         // Parse OpenWeather JSON format
//     } else if (provider == "WeatherAPI") {
//         var data = weatherApi.FetchWeatherData(coords);
//         // Parse WeatherAPI JSON format
//     } else if (provider == "AccuWeather") {
//         var data = accuWeather.RetrieveForecast(loc);
//         // Parse AccuWeather JSON format
//     }
//   → Client code tightly coupled to all APIs
//   → Different method names and parameters everywhere
//   → Switching providers requires code changes throughout app
//
// With Adapter:
//   → IWeatherService service = GetWeatherService(provider);
//   → var weather = service.GetWeather(location);  // Uniform interface
//   → // OpenWeatherAdapter translates to GetCurrentWeather()
//   → // WeatherAPIAdapter translates to FetchWeatherData()
//   → // AccuWeatherAdapter translates to RetrieveForecast()
//   → ✅ Client code uses single interface
//   → ✅ Easy to add/switch providers (just add new adapter)
//   → ✅ Each adapter handles its own JSON parsing
//   → ✅ Can mock IWeatherService for testing
//
// ADAPTER TYPES
// -------------
// Object Adapter (Composition - Recommended):
//   class Adapter : ITarget
//   {
//       private readonly Adaptee _adaptee;
//       public Adapter(Adaptee adaptee) => _adaptee = adaptee;
//   }
//   • Uses composition (has-a relationship)
//   • More flexible, follows composition over inheritance
//
// Class Adapter (Inheritance - Less Common in C#):
//   class Adapter : ITarget, Adaptee  // Multiple inheritance
//   • C# doesn't support multiple class inheritance
//   • Possible with interfaces only
//
// MODERN .NET CONSIDERATION
// -------------------------
// From Revision Notes: "Adapter – most .NET libraries already provide abstractions"
//
// Many modern libraries provide standard interfaces:
//   • ILogger, IConfiguration, IMemoryCache
//   • IHttpClientFactory
//   • IOptions<T>
//
// But Adapter still useful for:
//   • Legacy code integration
//   • Third-party libraries without standard interfaces
//   • External APIs with custom formats
//
// ============================================================================

namespace RevisionNotesDemo.DesignPatterns.Structural;

// Target interface (what our application expects)
public interface IPaymentProcessor
{
    bool ProcessPayment(string accountNumber, decimal amount);
    string GetProviderName();
}

// Legacy payment system with incompatible interface
public class LegacyPaymentSystem
{
    public void MakePayment(int accountId, double paymentAmount, string currency)
    {
        Console.WriteLine($"[ADAPTER] Legacy system processing: Account={accountId}, Amount={paymentAmount} {currency}");
    }

    public bool ValidateAccount(int accountId)
    {
        Console.WriteLine($"[ADAPTER] Legacy system validating account: {accountId}");
        return true;
    }
}

// Adapter that makes legacy system compatible with new interface
public class LegacyPaymentAdapter : IPaymentProcessor
{
    private readonly LegacyPaymentSystem _legacySystem;

    public LegacyPaymentAdapter(LegacyPaymentSystem legacySystem)
    {
        _legacySystem = legacySystem;
        Console.WriteLine("[ADAPTER] Legacy payment adapter created");
    }

    public bool ProcessPayment(string accountNumber, decimal amount)
    {
        // Convert string account number to int
        if (!int.TryParse(accountNumber, out int accountId))
        {
            Console.WriteLine("[ADAPTER] Invalid account number format");
            return false;
        }

        // Validate using legacy system
        if (!_legacySystem.ValidateAccount(accountId))
        {
            return false;
        }

        // Adapt the call to legacy system format
        _legacySystem.MakePayment(accountId, (double)amount, "USD");
        return true;
    }

    public string GetProviderName() => "Legacy Payment System (Adapted)";
}

// Modern payment system (already compatible)
public class ModernPaymentSystem : IPaymentProcessor
{
    public bool ProcessPayment(string accountNumber, decimal amount)
    {
        Console.WriteLine($"[ADAPTER] Modern system processing: Account={accountNumber}, Amount=${amount:F2}");
        return true;
    }

    public string GetProviderName() => "Modern Payment System";
}

// Another example: Third-party weather API adapter
public class ThirdPartyWeatherAPI
{
    public string GetTemperatureData(double latitude, double longitude)
    {
        return $"{{\"temp_celsius\": 25, \"lat\": {latitude}, \"lon\": {longitude}}}";
    }
}

// Our application's expected interface
public interface IWeatherService
{
    int GetTemperature(string city);
}

// Adapter for the third-party API
public class WeatherAPIAdapter : IWeatherService
{
    private readonly ThirdPartyWeatherAPI _api;
    private readonly Dictionary<string, (double, double)> _cityCoordinates = new()
    {
        { "London", (51.5074, -0.1278) },
        { "NewYork", (40.7128, -74.0060) },
        { "Tokyo", (35.6762, 139.6503) }
    };

    public WeatherAPIAdapter(ThirdPartyWeatherAPI api)
    {
        _api = api;
    }

    public int GetTemperature(string city)
    {
        if (!_cityCoordinates.TryGetValue(city, out var coords))
        {
            Console.WriteLine($"[ADAPTER] City {city} not found");
            return 0;
        }

        // Adapt third-party API call
        var data = _api.GetTemperatureData(coords.Item1, coords.Item2);
        Console.WriteLine($"[ADAPTER] Retrieved weather data for {city}: {data}");

        // Parse and adapt the response (simplified)
        return 25; // In real scenario, parse JSON
    }
}

// Usage demonstration
public class AdapterDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== ADAPTER PATTERN DEMO ===\n");

        Console.WriteLine("--- Example 1: Payment System Adapter ---");

        // Legacy system wrapped with adapter
        var legacySystem = new LegacyPaymentSystem();
        IPaymentProcessor legacyAdapter = new LegacyPaymentAdapter(legacySystem);

        // Modern system (no adapter needed)
        IPaymentProcessor modernSystem = new ModernPaymentSystem();

        // Client code works with both through the same interface
        var processors = new List<IPaymentProcessor> { legacyAdapter, modernSystem };

        foreach (var processor in processors)
        {
            Console.WriteLine($"\n[ADAPTER] Using: {processor.GetProviderName()}");
            processor.ProcessPayment("12345", 99.99m);
        }

        Console.WriteLine("\n--- Example 2: Weather API Adapter ---");
        var weatherAPI = new ThirdPartyWeatherAPI();
        IWeatherService weatherService = new WeatherAPIAdapter(weatherAPI);

        var cities = new[] { "London", "NewYork", "Tokyo" };
        foreach (var city in cities)
        {
            int temp = weatherService.GetTemperature(city);
            Console.WriteLine($"[ADAPTER] Temperature in {city}: {temp}°C");
        }

        Console.WriteLine("\n💡 Benefit: Makes incompatible interfaces work together");
        Console.WriteLine("💡 Benefit: Allows integration with legacy or third-party systems");
        Console.WriteLine("💡 From Revision Notes: Most .NET libraries already provide abstractions");
    }
}
