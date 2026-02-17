# Custom Attributes

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Strong Core C# fundamentals and familiarity with reflection, delegates, and generics.
- Related examples: docs/Advanced-CSharp/README.md
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


## Interview Answer Block
30-second answer:
- Custom Attributes is about runtime and advanced type-system behavior in C#. It matters because it helps solve specialized problems without sacrificing reliability.
- Use it when building framework-like components and diagnostics tooling.

2-minute answer:
- Start with the problem Custom Attributes solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: powerful features vs maintainability for less-experienced maintainers.
- Close with one failure mode and mitigation: using advanced mechanisms where straightforward code would be clearer.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Custom Attributes but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Custom Attributes, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Custom Attributes and map it to one concrete implementation in this module.
- 3 minutes: compare Custom Attributes with an alternative, then walk through one failure mode and mitigation.