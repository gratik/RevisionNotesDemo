// ============================================================================
// COMPOSITE PATTERN - Treat Individual Objects and Compositions Uniformly
// Reference: Revision Notes - Design Patterns (Structural) - Page 3
// ============================================================================
//
// WHAT IS THE COMPOSITE PATTERN?
// -------------------------------
// Composes objects into tree structures to represent part-whole hierarchies.
// Allows clients to treat individual objects and compositions of objects uniformly.
// Both leaf nodes and branches implement the same interface.
//
// Think of it as: "File system - folders contain files OR other folders,
// but you can perform same operations (delete, move, size) on both"
//
// Core Concepts:
//   • Component: Interface for all objects in composition
//   • Leaf: Individual object with no children (end node)
//   • Composite: Object that contains children (branch node)
//   • Tree Structure: Recursive hierarchy of components
//   • Uniform Treatment: Same interface for leaf and composite
//
// WHY IT MATTERS
// --------------
// ✅ SIMPLICITY: Treat individual and composite objects uniformly
// ✅ RECURSIVE STRUCTURES: Natural representation of tree hierarchies
// ✅ FLEXIBILITY: Easy to add new component types
// ✅ OPEN/CLOSED: Add new components without changing client code
// ✅ POLYMORPHISM: Client works with Component interface, not concrete types
// ✅ NATURAL NAVIGATION: Traverse entire tree with same operations
//
// WHEN TO USE IT
// --------------
// ✅ Represent part-whole hierarchies (tree structures)
// ✅ Want clients to ignore difference between compositions and individual objects
// ✅ Structure can be represented recursively
// ✅ Need to apply same operations to individual and composite objects
// ✅ Building UI components, file systems, organization charts
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Structure is not hierarchical (flat list)
// ❌ Need very different operations for leaf vs composite
// ❌ Type safety more important than uniformity
// ❌ Simple parent-child relationship suffices
//
// REAL-WORLD EXAMPLE - Company Organization Chart
// -----------------------------------------------
// Corporate hierarchy:
//   • CEO (composite) → has VPs
//   • VP Engineering (composite) → has Directors
//   • Director (composite) → has Managers
//   • Manager (composite) → has Individual Contributors
//   • Individual Contributor (leaf) → no reports
//
// Operations needed:
//   • GetSalaryTotal() - for budgeting
//   • GetHeadcount() - for reporting
//   • PrintHierarchy() - for org chart
//
// Without Composite:
//   → decimal GetTotalSalary() {
//         decimal total = CEO.Salary;
//         foreach (VP vp in CEO.Reports) {
//             total += vp.Salary;
//             foreach (Director dir in vp.Reports) {
//                 total += dir.Salary;
//                 foreach (Manager mgr in dir.Reports) {
//                     total += mgr.Salary;
//                     foreach (IC ic in mgr.Reports) {
//                         total += ic.Salary;  // Finally!
//                     }
//                 }
//             }
//         }
//         return total;
//     }
//   → Nightmare code with nested loops
//   → Breaks if hierarchy depth changes
//   → Different logic for each level
//
// With Composite:
//   → interface IEmployee {
//         decimal GetSalaryTotal();
//         void PrintHierarchy(int indent);
//     }
//   → class IndividualContributor : IEmployee {  // Leaf
//         public decimal GetSalaryTotal() => Salary;
//     }
//   → class Manager : IEmployee {  // Composite
//         private List<IEmployee> _reports;
//         public decimal GetSalaryTotal() => Salary + _reports.Sum(r => r.GetSalaryTotal());
//     }
//   → decimal total = CEO.GetSalaryTotal();  // Recursively calculates entire tree!
//   → Same code works for any level
//   → Adding levels doesn't change code
//
// ANOTHER EXAMPLE - UI Component Tree
// -----------------------------------
// GUI framework (like WPF, React, SwiftUI):
//   • Window (composite) → contains Panels
//   • Panel (composite) → contains Buttons, TextBoxes, other Panels
//   • Button (leaf) → no children
//   • TextBox (leaf) → no children
//
// Operations:
//   • Render() - draw to screen
//   • HandleClick(x, y) - respond to mouse
//   • SetVisibility(bool) - show/hide
//
// Example:
//   Panel panel = new Panel();
//   panel.Add(new Button("OK"));
//   panel.Add(new Button("Cancel"));
//   panel.Add(new TextBox("Enter name"));
//   
//   Panel nestedPanel = new Panel();
//   nestedPanel.Add(new Button("Submit"));
//   panel.Add(nestedPanel);  // Panel contains panel!
//   
//   panel.Render();  // Renders everything recursively
//   panel.SetVisibility(false);  // Hides entire tree
//
// ANOTHER EXAMPLE - File System
// -----------------------------
// Windows/Linux file system:
//   • Directory (composite) → contains files and subdirectories
//   • File (leaf) → no children
//
// Operations:
//   • GetSize() - calculate total size
//   • Delete() - remove from disk
//   • Display(indent) - show tree structure
//
// Example:
//   Directory root = new Directory("C:\\");
//   Directory docs = new Directory("Documents");
//   docs.Add(new File("report.pdf", 1024000));
//   docs.Add(new File("notes.txt", 5000));
//   
//   Directory pics = new Directory("Pictures");
//   pics.Add(new File("photo1.jpg", 2048000));
//   pics.Add(new File("photo2.jpg", 1536000));
//   
//   root.Add(docs);
//   root.Add(pics);
//   
//   long totalSize = root.GetSize();  // Recursively sums: 1024000+5000+2048000+1536000
//   root.Display();  // Shows entire tree structure
//
// .NET FRAMEWORK EXAMPLES
// -----------------------
// Composite pattern used in .NET:
//   • WPF/WinForms controls: Panel contains Controls
//   • ASP.NET controls: WebControl hierarchy
//   • System.Drawing: Graphics objects
//   • XML DOM: Node hierarchy (Element, Text, Comment)
//
// DESIGN CONSIDERATIONS
// ---------------------
// 1. Where to store child management?
//    • Option A: In Component interface (simpler, less type-safe)
//    • Option B: Only in Composite class (more type-safe, less uniform)
//
// 2. Ordering of children:
//    • List<> for ordered children
//    • HashSet<> for unordered
//    • Dictionary<> for keyed children
//
// 3. Parent references:
//    • Can add Parent property for upward navigation
//    • Makes tree manipulation more complex
//
// COMPOSITE VS SIMILAR PATTERNS
// -----------------------------
// Composite vs Decorator:
//   • Composite: Represents part-whole (tree), many children
//   • Decorator: Adds responsibilities, one wrapped object
//
// Composite vs Chain of Responsibility:
//   • Composite: All children processed
//   • Chain: Stop at first handler that processes
//
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
