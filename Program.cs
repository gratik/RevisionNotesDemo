using Microsoft.Extensions.Caching.Memory;
using RevisionNotesDemo.Demo;
using RevisionNotesDemo.DotNetConcepts;

Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║   C# & OOP REVISION NOTES - COMPREHENSIVE DEMONSTRATION        ║");
Console.WriteLine("║   All Principles, Patterns & Concepts from the Revision Notes  ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");

var builder = WebApplication.CreateBuilder(args);

// From Revision Notes - Page 11: Dependency Injection
builder.Services.AddMemoryCache();
builder.Services.AddTransient<IGreeter, Greeter>();

var app = builder.Build();

await DemoOrchestrator.RunAsync(args);

// From Revision Notes - Page 11: Dependency Injection endpoint
app.MapGet("/hello", (IGreeter greeter) => greeter.Greet("World"))
    .WithName("Greeting");

var rootEndpoints = new[] { "/hello" };

app.MapGet("/", () => Results.Ok(new
{
    Message = "C# Revision Notes Demo API",
    Documentation = "Check the console output for comprehensive demonstrations",
    Endpoints = rootEndpoints
}));

app.Run();
