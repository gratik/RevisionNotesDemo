// ==============================================================================
// FLYWEIGHT PATTERN - Share Common State to Reduce Memory
// Reference: Revision Notes - Design Patterns
// ==============================================================================
//
// WHAT IS THE FLYWEIGHT PATTERN?
// -------------------------------
// Minimizes memory usage by sharing as much data as possible with similar objects.
// Separates intrinsic state (shared, immutable) from extrinsic state (unique, context-specific).
// Stores shared state once and references it from many objects.
//
// Think of it as: "Chess game - pieces share color/type (intrinsic) but have unique
// positions (extrinsic). Don't store 'black knight' data 2 times, share it!"
//
// Core Concepts:
//   • Flyweight: Stores shared intrinsic state (immutable)
//   • Intrinsic State: Shared among many objects (font, color, type)
//   • Extrinsic State: Unique per object, passed as context (position, size)
//   • Flyweight Factory: Creates and caches flyweight objects
//   • Memory Savings: n objects share m flyweights where m << n
//
// WHY IT MATTERS
// --------------
// ✅ MEMORY EFFICIENCY: Reduce memory from GB to MB in data-heavy scenarios
// ✅ PERFORMANCE: Faster object creation (reuse vs create)
// ✅ SCALABILITY: Support far more objects than would fit in memory
// ✅ CACHE FRIENDLY: Shared objects improve CPU cache hit rates
// ✅ IMMUTABILITY: Shared state is immutable (thread-safe by design)
//
// WHEN TO USE IT
// --------------
// ✅ Application uses large number of similar objects
// ✅ Storage cost of objects is high
// ✅ Most object state can be made extrinsic (shared)
// ✅ Many objects can be replaced by few shared objects
// ✅ Application doesn't depend on object identity (reference equality)
// ✅ Examples: text editors, game sprites, rendering engines
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Few objects or low memory usage (premature optimization)
// ❌ Most state is extrinsic (can't share much)
// ❌ Need object identity (reference comparison)
// ❌ Complexity not justified by memory savings
// ❌ Objects are mutated frequently
//
// REAL-WORLD EXAMPLE - Text Editor
// ---------------------------------
// Microsoft Word document with 100,000 characters:
//   • Without Flyweight: Each character object stores font, size, color, bold, italic
//   • 100,000 characters × 50 bytes = 5 MB just for formatting!
//   • Most characters share same formatting (e.g., Arial 12pt Black)
//
// WITHOUT FLYWEIGHT:
//   → class Character {
//         char Value;           // 2 bytes
//         int Row, Column;      // 8 bytes (extrinsic - unique position)
//         string FontFamily;    // 20 bytes (intrinsic - often shared)
//         int FontSize;         // 4 bytes (intrinsic)
//         string Color;         // 10 bytes (intrinsic)
//         bool Bold, Italic;    // 2 bytes (intrinsic)
//       }  // Total: ~50 bytes per character
//   → 100,000 characters × 50 bytes = 5 MB
//   → Most have identical formatting (wasted memory)
//
// WITH FLYWEIGHT:
//   → class CharacterFormat {  // Flyweight (immutable, shared)
//         string FontFamily;    // Shared
//         int FontSize;         // Shared
//         string Color;         // Shared
//         bool Bold, Italic;    // Shared
//       }  // 36 bytes
//   → class Character {
//         char Value;              // 2 bytes
//         int Row, Column;         // 8 bytes (unique)
//         CharacterFormat Format;  // 8 bytes (reference to shared)
//       }  // Total: ~18 bytes per character
//   → Typical document: 5-10 different formats shared by 100,000 characters
//   → Memory: (100,000 × 18) + (10 × 36) = 1.8 MB + 360 bytes ≈ 1.8 MB
//   → SAVINGS: 5 MB → 1.8 MB (64% reduction!)
//
// ANOTHER EXAMPLE - Game Sprites
// ------------------------------
// Forest scene with 10,000 tree objects:
//   • 3 tree types: Oak, Pine, Birch
//   • Each type: 3D model (5MB), textures (2MB), animations (1MB) = 8MB
//   • Each tree instance: position (12 bytes), rotation (4 bytes), scale (4 bytes)
//
// Without Flyweight:
//   → 10,000 trees × 8 MB = 80 GB (impossible!)
//
// With Flyweight:
//   → 3 tree types × 8 MB = 24 MB (shared)
//   → 10,000 instances × 20 bytes = 200 KB (unique positions)
//   → Total: 24 MB + 200 KB ≈ 24 MB
//   → SAVINGS: 80 GB → 24 MB (99.97% reduction!)
//
// Code:
//   class TreeType {  // Flyweight
//       Mesh model;
//       Texture texture;
//       Animation anim;
//   }
//   
//   class Tree {  // Context
//       TreeType type;  // Reference to shared flyweight
//       Vector3 position;  // Unique
//       float rotation;    // Unique
//   }
//   
//   TreeType oakType = TreeFactory.GetTreeType("Oak");  // Shared by all oaks
//   for (int i = 0; i < 5000; i++)
//       new Tree(oakType, RandomPosition());  // 5000 oaks share 1 TreeType
//
// ANOTHER EXAMPLE - String Interning
// -----------------------------------
// .NET's string.Intern() is Flyweight pattern:
//   string a = "hello";
//   string b = "hello";
//   string c = string.Intern("hello");
//   
//   // Without interning: 3 separate "hello" objects in memory
//   // With interning: All reference same "hello" object
//   
//   Console.WriteLine(ReferenceEquals(a, c));  // True - same object!
//
// INTRINSIC VS EXTRINSIC STATE
// ----------------------------
// INTRINSIC (Shared):
//   • Independent of context
//   • Can be shared (font, color, type, mesh)
//   • Immutable
//   • Stored in flyweight
//
// EXTRINSIC (Unique):
//   • Depends on context
//   • Cannot be shared (position, state, id)
//   • Can be mutable
//   • Passed to flyweight methods
//
// Example:
//   flyweight.Render(position, rotation);  // Extrinsic passed as parameters
//
// FLYWEIGHT FACTORY
// -----------------
// Manages flyweight pool:
//   public class FlyweightFactory<T>
//   {
//       private Dictionary<string, T> _cache = new();
//       
//       public T GetFlyweight(string key, Func<T> create)
//       {
//           if (!_cache.ContainsKey(key))
//               _cache[key] = create();
//           return _cache[key];
//       }
//   }
//
// .NET FRAMEWORK EXAMPLES
// -----------------------
// Flyweight pattern in .NET:
//   • String interning: string.Intern()
//   • Integer caching: Integer.valueOf() in Java (same concept)
//   • Font caching in UI frameworks
//   • ObjectPool<T> pattern (similar concept)
//
// PERFORMANCE CONSIDERATIONS
// --------------------------
// Benefits:
//   • Reduced memory (often 50-90%)
//   • Fewer allocations (faster)
//   • Better cache locality
//
// Trade-offs:
//   • Complexity of separating intrinsic/extrinsic
//   • Dictionary lookup overhead (usually negligible)
//   • Must manage flyweight lifecycle
//
// BEST PRACTICES
// --------------
// ✅ Make flyweights immutable (thread-safe)
// ✅ Use factory to manage flyweight pool
// ✅ Profile memory before optimizing (avoid premature optimization)
// ✅ Clear separation between intrinsic and extrinsic state
// ✅ Consider weak references for large rarely-used flyweights
//
// ==============================================================================

namespace RevisionNotesDemo.DesignPatterns.Structural;

// ========================================================================
// FLYWEIGHT - Shared immutable state (intrinsic)
// ========================================================================

/// <summary>
/// Flyweight storing shared character formatting
/// </summary>
public class CharacterFormat
{
    // Intrinsic state (shared among many characters)
    public string FontFamily { get; }
    public int FontSize { get; }
    public string Color { get; }
    public bool Bold { get; }
    public bool Italic { get; }

    public CharacterFormat(string fontFamily, int fontSize, string color, bool bold, bool italic)
    {
        FontFamily = fontFamily;
        FontSize = fontSize;
        Color = color;
        Bold = bold;
        Italic = italic;
    }

    public string GetFormatKey() =>
        $"{FontFamily}-{FontSize}-{Color}-{Bold}-{Italic}";

    public override string ToString() =>
        $"{FontFamily} {FontSize}pt {Color} {(Bold ? "Bold" : "")} {(Italic ? "Italic" : "")}".Trim();
}

// ========================================================================
// CONTEXT - Uses flyweight with extrinsic state
// ========================================================================

/// <summary>
/// Character with position (extrinsic) and shared format (intrinsic via flyweight)
/// </summary>
public class Character
{
    // Extrinsic state (unique to this character)
    public char Value { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }

    // Intrinsic state (shared via flyweight)
    public CharacterFormat Format { get; set; }

    public Character(char value, int row, int column, CharacterFormat format)
    {
        Value = value;
        Row = row;
        Column = column;
        Format = format;
    }

    public void Display()
    {
        Console.WriteLine($"  '{Value}' at ({Row},{Column}) - [{Format}]");
    }
}

// ========================================================================
// FLYWEIGHT FACTORY - Manages flyweight pool
// ========================================================================

public class CharacterFormatFactory
{
    private readonly Dictionary<string, CharacterFormat> _formats = new();

    public CharacterFormat GetFormat(string fontFamily, int fontSize, string color, bool bold, bool italic)
    {
        var key = $"{fontFamily}-{fontSize}-{color}-{bold}-{italic}";

        if (_formats.TryGetValue(key, out var existing))
        {
            Console.WriteLine($"[FLYWEIGHT] ♻️  Reusing existing format: {existing}");
            return existing;
        }

        var format = new CharacterFormat(fontFamily, fontSize, color, bold, italic);
        _formats[key] = format;
        Console.WriteLine($"[FLYWEIGHT] 🆕 Created new format: {format}");
        return format;
    }

    public int GetFormatCount() => _formats.Count;

    public void ShowStats()
    {
        Console.WriteLine($"\n[FLYWEIGHT] 📊 Format objects in pool: {_formats.Count}");
        Console.WriteLine($"[FLYWEIGHT] 💾 Memory saved by sharing formats!\n");
    }
}

// ========================================================================
// EXAMPLE 2: TREE RENDERING (Classic Flyweight Example)
// ========================================================================

/// <summary>
/// Flyweight - shared tree type data
/// </summary>
public class TreeType
{
    public string Name { get; }
    public string Color { get; }
    public string Texture { get; }

    public TreeType(string name, string color, string texture)
    {
        Name = name;
        Color = color;
        Texture = texture;
        Console.WriteLine($"[TREE TYPE] 🌳 Created TreeType: {name} ({color})");
    }

    public void Draw(int x, int y)
    {
        Console.WriteLine($"  🌲 Drawing {Name} tree ({Color}) at ({x},{y})");
    }
}

/// <summary>
/// Context - tree with position (extrinsic) and shared type (intrinsic)
/// </summary>
public class Tree
{
    public int X { get; set; }
    public int Y { get; set; }
    public TreeType Type { get; set; }  // Flyweight

    public Tree(int x, int y, TreeType type)
    {
        X = x;
        Y = y;
        Type = type;
    }

    public void Draw()
    {
        Type.Draw(X, Y);
    }
}

public class TreeFactory
{
    private readonly Dictionary<string, TreeType> _treeTypes = new();

    public TreeType GetTreeType(string name, string color, string texture)
    {
        var key = $"{name}-{color}";

        if (_treeTypes.TryGetValue(key, out var existing))
        {
            return existing;
        }

        var treeType = new TreeType(name, color, texture);
        _treeTypes[key] = treeType;
        return treeType;
    }

    public int GetTreeTypeCount() => _treeTypes.Count;
}

public class Forest
{
    private readonly List<Tree> _trees = new();
    private readonly TreeFactory _treeFactory = new();

    public void PlantTree(int x, int y, string name, string color, string texture)
    {
        var type = _treeFactory.GetTreeType(name, color, texture);
        var tree = new Tree(x, y, type);
        _trees.Add(tree);
    }

    public void Draw()
    {
        Console.WriteLine($"\n[FOREST] Drawing {_trees.Count} trees...");
        foreach (var tree in _trees)
        {
            tree.Draw();
        }
        Console.WriteLine($"\n[FOREST] 📊 Unique tree types: {_treeFactory.GetTreeTypeCount()}");
        Console.WriteLine($"[FOREST] 💾 Memory saved: {_trees.Count} trees share {_treeFactory.GetTreeTypeCount()} type objects!\n");
    }
}

// ========================================================================
// DEMONSTRATION
// ========================================================================

public class FlyweightDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== FLYWEIGHT PATTERN DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Design Patterns\n");

        // Example 1: Text Editor Character Formatting
        Console.WriteLine("=== EXAMPLE 1: Text Editor (Character Formatting) ===\n");
        Console.WriteLine("Scenario: Document with many characters sharing few formats\n");

        var formatFactory = new CharacterFormatFactory();
        var document = new List<Character>();

        // Create format 1: Arial 12pt Black
        var format1 = formatFactory.GetFormat("Arial", 12, "Black", false, false);
        document.Add(new Character('H', 0, 0, format1));
        document.Add(new Character('e', 0, 1, format1));
        document.Add(new Character('l', 0, 2, format1));
        document.Add(new Character('l', 0, 3, format1));
        document.Add(new Character('o', 0, 4, format1));

        // Create format 2: Arial 12pt Black Bold (new format)
        var format2 = formatFactory.GetFormat("Arial", 12, "Black", true, false);
        document.Add(new Character('W', 0, 6, format2));
        document.Add(new Character('o', 0, 7, format2));
        document.Add(new Character('r', 0, 8, format2));
        document.Add(new Character('l', 0, 9, format2));
        document.Add(new Character('d', 0, 10, format2));

        // Reuse format 1 (no new object created)
        var format1Again = formatFactory.GetFormat("Arial", 12, "Black", false, false);
        document.Add(new Character('!', 0, 11, format1Again));

        Console.WriteLine("\n--- Document Characters ---");
        foreach (var character in document)
        {
            character.Display();
        }

        formatFactory.ShowStats();

        Console.WriteLine($"💾 Memory Benefit:");
        Console.WriteLine($"   Without Flyweight: {document.Count} format objects");
        Console.WriteLine($"   With Flyweight: {formatFactory.GetFormatCount()} format objects");
        Console.WriteLine($"   Savings: {document.Count - formatFactory.GetFormatCount()} objects!\n");

        // Example 2: Forest (Many Trees)
        Console.WriteLine("=== EXAMPLE 2: Forest Rendering (Tree Types) ===\n");
        Console.WriteLine("Scenario: Rendering thousands of trees with few types\n");

        var forest = new Forest();

        // Plant oak trees
        Console.WriteLine("--- Planting Oak Trees ---");
        forest.PlantTree(10, 20, "Oak", "Green", "oak_texture.png");
        forest.PlantTree(30, 40, "Oak", "Green", "oak_texture.png");  // Reuses TreeType
        forest.PlantTree(50, 60, "Oak", "Green", "oak_texture.png");  // Reuses TreeType

        // Plant pine trees
        Console.WriteLine("\n--- Planting Pine Trees ---");
        forest.PlantTree(15, 25, "Pine", "DarkGreen", "pine_texture.png");
        forest.PlantTree(35, 45, "Pine", "DarkGreen", "pine_texture.png");  // Reuses TreeType

        // Plant maple trees
        Console.WriteLine("\n--- Planting Maple Trees ---");
        forest.PlantTree(20, 30, "Maple", "Red", "maple_texture.png");
        forest.PlantTree(40, 50, "Maple", "Red", "maple_texture.png");  // Reuses TreeType
        forest.PlantTree(60, 70, "Maple", "Red", "maple_texture.png");  // Reuses TreeType

        // Draw forest
        forest.Draw();

        Console.WriteLine("💡 Flyweight Pattern Benefits:");
        Console.WriteLine("   ✅ Reduced memory - share intrinsic state among objects");
        Console.WriteLine("   ✅ Performance - fewer object allocations");
        Console.WriteLine("   ✅ Scalability - handle large numbers of objects");
        Console.WriteLine("   ✅ Immutable flyweights - thread-safe shared state");

        Console.WriteLine("\n💡 Key Concepts:");
        Console.WriteLine("   🔹 Intrinsic State: Shared, immutable (e.g., tree type, format)");
        Console.WriteLine("   🔹 Extrinsic State: Unique per object (e.g., position, character)");
        Console.WriteLine("   🔹 Flyweight Factory: Manages object pool");

        Console.WriteLine("\n💡 Real-World Examples:");
        Console.WriteLine("   • Text editors (character formatting)");
        Console.WriteLine("   • Games (particles, tiles, sprites)");
        Console.WriteLine("   • UI frameworks (icons, fonts, colors)");
        Console.WriteLine("   • String interning in .NET");
        Console.WriteLine("   • Database connection pools");
    }
}
