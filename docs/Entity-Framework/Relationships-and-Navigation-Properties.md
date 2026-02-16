# Relationships and Navigation Properties

> Subject: [Entity-Framework](../README.md)

## Relationships and Navigation Properties

### One-to-Many

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Navigation property
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    // Navigation property
    public User User { get; set; } = null!;
}

// Configuration
modelBuilder.Entity<Order>()
    .HasOne(o => o.User)
    .WithMany(u => u.Orders)
    .HasForeignKey(o => o.UserId);
```

### Many-to-Many

```csharp
// ✅ Many-to-many with join entity (explicit)
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
}

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
}

public class StudentCourse
{
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
    
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    
    public DateTime EnrolledDate { get; set; }  // Extra data on join table
}

// Configuration
modelBuilder.Entity<StudentCourse>()
    .HasKey(sc => new { sc.StudentId, sc.CourseId });

// ✅ Query many-to-many
var student = await _context.Students
    .Include(s => s.StudentCourses)
        .ThenInclude(sc => sc.Course)
    .FirstAsync(s => s.Id == 1);
```

---


