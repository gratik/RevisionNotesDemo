# Serialization

> Subject: [Practical-Patterns](../README.md)

## Serialization

### System.Text.Json

```csharp
// ✅ Serialize/Deserialize
var user = new User { Name = "Alice", Email = "alice@example.com" };

// To JSON
string json = JsonSerializer.Serialize(user, new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
});

// From JSON
var deserializedUser = JsonSerializer.Deserialize<User>(json);

// ✅ Custom converter for DateTime
public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.Parse(reader.GetString()!);
    }
    
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
    }
}
```

---


