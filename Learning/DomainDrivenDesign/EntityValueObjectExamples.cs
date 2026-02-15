// ==============================================================================
// DOMAIN-DRIVEN DESIGN: ENTITIES AND VALUE OBJECTS
// ==============================================================================
// PURPOSE: Distinguish between identity-based entities and value-based objects
// WHY: Better modeling, equality semantics, immutability where appropriate
// USE WHEN: Designing domain models in DDD applications
// ==============================================================================
// WHAT ARE ENTITIES AND VALUE OBJECTS?
// Domain-Driven Design distinguishes two fundamental building blocks:
//
// ENTITY: An object defined by its IDENTITY (ID), not its attributes.
// - Has a unique identifier that persists over time
// - Can change attributes but remains the same entity
// - Equality based on ID (two customers with same ID are the SAME customer)
// - Example: Customer, Order, Product (have lifecycle, track changes)
//
// VALUE OBJECT: An object defined by its VALUES, not identity.
// - No unique identifier - defined entirely by its properties
// - Immutable - cannot change after creation
// - Equality based on all values (two Addresses with same values are identical)
// - Example: Money, Address, Email, DateRange (descriptive, replaceable)
//
// WHY IT MATTERS:
// • RICH DOMAIN MODELS: Entities encapsulate business logic (not just data bags)
// • IMMUTABILITY: Value Objects prevent accidental mutations and bugs
// • SELF-VALIDATION: Objects validate themselves, ensuring consistent state
// • SEMANTIC CLARITY: Express domain concepts explicitly (Money vs decimal)
// • TESTABILITY: Immutable value objects are easier to test
//
// WHEN TO USE:
// ✅ Use ENTITIES when object identity matters (Customer, Order, User)
// ✅ Use VALUE OBJECTS for descriptive attributes (Address, Money, Email)
// ✅ Prefer Value Objects when possible (simpler, immutable, shareable)
// ✅ Encapsulate primitive types (avoid "primitive obsession")
//
// THE ANEMIC DOMAIN MODEL ANTI-PATTERN:
// Many applications use "anemic" entities that are just data containers with
// getters/setters and no behavior. Business logic lives in separate service
// classes, violating encapsulation. DDD advocates for "rich" domain models
// where entities contain both data AND the behaviors that operate on that data.
//
// REAL-WORLD EXAMPLE:
// E-commerce: Order (entity with ID) contains OrderLines and Money (value objects).
// When adding a line, Order validates business rules ("max 10 items"), updates
// Total, and raises domain events - all within the Order aggregate.
// ==============================================================================

namespace RevisionNotesDemo.DomainDrivenDesign;

public static class EntityValueObjectExamples
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== DDD: ENTITIES VS VALUE OBJECTS ===\n");
        
        Example1_AnemicVsRichDomain();
        Example2_EntityExample();
        Example3_ValueObjectExample();
        Example4_DomainValidation();
        Example5_EqualitySemantics();
        
        Console.WriteLine("\n💡 Key Takeaways:");
        Console.WriteLine("   ✅ Entities have identity (defined by ID)");
        Console.WriteLine("   ✅ Value Objects have no identity (defined by values)");
        Console.WriteLine("   ✅ Value Objects should be immutable");
        Console.WriteLine("   ✅ Rich domain models encapsulate business logic");
        Console.WriteLine("   ✅ Validate in domain, not in application layer");
    }
    
    private static void Example1_AnemicVsRichDomain()
    {
        Console.WriteLine("=== EXAMPLE 1: Anemic vs Rich Domain Model ===\n");
        
        Console.WriteLine("❌ BAD: Anemic domain model (data bags)\n");
        // Antipattern: Just getters/setters, no behavior
        // public class Order {
        //     public int Id { get; set; }
        //     public decimal Total { get; set; }
        //     public string Status { get; set; }
        //     public List<OrderLine> Lines { get; set; }
        // }
        // Business logic scattered in services!
        
        Console.WriteLine("\n✅ GOOD: Rich domain model (behavior + data)\n");
        // Best practice: Domain logic in the entity
        // public class Order {
        //     public int Id { get; private set; }
        //     public Money Total { get; private set; }
        //     public OrderStatus Status { get; private set; }
        //     private readonly List<OrderLine> _lines = new();
        //     
        //     public void AddLine(Product product, int quantity) {
        //         if (Status != OrderStatus.Draft)
        //             throw new InvalidOperationException("Cannot modify submitted order");
        //         _lines.Add(new OrderLine(product, quantity));
        //         RecalculateTotal();
        //     }
        // }
        
        Console.WriteLine("\n📊 Benefits:");
        Console.WriteLine("   • Business rules enforced in domain");
        Console.WriteLine("   • Cannot create invalid state");
        Console.WriteLine("   • Single responsibility");
        Console.WriteLine("   • Easy to test");
    }
    
    private static void Example2_EntityExample()
    {
        Console.WriteLine("\n=== EXAMPLE 2: Entity (Identity-Based) ===\n");
        
        Console.WriteLine("✅ Entity: Equality based on ID, mutable");
        // public class Customer : Entity<CustomerId> {
        //     public string Name { get; private set; }
        //     public Email Email { get; private set; }
        //     public Address BillingAddress { get; private set; }
        //     
        //     public void ChangeName(string newName) {
        //         if (string.IsNullOrWhiteSpace(newName))
        //             throw new ArgumentException("Name required");
        //         Name = newName;
        //     }
        //     
        //     public void ChangeEmail(Email newEmail) {
        //         Email = newEmail ?? throw new ArgumentNullException();
        //         RaiseDomainEvent(new CustomerEmailChanged(Id, newEmail));
        //     }
        // }
        // Two customers with same name are DIFFERENT if IDs differ
        
        Console.WriteLine("\n📊 Characteristics:");
        Console.WriteLine("   • Has unique identifier");
        Console.WriteLine("   • Can change over time");
        Console.WriteLine("   • Equality by ID");
        Console.WriteLine("   • Lifecycle (created, modified, deleted)");
    }
    
    private static void Example3_ValueObjectExample()
    {
        Console.WriteLine("\n=== EXAMPLE 3: Value Object (Value-Based) ===\n");
        
        Console.WriteLine("✅ Value Object: Immutable, equality by value");
        // public record Money(decimal Amount, string Currency) {
        //     public Money Add(Money other) {
        //         if (Currency != other.Currency)
        //             throw new InvalidOperationException("Currency mismatch");
        //         return new Money(Amount + other.Amount, Currency);
        //     }
        //     
        //     public Money MultiplyBy(decimal factor) =>
        //         new Money(Amount * factor, Currency);
        // }
        //
        // public record Address(string Street, string City, string ZipCode, string Country);
        //
        // public record Email {
        //     public string Value { get; }
        //     public Email(string value) {
        //         if (!IsValid(value))
        //             throw new ArgumentException("Invalid email");
        //         Value = value;
        //     }
        //     private static bool IsValid(string email) => email.Contains('@');
        // }
        
        Console.WriteLine("\n📊 Characteristics:");
        Console.WriteLine("   • No identity");
        Console.WriteLine("   • Immutable");
        Console.WriteLine("   • Equality by value");
        Console.WriteLine("   • Can be shared/reused");
        Console.WriteLine("   • Self-validating");
    }
    
    private static void Example4_DomainValidation()
    {
        Console.WriteLine("\n=== EXAMPLE 4: Domain Validation ===\n");
        
        Console.WriteLine("❌ BAD: Validation in application layer\n");
        // if (name == null || name.Length < 3) throw new Exception();
        // var customer = new Customer { Name = name };
        
        Console.WriteLine("\n✅ GOOD: Self-validating domain objects\n");
        // public class Customer {
        //     public string Name { get; private set; }
        //     
        //     public static Customer Create(string name, Email email) {
        //         ValidateName(name);
        //         return new Customer { Name = name, Email = email };
        //     }
        //     
        //     private static void ValidateName(string name) {
        //         if (string.IsNullOrWhiteSpace(name))
        //             throw new DomainException("Name required");
        //         if (name.Length < 3 || name.Length > 100)
        //             throw new DomainException("Name must be 3-100 characters");
        //     }
        // }
        
        Console.WriteLine("\n📊 Benefits:");
        Console.WriteLine("   • Validation close to data");
        Console.WriteLine("   • Cannot bypass rules");
        Console.WriteLine("   • Clear business meaning");
    }
    
    private static void Example5_EqualitySemantics()
    {
        Console.WriteLine("\n=== EXAMPLE 5: Equality Semantics ===\n");
        
        Console.WriteLine("Entities: Equal if same ID");
        // var customer1 = new Customer { Id = 1, Name = "John" };
        // var customer2 = new Customer { Id = 1, Name = "Jane" };
        // customer1 == customer2  // TRUE (same ID)
        
        Console.WriteLine("\nValue Objects: Equal if all values match");
        // var addr1 = new Address("123 Main St", "Seattle", "98101", "USA");
        // var addr2 = new Address("123 Main St", "Seattle", "98101", "USA");
        // addr1 == addr2  // TRUE (all values same)
        
        Console.WriteLine("\n📊 Implementation:");
        Console.WriteLine("   • Entities: Override Equals to compare IDs");
        Console.WriteLine("   • Value Objects: Use records or override Equals for all fields");
    }
}
