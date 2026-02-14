// ==============================================================================
// FLYWEIGHT PATTERN
// Reference: Revision Notes - Design Patterns
// ==============================================================================
// PURPOSE: Share common state among many objects to reduce memory usage
// BENEFIT: Reduced memory footprint, improved performance with large object counts
// USE WHEN: Many similar objects, most state can be extrinsic (shared)
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

        if (!_formats.ContainsKey(key))
        {
            var format = new CharacterFormat(fontFamily, fontSize, color, bold, italic);
            _formats[key] = format;
            Console.WriteLine($"[FLYWEIGHT] 🆕 Created new format: {format}");
        }
        else
        {
            Console.WriteLine($"[FLYWEIGHT] ♻️  Reusing existing format: {_formats[key]}");
        }

        return _formats[key];
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

        if (!_treeTypes.ContainsKey(key))
        {
            _treeTypes[key] = new TreeType(name, color, texture);
        }

        return _treeTypes[key];
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