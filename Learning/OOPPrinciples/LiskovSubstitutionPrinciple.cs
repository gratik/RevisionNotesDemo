// ============================================================================
// LISKOV SUBSTITUTION PRINCIPLE (LSP)
// Reference: Revision Notes - OOP (Object Oriented Principals) - Page 2
// ============================================================================
//
// WHAT IS LSP?
// ------------
// Objects of a superclass should be replaceable with objects of a subclass
// without breaking the application. Subtypes must honor the contracts of
// their base types (behavioral compatibility, not just method signatures).
//
// WHY IT MATTERS
// --------------
// - Safe polymorphism without surprises
// - Fewer runtime errors and special cases
// - Cleaner inheritance hierarchies
// - More reliable reuse of base classes
//
// WHEN TO USE
// -----------
// - YES: Designing inheritance hierarchies and base classes
// - YES: When a subtype must be used anywhere the base type is expected
// - YES: When defining contracts for frameworks or libraries
//
// WHEN NOT TO USE
// ---------------
// - NO: When behavior differs fundamentally (prefer composition or interfaces)
// - NO: When subtype must throw new exceptions or change preconditions
//
// REAL-WORLD EXAMPLE
// ------------------
// Shipping providers:
// - IShippingProvider.CalculateCost() must always return a non-negative cost
// - A FreeShippingProvider can return 0 but must not throw or change semantics
// - A provider that throws on small packages violates the base contract
// ============================================================================

namespace RevisionNotesDemo.OOPPrinciples;

// ❌ BAD EXAMPLE - Violates LSP
// Not all birds can fly, so this abstraction is flawed
public class BirdBad
{
    public virtual void Fly()
    {
        Console.WriteLine("Bird is flying");
    }
}

public class SparrowBad : BirdBad
{
    public override void Fly()
    {
        Console.WriteLine("Sparrow is flying");
    }
}

// Problem: Penguins can't fly!
public class PenguinBad : BirdBad
{
    public override void Fly()
    {
        // Violates LSP - we can't substitute Penguin for Bird without issues
        throw new NotSupportedException("Penguins can't fly!");
    }
}

// ✅ GOOD EXAMPLE - Follows LSP
// Proper abstraction that respects the capabilities of each subtype

public abstract class Bird
{
    public string Name { get; protected set; } = string.Empty;

    public virtual void Eat()
    {
        Console.WriteLine($"[LSP] {Name} is eating");
    }

    public virtual void Move()
    {
        Console.WriteLine($"[LSP] {Name} is moving");
    }
}

// Interface for flying capability (not all birds fly)
public interface IFlyable
{
    void Fly();
    int GetAltitude();
}

// Interface for swimming capability (not all birds swim)
public interface ISwimmable
{
    void Swim();
    int GetDivingDepth();
}

public class Duck : Bird, IFlyable, ISwimmable
{
    public Duck()
    {
        Name = "Duck";
    }

    public void Fly()
    {
        Console.WriteLine($"[LSP] {Name} is flying");
    }

    public int GetAltitude()
    {
        return 1000; // feet
    }

    public void Swim()
    {
        Console.WriteLine($"[LSP] {Name} is swimming");
    }

    public int GetDivingDepth()
    {
        return 10; // feet
    }

    public override void Move()
    {
        Console.WriteLine($"[LSP] {Name} can fly, swim, or walk");
    }
}

public class Sparrow : Bird, IFlyable
{
    public Sparrow()
    {
        Name = "Sparrow";
    }

    public void Fly()
    {
        Console.WriteLine($"[LSP] {Name} is flying");
    }

    public int GetAltitude()
    {
        return 500; // feet
    }

    public override void Move()
    {
        Console.WriteLine($"[LSP] {Name} prefers to fly");
    }
}

public class Penguin : Bird, ISwimmable
{
    public Penguin()
    {
        Name = "Penguin";
    }

    public void Swim()
    {
        Console.WriteLine($"[LSP] {Name} is swimming");
    }

    public int GetDivingDepth()
    {
        return 500; // feet - penguins are excellent divers!
    }

    public override void Move()
    {
        Console.WriteLine($"[LSP] {Name} waddles on land and swims in water");
    }
}

// These methods work with any Bird - LSP is satisfied
public class BirdSanctuary
{
    public void FeedBird(Bird bird)
    {
        // Works with any Bird subtype
        bird.Eat();
    }

    public void ObserveBirdMovement(Bird bird)
    {
        // Works with any Bird subtype
        bird.Move();
    }

    public void ObserveFlyingBirds(IEnumerable<IFlyable> flyingBirds)
    {
        // Only works with birds that can fly
        foreach (var bird in flyingBirds)
        {
            bird.Fly();
            Console.WriteLine($"   Altitude: {bird.GetAltitude()} feet");
        }
    }

    public void ObserveSwimmingBirds(IEnumerable<ISwimmable> swimmingBirds)
    {
        // Only works with birds that can swim
        foreach (var bird in swimmingBirds)
        {
            bird.Swim();
            Console.WriteLine($"   Diving depth: {bird.GetDivingDepth()} feet");
        }
    }
}

// Usage demonstration
public class LSPDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== LISKOV SUBSTITUTION PRINCIPLE DEMO ===\n");

        var sanctuary = new BirdSanctuary();

        // All birds can be substituted for the Bird base class
        var allBirds = new List<Bird>
        {
            new Duck(),
            new Sparrow(),
            new Penguin()
        };

        Console.WriteLine("Feeding all birds (LSP satisfied - all subtypes work):");
        foreach (var bird in allBirds)
        {
            sanctuary.FeedBird(bird);
        }

        Console.WriteLine("\nObserving bird movement:");
        foreach (var bird in allBirds)
        {
            sanctuary.ObserveBirdMovement(bird);
        }

        // Only flying birds
        var flyingBirds = allBirds.OfType<IFlyable>().ToList();
        Console.WriteLine("\nObserving flying birds:");
        sanctuary.ObserveFlyingBirds(flyingBirds);

        // Only swimming birds
        var swimmingBirds = allBirds.OfType<ISwimmable>().ToList();
        Console.WriteLine("\nObserving swimming birds:");
        sanctuary.ObserveSwimmingBirds(swimmingBirds);

        Console.WriteLine("\nBenefit: Subtypes are truly substitutable without breaking functionality!");
    }
}
