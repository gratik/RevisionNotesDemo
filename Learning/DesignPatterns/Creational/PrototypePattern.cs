// ============================================================================
// PROTOTYPE PATTERN - Clone Objects Instead of Creating from Scratch
// Reference: Revision Notes - Design Patterns (Creational) - Page 3
// ============================================================================
//
// WHAT IS THE PROTOTYPE PATTERN?
// -------------------------------
// Creates new objects by copying an existing object (prototype) rather than
// creating from scratch. Useful when object creation is expensive or complex.
// Defines a Clone() method that creates a copy of the object including all
// its internal state.
//
// Think of it as: "Photocopying a filled form instead of filling out a new blank form"
//
// Core Concepts:
//   • Prototype: Interface with Clone() method
//   • Concrete Prototype: Class implementing Clone()
//   • Shallow Copy: Copies object references (default with MemberwiseClone)
//   • Deep Copy: Recursively copies referenced objects
//   • Clone Registry: Optional - stores and retrieves commonly used prototypes
//
// WHY IT MATTERS
// --------------
// ✅ PERFORMANCE: Avoid expensive initialization when cloning is faster
// ✅ REDUCED COMPLEXITY: Clone complex objects without knowing their internals
// ✅ DYNAMIC CONFIG: Create objects at runtime without knowing concrete class
// ✅ AVOID SUBCLASSING: Don't need factory subclass for each configuration
// ✅ PRESERVE STATE: Keep current state while creating variations
// ✅ PROTOTYPE REGISTRY: Reuse common configurations
//
// WHEN TO USE IT
// --------------
// ✅ Object creation is expensive (database load, file parsing, network call)
// ✅ Need to avoid subclasses of object creator (like Factory Method requires)
// ✅ Objects have similar configurations (clone and modify vs create from scratch)
// ✅ Want to hide complexity of object creation from client
// ✅ System should be independent of how products are created
// ✅ Game development: Clone enemy templates with preset stats
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Object creation is simple and cheap (new MyClass() is fine)
// ❌ Classes rarely change (no need for runtime configuration)
// ❌ Deep vs shallow copy concerns add complexity
// ❌ Circular references make cloning complex
// ❌ Objects don't have internal state to preserve
//
// REAL-WORLD EXAMPLE
// ------------------
// Imagine a game like World of Warcraft spawning monsters:
//   • Thousands of enemy instances needed per zone
//   • Each enemy type (Dragon, Orc, Skeleton) has complex initialization:
//     - Load 3D model from file (slow)
//     - Initialize AI behavior trees
//     - Set up animation controllers
//     - Configure collision detection
//   • Creating from scratch = 100ms per enemy = VERY SLOW
//   • All enemies of same type share 90% of properties
//
// Without Prototype:
//   → for (int i = 0; i < 1000; i++) {
//         var dragon = new Dragon(); // Load model, init AI, setup animations (100ms × 1000 = 100 seconds!)
//     }
//   → 100 seconds to spawn 1000 dragons = game freezes
//   → Duplicate expensive initialization logic
//
// With Prototype:
//   → var dragonPrototype = new Dragon(); // Load once (100ms)
//   → for (int i = 0; i < 1000; i++) {
//         var dragon = dragonPrototype.Clone(); // Just copy (0.1ms × 1000 = 100ms)
//         dragon.Position = GetRandomPosition();
//     }
//   → 100ms total to spawn 1000 dragons = 1000× faster!
//   → Prototype registry stores common enemy templates
//
// SHALLOW VS DEEP COPY
// --------------------
// Shallow Copy (default MemberwiseClone()):
//   • Copies value types (int, bool, struct)
//   • Copies references (not the referenced objects)
//   • Fast but can cause unintended sharing
//
// Deep Copy:
//   • Recursively copies referenced objects
//   • Slower but creates truly independent copy
//   • Required when objects have complex state
//
// Example:
//   // Shallow: Both clones share same Address object
//   var person2 = person1.ShallowClone();
//   person2.Address.City = "NYC"; // person1.Address also changed!
//
//   // Deep: Each clone has own Address
//   var person2 = person1.DeepClone();
//   person2.Address.City = "NYC"; // person1.Address unchanged ✓
//
// C# IMPLEMENTATION OPTIONS
// -------------------------
// 1. MemberwiseClone(): Built-in protected method (shallow copy)
// 2. IClonable Interface: Legacy, not recommended (no generic support)
// 3. Copy Constructor: public MyClass(MyClass original) { ... }
// 4. Serialization: Deep copy via JSON serialization (slow but easy)
// 5. Custom Clone() method: Most flexible, shown in examples below
//
// ============================================================================

namespace RevisionNotesDemo.DesignPatterns.Creational;

// Prototype interface
public interface IPrototype<T>
{
    T Clone();
}

// Complex object with nested properties
public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public Address DeepClone()
    {
        return new Address
        {
            Street = this.Street,
            City = this.City,
            Country = this.Country
        };
    }

    public override string ToString() => $"{Street}, {City}, {Country}";
}

// Game Character example
public class GameCharacter : IPrototype<GameCharacter>
{
    public string Name { get; set; } = string.Empty;
    public int Health { get; set; }
    public int Mana { get; set; }
    public int Strength { get; set; }
    public int Intelligence { get; set; }
    public List<string> Skills { get; set; } = new();
    public Address? HomeLocation { get; set; }

    // Deep clone - creates a complete copy
    public GameCharacter Clone()
    {
        Console.WriteLine($"[PROTOTYPE] Cloning character: {Name}");

        return new GameCharacter
        {
            Name = this.Name + " (Clone)",
            Health = this.Health,
            Mana = this.Mana,
            Strength = this.Strength,
            Intelligence = this.Intelligence,
            Skills = new List<string>(this.Skills), // Deep copy of list
            HomeLocation = this.HomeLocation?.DeepClone() // Deep copy of nested object
        };
    }

    public void Display()
    {
        Console.WriteLine($"\n[PROTOTYPE] Character: {Name}");
        Console.WriteLine($"[PROTOTYPE]   Health: {Health}");
        Console.WriteLine($"[PROTOTYPE]   Mana: {Mana}");
        Console.WriteLine($"[PROTOTYPE]   Strength: {Strength}");
        Console.WriteLine($"[PROTOTYPE]   Intelligence: {Intelligence}");
        Console.WriteLine($"[PROTOTYPE]   Skills: {string.Join(", ", Skills)}");
        Console.WriteLine($"[PROTOTYPE]   Home: {HomeLocation}");
    }
}

// Prototype registry - stores and retrieves prototypes
public class CharacterRegistry
{
    private readonly Dictionary<string, GameCharacter> _prototypes = new();

    public void RegisterPrototype(string key, GameCharacter prototype)
    {
        _prototypes[key] = prototype;
        Console.WriteLine($"[PROTOTYPE] Registered prototype: {key}");
    }

    public GameCharacter? GetPrototype(string key)
    {
        if (_prototypes.TryGetValue(key, out var prototype))
        {
            Console.WriteLine($"[PROTOTYPE] Retrieving prototype: {key}");
            return prototype.Clone();
        }
        return null;
    }
}

// Another example: Document templates
public class DocumentTemplate : IPrototype<DocumentTemplate>
{
    public string TemplateName { get; set; } = string.Empty;
    public List<string> Sections { get; set; } = new();
    public Dictionary<string, string> PlaceFolders { get; set; } = new();
    public string Formatting { get; set; } = string.Empty;

    public DocumentTemplate Clone()
    {
        Console.WriteLine($"[PROTOTYPE] Cloning document template: {TemplateName}");

        return new DocumentTemplate
        {
            TemplateName = this.TemplateName,
            Sections = new List<string>(this.Sections),
            PlaceFolders = new Dictionary<string, string>(this.PlaceFolders),
            Formatting = this.Formatting
        };
    }

    public void FillPlaceholder(string key, string value)
    {
        if (PlaceFolders.ContainsKey(key))
        {
            PlaceFolders[key] = value;
        }
    }

    public void Display()
    {
        Console.WriteLine($"\n[PROTOTYPE] Document: {TemplateName}");
        Console.WriteLine($"[PROTOTYPE] Sections: {string.Join(", ", Sections)}");
        Console.WriteLine($"[PROTOTYPE] Placeholders:");
        foreach (var ph in PlaceFolders)
        {
            Console.WriteLine($"[PROTOTYPE]   {ph.Key}: {ph.Value}");
        }
        Console.WriteLine($"[PROTOTYPE] Formatting: {Formatting}\n");
    }
}

// Usage demonstration
public class PrototypeDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== PROTOTYPE PATTERN DEMO ===\n");

        Console.WriteLine("--- Example 1: Game Character Cloning ---");

        // Create a prototype warrior
        var warriorPrototype = new GameCharacter
        {
            Name = "Warrior Prototype",
            Health = 100,
            Mana = 20,
            Strength = 80,
            Intelligence = 30,
            Skills = new List<string> { "Bash", "Shield Block", "War Cry" },
            HomeLocation = new Address
            {
                Street = "123 Castle Road",
                City = "Ironforge",
                Country = "Azeroth"
            }
        };
        warriorPrototype.Display();

        // Clone warriors and customize
        var warrior1 = warriorPrototype.Clone();
        warrior1.Name = "Warrior 1";
        warrior1.Skills.Add("Berserk"); // Modify clone without affecting prototype
        warrior1.Display();

        var warrior2 = warriorPrototype.Clone();
        warrior2.Name = "Warrior 2";
        warrior2.Health = 120; // Different stats
        warrior2.Display();

        // Verify prototype wasn't changed
        Console.WriteLine("--- Verifying prototype unchanged ---");
        warriorPrototype.Display();

        Console.WriteLine("\n--- Example 2: Character Registry ---");

        var registry = new CharacterRegistry();

        // Register multiple prototypes
        var magePrototype = new GameCharacter
        {
            Name = "Mage Prototype",
            Health = 60,
            Mana = 100,
            Strength = 20,
            Intelligence = 90,
            Skills = new List<string> { "Fireball", "Ice Blast", "Teleport" }
        };

        registry.RegisterPrototype("Warrior", warriorPrototype);
        registry.RegisterPrototype("Mage", magePrototype);

        // Create characters from registry
        var newWarrior = registry.GetPrototype("Warrior");
        newWarrior!.Name = "Sir Galahad";
        newWarrior.Display();

        var newMage = registry.GetPrototype("Mage");
        newMage!.Name = "Gandalf";
        newMage.Display();

        Console.WriteLine("\n--- Example 3: Document Template Cloning ---");

        var invoiceTemplate = new DocumentTemplate
        {
            TemplateName = "Invoice Template",
            Sections = new List<string> { "Header", "Items", "Total", "Footer" },
            PlaceFolders = new Dictionary<string, string>
            {
                { "CustomerName", "[CUSTOMER]" },
                { "InvoiceNumber", "[INVOICE#]" },
                { "Date", "[DATE]" },
                { "Amount", "[AMOUNT]" }
            },
            Formatting = "Professional"
        };

        // Clone and customize for specific customers
        var invoice1 = invoiceTemplate.Clone();
        invoice1.TemplateName = "Invoice #001";
        invoice1.FillPlaceholder("CustomerName", "Acme Corp");
        invoice1.FillPlaceholder("InvoiceNumber", "INV-001");
        invoice1.FillPlaceholder("Amount", "$1,500.00");
        invoice1.Display();

        var invoice2 = invoiceTemplate.Clone();
        invoice2.TemplateName = "Invoice #002";
        invoice2.FillPlaceholder("CustomerName", "Tech Solutions");
        invoice2.FillPlaceholder("InvoiceNumber", "INV-002");
        invoice2.FillPlaceholder("Amount", "$2,300.00");
        invoice2.Display();

        Console.WriteLine("💡 Benefit: Avoid expensive initialization by cloning existing objects");
        Console.WriteLine("💡 Benefit: Create new objects without knowing their concrete classes");
        Console.WriteLine("💡 Use case: When object creation is more expensive than cloning");
    }
}
