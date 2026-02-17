# Practical Use Cases

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Advanced-CSharp](../README.md)

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


## Interview Answer Block
30-second answer:
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

