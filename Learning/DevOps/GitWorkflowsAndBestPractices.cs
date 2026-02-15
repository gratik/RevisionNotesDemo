// ==============================================================================
// Git Workflows & Best Practices
// ==============================================================================
// WHAT IS THIS?
// Git workflows are structured approaches to managing code changes in teams. Workflows like Git Flow, GitHub Flow, and Trunk-Based Development standardize how features are developed, reviewed, tested, and deployed.
//
// WHY IT MATTERS
// ✅ CODE QUALITY: Pull requests enforce review before merging | ✅ AUDITABILITY: Clear history of who changed what when | ✅ COLLABORATION: Multiple developers work on different features simultaneously | ✅ STABILITY: Main branch always deployable | ✅ RELEASE MANAGEMENT: Organized release schedules | ✅ ROLLBACK READY: Easy to revert problematic changes
//
// WHEN TO USE
// ✅ Team-based development (2+ developers) | ✅ Production deployments | ✅ Need clear change tracking | ✅ Multiple release environments
//
// WHEN NOT TO USE
// ❌ Solo projects with simple deployment | ❌ Proof-of-concept work
//
// REAL-WORLD EXAMPLE
// E-commerce platform: Feature/checkout-redesign → review → staging → approved → merge → auto-deploy to prod. Meanwhile feature/payment-gateway in parallel. Main branch stays stable, deployable anytime.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.DevOps;

public class GitWorkflowsAndBestPractices
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Git Workflows & Best Practices");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");

        Overview();
        GitFlowWorkflow();
        GitHubFlowWorkflow();
        TrunkBasedDevelopment();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Git workflows define how code moves from developer → review → test → production");
        Console.WriteLine("Three main approaches:");
        Console.WriteLine("  1. Git Flow: Complex, multiple branches, good for scheduled releases");
        Console.WriteLine("  2. GitHub Flow: Simple, feature branches, good for continuous deployment");
        Console.WriteLine("  3. Trunk-Based: Minimal branches, short-lived, good for DevOps\n");
    }

    private static void GitFlowWorkflow()
    {
        Console.WriteLine("🔀 GIT FLOW (Most Common):\n");

        Console.WriteLine("Branches:");
        Console.WriteLine("  main → Production-ready code only");
        Console.WriteLine("  develop → Integration branch (testing happens here)");
        Console.WriteLine("  feature/* → New features (from develop)");
        Console.WriteLine("  release/* → Release preparation (from develop)");
        Console.WriteLine("  hotfix/* → Production fixes (from main)\n");

        Console.WriteLine("Example flow:");
        Console.WriteLine("  1. git checkout -b feature/user-authentication");
        Console.WriteLine("  2. Make commits");
        Console.WriteLine("  3. Create PR: feature/user-authentication → develop");
        Console.WriteLine("  4. Code review + tests pass");
        Console.WriteLine("  5. Merge to develop");
        Console.WriteLine("  6. Later: Release branch release/1.0 from develop");
        Console.WriteLine("  7. Final release: release/1.0 → main + develop\n");

        Console.WriteLine("Pros: Clear structure, organized releases, roles defined");
        Console.WriteLine("Cons: Complex, many branches, slower to deploy\n");
    }

    private static void GitHubFlowWorkflow()
    {
        Console.WriteLine("⚡ GITHUB FLOW (Simple, Modern):\n");

        Console.WriteLine("Branches:");
        Console.WriteLine("  main → Always production-ready");
        Console.WriteLine("  feature/* → Any feature/bugfix (short-lived)\n");

        Console.WriteLine("Example flow:");
        Console.WriteLine("  1. git checkout -b feature/api-v2");
        Console.WriteLine("  2. Commit changes");
        Console.WriteLine("  3. Push, create PR");
        Console.WriteLine("  4. Automated tests run");
        Console.WriteLine("  5. Code review and approval");
        Console.WriteLine("  6. Merge to main");
        Console.WriteLine("  7. Auto-deploy to production (CD)\n");

        Console.WriteLine("Pros: Simple, fast, enables continuous deployment");
        Console.WriteLine("Cons: Less structure, requires strong CI/CD\n");
    }

    private static void TrunkBasedDevelopment()
    {
        Console.WriteLine("🎯 TRUNK-BASED DEVELOPMENT (Google/Meta):\n");

        Console.WriteLine("Approach:");
        Console.WriteLine("  main branch = trunk");
        Console.WriteLine("  Feature branches < 1 day old");
        Console.WriteLine("  No release branches (use feature flags)\n");

        Console.WriteLine("Example:");
        Console.WriteLine("  Day 1 morning: git checkout -b feature/dark-mode");
        Console.WriteLine("  Day 1 afternoon:");
        Console.WriteLine("    - Code written + tested");
        Console.WriteLine("    - PR reviewed");
        Console.WriteLine("    - Merged to main");
        Console.WriteLine("  - Feature flag disabled in prod (gradual rollout)\n");

        Console.WriteLine("Pros: Minimal merge conflicts, continuous integration, fast");
        Console.WriteLine("Cons: Requires feature flags, good testing culture\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✅ BEST PRACTICES:\n");

        Console.WriteLine("Commit Messages:");
        Console.WriteLine("  Bad:  git commit -m \"update code\"");
        Console.WriteLine("  Good: git commit -m \"feat: Add OAuth2 login endpoint (#456)\"\n");

        Console.WriteLine("PR Guidelines:");
        Console.WriteLine("  ✅ One feature per PR (easier review)");
        Console.WriteLine("  ✅ Link to issue: \"Fixes #123\"");
        Console.WriteLine("  ✅ Pass all automated tests");
        Console.WriteLine("  ✅ Peer review before merge\n");

        Console.WriteLine("Branch Naming:");
        Console.WriteLine("  feature/user-auth → Feature");
        Console.WriteLine("  bugfix/login-crash → Bug fix");
        Console.WriteLine("  hotfix/security-patch → Production fix");
        Console.WriteLine("  refactor/legacy-api → Refactoring\n");

        Console.WriteLine("Merge Strategies:");
        Console.WriteLine("  --ff (fast-forward): Clean history, loses merge context");
        Console.WriteLine("  --no-ff (merge commit): Shows merge happened, more details");
        Console.WriteLine("  --squash: Combine all commits into one (clean history)\n");
    }
}
