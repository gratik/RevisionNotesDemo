// ============================================================================
// C# REVISION NOTES DEMONSTRATION PROJECT
// ============================================================================
// This project demonstrates ALL principles from the Revision Notes document:
// - OOP Principles (SRP, OCP, LSP, ISP, DIP)
// - KISS, DRY, YAGNI, TDA
// - Design Patterns (Creational, Structural, Behavioral)
// - Memory Management (Stack/Heap, GC, Struct vs Class)
// - Multithreading & Async
// - .NET Framework Concepts
// - Practical Scenarios
// ============================================================================

using RevisionNotesDemo.OOPPrinciples;
using RevisionNotesDemo.DesignPatterns.Creational;
using RevisionNotesDemo.DesignPatterns.Structural;
using RevisionNotesDemo.DesignPatterns.Behavioral;
using RevisionNotesDemo.MemoryManagement;
using RevisionNotesDemo.AsyncMultithreading;
using RevisionNotesDemo.DotNetConcepts;
using RevisionNotesDemo.CoreCSharpFeatures;
using RevisionNotesDemo.LINQAndQueries;
using RevisionNotesDemo.AdvancedCSharp;
using RevisionNotesDemo.PracticalPatterns;
using RevisionNotesDemo.Testing;
using RevisionNotesDemo.Models;
using RevisionNotesDemo.Appendices;
using Microsoft.Extensions.Caching.Memory;

Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║   C# & OOP REVISION NOTES - COMPREHENSIVE DEMONSTRATION        ║");
Console.WriteLine("║   All Principles, Patterns & Concepts from the Revision Notes  ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");

var builder = WebApplication.CreateBuilder(args);

// From Revision Notes - Page 11: Dependency Injection
builder.Services.AddMemoryCache();
builder.Services.AddTransient<IGreeter, Greeter>();

var app = builder.Build();

// ============================================================================
// CONSOLE DEMONSTRATIONS - Run all examples
// ============================================================================

Console.WriteLine("\n\n▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀");
Console.WriteLine("  PART 1: OOP PRINCIPLES");
Console.WriteLine("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");

SRPDemo.RunDemo();
OCPDemo.RunDemo();
LSPDemo.RunDemo();
ISPDemo.RunDemo();
DIPDemo.RunDemo();

Console.WriteLine("\n\n▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀");
Console.WriteLine("  PART 2: KISS, DRY, YAGNI, TDA");
Console.WriteLine("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");

KISSDRYYAGNIDemo.RunDemo();
TDADemo.RunDemo();

Console.WriteLine("\n\n▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀");
Console.WriteLine("  PART 3: DESIGN PATTERNS - CREATIONAL");
Console.WriteLine("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");

SingletonDemo.RunDemo();
FactoryMethodDemo.RunDemo();
AbstractFactoryDemo.RunDemo();
BuilderDemo.RunDemo();
PrototypeDemo.RunDemo();

Console.WriteLine("\n\n▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀");
Console.WriteLine("  PART 4: DESIGN PATTERNS - STRUCTURAL");
Console.WriteLine("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");

AdapterDemo.RunDemo();
DecoratorDemo.RunDemo();
FacadeDemo.RunDemo();
CompositeDemo.RunDemo();
ProxyDemo.RunDemo();
CQRSDemo.RunDemo();
await RepositoryDemo.RunDemoAsync();
UnitOfWorkDemo.RunDemo();
FlyweightDemo.RunDemo();
BridgeDemo.RunDemo();

Console.WriteLine("\n\n▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀");
Console.WriteLine("  PART 5: DESIGN PATTERNS - BEHAVIORAL");
Console.WriteLine("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");

ObserverDemo.RunDemo();
StrategyDemo.RunDemo();
CommandDemo.RunDemo();
MediatorDemo.RunDemo();
StateDemo.RunDemo();
ChainOfResponsibilityDemo.RunDemo();
SpecificationDemo.RunDemo();
NullObjectDemo.RunDemo();
TemplateMethodDemo.RunDemo();
VisitorDemo.RunDemo();
MementoDemo.RunDemo();

Console.WriteLine("\n\n▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀");
Console.WriteLine("  PART 6: MEMORY MANAGEMENT");
Console.WriteLine("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");

StackVsHeapDemo.RunDemo();
GarbageCollectionDemo.RunDemo();
MemoryLeakDemo.RunDemo();
StructVsClassDemo.RunDemo();

Console.WriteLine("\n\n▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀");
Console.WriteLine("  PART 7: MULTITHREADING & ASYNC");
Console.WriteLine("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");

await TaskThreadValueTaskDemo.RunDemo();
await AsyncAwaitDemo.RunDemo();
await DeadlockPreventionDemo.RunDemo();
ConcurrentCollectionsDemo.RunDemo();

Console.WriteLine("\n\n▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀");
Console.WriteLine("  PART 8: .NET FRAMEWORK CONCEPTS");
Console.WriteLine("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");

AbstractVsInterfaceDemo.RunDemo();
PolymorphismDemo.RunDemo();
CovarianceContravarianceDemo.RunDemo();
ExtensionMethodsDemo.RunDemo();
IQueryableVsIEnumerableDemo.RunDemo();
LINQExamples.RunDemo();
GenericsDemo.RunDemo();
DelegatesAndEventsDemo.RunDemo();
ReflectionDemo.RunDemo();

Console.WriteLine("\n\n▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀");
Console.WriteLine("  PART 9: PRACTICAL SCENARIOS");
Console.WriteLine("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");

OptionsPatternDemo.RunDemo();
await BackgroundServicesDemo.RunDemoAsync();
ValidationPatternsDemo.RunDemo();
MappingPatternsDemo.RunDemo();
CachingDemo.RunDemo();
await ApiOptimizationDemo.RunDemoAsync();
GlobalExceptionHandlingDemo.RunDemo();
CommonModelsDemo.RunDemo();
SerializationDemo.RunDemo();

Console.WriteLine("\n\n▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀");
Console.WriteLine("  PART 10: APPENDICES");
Console.WriteLine("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");

PatternsOverratedNowDemo.RunDemo();
QuickReferenceTablesDemo.RunDemo();
CommonInterviewQuestionsDemo.RunDemo();

Console.WriteLine("\n\n╔════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║  ✅ ALL DEMONSTRATIONS COMPLETED SUCCESSFULLY!                  ║");
Console.WriteLine("║  Every principle from the Revision Notes has been demonstrated ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");

Console.WriteLine("\n\nPress Ctrl+C to exit, or the application will continue running...\n");

// ============================================================================
// WEB API ENDPOINTS (Demonstrating ASP.NET Core concepts)
// ============================================================================

// From Revision Notes - Page 11: Dependency Injection endpoint
app.MapGet("/hello", (IGreeter greeter) => greeter.Greet("World"))
    .WithName("Greeting");

app.MapGet("/", () => Results.Ok(new
{
    Message = "C# Revision Notes Demo API",
    Documentation = "Check the console output for comprehensive demonstrations",
    Endpoints = new[] { "/hello" }
}));

app.Run();
