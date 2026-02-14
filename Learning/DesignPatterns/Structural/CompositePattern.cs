// ============================================================================
// COMPOSITE PATTERN
// Reference: Revision Notes - Design Patterns (Structural) - Page 3
// ============================================================================
// PURPOSE: "Composes objects into tree structures to represent part-whole hierarchies."
// EXAMPLE: File system directories and files.
// ============================================================================

namespace RevisionNotesDemo.DesignPatterns.Structural;

// Component interface
public abstract class FileSystemComponent
{
    protected string Name { get; set; }

    protected FileSystemComponent(string name)
    {
        Name = name;
    }

    public abstract void Display(int depth = 0);
    public abstract long GetSize();

    protected string GetIndent(int depth) => new string(' ', depth * 2);
}

// Leaf - File
public class File : FileSystemComponent
{
    private long _size;

    public File(string name, long size) : base(name)
    {
        _size = size;
    }

    public override void Display(int depth = 0)
    {
        Console.WriteLine($"{GetIndent(depth)}📄 {Name} ({_size} bytes)");
    }

    public override long GetSize() => _size;
}

// Composite - Directory
public class Directory : FileSystemComponent
{
    private List<FileSystemComponent> _children = new();

    public Directory(string name) : base(name) { }

    public void Add(FileSystemComponent component)
    {
        _children.Add(component);
        Console.WriteLine($"[COMPOSITE] Added {component.GetType().Name} to directory '{Name}'");
    }

    public void Remove(FileSystemComponent component)
    {
        _children.Remove(component);
    }

    public override void Display(int depth = 0)
    {
        Console.WriteLine($"{GetIndent(depth)}📁 {Name}/");
        foreach (var child in _children)
        {
            child.Display(depth + 1);
        }
    }

    public override long GetSize()
    {
        return _children.Sum(child => child.GetSize());
    }

    public int GetFileCount()
    {
        int count = 0;
        foreach (var child in _children)
        {
            if (child is File)
                count++;
            else if (child is Directory dir)
                count += dir.GetFileCount();
        }
        return count;
    }
}

// Another example: Organization hierarchy
public abstract class OrganizationComponent
{
    protected string Name { get; set; }

    protected OrganizationComponent(string name)
    {
        Name = name;
    }

    public abstract void ShowDetails(int depth = 0);
    public abstract decimal GetTotalSalary();
    protected string GetIndent(int depth) => new string(' ', depth * 3);
}

// Leaf - Employee
public class Employee : OrganizationComponent
{
    private decimal _salary;
    private string _position;

    public Employee(string name, string position, decimal salary) : base(name)
    {
        _position = position;
        _salary = salary;
    }

    public override void ShowDetails(int depth = 0)
    {
        Console.WriteLine($"{GetIndent(depth)}👤 {Name} - {_position} (${_salary:N0})");
    }

    public override decimal GetTotalSalary() => _salary;
}

// Composite - Department
public class Department : OrganizationComponent
{
    private List<OrganizationComponent> _members = new();

    public Department(string name) : base(name) { }

    public void Add(OrganizationComponent component)
    {
        _members.Add(component);
    }

    public void Remove(OrganizationComponent component)
    {
        _members.Remove(component);
    }

    public override void ShowDetails(int depth = 0)
    {
        Console.WriteLine($"{GetIndent(depth)}🏢 {Name} Department");
        foreach (var member in _members)
        {
            member.ShowDetails(depth + 1);
        }
    }

    public override decimal GetTotalSalary()
    {
        return _members.Sum(member => member.GetTotalSalary());
    }

    public int GetEmployeeCount()
    {
        int count = 0;
        foreach (var member in _members)
        {
            if (member is Employee)
                count++;
            else if (member is Department dept)
                count += dept.GetEmployeeCount();
        }
        return count;
    }
}

// Usage demonstration
public class CompositeDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== COMPOSITE PATTERN DEMO ===\n");

        Console.WriteLine("--- Example 1: File System ---");

        // Create file system structure
        var root = new Directory("root");

        var documents = new Directory("documents");
        documents.Add(new File("resume.pdf", 1024));
        documents.Add(new File("cover_letter.docx", 2048));

        var photos = new Directory("photos");
        photos.Add(new File("vacation1.jpg", 5120));
        photos.Add(new File("vacation2.jpg", 6144));

        var work = new Directory("work");
        work.Add(new File("project.xlsx", 3072));

        root.Add(documents);
        root.Add(photos);
        root.Add(work);
        root.Add(new File("readme.txt", 512));

        Console.WriteLine("\n[COMPOSITE] File System Structure:");
        root.Display();

        Console.WriteLine($"\n[COMPOSITE] Total size: {root.GetSize()} bytes");
        Console.WriteLine($"[COMPOSITE] Total files: {root.GetFileCount()}");

        Console.WriteLine("\n--- Example 2: Organization Hierarchy ---");

        // Create organization structure
        var company = new Department("Company");

        var engineering = new Department("Engineering");
        engineering.Add(new Employee("Alice", "Senior Developer", 120000));
        engineering.Add(new Employee("Bob", "Developer", 90000));
        engineering.Add(new Employee("Charlie", "Junior Developer", 60000));

        var marketing = new Department("Marketing");
        marketing.Add(new Employee("Diana", "Marketing Manager", 100000));
        marketing.Add(new Employee("Eve", "Marketing Specialist", 70000));

        var sales = new Department("Sales");
        sales.Add(new Employee("Frank", "Sales Director", 110000));
        sales.Add(new Employee("Grace", "Sales Rep", 65000));
        sales.Add(new Employee("Henry", "Sales Rep", 65000));

        company.Add(new Employee("CEO", "Chief Executive Officer", 250000));
        company.Add(engineering);
        company.Add(marketing);
        company.Add(sales);

        Console.WriteLine("\n[COMPOSITE] Organization Structure:");
        company.ShowDetails();

        Console.WriteLine($"\n[COMPOSITE] Total employees: {company.GetEmployeeCount()}");
        Console.WriteLine($"[COMPOSITE] Total salary budget: ${company.GetTotalSalary():N0}");

        Console.WriteLine("\n💡 Benefit: Treats individual objects and compositions uniformly");
        Console.WriteLine("💡 Benefit: Perfect for tree structures (file systems, org charts, UI components)");
        Console.WriteLine("💡 Benefit: Easy to add new component types");
    }
}
