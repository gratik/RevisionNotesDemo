// ============================================================================
// REFLECTION AND ATTRIBUTES
// Reference: Revision Notes - .NET Framework - Page 7
// ============================================================================
// REFLECTION: Runtime type inspection and invocation
// ATTRIBUTES: Metadata tags for types, methods, properties
// USE CASES: Serialization, validation, DI, testing frameworks
// ============================================================================

using System.Reflection;

namespace RevisionNotesDemo.AdvancedCSharp;

// ============================================================================
// CUSTOM ATTRIBUTES
// ============================================================================

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class AuthorAttribute : Attribute
{
    public string Name { get; }
    public string Date { get; set; } = string.Empty;

    public AuthorAttribute(string name)
    {
        Name = name;
    }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RequiredAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
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

// ============================================================================
// EXAMPLE CLASS WITH ATTRIBUTES
// ============================================================================

[Author("Alice", Date = "2026-02-13")]
public class Employee
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(18, 65)]
    public int Age { get; set; }

    public string Department { get; set; } = string.Empty;

    [Author("Bob")]
    public void CalculateSalary()
    {
        Console.WriteLine("[REFLECTION] Calculating salary...");
    }

    public string GetInfo()
    {
        return $"{Name}, Age: {Age}, Dept: {Department}";
    }
}

// ============================================================================
// REFLECTION UTILITIES
// ============================================================================

public class ReflectionHelper
{
    public static void AnalyzeType(Type type)
    {
        Console.WriteLine($"[REFLECTION] Analyzing type: {type.Name}\n");

        // Get properties
        Console.WriteLine("[REFLECTION] Properties:");
        var properties = type.GetProperties();
        foreach (var prop in properties)
        {
            Console.WriteLine($"[REFLECTION]   - {prop.Name}: {prop.PropertyType.Name}");

            // Check for attributes on properties
            var attrs = prop.GetCustomAttributes();
            foreach (var attr in attrs)
            {
                Console.WriteLine($"[REFLECTION]     Attribute: {attr.GetType().Name}");
            }
        }

        // Get methods
        Console.WriteLine("\n[REFLECTION] Public Methods:");
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            var paramStr = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
            Console.WriteLine($"[REFLECTION]   - {method.ReturnType.Name} {method.Name}({paramStr})");
        }
    }

    public static object? InvokeMethod(object instance, string methodName, params object[] parameters)
    {
        var type = instance.GetType();
        var method = type.GetMethod(methodName);

        if (method == null)
        {
            Console.WriteLine($"[REFLECTION] Method '{methodName}' not found!");
            return null;
        }

        Console.WriteLine($"[REFLECTION] Invoking method: {methodName}");
        return method.Invoke(instance, parameters);
    }

    public static T CreateInstance<T>() where T : class
    {
        var type = typeof(T);
        Console.WriteLine($"[REFLECTION] Creating instance of {type.Name} using reflection");
        return (T)Activator.CreateInstance(type)!;
    }

    public static void SetProperty(object instance, string propertyName, object value)
    {
        var type = instance.GetType(); var property = type.GetProperty(propertyName);

        if (property == null)
        {
            Console.WriteLine($"[REFLECTION] Property '{propertyName}' not found!");
            return;
        }

        property.SetValue(instance, value);
        Console.WriteLine($"[REFLECTION] Set {propertyName} = {value}");
    }

    public static object? GetProperty(object instance, string propertyName)
    {
        var type = instance.GetType();
        var property = type.GetProperty(propertyName);

        if (property == null)
        {
            Console.WriteLine($"[REFLECTION] Property '{propertyName}' not found!");
            return null;
        }

        var value = property.GetValue(instance);
        Console.WriteLine($"[REFLECTION] Get {propertyName} = {value}");
        return value;
    }
}

// ============================================================================
// ATTRIBUTE VALIDATOR
// ============================================================================

public class Validator
{
    public static bool Validate(object obj)
    {
        var type = obj.GetType();
        var properties = type.GetProperties();
        bool isValid = true;

        Console.WriteLine($"[VALIDATION] Validating {type.Name}...");

        foreach (var property in properties)
        {
            var value = property.GetValue(obj);

            // Check Required attribute
            var requiredAttr = property.GetCustomAttribute<RequiredAttribute>();
            if (requiredAttr != null)
            {
                if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    Console.WriteLine($"[VALIDATION] ❌ {property.Name} is required but empty");
                    isValid = false;
                }
                else
                {
                    Console.WriteLine($"[VALIDATION] ✅ {property.Name} is valid");
                }
            }

            // Check Range attribute
            var rangeAttr = property.GetCustomAttribute<RangeAttribute>();
            if (rangeAttr != null && value is int intValue)
            {
                if (intValue < rangeAttr.Min || intValue > rangeAttr.Max)
                {
                    Console.WriteLine($"[VALIDATION] ❌ {property.Name} must be between {rangeAttr.Min} and {rangeAttr.Max}");
                    isValid = false;
                }
                else
                {
                    Console.WriteLine($"[VALIDATION] ✅ {property.Name} is in valid range");
                }
            }
        }

        return isValid;
    }
}

// ============================================================================
// DEMONSTRATION
// ============================================================================

public class ReflectionDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== REFLECTION AND ATTRIBUTES DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - .NET Framework - Page 7\n");

        // 1. Type Inspection
        Console.WriteLine("--- 1. Type Inspection with Reflection ---");
        ReflectionHelper.AnalyzeType(typeof(Employee));
        Console.WriteLine();

        // 2. Reading Attributes
        Console.WriteLine("--- 2. Reading Custom Attributes ---");
        var classAttr = typeof(Employee).GetCustomAttribute<AuthorAttribute>();
        if (classAttr != null)
        {
            Console.WriteLine($"[REFLECTION] Class Author: {classAttr.Name}, Date: {classAttr.Date}");
        }

        var methodInfo = typeof(Employee).GetMethod("CalculateSalary");
        var methodAttr = methodInfo?.GetCustomAttribute<AuthorAttribute>();
        if (methodAttr != null)
        {
            Console.WriteLine($"[REFLECTION] Method Author: {methodAttr.Name}\n");
        }

        // 3. Creating Instances
        Console.WriteLine("--- 3. Creating Instances Dynamically ---");
        var employee = ReflectionHelper.CreateInstance<Employee>();
        Console.WriteLine();

        // 4. Setting/Getting Properties
        Console.WriteLine("--- 4. Dynamic Property Access ---");
        ReflectionHelper.SetProperty(employee, "Name", "John Doe");
        ReflectionHelper.SetProperty(employee, "Age", 30);
        ReflectionHelper.SetProperty(employee, "Department", "IT");
        ReflectionHelper.GetProperty(employee, "Name");
        Console.WriteLine();

        // 5. Invoking Methods
        Console.WriteLine("--- 5. Dynamic Method Invocation ---");
        ReflectionHelper.InvokeMethod(employee, "CalculateSalary");
        var info = ReflectionHelper.InvokeMethod(employee, "GetInfo");
        Console.WriteLine($"[REFLECTION] Result: {info}\n");

        // 6. Attribute-Based Validation
        Console.WriteLine("--- 6. Attribute-Based Validation ---");
        var validEmployee = new Employee { Name = "Alice", Age = 25, Department = "HR" };
        Validator.Validate(validEmployee);

        Console.WriteLine();
        var invalidEmployee = new Employee { Name = "", Age = 70, Department = "HR" };
        Validator.Validate(invalidEmployee);
        Console.WriteLine();

        // 7. Generic Type Information
        Console.WriteLine("--- 7. Type Information ---");
        var type = typeof(Employee);
        Console.WriteLine($"[REFLECTION] Namespace: {type.Namespace}");
        Console.WriteLine($"[REFLECTION] Assembly: {type.Assembly.GetName().Name}");
        Console.WriteLine($"[REFLECTION] Is Class: {type.IsClass}");
        Console.WriteLine($"[REFLECTION] Is Abstract: {type.IsAbstract}");
        Console.WriteLine($"[REFLECTION] Is Sealed: {type.IsSealed}\n");

        Console.WriteLine("💡 Reflection Use Cases:");
        Console.WriteLine("   ✅ Dependency Injection containers");
        Console.WriteLine("   ✅ Serialization/Deserialization");
        Console.WriteLine("   ✅ Object-Relational Mappers (ORMs)");
        Console.WriteLine("   ✅ Unit testing frameworks");
        Console.WriteLine("   ✅ Validation frameworks");
        Console.WriteLine("   ⚠️  Performance cost - cache reflection results when possible");
    }
}
