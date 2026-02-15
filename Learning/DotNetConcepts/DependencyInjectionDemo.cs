// ============================================================================
// DEPENDENCY INJECTION IN .NET CORE
// Reference: Revision Notes - .NET & Framework - Page 11
// ============================================================================
// WHAT IS THIS?
// -------------
// Built-in DI container with singleton, scoped, and transient lifetimes.
//
// WHY IT MATTERS
// --------------
// ✅ Improves testability and decouples implementations
// ✅ Centralizes service construction and lifetimes
//
// WHEN TO USE
// -----------
// ✅ Any ASP.NET Core app or service with dependencies
// ✅ Codebases that benefit from clear composition roots
//
// WHEN NOT TO USE
// ---------------
// ❌ Tiny scripts where DI adds unnecessary complexity
// ❌ Overengineering simple console utilities
//
// REAL-WORLD EXAMPLE
// ------------------
// Register and inject `IGreeter`.
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
