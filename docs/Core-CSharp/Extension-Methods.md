# Extension Methods

> Subject: [Core-CSharp](../README.md)

## Extension Methods

### Basic Extension Methods

```csharp
// ✅ Extend string without modifying the class
public static class StringExtensions
{
    public static bool IsBlank(this string value)
    {
        return string.IsNullOrWhiteSpace(value);
    }
    
    public static string Truncate(this string value, int maxLength)
    {
        if (value.Length <= maxLength) return value;
        return value.Substring(0, maxLength) + "...";
    }
}

// Usage
string name = "  ";
if (name.IsBlank())  // ✅ Reads like an instance method
{
    Console.WriteLine("Name is blank");
}

string longText = "This is a very long text";
string short = longText.Truncate(10);  // "This is a ..."
```

### Extension Methods on Generics

```csharp
// ✅ Extend IEnumerable<T>
public static class EnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> source)
    {
        return !source.Any();
    }
    
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
        where T : class
    {
        return source.Where(x => x != null)!;
    }
}

// Usage
var numbers = new List<int>();
if (numbers.IsEmpty())  // ✅ More readable than !numbers.Any()
{
    Console.WriteLine("List is empty");
}
```

---


