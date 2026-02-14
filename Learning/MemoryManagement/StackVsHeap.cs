// ============================================================================
// STACK VS HEAP
// Reference: Revision Notes - Stack vs Heap - Page 5, Memory Management & Performance - Page 8
// ============================================================================
// DEFINITION:
//   Stack and Heap are two memory regions where .NET stores data. Understanding
//   the difference is crucial for writing efficient, memory-safe applications.
//
// STACK:
//   • What it stores: Value types (int, bool, struct, enum), method parameters, 
//     local variables, return addresses
//   • Characteristics: Fast (LIFO - Last In, First Out), automatically managed, 
//     limited size (~1MB per thread)
//   • Lifetime: Variables exist only within their scope
//   • Access: Very fast (just move stack pointer)
//   • Thread: Each thread has its own stack
//
// HEAP:
//   • What it stores: Reference types (class, string, array, delegates)
//   • Characteristics: Larger size, managed by Garbage Collector, slower allocation
//   • Lifetime: Until Garbage Collector determines no references exist
//   • Access: Slower (pointer dereference required)
//   • Thread: Shared across all threads
//
// MEMORY STORAGE:
//   • Value types → Stack (when local variable or method parameter)
//   • Reference types → Heap (object itself)
//   • Reference variable → Stack (pointer to heap object)
//
// EXAMPLE:
//   int x = 10;              // Stack - value type
//   string name = \"John\";    // \"name\" reference on stack, \"John\" object on heap
//   Person p = new Person(); // \"p\" reference on stack, Person object on heap
//
// COPYING BEHAVIOUR:
//   • Value types: Create full copy when assigned
//   • Reference types: Copy reference (both variables point to same object)
//
// EQUALITY COMPARISON:
//   • Value types: By value (same contents = equal)
//   • Reference types: By reference (same memory address = equal, unless overridden)
//
// KEY POINTS:
//   • Stack overflow = too much recursion or large value types
//   • Heap fragmentation = GC's job to compact
//   • Boxing = value type wrapped in reference type (goes to heap)
//   • Unboxing = extracting value type from boxed object
//
// WHEN TO USE STRUCT (STACK):
//   • Small data structures (< 16 bytes recommended)
//   • Immutable types (readonly struct)
//   • Value semantics required (copies should be independent)
//   • Short-lived, high-frequency allocations
//   • Examples: Point, Rectangle, Money, Complex numbers
//
// WHEN TO USE CLASS (HEAP):
//   • Larger objects (> 16 bytes)
//   • Reference semantics needed (share same instance)
//   • Inheritance required (structs can't inherit)
//   • Long-lived objects
//   • Polymorphism needed
//   • Examples: Customer, Order, Service classes
//
// PERFORMANCE CONSIDERATIONS:
//   • Stack allocation/deallocation is very fast
//   • Heap allocation triggers GC (overhead)
//   • Large structs cause excessive copying (slow)
//   • Boxing/unboxing has performance cost
//
// BEST PRACTICES:
//   • Default to class unless you have specific reason for struct
//   • Keep structs small and immutable
//   • Use readonly struct to enforce immutability
//   • Avoid boxing when possible
//   • Be aware of closure allocations in lambdas
// ============================================================================

namespace RevisionNotesDemo.MemoryManagement;

// Value type (stored on stack when local variable)
public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"({X}, {Y})";
}

// Reference type (stored on heap)
public class Person
{
    public string Name { get; set; }

    public Person(string name)
    {
        Name = name;
    }

    public override string ToString() => Name;
}

// From Revision Notes - Page 8
// Example: Value type (stack) vs Reference type (heap)
public class StackVsHeapDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== STACK VS HEAP DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Page 5 & 8\n");

        // VALUE TYPES (Stack behavior)
        Console.WriteLine("--- Value Types (Stack) ---");
        Point p1 = new Point(1, 2);
        Point p2 = p1; // COPY created
        p2.X = 99;

        Console.WriteLine($"[STACK] p1: {p1}"); // p1.X still 1
        Console.WriteLine($"[STACK] p2: {p2}"); // p2.X is 99
        Console.WriteLine("[STACK] Proof: Value types create COPIES\n");

        // REFERENCE TYPES (Heap behavior)
        Console.WriteLine("--- Reference Types (Heap) ---");
        var alice = new Person("Alice"); // reference on heap
        var bob = alice; // Same reference, not a copy
        bob.Name = "Bob";

        Console.WriteLine($"[HEAP] alice.Name: {alice.Name}"); // Now "Bob"
        Console.WriteLine($"[HEAP] bob.Name: {bob.Name}"); // Also "Bob"
        Console.WriteLine("[HEAP] Proof: Reference types share the same object\n");

        // EQUALITY COMPARISON
        Console.WriteLine("--- Equality Comparison ---");
        Point p3 = new Point(1, 2);
        Point p4 = new Point(1, 2);
        Console.WriteLine($"[STACK] p3 == p4: {p3.X == p4.X && p3.Y == p4.Y} (value comparison)");

        var person1 = new Person("Alice");
        var person2 = new Person("Alice");
        Console.WriteLine($"[HEAP] person1 == person2: {ReferenceEquals(person1, person2)} (reference comparison)");
        Console.WriteLine($"[HEAP] Same name but different objects!");

        // From Revision Notes - Tip for C# Developers
        Console.WriteLine("\n💡 From Revision Notes:");
        Console.WriteLine("   - Value types: Stack, copied by value, compared by value");
        Console.WriteLine("   - Reference types: Heap, same instance, compared by reference");
        Console.WriteLine("   - Use structs for small immutable data");
        Console.WriteLine("   - Use classes for larger mutable objects with identity");
    }
}
