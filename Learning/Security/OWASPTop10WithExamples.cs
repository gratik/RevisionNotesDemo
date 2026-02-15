// ============================================================================
// OWASP TOP 10 WITH EXAMPLES
// ============================================================================
// WHAT IS THIS?
// -------------
// Practical mapping of common web security categories to prevention patterns
// used in modern .NET applications.
//
// WHY IT MATTERS
// --------------
// ✅ Provides a threat-modeling baseline for teams
// ✅ Helps prioritize controls by exploit prevalence and impact
// ✅ Connects abstract categories to concrete coding practices
//
// WHEN TO USE
// -----------
// ✅ Secure design reviews and release readiness checks
// ✅ Engineering onboarding for secure coding standards
//
// WHEN NOT TO USE
// ---------------
// ❌ As a substitute for threat modeling or penetration testing
//
// REAL-WORLD EXAMPLE
// ------------------
// API threat review checks auth failures, injection defenses, vulnerable
// dependencies, and logging coverage before production rollout.
// ============================================================================

namespace RevisionNotesDemo.Security;

public static class OWASPTop10WithExamples
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  OWASP Top 10 with .NET Examples                     ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        ShowTopCategories();
        ShowMitigationExamples();
        ShowOperationalControls();
        ShowReviewChecklist();
    }

    private static void ShowTopCategories()
    {
        Console.WriteLine("1) HIGH-IMPACT CATEGORIES");
        Console.WriteLine("- Broken access control");
        Console.WriteLine("- Cryptographic failures");
        Console.WriteLine("- Injection and insecure design");
        Console.WriteLine("- Security misconfiguration and vulnerable components\n");
    }

    private static void ShowMitigationExamples()
    {
        Console.WriteLine("2) MITIGATION EXAMPLES");

        var mitigations = new Dictionary<string, string>
        {
            ["Injection"] = "Parameterized queries and strict input validation",
            ["Broken Auth"] = "MFA, lockout, and robust session controls",
            ["Access Control"] = "Server-side policy checks on every endpoint"
        };

        Console.WriteLine($"- Mitigation mappings: {mitigations.Count}");
        Console.WriteLine($"- Injection control: {mitigations["Injection"]}");
        Console.WriteLine("- Validate controls in tests, not just implementation docs\n");
    }

    private static void ShowOperationalControls()
    {
        Console.WriteLine("3) OPERATIONAL CONTROLS");
        Console.WriteLine("- SCA in CI for vulnerable dependencies");
        Console.WriteLine("- Secure headers and hardening baseline");
        Console.WriteLine("- Tamper-evident audit logs for security actions\n");
    }

    private static void ShowReviewChecklist()
    {
        Console.WriteLine("4) REVIEW CHECKLIST");
        Console.WriteLine("- Are authorization checks centralized and tested?");
        Console.WriteLine("- Are secrets externalized and rotated?");
        Console.WriteLine("- Are error responses non-sensitive?");
        Console.WriteLine("- Are threat model assumptions documented and current?\n");
    }
}
