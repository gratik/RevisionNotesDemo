// ============================================================================
// IQUERYABLE VS IENUMERABLE
// Reference: Revision Notes - Page 12
// ============================================================================
// WHAT IS THIS?
// -------------
// IQueryable builds expression trees for remote execution; IEnumerable runs in memory.
//
// WHY IT MATTERS
// --------------
// ✅ Prevents accidental client-side filtering
// ✅ Avoids loading entire tables into memory
//
// WHEN TO USE
// -----------
// ✅ IQueryable for database queries
// ✅ IEnumerable for in-memory data
//
// WHEN NOT TO USE
// ---------------
// ❌ IQueryable for LINQ-to-Objects only
// ❌ Early `ToList()` that forces client-side filtering
//
// REAL-WORLD EXAMPLE
// ------------------
// Keep EF queries as IQueryable until `ToList()`.
// ============================================================================

namespace RevisionNotesDemo.LINQAndQueries;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class IQueryableVsIEnumerableDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== IQUERYABLE VS IENUMERABLE DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Page 12\n");

        // Simulated data
        var customers = new List<Customer>
        {
            new() { Id = 1, Name = "Alice", IsActive = true },
            new() { Id = 2, Name = "Bob", IsActive = true },
            new() { Id = 3, Name = "Charlie", IsActive = false },
            new() { Id = 4, Name = "Anna", IsActive = true }
        };

        // IQueryable simulation (would translate to SQL in real DB)
        Console.WriteLine("--- IQueryable (Server-side filtering) ---");
        IQueryable<Customer> query = customers.AsQueryable().Where(c => c.IsActive);
        var projected = query.Select(c => new { c.Id, c.Name });
        Console.WriteLine("[QUERY] IQueryable: Filtering would happen at database");
        Console.WriteLine($"[QUERY] Results: {string.Join(", ", projected.Select(c => c.Name))}");

        // IEnumerable (client-side filtering)
        Console.WriteLine("\n--- IEnumerable (Client-side filtering) ---");
        IEnumerable<Customer> enumerable = projected.AsEnumerable()
            .Select(c => customers.First(x => x.Id == c.Id))
            .Where(c => c.Name.StartsWith('A'));
        Console.WriteLine("[ENUM] IEnumerable: Filtering happens in memory");
        Console.WriteLine($"[ENUM] Results: {string.Join(", ", enumerable.Select(c => c.Name))}");

        Console.WriteLine("\n💡 From Revision Notes:");
        Console.WriteLine("   - IEnumerable: In-memory execution");
        Console.WriteLine("   - IQueryable: Database execution, expression trees");
        Console.WriteLine("   - Use IQueryable for DB queries");
        Console.WriteLine("   - Use IEnumerable for in-memory operations");
    }
}
