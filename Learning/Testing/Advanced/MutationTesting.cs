// ==============================================================================
// MUTATION TESTING - Test Quality Assessment and Validation
// ==============================================================================
// WHAT IS THIS?
// -------------
// Mutation testing evaluates test quality by modifying source code (mutations)
// and checking if tests catch those changes. High mutation score means your
// tests are effective at detecting bugs. Low score means tests miss issues.
//
// WHY IT MATTERS
// --------------
// ✅ TEST QUALITY: Code coverage ≠ bug detection. Mutation shows real quality
// ✅ DEFENSIVE: Catch weak tests that pass but don't validate behavior
// ✅ TRUST: 90% mutation score > 100% code coverage with bad tests
// ✅ GAPS: Identify specific logic areas where tests are missing
// ✅ CONFIDENCE: Prove critical code is well-tested
// ✅ REGRESSION: Detect coverage creep from refactoring
//
// WHEN TO USE
// -----------
// ✅ After reaching high code coverage (>80%)
// ✅ On critical business logic
// ✅ For test-driven development validation
// ✅ Before security-sensitive releases
// ✅ To compare test suite quality
//
// WHEN NOT TO USE
// ---------------
// ❌ On new code (stabilize first)
// ❌ As only metric (use with code coverage)
// ❌ For quick feedback loops (slow, use occasionally)
// ❌ On infrastructure/framework code
//
// REAL-WORLD EXAMPLE
// ------------------
// Banking transfer logic:
// - 95% code coverage but tests don't check edge cases
// - Run Stryker mutation testing
// - Finds: Tests pass even if boundary checks removed
// - Issue: Off-by-one error not caught
// - Fix: Add mutation-killing tests for every boundary
// - New score: 98% mutation (now actually safe)
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Testing.Advanced;

public class MutationTesting
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║      MUTATION TESTING - TEST QUALITY ASSESSMENT           ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");
        
        WhatIsMutationTesting();
        StrykerDotNet();
        MutationVsCoverage();
        FindingWeakTests();
        BestPractices();
    }

    private static void WhatIsMutationTesting()
    {
        Console.WriteLine("🧬 MUTATION TESTING EXPLAINED:\n");
        
        Console.WriteLine("MUTATION: Small code modification");
        Console.WriteLine("KILLED: Test catches the mutation (good!)");
        Console.WriteLine("SURVIVED: Test doesn't catch mutation (bad!)\n");
        
        Console.WriteLine("EXAMPLE MUTATIONS:\n");
        
        Console.WriteLine("Original code:");
        Console.WriteLine("   if (price > 100) discount = 0.1m;");
        Console.WriteLine(@"
Mutations:
   ✗ if (price >= 100) discount = 0.1m;   // Change operator
   ✗ if (price > 99) discount = 0.1m;     // Change constant
   ✗ if (price > 100) discount = 0.2m;    // Change value
   ✗ if (price > 100) discount = 0m;      // Remove statement
   ✗ if (true) discount = 0.1m;           // Replace condition
");

        Console.WriteLine("MUTATION SCORE = (Killed Mutations / Total Mutations) × 100%\n");
        
        Console.WriteLine("Interpretation:");
        Console.WriteLine("   90-100%: Excellent test suite");
        Console.WriteLine("   70-89%:  Good, could improve");
        Console.WriteLine("   50-69%:  Fair, many gaps");
        Console.WriteLine("   <50%:    Weak tests, high risk\n");
    }

    private static void StrykerDotNet()
    {
        Console.WriteLine("⚙️  STRYKER.NET (C# Mutation Testing):\n");
        
        Console.WriteLine("Installation:");
        Console.WriteLine("   dotnet tool install -g dotnet-stryker\n");
        
        Console.WriteLine("Run mutation tests:");
        Console.WriteLine("   dotnet stryker\n");
        
        Console.WriteLine("With coverage:");
        Console.WriteLine("   dotnet stryker --since git-source\n");
        
        Console.WriteLine("Report output includes:");
        Console.WriteLine("   • Overall mutation score");
        Console.WriteLine("   • Survived mutations (by file/line)");
        Console.WriteLine("   • HTML dashboard");
        Console.WriteLine("   • JSON results for CI/CD integration\n");
        
        Console.WriteLine("Example stryker.json config:");
        Console.WriteLine(@"
{
  ""mutate"": [
    ""src/**/*.cs"",
    ""!src/**/bin/**/*.cs""
  ],
  ""testRunner"": ""nunit"",
  ""reporters"": [ ""json"", ""html"", ""dots"" ],
  ""threshold"": 80,
  ""timeoutMs"": 5000
}
");
    }

    private static void MutationVsCoverage()
    {
        Console.WriteLine("\n📊 CODE COVERAGE VS MUTATION SCORE:\n");
        
        Console.WriteLine("BAD TEST EXAMPLE:");
        Console.WriteLine(@"
public decimal CalculateDiscount(decimal price)
{
    if (price > 100) return 0.1m;
    return 0m;
}

// Test (99% code coverage):
[Test]
public void TestDiscount()
{
    var result = CalculateDiscount(150);
    Assert.That(result, Is.GreaterThan(0));  // ❌ Very weak!
}

Coverage: 100% (all lines tested)
Mutation: 10% (test killed 1 of 10 mutations)
Why: Test calls method but barely validates behavior
");

        Console.WriteLine("IMPROVED TEST EXAMPLE:");
        Console.WriteLine(@"
// Better test (100% coverage AND high mutation score):
[Test]
public void TestDiscount_Above100_Returns10Percent()
{
    var result = CalculateDiscount(150);
    Assert.That(result, Is.EqualTo(0.1m));  // ✅ Precise!
}

[Test]
public void TestDiscount_ExactlyAt100_Returns10Percent()
{
    var result = CalculateDiscount(100);
    Assert.That(result, Is.EqualTo(0.1m));  // ✅ Tests boundary!
}

[Test]
public void TestDiscount_BelowThreshold_Returns0()
{
    var result = CalculateDiscount(50);
    Assert.That(result, Is.EqualTo(0m));    // ✅ Validates logic!
}

Coverage: 100%(same)
Mutation: 90%(tests kill most mutations)
Confidence: Much higher that discount logic is correct
");
    }

    private static void FindingWeakTests()
    {
        Console.WriteLine("\n🔍 USING MUTATION RESULTS TO IMPROVE TESTS:\n");
        
        Console.WriteLine("REPORT SHOWS:");
        Console.WriteLine("   'Survived: > operator at line 42 changed to >='\n");
        
        Console.WriteLine("ACTION STEPS:");
        Console.WriteLine("   1. Look at line 42");
        Console.WriteLine("   2. Consider: What if > becomes >=?");
        Console.WriteLine("   3. Add test that would break with >= instead of >");
        Console.WriteLine("   4. Re-run mutation testing");
        Console.WriteLine("   5. Verify mutation now killed\n");
        
        Console.WriteLine("COMMON SURVIVED MUTATIONS:");
        Console.WriteLine("   • Boundary off-by-one (add test for boundary)");
        Console.WriteLine("   • Arithmetic operators (test +1 and -1 variants)");
        Console.WriteLine("   • Boolean logic (test all true/false combinations)");
        Console.WriteLine("   • String operations (empty, null, special chars)");
        Console.WriteLine("   • Collection operations (empty, single, many items)\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✅ MUTATION TESTING BEST PRACTICES:\n");
        
        Console.WriteLine("ADOPTION:");
        Console.WriteLine("   • Start on critical business logic only");
        Console.WriteLine("   • Build toward project goal (e.g., 80% target)");
        Console.WriteLine("   • Don't chase perfect score (diminishing returns)");
        Console.WriteLine("   • Use alongside code coverage (30_COVER < 80_MUTATE)\n");
        
        Console.WriteLine("WORKFLOW:");
        Console.WriteLine("   • Run mutation tests on CI/CD for key changes");
        Console.WriteLine("   • Failed mutation = test that needs improvement");
        Console.WriteLine("   • Review report to understand what gaps exist");
        Console.WriteLine("   • Add targeted tests for survived mutations\n");
        
        Console.WriteLine("CONFIGURATION:");
        Console.WriteLine("   • Set reasonable timeout (mutations can hang)");
        Console.WriteLine("   • Focus on src/, exclude test code");
        Console.WriteLine("   • Skip obvious irrelevant files");
        Console.WriteLine("   • Use incremental runs (--since) when possible\n");
        
        Console.WriteLine("INTERPRETATION:");
        Console.WriteLine("   • High mutation ≠ High quality (also check logic)");
        Console.WriteLine("   • Low coverage ≠ Low mutation possible");
        Console.WriteLine("   • Goal: Confidence that tests validate behavior");
        Console.WriteLine("   • Avoid: Gaming the score with trivial tests");
    }
}
