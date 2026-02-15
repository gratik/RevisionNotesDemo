// ==============================================================================
// ENTITY FRAMEWORK CORE - RELATIONSHIPS & NAVIGATION PROPERTIES
// Reference: Revision Notes - Entity Framework Core (Section 8.6.2)
// ==============================================================================
//
// WHAT IS IT?
// -----------
// Modeling relationships (one-to-many, one-to-one, many-to-many) and using
// navigation properties to traverse related data efficiently.
//
// WHY IT MATTERS
// --------------
// - Correct relationship modeling drives correct SQL
// - Avoids N+1 query performance problems
// - Enables clean domain modeling with navigation properties
//
// WHEN TO USE
// -----------
// - YES: Any EF Core model with related entities
// - YES: When you need Include/ThenInclude to load related data
// - YES: When designing schemas for real-world domains
//
// WHEN NOT TO USE
// ---------------
// - NO: Do not rely on lazy loading in performance-critical APIs
// - NO: Do not load entire object graphs if only small data is needed
//
// REAL-WORLD EXAMPLE
// ------------------
// E-commerce:
// - Customer has many Orders
// - Order has many OrderItems
// - Use Include/ThenInclude to load customer with orders in one query
// ==============================================================================

using Microsoft.EntityFrameworkCore;

namespace RevisionNotesDemo.DataAccess.EntityFramework;

/// <summary>
/// ONE-TO-MANY RELATIONSHIP: One customer can have many orders
/// This is the MOST COMMON relationship type in real applications.
/// 
/// NAVIGATION PROPERTIES:
///   - Customer.Orders (collection) → Navigate from customer to their orders
///   - Order.Customer (reference) → Navigate from order back to customer
/// 
/// REAL-WORLD EXAMPLES:
///   - Blog → Posts, Department → Employees, Category → Products
/// </summary>
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Collection navigation property (one customer has many orders)
    // IMPORTANT: Initialize to empty list to avoid NullReferenceException
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

/// <summary>
/// Order entity - the "many" side of the Customer-Order relationship
/// </summary>
public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }

    // Foreign key (points to Customer.Id)
    public int CustomerId { get; set; }

    // Reference navigation property (order belongs to one customer)
    public Customer Customer { get; set; } = null!;
}

/// <summary>
/// ONE-TO-ONE RELATIONSHIP: One user has exactly one profile
/// Less common than one-to-many, but important for splitting entities.
/// 
/// WHEN TO USE:
///   - Separating frequently-accessed data from rarely-accessed data
///   - Security (User vs UserPasswordHash)
///   - Different lifecycles (User vs UserPreferences)
/// 
/// CONFIGURATION:
///   Must specify which side owns the foreign key using HasForeignKey
/// 
/// REAL-WORLD EXAMPLES:
///   - User → UserProfile, Person → Passport, Order → Invoice
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;

    // Reference navigation property (one-to-one)
    public UserProfile Profile { get; set; } = null!;
}

/// <summary>
/// UserProfile - the dependent side of User-UserProfile relationship
/// This entity "depends" on User (has the foreign key)
/// </summary>
public class UserProfile
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    // Foreign key pointing to User
    public int UserId { get; set; }

    // Reference navigation property back to User
    public User User { get; set; } = null!;
}

/// <summary>
/// MANY-TO-MANY RELATIONSHIP: Students can enroll in many courses,
///                            Courses can have many students enrolled.
/// 
/// EF CORE 5+ FEATURE:
///   Automatically creates join table (StudentCourse) behind the scenes.
///   You don't need to create a join entity explicitly!
/// 
/// PRE-EF CORE 5:
///   Had to manually create join entity:
///   public class StudentCourse {
///       public int StudentId { get; set; }
///       public int CourseId { get; set; }
///       public Student Student { get; set; }
///       public Course Course { get; set; }
///   }
/// 
/// WHEN TO USE:
///   - Tags ↔ Posts, Roles ↔ Users, Products ↔ Categories
///   - Any scenario where "both sides can have many"
/// 
/// REAL-WORLD EXAMPLES:
///   - E-learning: Students ↔ Courses
///   - Social: Users ↔ Friends (self-referencing)
///   - Marketplace: Products ↔ Tags
/// </summary>
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Collection navigation property (many-to-many)
    // EF Core 5+ automatically creates join table
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}

/// <summary>
/// Course entity - the other side of the many-to-many relationship
/// </summary>
public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    // Collection navigation property (many-to-many)
    public ICollection<Student> Students { get; set; } = new List<Student>();
}

public class RelationshipsDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("RelationshipsDb");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ONE-TO-MANY configuration
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId);

        // ONE-TO-ONE configuration
        modelBuilder.Entity<UserProfile>()
            .HasOne(p => p.User)
            .WithOne(u => u.Profile)
            .HasForeignKey<UserProfile>(p => p.UserId);
    }
}

public class RelationshipsNavigationExamples
{
    /// <summary>
    /// EXAMPLE 1: Eager Loading with Include - The Right Way to Load Related Data
    /// 
    /// EAGER LOADING:
    ///   Load related entities in the SAME query using SQL JOIN.
    ///   This is the BEST approach for most scenarios.
    /// 
    /// WHY IT'S CALLED "EAGER":
    ///   We're "eagerly" loading the data upfront, not waiting until accessed.
    /// 
    /// BENEFIT:
    ///   - Single database query (efficient)
    ///   - All data loaded upfront (no surprises later)
    ///   - Prevents N+1 query problem
    /// 
    /// GENERATED SQL:
    ///   SELECT c.*, o.*
    ///   FROM Customers c
    ///   LEFT JOIN Orders o ON c.Id = o.CustomerId
    /// 
    /// NESTED INCLUDES:
    ///   Need to load Orders and their OrderItems?
    ///   .Include(c => c.Orders).ThenInclude(o => o.OrderItems)
    /// 
    /// MULTIPLE INCLUDES:
    ///   .Include(c => c.Orders).Include(c => c.Addresses)
    /// 
    /// GOTCHA: Be careful with multiple includes - can cause "cartesian explosion"
    ///         (exponential increase in rows returned). Consider AsSplitQuery().
    /// </summary>
    public static async Task EagerLoadingExample()
    {
        using var context = new RelationshipsDbContext();

        // Create test data with related entities
        var customer = new Customer
        {
            Name = "John Doe",
            Orders = new List<Order>
            {
                new Order { OrderDate = DateTime.Now, Total = 199.99m },
                new Order { OrderDate = DateTime.Now.AddDays(-7), Total = 49.99m }
            }
        };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        // Eager load customer WITH orders using Include
        // This generates ONE query with JOIN - efficient!
        var loadedCustomer = await context.Customers
            .Include(c => c.Orders)  // ← Load related Orders in same query
            .FirstOrDefaultAsync();

        // Now we can access Orders without triggering additional queries
        // The data is already loaded in memory
        Console.WriteLine($"Customer: {loadedCustomer?.Name}, Orders: {loadedCustomer?.Orders.Count}");

        // TIP: Without Include, accessing customer.Orders would either:
        //      1. Return empty collection (if lazy loading disabled) - confusing!
        //      2. Trigger separate query (if lazy loading enabled) - N+1 problem!
    }

    /// <summary>
    /// EXAMPLE 2: The N+1 Query Problem - The #1 Performance Killer in ORMs
    /// 
    /// THE PROBLEM:
    ///   Loading a collection (e.g., customers), then accessing related data
    ///   (e.g., orders) in a loop triggers 1 + N database queries:
    ///   - 1 query to load customers
    ///   - N queries (one per customer) to load their orders
    /// 
    /// WHY IT'S BAD:
    ///   - 100 customers = 101 database queries!
    ///   - Network latency multiplied by N
    ///   - Database connection pool exhaustion
    ///   - Can bring down production systems!
    /// 
    /// HOW TO DETECT:
    ///   - Enable SQL logging: options.LogTo(Console.WriteLine)
    ///   - Use profiling tools (MiniProfiler, Application Insights)
    ///   - Watch for many similar queries in logs
    /// 
    /// THE FIX:
    ///   Use .Include() to load related data in a single query with JOIN
    /// 
    /// REAL-WORLD IMPACT:
    ///   Fixing N+1 can improve response times from 5 seconds to 50ms!
    /// 
    /// ⚠️ WARNING: This is one of the most common bugs in web applications.
    ///            Always be aware of this pattern!
    /// </summary>
    public static async Task AvoidN1Problem()
    {
        using var context = new RelationshipsDbContext();

        // ❌ BAD: N+1 queries - THE ANTI-PATTERN
        Console.WriteLine("❌ BAD - N+1 queries:");
        var customers = await context.Customers.ToListAsync();  // Query 1: Load customers
        foreach (var customer in customers)
        {
            // Query 2, 3, 4, ... N+1: Load orders for EACH customer
            var orderCount = await context.Orders
                .Where(o => o.CustomerId == customer.Id)
                .CountAsync();  // Separate query for EACH customer!
            // This is a DISASTER in production with 1000+ customers
        }

        // ✅ GOOD: Single query with Include - THE SOLUTION
        Console.WriteLine("✅ GOOD - Single query with Include:");
        var customersGood = await context.Customers
            .Include(c => c.Orders)  // JOIN Orders in the same query
            .ToListAsync();  // Only ONE database query!
        // Now we can access Orders without additional queries
        foreach (var customer in customersGood)
        {
            var orderCount = customer.Orders.Count;  // No database query!
            // Data already in memory
        }

        // TIP: Modern profiling tools will warn you about N+1 queries
        // Always test with realistic data volumes (not just 3 test records)
    }

    /// <summary>
    /// EXAMPLE 3: One-to-One Relationship - User and UserProfile
    /// 
    /// WHEN TO USE ONE-TO-ONE:
    ///   1. Split large entities (frequently vs rarely accessed columns)
    ///   2. Security separation (User vs UserPasswordHash)
    ///   3. Different lifecycles (User vs UserSettings)
    ///   4. Optional relationships (Person vs Passport - not everyone has passport)
    /// 
    /// CONFIGURATION:
    ///   EF Core needs to know which side "owns" the relationship.
    ///   Use HasForeignKey to specify the dependent side.
    /// 
    /// LOADING:
    ///   Same as one-to-many - use .Include() to load profile with user
    /// 
    /// GOTCHA: Without Include, Profile will be null even if it exists in DB!
    ///         This confuses many developers. Always Include for one-to-one.
    /// 
    /// ALTERNATIVE DESIGN:
    ///   Instead of separate tables, could use JSON column (EF Core 7+):
    ///   public string ProfileJson { get; set; }  // Store profile as JSON
    /// </summary>
    public static async Task OneToOneExample()
    {
        using var context = new RelationshipsDbContext();

        // Create user with profile (both saved together)
        var user = new User
        {
            Username = "johndoe",
            Profile = new UserProfile
            {
                FirstName = "John",
                LastName = "Doe"
            }
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Load user WITH profile using Include
        // Without Include, loadedUser.Profile would be null!
        var loadedUser = await context.Users
            .Include(u => u.Profile)  // ← Essential for one-to-one
            .FirstOrDefaultAsync();

        // Now we can safely access Profile
        Console.WriteLine($"User: {loadedUser?.Username}, Profile: {loadedUser?.Profile.FirstName}");

        // COMMON BUG: Forgetting Include, then getting NullReferenceException
        // when accessing loadedUser.Profile.FirstName
    }

    /// <summary>
    /// EXAMPLE 4: Many-to-Many Relationship - Students and Courses
    /// 
    /// HOW EF CORE 5+ HANDLES MANY-TO-MANY:
    ///   Automatically creates a join table behind the scenes.
    ///   No need to create StudentCourse entity manually!
    /// 
    /// JOIN TABLE:
    ///   EF creates table named: StudentCourse (or StudentsCoursestypically)
    ///   Columns: StudentsId, CoursesId (foreign keys to both tables)
    /// 
    /// ADDING RELATIONSHIPS:
    ///   student.Courses.Add(course);  // Add course to student
    ///   course.Students.Add(student);  // Add student to course
    ///   Both accomplish the SAME thing (adds row to join table)
    /// 
    /// LOADING:
    ///   Use Include just like one-to-many:
    ///   - Load students with courses: Include(s => s.Courses)
    ///   - Load courses with students: Include(c => c.Students)
    /// 
    /// WHEN YOU NEED EXTRA DATA IN JOIN TABLE:
    ///   Example: EnrollmentDate, Grade in StudentCourse
    ///   Solution: Manually create join entity with extra properties:
    ///   public class Enrollment {
    ///       public int StudentId { get; set; }
    ///       public int CourseId { get; set; }
    ///       public DateTime EnrollmentDate { get; set; }
    ///       public string Grade { get; set; }
    ///   }
    /// 
    /// REAL-WORLD EXAMPLES:
    ///   - Products ↔ Categories
    ///   - Posts ↔ Tags
    ///   - Users ↔ Roles
    ///   - Actors ↔ Movies
    /// </summary>
    public static async Task ManyToManyExample()
    {
        using var context = new RelationshipsDbContext();

        // Create student enrolled in multiple courses
        var student = new Student
        {
            Name = "Alice Smith",
            Courses = new List<Course>
            {
                new Course { Title = "Mathematics" },
                new Course { Title = "Physics" }
            }
        };

        // This creates:
        // - 1 row in Students table
        // - 2 rows in Courses table
        // - 2 rows in join table (StudentCourse)
        context.Students.Add(student);
        await context.SaveChangesAsync();

        // Load student WITH courses
        var loadedStudent = await context.Students
            .Include(s => s.Courses)  // Load related courses
            .FirstOrDefaultAsync();

        Console.WriteLine($"Student: {loadedStudent?.Name}, Courses: {loadedStudent?.Courses.Count}");

        // TIP: To add a course to existing student:
        // var existingCourse = await context.Courses.FindAsync(courseId);
        // student.Courses.Add(existingCourse);
        // await context.SaveChangesAsync();  // Adds row to join table
    }

    public static async Task RunAllExamples()
    {
        Console.WriteLine("\n=== ENTITY FRAMEWORK - RELATIONSHIPS & NAVIGATION ===\n");
        await EagerLoadingExample();
        await AvoidN1Problem();
        await OneToOneExample();
        await ManyToManyExample();
        Console.WriteLine("\nRelationships examples completed!\n");
    }
}
