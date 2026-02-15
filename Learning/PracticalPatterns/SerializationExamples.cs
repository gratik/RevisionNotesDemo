// ============================================================================
// SERIALIZATION EXAMPLES
// Reference: Revision Notes - Practical Scenarios - Page 13
// ============================================================================
// WHAT IS THIS?
// -------------
// JSON serialization with System.Text.Json, attributes, and converters.
//
// WHY IT MATTERS
// --------------
// ✅ Controls payload shape and naming
// ✅ Protects sensitive data and improves performance
//
// WHEN TO USE
// -----------
// ✅ API payloads and caching snapshots
// ✅ Persisting objects as JSON
//
// WHEN NOT TO USE
// ---------------
// ❌ When legacy features require a different serializer
// ❌ For binary protocols where JSON is inefficient
//
// REAL-WORLD EXAMPLE
// ------------------
// Hide internal fields and rename API properties.
// ============================================================================

using System.Text.Json;
using System.Text.Json.Serialization;

namespace RevisionNotesDemo.PracticalPatterns;

public class ProductData
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]  // Won't be serialized
    public string InternalCode { get; set; } = string.Empty;

    [JsonPropertyName("price_usd")]  // Custom JSON property name
    public decimal Price { get; set; }

    public DateTime CreatedDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]  // Serialize as string
    public ProductStatus Status { get; set; }
}

public enum ProductStatus
{
    Active,
    Discontinued,
    OutOfStock
}

public class SerializationDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== SERIALIZATION EXAMPLES DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Practical Scenarios - Page 13\n");

        var product = new ProductData
        {
            Id = 1,
            Name = "Laptop",
            InternalCode = "INTERNAL-123",
            Price = 999.99m,
            CreatedDate = DateTime.Now,
            Status = ProductStatus.Active
        };

        // Serialization
        Console.WriteLine("--- Serialization (Object to JSON) ---");
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(product, options);
        Console.WriteLine("[JSON] Serialized:");
        Console.WriteLine(json);
        Console.WriteLine();

        // Deserialization
        Console.WriteLine("--- Deserialization (JSON to Object) ---");
        var deserialized = JsonSerializer.Deserialize<ProductData>(json, options);
        Console.WriteLine($"[JSON] Deserialized: {deserialized?.Name}, Price: ${deserialized?.Price}\n");

        Console.WriteLine("💡 Serialization Tips:");
        Console.WriteLine("   ✅ Use [JsonIgnore] for sensitive data");
        Console.WriteLine("   ✅ Use [JsonPropertyName] for API compatibility");
        Console.WriteLine("   ✅ JsonStringEnumConverter for readable enums");
        Console.WriteLine("   ✅ System.Text.Json is faster than Newtonsoft.Json");
    }
}
