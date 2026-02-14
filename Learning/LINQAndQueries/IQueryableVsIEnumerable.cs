// ============================================================================
// IQUERYABLE VS IENUMERABLE
// Reference: Revision Notes - Page 12
// ============================================================================
// DEFINITION:
//   IEnumerable<T> and IQueryable<T> both represent collections that can be
//   enumerated, but they execute queries very differently.
//
// KEY DIFFERENCES:
//   
//   IENUMERABLE<T>:
//     • Location: LINQ-to-Objects (in-memory)
//     • Execution: Client-side (in your application)
//     • Uses: Func<T, bool> (compiled code)
//     • Best for: In-memory collections (List, Array)
//     • Namespace: System.Collections.Generic
//   
//   IQUERYABLE<T>:
//     • Location: LINQ-to-SQL, LINQ-to-Entities (database)
//     • Execution: Server-side (database)
//     • Uses: Expression<Func<T, bool>> (expression trees)
//     • Best for: Database queries
//     • Namespace: System.Linq
//     • Inherits from: IEnumerable<T>
//
// CRITICAL DIFFERENCE - EXPRESSION TREES:
//   IQueryable uses Expression Trees that can be translated to SQL. This means
//   filtering happens on the database server, not in your application.
//
// EXAMPLE - WHY IT MATTERS:
//   
//   ❌ BAD (IEnumerable - loads all data):
//     IEnumerable<Customer> customers = dbContext.Customers;  // All loaded
//     var active = customers.Where(c => c.IsActive);           // Filter in C#
//     var first10 = active.Take(10);                           // Take in C#
//     // SQL: SELECT * FROM Customers (loads everything!)
//   
//   ✅ GOOD (IQueryable - translates to SQL):
//     IQueryable<Customer> customers = dbContext.Customers;   // Not executed
//     var active = customers.Where(c => c.IsActive);          // Still not executed
//     var first10 = active.Take(10);                          // Still not executed
//     var result = first10.ToList();                          // NOW executed
//     // SQL: SELECT TOP 10 * FROM Customers WHERE IsActive = 1
//
// DEFERRED EXECUTION:
//   Both support deferred execution - query not executed until enumerated.
//   But IQueryable builds up an expression tree that's translated to SQL.
//
// WHEN TO USE:
//   
//   USE IQUERYABLE:
//     • Always when querying databases
//     • When you need server-side filtering
//     • Large datasets
//     • Entity Framework, LINQ-to-SQL
//   
//   USE IENUMERABLE:
//     • After data is in memory (after ToList())
//     • In-memory collections
//     • Complex operations not translatable to SQL
//
// COMMON MISTAKE:
//   ❌ Accidentally converting to IEnumerable<T> too early:
//     var customers = dbContext.Customers
//         .ToList()                      // Executes query, loads all - now IEnumerable
//         .Where(c => c.IsActive)        // Filters in memory
//         .Take(10);
//   
//   ✅ Keep as IQueryable<T> until the end:
//     var customers = dbContext.Customers
//         .Where(c => c.IsActive)        // Still IQueryable
//         .Take(10)
//         .ToList();                     // Only now execute
//
// PERFORMANCE IMPLICATIONS:
//   Misusing these can cause:
//     • Loading entire tables into memory
//     • Inefficient queries
//     • Network traffic
//     • Memory issues
//
// CONVERSIONS:
//   • .AsEnumerable() - Convert IQueryable to IEnumerable
//   • .AsQueryable() - Convert IEnumerable to IQueryable
//
// BEST PRACTICES:
//   • Always use IQueryable<T> for database queries
//   • Keep queries as IQueryable as long as possible
//   • Only call ToList()/ToArray() when you need the data
//   • Be aware when providers fall back to client-side evaluation
//   • Use logging to see generated SQL
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
            .Where(c => c.Name.StartsWith("A"));
        Console.WriteLine("[ENUM] IEnumerable: Filtering happens in memory");
        Console.WriteLine($"[ENUM] Results: {string.Join(", ", enumerable.Select(c => c.Name))}");

        Console.WriteLine("\n💡 From Revision Notes:");
        Console.WriteLine("   - IEnumerable: In-memory execution");
        Console.WriteLine("   - IQueryable: Database execution, expression trees");
        Console.WriteLine("   - Use IQueryable for DB queries");
        Console.WriteLine("   - Use IEnumerable for in-memory operations");
    }
}
