// ============================================================================
// STACK VS HEAP
// Reference: Revision Notes - Stack vs Heap - Page 5, Memory Management & Performance - Page 8
// ============================================================================
// WHAT IS THIS?
// -------------
// Stack vs heap memory regions and how .NET stores values and objects.
//
// WHY IT MATTERS
// --------------
// ✅ Explains allocation cost and copying behavior
// ✅ Helps reason about GC and lifetime
//
// WHEN TO USE
// -----------
// ✅ Performance investigations and memory profiling
// ✅ Choosing between struct and class
//
// WHEN NOT TO USE
// ---------------
// ❌ Over-optimizing trivial code paths
// ❌ Assuming all structs are stack-allocated
//
// REAL-WORLD EXAMPLE
// ------------------
// Struct copy on stack vs shared class on heap.
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
