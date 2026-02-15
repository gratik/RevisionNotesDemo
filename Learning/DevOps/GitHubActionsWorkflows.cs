// ==============================================================================
// CI/CD Workflows with GitHub Actions
// ==============================================================================
// WHAT IS THIS?
// GitHub Actions enables automated workflows triggered by repository events: push code → run tests → build container → deploy. CI/CD pipeline defined in YAML, runs on GitHub-hosted runners.
//
// WHY IT MATTERS
// ✅ AUTOMATE TESTING: Tests run on every commit | ✅ CONTINUOUS DEPLOYMENT: Automated deployments to prod | ✅ FAST FEEDBACK: Know if code broken in seconds | ✅ INFRASTRUCTURE AS CODE: Workflows versioned with code | ✅ NO EXTERNAL TOOLS: Built into GitHub, no Jenkins/Jenkins/CircleCI needed
//
// WHEN TO USE
// ✅ GitHub-hosted repositories | ✅ Standard workflows (test, build, deploy) | ✅ Quick setup preferred | ✅ Cost-sensitive (free tiers generous)
//
// WHEN NOT TO USE
// ❌ Complex enterprise workflows (use Jenkins) | ❌ On-premise only (Actions needs GitHub) | ❌ Self-hosted runners required
//
// REAL-WORLD EXAMPLE
// Push C# code → GitHub Actions runs. Builds on .NET 6, 7, 8 (matrix). Runs unit tests. If tests pass, builds Docker image, pushes to registry. Auto-deploys to staging. Manual approval deploys to prod.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.DevOps;

public class GitHubActionsWorkflows
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  CI/CD Workflows with GitHub Actions");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        Overview();
        WorkflowStructure();
        YAMLExample();
        MatrixTesting();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Workflow = automated process");
        Console.WriteLine("Trigger: push, pull_request, schedule, manual");
        Console.WriteLine("Jobs: sequential or parallel tasks");
        Console.WriteLine("Steps: individual actions within job\n");
    }

    private static void WorkflowStructure()
    {
        Console.WriteLine("📋 WORKFLOW ANATOMY:\n");

        Console.WriteLine("name: CI/CD Pipeline");
        Console.WriteLine("on:");
        Console.WriteLine("  push:  # Trigger: code pushed");
        Console.WriteLine("    branches: [main, develop]");
        Console.WriteLine("  pull_request:  # Trigger: PR opened");
        Console.WriteLine("jobs:");
        Console.WriteLine("  test:  # Job 1");
        Console.WriteLine("    runs-on: ubuntu-latest");
        Console.WriteLine("    steps:");
        Console.WriteLine("      - uses: actions/checkout@v3  # Built-in action");
        Console.WriteLine("      - run: dotnet test  # Custom command\n");
    }

    private static void YAMLExample()
    {
        Console.WriteLine("🔧 YAML WORKFLOW EXAMPLE:\n");

        Console.WriteLine("name: .NET Build and Test");
        Console.WriteLine("on: [push, pull_request]");
        Console.WriteLine("jobs:");
        Console.WriteLine("  build:");
        Console.WriteLine("    runs-on: ubuntu-latest");
        Console.WriteLine("    steps:");
        Console.WriteLine("      - uses: actions/checkout@v3");
        Console.WriteLine("      - uses: actions/setup-dotnet@v3");
        Console.WriteLine("        with:");
        Console.WriteLine("          dotnet-version: '7.0.x'");
        Console.WriteLine("      - run: dotnet restore");
        Console.WriteLine("      - run: dotnet build");
        Console.WriteLine("      - run: dotnet test --no-build\n");
    }

    private static void MatrixTesting()
    {
        Console.WriteLine("🔀 MATRIX TESTING (Multiple Versions):\n");

        Console.WriteLine("jobs:");
        Console.WriteLine("  test:");
        Console.WriteLine("    runs-on: ubuntu-latest");
        Console.WriteLine("    strategy:");
        Console.WriteLine("      matrix:");
        Console.WriteLine("        dotnet-version: ['6.0.x', '7.0.x', '8.0.x']");
        Console.WriteLine("    steps:");
        Console.WriteLine("      - uses: actions/setup-dotnet@v3");
        Console.WriteLine("        with:");
        Console.WriteLine("          dotnet-version: ${{ matrix.dotnet-version }}");
        Console.WriteLine("      - run: dotnet test\n");

        Console.WriteLine("Result: 3 separate test jobs (parallel)");
        Console.WriteLine("  Job 1: .NET 6.0");
        Console.WriteLine("  Job 2: .NET 7.0");
        Console.WriteLine("  Job 3: .NET 8.0\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");

        Console.WriteLine("1. CACHE DEPENDENCIES");
        Console.WriteLine("   - uses: actions/cache@v3");
        Console.WriteLine("   - Speeds up repeated runs\n");

        Console.WriteLine("2. SECRETS FOR CREDENTIALS");
        Console.WriteLine("   ${{ secrets.DOCKER_PAT }}\n");

        Console.WriteLine("3. CONDITIONAL STEPS");
        Console.WriteLine("   if: github.event_name == 'push'\n");

        Console.WriteLine("4. TIMEOUT FOR RUNAWAY JOBS");
        Console.WriteLine("   timeout-minutes: 30\n");

        Console.WriteLine("5. NOTIFICATIONS");
        Console.WriteLine("   Slack, email on failure\n");
    }
}
