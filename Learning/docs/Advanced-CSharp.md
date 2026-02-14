# Advanced C# Features (Reflection and Attributes)

> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../../README.md)

## Overview

Reflection and attributes enable runtime type inspection, metadata manipulation, and dynamic code execution.
These powerful features are the foundation of serialization frameworks, dependency injection containers,
testing frameworks, and ORM tools. Use them carefully - reflection has performance costs.

---

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

## Practical Use Cases

### Simple Validation Framework

`csharp
// ✅ Validation using reflection and attributes
public class SimpleValidator
{
    public static List<string> Validate(object obj)
    {
        var errors = new List<string>();
        Type type = obj.GetType();
        
        foreach (var prop in type.GetProperties())
        {
            var value = prop.GetValue(obj);
            
            // Check Required
            var requiredAttr = prop.GetCustomAttribute<RequiredAttribute>();
            if (requiredAttr != null)
            {
                if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    errors.Add({prop.Name} is required);
                }
            }
            
            // Check Range
            var rangeAttr = prop.GetCustomAttribute<RangeAttribute>();
            if (rangeAttr != null && value is int intValue)
            {
                if (intValue < rangeAttr.Min || intValue > rangeAttr.Max)
                {
                    errors.Add({prop.Name} must be between {rangeAttr.Min} and {rangeAttr.Max});
                }
            }
        }
        
        return errors;
    }
}

// Usage
var employee = new Employee { Name = "", Age = 70 };
var errors = SimpleValidator.Validate(employee);
foreach (var error in errors)
{
    Console.WriteLine(Error: {error});
}
// Output:
// Error: Name is required
// Error: Age must be between 18 and 65
`

### Object Mapper

`csharp
// ✅ Simple object mapper using reflection
public class SimpleMapper
{
    public static TDestination Map<TSource, TDestination>(TSource source)
        where TDestination : new()
    {
        var destination = new TDestination();
        
        Type sourceType = typeof(TSource);
        Type destType = typeof(TDestination);
        
        foreach (var sourceProp in sourceType.GetProperties())
        {
            var destProp = destType.GetProperty(sourceProp.Name);
            if (destProp != null && destProp.CanWrite)
            {
                var value = sourceProp.GetValue(source);
                destProp.SetValue(destination, value);
            }
        }
        
        return destination;
    }
}

// Usage
var user = new User { Name = "Alice", Email = "alice@example.com" };
var userDto = SimpleMapper.Map<User, UserDto>(user);
`

### Plugin System

`csharp
// ✅ Load plugins dynamically
public interface IPlugin
{
    string Name { get; }
    void Execute();
}

public class PluginLoader
{
    public static List<IPlugin> LoadPlugins(string pluginDirectory)
    {
        var plugins = new List<IPlugin>();
        
        foreach (var file in Directory.GetFiles(pluginDirectory, "*.dll"))
        {
            var assembly = Assembly.LoadFrom(file);
            
            // Find types implementing IPlugin
            var pluginTypes = assembly.GetTypes()
                .Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface);
            
            foreach (var type in pluginTypes)
            {
                var plugin = (IPlugin?)Activator.CreateInstance(type);
                if (plugin != null)
                {
                    plugins.Add(plugin);
                }
            }
        }
        
        return plugins;
    }
}
`

---

## Performance Considerations

### Reflection is Slow

`csharp
// ❌ BAD: Reflection in hot path
for (int i = 0; i < 1000000; i++)
{
    PropertyInfo? prop = typeof(User).GetProperty("Name");
    prop?.SetValue(user, "Alice");  // ❌ Very slow!
}

// ✅ GOOD: Cache PropertyInfo
PropertyInfo? prop = typeof(User).GetProperty("Name");
for (int i = 0; i < 1000000; i++)
{
    prop?.SetValue(user, "Alice");  // ✅ Better
}

// ✅ BETTER: Use compiled expressions
var setter = CreateSetter<User, string>("Name");
for (int i = 0; i < 1000000; i++)
{
    setter(user, "Alice");  // ✅ Fast (near-native)
}
`

### Compiled Expressions for Performance

`csharp
// ✅ Create fast property setter using expressions
public static Action<T, TValue> CreateSetter<T, TValue>(string propertyName)
{
    var param1 = Expression.Parameter(typeof(T));
    var param2 = Expression.Parameter(typeof(TValue));
    var property = Expression.Property(param1, propertyName);
    var assign = Expression.Assign(property, param2);
    
    return Expression.Lambda<Action<T, TValue>>(assign, param1, param2).Compile();
}

// ✅ Create fast property getter
public static Func<T, TValue> CreateGetter<T, TValue>(string propertyName)
{
    var param = Expression.Parameter(typeof(T));
    var property = Expression.Property(param, propertyName);
    
    return Expression.Lambda<Func<T, TValue>>(property, param).Compile();
}
`

---

## Best Practices

### ✅ Reflection
- Cache Type and MemberInfo objects (expensive to retrieve)
- Use compiled expressions for hot paths
- Prefer generic constraints over reflection when possible
- Use reflection for frameworks/tools, not business logic
- Consider source generators (compile-time alternative)

### ✅ Attributes
- Keep attributes simple and data-focused
- Use AttributeUsage to control where applied
- Make attribute properties immutable when possible
- Document attribute behavior clearly
- Don't put complex logic in attributes

### ✅ Performance
- Measure impact with BenchmarkDotNet
- Cache reflection results
- Use expression trees for repeated calls
- Consider source generators for compile-time alternatives

---

## Common Pitfalls

### ❌ Not Caching Reflection Results

`csharp
// ❌ BAD: Repeated reflection calls
public void UpdateProperty(object obj, string propertyName, object value)
{
    var prop = obj.GetType().GetProperty(propertyName);  // ❌ Every time!
    prop?.SetValue(obj, value);
}

// ✅ GOOD: Cache results
private static readonly ConcurrentDictionary<(Type, string), PropertyInfo?> _propertyCache = new();

public void UpdateProperty(object obj, string propertyName, object value)
{
    var key = (obj.GetType(), propertyName);
    var prop = _propertyCache.GetOrAdd(key, k => k.Item1.GetProperty(k.Item2));
    prop?.SetValue(obj, value);
}
`

### ❌ Ignoring Null Checks

`csharp
// ❌ BAD: Can throw NullReferenceException
PropertyInfo prop = type.GetProperty("NonExistent");  // null!
prop.SetValue(obj, value);  // ❌ Boom!

// ✅ GOOD: Check for null
PropertyInfo? prop = type.GetProperty("Name");
if (prop != null && prop.CanWrite)
{
    prop.SetValue(obj, value);
}
`

---

## Related Files

- [AdvancedCSharp/ReflectionAndAttributes.cs](../AdvancedCSharp/ReflectionAndAttributes.cs)

---

## See Also

- [Core C# Features](Core-CSharp.md) - Generics and extension methods
- [Modern C# Features](Modern-CSharp.md) - Source generators as alternative
- [Performance](Performance.md) - Performance implications of reflection
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14
