// ==============================================================================
// GENERICS WITH CONSTRAINTS
// Reference: Revision Notes - .NET Framework - Page 6-7
// ==============================================================================
// WHAT IS THIS?
// -------------
// Generic types and methods with constraints using `where` clauses.
//
// WHY IT MATTERS
// --------------
// ✅ Enables reusable, type-safe code
// ✅ Avoids boxing for value types
//
// WHEN TO USE
// -----------
// ✅ Repositories, factories, and utilities across types
// ✅ Algorithms that need type capabilities enforced
//
// WHEN NOT TO USE
// ---------------
// ❌ When a concrete type is simpler
// ❌ Overly restrictive constraints that limit reuse
//
// REAL-WORLD EXAMPLE
// ------------------
// IRepository<T> constrained to `class` and `new()`.
// ==============================================================================

namespace RevisionNotesDemo.CoreCSharpFeatures;

// ========================================================================
// 1. GENERIC CLASSES
// ========================================================================

// Generic container without constraints
public class Box<T>
{
    public T Value { get; set; }

    public Box(T value)
    {
        Value = value;
    }

    public void Display()
    {
        Console.WriteLine($"[GENERIC] Box contains: {Value} (Type: {typeof(T).Name})");
    }
}

// ============================================================================
// 2. CONSTRAINTS: where T : class (reference type)
// ============================================================================

public class ReferenceTypeStore<T> where T : class
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        _items.Add(item);
        Console.WriteLine($"[CONSTRAINT] Added reference type: {item}");
    }

    public T? Find(Predicate<T> predicate)
    {
        return _items.Find(predicate);
    }
}

// ============================================================================
// 3. CONSTRAINTS: where T : struct (value type)
// ============================================================================

public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public override string ToString() => $"({X}, {Y})";
}

public class ValueTypeStore<T> where T : struct
{
    private readonly List<T> _items = new();

    public void Add(T item)
    {
        _items.Add(item);
        Console.WriteLine($"[CONSTRAINT] Added value type: {item}");
    }

    public T GetDefault()
    {
        return default(T); // For value types, returns initialized struct
    }
}

// ============================================================================
// 4. CONSTRAINTS: where T : new() (parameterless constructor)
// ============================================================================

public class Factory<T> where T : new()
{
    public T CreateInstance()
    {
        var instance = new T();
        Console.WriteLine($"[CONSTRAINT] Created instance of {typeof(T).Name}");
        return instance;
    }

    public List<T> CreateMultiple(int count)
    {
        return Enumerable.Range(0, count).Select(_ => new T()).ToList();
    }
}

// ============================================================================
// 5. CONSTRAINTS: where T : Interface
// ============================================================================

public interface IIdentifiable
{
    int Id { get; set; }
    string Name { get; set; }
}

public class Person : IIdentifiable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class EntityManager<T> where T : IIdentifiable
{
    private readonly Dictionary<int, T> _entities = new();

    public void Register(T entity)
    {
        _entities[entity.Id] = entity;
        Console.WriteLine($"[CONSTRAINT] Registered {typeof(T).Name}: {entity.Name} (ID: {entity.Id})");
    }

    public T? GetById(int id)
    {
        _entities.TryGetValue(id, out var entity);
        return entity;
    }
}

// ========================================================================
// 6. CONSTRAINTS: where T : Base Class
// ========================================================================

public abstract class GenericAnimal
{
    public abstract string Speak();
}

public class GenericDog : GenericAnimal
{
    public override string Speak() => "Woof!";
}

public class GenericCat : GenericAnimal
{
    public override string Speak() => "Meow!";
}

public class GenericAnimalShelter<T> where T : GenericAnimal
{
    private readonly List<T> _animals = new();

    public void AddAnimal(T animal)
    {
        _animals.Add(animal);
        Console.WriteLine($"[CONSTRAINT] Added animal: {animal.Speak()}");
    }

    public void MakeAllSpeak()
    {
        foreach (var animal in _animals)
        {
            Console.WriteLine($"[CONSTRAINT] {typeof(T).Name} says: {animal.Speak()}");
        }
    }
}

// ============================================================================
// 7. MULTIPLE CONSTRAINTS
// ============================================================================

public interface IResettable
{
    void Reset();
}

public class Counter : IResettable
{
    public int Value { get; private set; }

    public void Increment() => Value++;
    public void Reset() => Value = 0;
}

public class Manager<T> where T : class, IResettable, new()
{
    public T CreateAndReset()
    {
        var instance = new T();
        instance.Reset();
        Console.WriteLine($"[CONSTRAINT] Created and reset {typeof(T).Name}");
        return instance;
    }
}

// ============================================================================
// 8. GENERIC METHODS
// ============================================================================

public class GenericMethods
{
    public static void Swap<T>(ref T a, ref T b)
    {
        Console.WriteLine($"[GENERIC] Before swap: a={a}, b={b}");
        (a, b) = (b, a);
        Console.WriteLine($"[GENERIC] After swap: a={a}, b={b}");
    }

    public static T Max<T>(T a, T b) where T : IComparable<T>
    {
        return a.CompareTo(b) > 0 ? a : b;
    }

    public static void ProcessCollection<T>(IEnumerable<T> items) where T : notnull
    {
        foreach (var item in items)
        {
            Console.WriteLine($"[GENERIC] Processing: {item}");
        }
    }
}

// ============================================================================
// 9. COVARIANCE AND CONTRAVARIANCE WITH GENERICS
// ============================================================================

public interface IProcessor<in TInput, out TOutput>
{
    TOutput Process(TInput input);
}

public class StringToIntProcessor : IProcessor<string, int>
{
    public int Process(string input)
    {
        return int.TryParse(input, out int result) ? result : 0;
    }
}

// ============================================================================
// DEMONSTRATION
// ============================================================================

public class GenericsDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== GENERICS WITH CONSTRAINTS DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - .NET Framework - Page 6-7\n");

        // 1. Basic Generic Class
        Console.WriteLine("--- 1. Basic Generic Class ---");
        var intBox = new Box<int>(42);
        intBox.Display();

        var stringBox = new Box<string>("Hello");
        stringBox.Display();
        Console.WriteLine();

        // 2. where T : class
        Console.WriteLine("--- 2. Constraint: where T : class ---");
        var refStore = new ReferenceTypeStore<string>();
        refStore.Add("Item 1");
        refStore.Add("Item 2");
        Console.WriteLine();

        // 3. where T : struct
        Console.WriteLine("--- 3. Constraint: where T : struct ---");
        var valueStore = new ValueTypeStore<Point>();
        valueStore.Add(new Point { X = 10, Y = 20 });
        valueStore.Add(new Point { X = 30, Y = 40 });
        Console.WriteLine($"[CONSTRAINT] Default Point: {valueStore.GetDefault()}\n");

        // 4. where T : new()
        Console.WriteLine("--- 4. Constraint: where T : new() ---");
        var factory = new Factory<Point>();
        var point = factory.CreateInstance();
        Console.WriteLine();

        // 5. where T : Interface
        Console.WriteLine("--- 5. Constraint: where T : IIdentifiable ---");
        var entityManager = new EntityManager<Person>();
        entityManager.Register(new Person { Id = 1, Name = "Alice" });
        entityManager.Register(new Person { Id = 2, Name = "Bob" });
        var found = entityManager.GetById(1);
        Console.WriteLine($"[CONSTRAINT] Found: {found?.Name}\n");

        // 6. where T : Base Class
        Console.WriteLine("--- 6. Constraint: where T : GenericAnimal ---");
        var dogShelter = new GenericAnimalShelter<GenericDog>();
        dogShelter.AddAnimal(new GenericDog());
        dogShelter.MakeAllSpeak();
        Console.WriteLine();

        // 7. Multiple Constraints
        Console.WriteLine("--- 7. Multiple Constraints ---");
        var manager = new Manager<Counter>();
        var counter = manager.CreateAndReset();
        Console.WriteLine();

        // 8. Generic Methods
        Console.WriteLine("--- 8. Generic Methods ---");
        int x = 5, y = 10;
        GenericMethods.Swap(ref x, ref y);
        Console.WriteLine($"[GENERIC] Max(100, 200) = {GenericMethods.Max(100, 200)}\n");

        // 9. Variance
        Console.WriteLine("--- 9. Covariance/Contravariance ---");
        IProcessor<string, int> processor = new StringToIntProcessor();
        var result = processor.Process("123");
        Console.WriteLine($"[GENERIC] Processed '123' to: {result}\n");

        Console.WriteLine("💡 Generic Constraints Benefits:");
        Console.WriteLine("   ✅ Type safety at compile time");
        Console.WriteLine("   ✅ No boxing/unboxing for value types");
        Console.WriteLine("   ✅ IntelliSense support");
        Console.WriteLine("   ✅ Code reuse without duplication");
        Console.WriteLine("   ✅ Constrain types to ensure required capabilities");
    }
}
