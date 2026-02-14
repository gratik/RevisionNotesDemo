// ============================================================================
// LINQ (LANGUAGE INTEGRATED QUERY)
// Reference: Revision Notes - .NET Framework - Page 11-12
// ============================================================================
// DEFINITION:
//   Language Integrated Query - Unified syntax for querying data from different
//   sources (collections, databases, XML, etc.). Provides type-safe queries with
//   IntelliSense support.
//
// PURPOSE:
//   "Unified query syntax for collections, databases, XML"
//   Write queries in C# directly instead of string-based queries.
//
// TWO SYNTAXES:
//   
//   1. QUERY SYNTAX (SQL-like, declarative):
//      var results = from customer in customers
//                    where customer.Age > 18
//                    select customer.Name;
//   
//   2. METHOD SYNTAX (Fluent API, more powerful):
//      var results = customers
//                    .Where(c => c.Age > 18)
//                    .Select(c => c.Name);
//
// KEY CONCEPTS:
//   • DEFERRED EXECUTION: Query not executed until enumerated (foreach, ToList())
//   • COMPOSITION: Chain multiple operators
//   • TYPE SAFETY: Compile-time checking
//   • LAMBDA EXPRESSIONS: Inline functions (c => c.Age > 18)
//
// COMMON OPERATORS:
//   • Filtering: Where, First, FirstOrDefault, Single, Skip, Take
//   • Projection: Select, SelectMany
//   • Ordering: OrderBy, OrderByDescending, ThenBy
//   • Grouping: GroupBy
//   • Joining: Join, GroupJoin
//   • Aggregation: Count, Sum, Average, Min, Max, Aggregate
//   • Quantifiers: Any, All, Contains
//   • Set: Union, Intersect, Except, Distinct
//   • Element: First, Last, ElementAt, Single
//   • Conversion: ToList, ToArray, ToDictionary
//
// DEFERRED VS IMMEDIATE EXECUTION:
//   • DEFERRED: Where, Select, OrderBy (returns IEnumerable<T>, executed on enumeration)
//   • IMMEDIATE: ToList, ToArray, Count, First (executes immediately)
//
// EXAMPLE:
//   var query = numbers.Where(n => n > 5);  // NOT executed yet
//   foreach (var num in query) { }          // NOW executed
//
// LINQ PROVIDERS:
//   • LINQ to Objects: In-memory collections (List, Array)
//   • LINQ to SQL / Entity Framework: Database queries
//   • LINQ to XML: XML documents
//
// PERFORMANCE CONSIDERATIONS:
//   • Use IQueryable<T> for database queries (server-side execution)
//   • Use IEnumerable<T> for in-memory collections
//   • Be aware of multiple enumeration (cache with ToList if needed)
//   • Avoid N+1 queries (use Include for related data)
//
// BENEFITS:
//   • Consistent query syntax across data sources
//   • Type safety and IntelliSense
//   • Compile-time error checking
//   • More readable than loops
//   • Composable and reusable
//
// BEST PRACTICES:
//   • Use method syntax for complex queries
//   • Cache results with ToList() to avoid multiple executions
//   • Use meaningful variable names in lambdas
//   • Prefer Any() over Count() > 0
//   • Use FirstOrDefault instead of Where().First()
// ============================================================================

namespace RevisionNotesDemo.LINQAndQueries;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Major { get; set; } = string.Empty;
    public double GPA { get; set; }
}

public class Course
{
    public int CourseId { get; set; }
    public int StudentId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int Credits { get; set; }
}

// ============================================================================
// LINQ DEMONSTRATIONS
// ============================================================================

public class LINQExamples
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== LINQ (LANGUAGE INTEGRATED QUERY) DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - .NET Framework - Page 11-12\n");

        var students = GetStudents();
        var courses = GetCourses();

        // 1. Query Syntax vs Method Syntax
        Console.WriteLine("--- 1. Query Syntax vs Method Syntax ---");

        // Query syntax (SQL-like)
        var queryResult = from s in students
                          where s.GPA > 3.5
                          orderby s.Name
                          select s.Name;

        // Method syntax (fluent API)
        var methodResult = students
            .Where(s => s.GPA > 3.5)
            .OrderBy(s => s.Name)
            .Select(s => s.Name);

        Console.WriteLine($"[LINQ] Query syntax: {string.Join(", ", queryResult)}");
        Console.WriteLine($"[LINQ] Method syntax: {string.Join(", ", methodResult)}\n");

        // 2. Filtering (Where)
        Console.WriteLine("--- 2. Filtering with Where ---");
        var highPerformers = students.Where(s => s.GPA >= 3.8);
        Console.WriteLine($"[LINQ] Students with GPA >= 3.8:");
        foreach (var s in highPerformers)
            Console.WriteLine($"[LINQ]   - {s.Name}: {s.GPA}");
        Console.WriteLine();

        // 3. Projection (Select)
        Console.WriteLine("--- 3. Projection with Select ---");
        var studentSummaries = students.Select(s => new
        {
            s.Name,
            Performance = s.GPA >= 3.5 ? "Excellent" : s.GPA >= 3.0 ? "Good" : "Average"
        });
        foreach (var summary in studentSummaries)
            Console.WriteLine($"[LINQ] {summary.Name}: {summary.Performance}");
        Console.WriteLine();

        // 4. SelectMany (Flattening)
        Console.WriteLine("--- 4. SelectMany (Flattening) ---");
        var studentGroups = new[]
        {
            new { DeptName = "CS", Students = new[] { "Alice", "Bob" } },
            new { DeptName = "Math", Students = new[] { "Charlie", "Diana" } }
        };
        var allStudents = studentGroups.SelectMany(g => g.Students);
        Console.WriteLine($"[LINQ] All students: {string.Join(", ", allStudents)}\n");

        // 5. Ordering
        Console.WriteLine("--- 5. Ordering ---");
        var ordered = students.OrderByDescending(s => s.GPA).ThenBy(s => s.Name);
        Console.WriteLine("[LINQ] Ordered by GPA (desc), then Name:");
        foreach (var s in ordered)
            Console.WriteLine($"[LINQ]   {s.Name}: {s.GPA}");
        Console.WriteLine();

        // 6. Grouping
        Console.WriteLine("--- 6. GroupBy ---");
        var byMajor = students.GroupBy(s => s.Major);
        foreach (var group in byMajor)
        {
            Console.WriteLine($"[LINQ] Major: {group.Key}");
            foreach (var s in group)
                Console.WriteLine($"[LINQ]   - {s.Name}");
        }
        Console.WriteLine();

        // 7. Join
        Console.WriteLine("--- 7. Join ---");
        var enrollments = from s in students
                          join c in courses on s.Id equals c.StudentId
                          select new { s.Name, c.CourseName, c.Credits };

        Console.WriteLine("[LINQ] Student Enrollments:");
        foreach (var e in enrollments)
            Console.WriteLine($"[LINQ]   {e.Name} enrolled in {e.CourseName} ({e.Credits} credits)");
        Console.WriteLine();

        // 8. Aggregation
        Console.WriteLine("--- 8. Aggregation Functions ---");
        Console.WriteLine($"[LINQ] Count: {students.Count()}");
        Console.WriteLine($"[LINQ] Average GPA: {students.Average(s => s.GPA):F2}");
        Console.WriteLine($"[LINQ] Max GPA: {students.Max(s => s.GPA)}");
        Console.WriteLine($"[LINQ] Min Age: {students.Min(s => s.Age)}");
        Console.WriteLine($"[LINQ] Sum of Ages: {students.Sum(s => s.Age)}\n");

        // 9. Any, All, Contains
        Console.WriteLine("--- 9. Quantifiers (Any, All, Contains) ---");
        Console.WriteLine($"[LINQ] Any student with GPA > 3.9? {students.Any(s => s.GPA > 3.9)}");
        Console.WriteLine($"[LINQ] All students over 18? {students.All(s => s.Age > 18)}");
        Console.WriteLine($"[LINQ] Contains 'Alice'? {students.Select(s => s.Name).Contains("Alice")}\n");

        // 10. First, Single, FirstOrDefault
        Console.WriteLine("--- 10. Element Operators ---");
        var first = students.First(s => s.Major == "Computer Science");
        Console.WriteLine($"[LINQ] First CS student: {first.Name}");

        var firstOrDefault = students.FirstOrDefault(s => s.Major == "Physics");
        Console.WriteLine($"[LINQ] First Physics student: {firstOrDefault?.Name ?? "None"}\n");

        // 11. Take, Skip (Pagination)
        Console.WriteLine("--- 11. Pagination (Take, Skip) ---");
        int pageSize = 2;
        var page1 = students.OrderBy(s => s.Name).Take(pageSize);
        var page2 = students.OrderBy(s => s.Name).Skip(pageSize).Take(pageSize);
        Console.WriteLine($"[LINQ] Page 1: {string.Join(", ", page1.Select(s => s.Name))}");
        Console.WriteLine($"[LINQ] Page 2: {string.Join(", ", page2.Select(s => s.Name))}\n");

        // 12. Distinct, Union, Intersect
        Console.WriteLine("--- 12. Set Operations ---");
        var majors = students.Select(s => s.Major).Distinct();
        Console.WriteLine($"[LINQ] Distinct majors: {string.Join(", ", majors)}\n");

        // 13. Deferred vs Immediate Execution
        Console.WriteLine("--- 13. Deferred vs Immediate Execution ---");
        var deferredQuery = students.Where(s => s.GPA > 3.5); // Not executed yet
        Console.WriteLine($"[LINQ] Deferred query created (not executed)");

        var immediateList = students.Where(s => s.GPA > 3.5).ToList(); // Executed now
        Console.WriteLine($"[LINQ] Immediate execution with ToList(): {immediateList.Count} results\n");

        // 14. Complex Query
        Console.WriteLine("--- 14. Complex Query Example ---");
        var complexQuery = students
            .Where(s => s.Age >= 20)
            .GroupBy(s => s.Major)
            .Select(g => new
            {
                Major = g.Key,
                Count = g.Count(),
                AvgGPA = g.Average(s => s.GPA)
            })
            .OrderByDescending(x => x.AvgGPA);

        Console.WriteLine("[LINQ] Major statistics (Age >= 20):");
        foreach (var stat in complexQuery)
            Console.WriteLine($"[LINQ]   {stat.Major}: {stat.Count} students, Avg GPA: {stat.AvgGPA:F2}");
        Console.WriteLine();

        Console.WriteLine("💡 LINQ Best Practices:");
        Console.WriteLine("   ✅ Use method syntax for simple queries");
        Console.WriteLine("   ✅ Use query syntax for complex joins");
        Console.WriteLine("   ✅ Be aware of deferred execution");
        Console.WriteLine("   ✅ Use ToList()/ToArray() to force immediate execution");
        Console.WriteLine("   ✅ Chain operations for readability");
        Console.WriteLine("   ✅ Avoid multiple enumeration (cache with ToList if needed)");
    }

    private static List<Student> GetStudents()
    {
        return new List<Student>
        {
            new() { Id = 1, Name = "Alice", Age = 20, Major = "Computer Science", GPA = 3.8 },
            new() { Id = 2, Name = "Bob", Age = 22, Major = "Mathematics", GPA = 3.5 },
            new() { Id = 3, Name = "Charlie", Age = 19, Major = "Computer Science", GPA = 3.9 },
            new() { Id = 4, Name = "Diana", Age = 21, Major = "Physics", GPA = 3.7 },
            new() { Id = 5, Name = "Eve", Age = 20, Major = "Mathematics", GPA = 3.6 }
        };
    }

    private static List<Course> GetCourses()
    {
        return new List<Course>
        {
            new() { CourseId = 1, StudentId = 1, CourseName = "Data Structures", Credits = 4 },
            new() { CourseId = 2, StudentId = 1, CourseName = "Algorithms", Credits = 4 },
            new() { CourseId = 3, StudentId = 2, CourseName = "Calculus", Credits = 3 },
            new() { CourseId = 4, StudentId = 3, CourseName = "Operating Systems", Credits = 4 },
            new() { CourseId = 5, StudentId = 4, CourseName = "Quantum Mechanics", Credits = 4 }
        };
    }
}
