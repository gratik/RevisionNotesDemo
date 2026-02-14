// ============================================================================
// ADAPTER PATTERN
// Reference: Revision Notes - Design Patterns (Structural) - Page 3
// ============================================================================
// PURPOSE: "Allows incompatible interfaces to work together."
// EXAMPLE: Connecting a legacy payment system to a new API.
// NOTE: From Revision Notes - "Adapter – most .NET libraries already provide abstractions"
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
