// ============================================================================
// AZURE DEVOPS PIPELINES
// ============================================================================
// WHAT IS THIS?
// -------------
// CI/CD workflows for build, test, security scanning, artifact publishing, and
// staged deployments using Azure DevOps YAML pipelines.
//
// WHY IT MATTERS
// --------------
// ✅ Standardizes delivery quality gates across teams
// ✅ Reduces release risk through repeatable automation
// ✅ Improves traceability from commit to production deployment
//
// WHEN TO USE
// -----------
// ✅ Teams using Azure Repos/Boards or Azure-hosted infrastructure
// ✅ Services that need controlled promotion across environments
//
// WHEN NOT TO USE
// ---------------
// ❌ Very small repos where local scripts are sufficient
//
// REAL-WORLD EXAMPLE
// ------------------
// Build on pull request, run unit tests and SAST, publish artifact on main,
// then deploy to dev -> staging -> production with approvals.
// ============================================================================

namespace RevisionNotesDemo.DevOps;

public static class AzureDevOpsPipelines
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Azure DevOps Build and Release Pipelines            ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        ShowPipelineStages();
        ShowQualityGates();
        ShowReleasePromotion();
        ShowPitfalls();
    }

    private static void ShowPipelineStages()
    {
        Console.WriteLine("1) PIPELINE STAGES");
        Console.WriteLine("- Validate: restore, compile, lint, unit tests");
        Console.WriteLine("- Security: dependency and secret scanning");
        Console.WriteLine("- Package: immutable versioned artifact");
        Console.WriteLine("- Deploy: environment-specific rollout strategy\n");
    }

    private static void ShowQualityGates()
    {
        Console.WriteLine("2) QUALITY GATES");

        var gates = new[]
        {
            "Unit tests pass",
            "Coverage >= 80%",
            "No high/critical vulnerabilities",
            "IaC validation successful"
        };

        Console.WriteLine($"- Gate count: {gates.Length}");
        Console.WriteLine($"- First gate: {gates[0]}");
        Console.WriteLine("- Keep gates deterministic and fast to maintain developer trust\n");
    }

    private static void ShowReleasePromotion()
    {
        Console.WriteLine("3) RELEASE PROMOTION");
        Console.WriteLine("- Promote the same artifact across environments (no rebuild)");
        Console.WriteLine("- Require manual approval before production");
        Console.WriteLine("- Use canary or ring-based rollout for high-risk changes");
        Console.WriteLine("- Auto-rollback on health check failures\n");
    }

    private static void ShowPitfalls()
    {
        Console.WriteLine("4) COMMON PITFALLS");
        Console.WriteLine("- Pipeline logic duplicated across many YAML files");
        Console.WriteLine("- Long-running test suites in every PR path");
        Console.WriteLine("- Environment drift because of manual hotfixes");
        Console.WriteLine("- Secrets stored in variables instead of a vault\n");
    }
}
