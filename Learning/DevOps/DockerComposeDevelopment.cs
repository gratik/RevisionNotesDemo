// ============================================================================
// DOCKER COMPOSE FOR DEVELOPMENT
// ============================================================================
// WHAT IS THIS?
// -------------
// Declarative local multi-container setup for app dependencies such as
// databases, caches, queues, and supporting tools.
//
// WHY IT MATTERS
// --------------
// ✅ Makes onboarding reproducible across machines
// ✅ Mirrors production-like topology for integration testing
// ✅ Reduces "works on my machine" issues
//
// WHEN TO USE
// -----------
// ✅ Services with 2+ runtime dependencies
// ✅ Teams that need consistent local integration environments
//
// WHEN NOT TO USE
// ---------------
// ❌ Single-process apps with no external dependencies
//
// REAL-WORLD EXAMPLE
// ------------------
// API + PostgreSQL + Redis + message broker spun up with one command for local
// development and smoke tests.
// ============================================================================

namespace RevisionNotesDemo.DevOps;

public static class DockerComposeDevelopment
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Docker Compose Development Environments             ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        ShowServiceDesign();
        ShowProfileStrategy();
        ShowDataAndVolumes();
        ShowCommonMistakes();
    }

    private static void ShowServiceDesign()
    {
        Console.WriteLine("1) SERVICE DESIGN");
        Console.WriteLine("- Keep services minimal: app + required dependencies");
        Console.WriteLine("- Use explicit health checks and startup dependencies");
        Console.WriteLine("- Pin image tags to avoid random local breakages\n");
    }

    private static void ShowProfileStrategy()
    {
        Console.WriteLine("2) PROFILE STRATEGY");

        var profiles = new[] { "default", "observability", "perf-test" };
        Console.WriteLine($"- Available profiles: {string.Join(", ", profiles)}");
        Console.WriteLine("- Keep default profile lightweight for fast startup");
        Console.WriteLine("- Enable heavier stacks only when needed\n");
    }

    private static void ShowDataAndVolumes()
    {
        Console.WriteLine("3) DATA AND VOLUMES");
        Console.WriteLine("- Use named volumes for database persistence in local dev");
        Console.WriteLine("- Mount source code for rapid feedback loops");
        Console.WriteLine("- Reset state with a controlled cleanup command\n");
    }

    private static void ShowCommonMistakes()
    {
        Console.WriteLine("4) COMMON MISTAKES");
        Console.WriteLine("- Hardcoding secrets in compose files");
        Console.WriteLine("- Running all services as root inside containers");
        Console.WriteLine("- Ignoring resource limits and causing laptop instability");
        Console.WriteLine("- Diverging local compose from production assumptions\n");
    }
}
