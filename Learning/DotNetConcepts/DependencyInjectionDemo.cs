// ============================================================================
// DEPENDENCY INJECTION IN .NET CORE
// Reference: Revision Notes - .NET & Framework - Page 11
// ============================================================================
// Built-in DI container with AddSingleton, AddScoped, AddTransient
// Constructor injection is most common pattern
// ============================================================================

namespace RevisionNotesDemo.DotNetConcepts;

// From Revision Notes - Page 11: Service + contract
public interface IGreeter
{
    string Greet(string name);
}

public sealed class Greeter : IGreeter
{
    public string Greet(string name) => $"Hello, {name}!";
}

// Register in Program.cs:
// builder.Services.AddTransient<IGreeter, Greeter>();
// 
// Usage in endpoint:
// app.MapGet("/hello", (IGreeter g) => g.Greet("World"));
