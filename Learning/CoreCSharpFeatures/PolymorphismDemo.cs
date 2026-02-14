// ============================================================================
// POLYMORPHISM IN C#
// Reference: Revision Notes - Page 6
// ============================================================================
// Compile-time: Method overloading
// Runtime: Method overriding using virtual and override
// Allows different implementations through common interface/base class
// ============================================================================

namespace RevisionNotesDemo.CoreCSharpFeatures;

// From Revision Notes - Page 6: Compile-time polymorphism (overloading)
public class MathOps
{
    public int Add(int a, int b) => a + b;
    public double Add(double a, double b) => a + b;
    public string Add(string a, string b) => a + b;
}

// From Revision Notes - Page 6: Runtime polymorphism (overriding)
public class Animal
{
    public virtual string Speak() => "Some sound";
}

public class Dog : Animal
{
    public override string Speak() => "Woof";
}

public class Cat : Animal
{
    public override string Speak() => "Meow";
}

public class PolymorphismDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== POLYMORPHISM DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Page 6\n");

        // COMPILE-TIME (Overloading)
        Console.WriteLine("--- Compile-time Polymorphism (Overloading) ---");
        var ops = new MathOps();
        Console.WriteLine($"[POLY] Add(1, 2) = {ops.Add(1, 2)}");
        Console.WriteLine($"[POLY] Add(1.5, 2.5) = {ops.Add(1.5, 2.5)}");
        Console.WriteLine($"[POLY] Add(\"Hello\", \" World\") = {ops.Add("Hello", " World")}");

        // RUNTIME (Overriding)
        Console.WriteLine("\n--- Runtime Polymorphism (Overriding) ---");
        Animal a1 = new Dog();
        Animal a2 = new Cat();

        Console.WriteLine($"[POLY] Dog says: {a1.Speak()}");
        Console.WriteLine($"[POLY] Cat says: {a2.Speak()}");

        Console.WriteLine("\n💡 From Revision Notes:");
        Console.WriteLine("   - Compile-time: Method overloading");
        Console.WriteLine("   - Runtime: Method overriding (virtual/override)");
    }
}
