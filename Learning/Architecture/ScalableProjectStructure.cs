// ==============================================================================
// Scalable Project Structure & Organization
// ==============================================================================
// WHAT IS THIS?
// A scalable project structure organizes code to support growth: from 1 developer to teams of 100+. It balances ease-of-navigation with flexibility for different concerns (features, layers, infrastructure).
//
// WHY IT MATTERS
// ✅ ONBOARDING: New developers understand codebase in days, not weeks | ✅ SCALING: Teams can work independently without merge conflicts | ✅ MAINTENANCE: Easy to find code, understand relationships | ✅ TESTING: Clear separation enables isolated unit tests | ✅ DEPLOYMENT: Feature toggles and separation reduce deployment risk | ✅ PERFORMANCE: Layer structure makes optimization easier
//
// WHEN TO USE
// ✅ Any growing codebase (>1000 lines) | ✅ Team > 2 developers | ✅ Long-term projects | ✅ Multiple deployment environments
//
// WHEN NOT TO USE
// ❌ Proof-of-concept code | ❌ Solo projects with <2000 lines | ❌ Scripts and tools
//
// REAL-WORLD EXAMPLE
// Startup with 5 engineers: Monolithic folder structure works. Company scales 50 engineers → dozens of features in progress → folder by layer = merge conflicts. Refactor to vertical slices → teams own features end-to-end.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Architecture;

public class ScalableProjectStructure
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Scalable Project Structure & Organization");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");

        LayeredArchitecture();
        VerticalSliceArchitecture();
        ModularMonolith();
        Microservices();
        BestPractices();
    }

    private static void LayeredArchitecture()
    {
        Console.WriteLine("📚 LAYERED ARCHITECTURE:\n");

        Console.WriteLine("Structure:");
        Console.WriteLine("src/");
        Console.WriteLine("├── Presentation/        (Controllers, ViewModels)");
        Console.WriteLine("├── Application/         (Design patterns, orchestration)");
        Console.WriteLine("├── Domain/              (Business entities, interfaces)");
        Console.WriteLine("├── Infrastructure/      (DB, external APIs)");
        Console.WriteLine("└── Tests/               (Unit, integration tests)\n");

        Console.WriteLine("Data flow:");
        Console.WriteLine("  Request → Controller (Presentation)");
        Console.WriteLine("         → Service (Application)");
        Console.WriteLine("         → Repository (Infrastructure)");
        Console.WriteLine("         → Database\n");

        Console.WriteLine("Pros:");
        Console.WriteLine("  ✅ Clear separation of concerns");
        Console.WriteLine("  ✅ Dependency injection straightforward");
        Console.WriteLine("  ✅ Well-understood by teams\n");

        Console.WriteLine("Cons:");
        Console.WriteLine("  ❌ Adding feature spans multiple directories (hard to find)");
        Console.WriteLine("  ❌ Lots of interfaces/abstractions (accidental complexity)");
        Console.WriteLine("  ❌ Large teams = merge conflicts in same directories\n");
    }

    private static void VerticalSliceArchitecture()
    {
        Console.WriteLine("✂️ VERTICAL SLICE ARCHITECTURE:\n");

        Console.WriteLine("Structure (by feature):");
        Console.WriteLine("src/");
        Console.WriteLine("├── Features/");
        Console.WriteLine("│   ├── Products/");
        Console.WriteLine("│   │   ├── GetProductHandler.cs");
        Console.WriteLine("│   │   ├── CreateProductHandler.cs");
        Console.WriteLine("│   │   ├── ProductDto.cs");
        Console.WriteLine("│   │   ├── ProductRepository.cs");
        Console.WriteLine("│   │   └── Tests/");
        Console.WriteLine("│   ├── Orders/");
        Console.WriteLine("│   │   ├── CreateOrderHandler.cs");
        Console.WriteLine("│   │   ├── OrderDto.cs");
        Console.WriteLine("│   │   └── ...");
        Console.WriteLine("│   └── Users/");
        Console.WriteLine("│       ├── ...");
        Console.WriteLine("└── Shared/              (Cross-cutting: logging, auth)\n");

        Console.WriteLine("Decision making:");
        Console.WriteLine("  • Each feature teams owns: API endpoint → DB layer");
        Console.WriteLine("  • Independent features → parallel development");
        Console.WriteLine("  • Related tests in same folder as feature\n");

        Console.WriteLine("Pros:");
        Console.WriteLine("  ✅ One feature = one folder (easy to find)");
        Console.WriteLine("  ✅ Delete feature = delete folder (no orphaned code)");
        Console.WriteLine("  ✅ Teams work independently (fewer merge conflicts)");
        Console.WriteLine("  ✅ Easier to extract to microservice later\n");

        Console.WriteLine("Cons:");
        Console.WriteLine("  ❌ Potential duplication (UserDto in Users, AdminUserDto in Admin)");
        Console.WriteLine("  ❌ Shared logic must go to Shared (or strategic location)\n");
    }

    private static void ModularMonolith()
    {
        Console.WriteLine("🔧 MODULAR MONOLITH:\n");

        Console.WriteLine("Hybrid approach (layers + slices):");
        Console.WriteLine("src/");
        Console.WriteLine("├── Modules/");
        Console.WriteLine("│   ├── Products/");
        Console.WriteLine("│   │   ├── Application/  (Domain logic for Products)");
        Console.WriteLine("│   │   ├── Domain/       (Product entities)");
        Console.WriteLine("│   │   ├── Infrastructure/  (Product repository)");
        Console.WriteLine("│   │   ├── Presentation/  (Product controller)");
        Console.WriteLine("│   │   └── Tests/");
        Console.WriteLine("│   ├── Orders/");
        Console.WriteLine("│   │   ├── Application/");
        Console.WriteLine("│   │   ├── ...");
        Console.WriteLine("│   └── Shared/");
        Console.WriteLine("│       ├── Domain/      (Common entities, enums)");
        Console.WriteLine("│       ├── Infrastructure/  (DB context, migrations)");
        Console.WriteLine("│       └── Presentation/  (Shared mappers, filters)\n");

        Console.WriteLine("Architecture:");
        Console.WriteLine("  ✅ Each module self-contained (can extract to microservice)");
        Console.WriteLine("  ✅ Clear internal structure (application, domain, infrastructure)");
        Console.WriteLine("  ✅ Explicit dependencies (module.domain references via interfaces)\n");
    }

    private static void Microservices()
    {
        Console.WriteLine("🚀 MICROSERVICES:\n");

        Console.WriteLine("Each service = own project:");
        Console.WriteLine("solution/");
        Console.WriteLine("├── ProductService/");
        Console.WriteLine("│   ├── Features/ (vertical slices)");
        Console.WriteLine("│   ├── Domain/");
        Console.WriteLine("│   ├── Infrastructure/");
        Console.WriteLine("│   └── Tests/");
        Console.WriteLine("├── OrderService/");
        Console.WriteLine("│   ├── Features/");
        Console.WriteLine("│   ├── ...");
        Console.WriteLine("├── UserService/");
        Console.WriteLine("│   └── ...");
        Console.WriteLine("└── Shared.Messaging/  (Cross-service communication)\n");

        Console.WriteLine("Communication:");
        Console.WriteLine("  • Synchronous: ProductService.API → OrderService.API");
        Console.WriteLine("  • Asynchronous: OrderService sends \"order.created\" event");
        Console.WriteLine("                  ProductService listens and updates inventory\n");

        Console.WriteLine("Tradeoffs:");
        Console.WriteLine("  ✅ Independent scaling (scale ProductService alone)");
        Console.WriteLine("  ✅ Technology freedom (ProductService uses .NET, OrderService uses Node)");
        Console.WriteLine("  ❌ Complexity (distributed transactions, eventual consistency)");
        Console.WriteLine("  ❌ Operational overhead (many deployments)\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✅ BEST PRACTICES:\n");

        Console.WriteLine("Naming conventions:");
        Console.WriteLine("  ✅ Folders = plural (Features, Modules, Services)");
        Console.WriteLine("  ✅ Classes = singular (Product, Order, User)");
        Console.WriteLine("  ✅ Interfaces = I prefix (IProductRepository)");
        Console.WriteLine("  ✅ Handlers/Services = xxxHandler, xxxService\n");

        Console.WriteLine("Dependency flow:");
        Console.WriteLine("  ✅ Presentation → Application → Domain");
        Console.WriteLine("  ✅ Infrastructure → Domain (must depend DOWN, not UP)");
        Console.WriteLine("  ❌ Never: Domain references Presentation or Infrastructure\n");

        Console.WriteLine("Shared code:");
        Console.WriteLine("  ✅ Truly shared: Shared/ folder (auth, logging, validation)");
        Console.WriteLine("  ✅ Feature-specific: Keep in feature folder");
        Console.WriteLine("  ⚠️ Avoid premature abstraction (use after 3 duplicates)\n");

        Console.WriteLine("Testing:");
        Console.WriteLine("  ✅ Unit tests in Features/xxxx/Tests/");
        Console.WriteLine("  ✅ Integration tests in Features/xxxx/Tests/");
        Console.WriteLine("  ✅ Use feature folder name in test class (ProductFeatureTests)\n");
    }
}
