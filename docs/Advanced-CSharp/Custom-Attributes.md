# Custom Attributes

> Subject: [Advanced-CSharp](../README.md)

## Custom Attributes

### Creating Attributes

`csharp
// ✅ Simple attribute
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorAttribute : Attribute
{
    public string Name { get; }
    public string Date { get; set; } = string.Empty;
    
    public AuthorAttribute(string name)
    {
        Name = name;
    }
}

// ✅ Validation attribute
[AttributeUsage(AttributeTargets.Property)]
public class RequiredAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public class RangeAttribute : Attribute
{
    public int Min { get; }
    public int Max { get; }
    
    public RangeAttribute(int min, int max)
    {
        Min = min;
        Max = max;
    }
}
`

### Applying Attributes

`csharp
// ✅ Apply to class
[Author("Alice", Date = "2026-02-14")]
public class Employee
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Range(18, 65)]
    public int Age { get; set; }
    
    public string Department { get; set; } = string.Empty;
}
`

### Reading Attributes

`csharp
// ✅ Get attribute from type
Type type = typeof(Employee);
var authorAttr = type.GetCustomAttribute<AuthorAttribute>();
if (authorAttr != null)
{
    Console.WriteLine(Author: {authorAttr.Name}, Date: {authorAttr.Date});
}

// ✅ Get attributes from properties
foreach (var prop in type.GetProperties())
{
    var requiredAttr = prop.GetCustomAttribute<RequiredAttribute>();
    if (requiredAttr != null)
    {
        Console.WriteLine({prop.Name} is required);
    }
    
    var rangeAttr = prop.GetCustomAttribute<RangeAttribute>();
    if (rangeAttr != null)
    {
        Console.WriteLine({prop.Name} range: {rangeAttr.Min}-{rangeAttr.Max});
    }
}
`

---


