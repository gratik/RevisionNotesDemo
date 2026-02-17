# Reflection Basics

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Strong Core C# fundamentals and familiarity with reflection, delegates, and generics.
- Related examples: docs/Advanced-CSharp/README.md
> Subject: [Advanced-CSharp](../README.md)

## Reflection Basics

### What is Reflection?

**Reflection** = Inspect types, methods, properties, and attributes at runtime

`csharp
// ✅ Get type information
Type type = typeof(User);
Console.WriteLine(type.Name);        // "User"
Console.WriteLine(type.Namespace);   // "MyApp.Models"
Console.WriteLine(type.IsClass);     // true

// ✅ Get type from instance
var user = new User();
Type instanceType = user.GetType();

// ✅ Get type by name (string)
Type? dynamicType = Type.GetType("MyApp.Models.User");
`

### Inspecting Properties

`csharp
// ✅ Get all properties
Type type = typeof(User);
PropertyInfo[] properties = type.GetProperties();

foreach (var prop in properties)
{
    Console.WriteLine(Property: {prop.Name}, Type: {prop.PropertyType.Name});
}

// ✅ Get specific property
PropertyInfo? emailProp = type.GetProperty("Email");
if (emailProp != null)
{
    Console.WriteLine(Can read: {emailProp.CanRead});
    Console.WriteLine(Can write: {emailProp.CanWrite});
}

// ✅ Get/Set property value
var user = new User();
PropertyInfo? nameProp = type.GetProperty("Name");
nameProp?.SetValue(user, "Alice");  // ✅ Set value
var name = nameProp?.GetValue(user);  // ✅ Get value
`

### Inspecting Methods

`csharp
// ✅ Get all methods
MethodInfo[] methods = type.GetMethods();

foreach (var method in methods)
{
    Console.WriteLine(Method: {method.Name});
    Console.WriteLine(  Returns: {method.ReturnType.Name});
    
    // Parameters
    foreach (var param in method.GetParameters())
    {
        Console.WriteLine(  Param: {param.Name} ({param.ParameterType.Name}));
    }
}

// ✅ Invoke method
MethodInfo? calcMethod = type.GetMethod("Calculate");
if (calcMethod != null)
{
    var result = calcMethod.Invoke(user, new object[] { 10, 20 });
    Console.WriteLine(Result: {result});
}
`

### Creating Instances Dynamically

`csharp
// ✅ Create instance using Activator
Type type = typeof(User);
var instance = Activator.CreateInstance(type);  // Calls parameterless constructor

// ✅ Create with constructor parameters
var instanceWithParams = Activator.CreateInstance(
    type, 
    new object[] { "Alice", "alice@example.com" }
);

// ✅ Create generic type
Type genericType = typeof(List<>).MakeGenericType(typeof(int));
var list = Activator.CreateInstance(genericType);  // List<int>
`

---


## Interview Answer Block
30-second answer:
- Reflection Basics is about runtime and advanced type-system behavior in C#. It matters because it helps solve specialized problems without sacrificing reliability.
- Use it when building framework-like components and diagnostics tooling.

2-minute answer:
- Start with the problem Reflection Basics solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: powerful features vs maintainability for less-experienced maintainers.
- Close with one failure mode and mitigation: using advanced mechanisms where straightforward code would be clearer.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Reflection Basics but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Reflection Basics, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Reflection Basics and map it to one concrete implementation in this module.
- 3 minutes: compare Reflection Basics with an alternative, then walk through one failure mode and mitigation.