// ============================================================================
// BIOMETRIC AND MFA PATTERNS
// ============================================================================
// WHAT IS THIS?
// -------------
// Authentication strategies combining knowledge/possession/inherence factors,
// with phishing-resistant methods where possible.
//
// WHY IT MATTERS
// --------------
// ✅ Reduces account takeover risk from stolen passwords
// ✅ Increases assurance for sensitive operations
// ✅ Enables risk-based step-up authentication
//
// WHEN TO USE
// -----------
// ✅ User-facing applications handling sensitive data
// ✅ Admin portals and finance/security critical actions
//
// WHEN NOT TO USE
// ---------------
// ❌ Low-risk internal prototypes without internet exposure
//
// REAL-WORLD EXAMPLE
// ------------------
// Passwordless login using passkeys plus fallback TOTP, with mandatory step-up
// MFA for wire transfer approvals.
// ============================================================================

namespace RevisionNotesDemo.Security;

public static class BiometricAndMFAPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Biometric and MFA Patterns                          ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        ShowFactorTypes();
        ShowRiskBasedPrompting();
        ShowRecoveryControls();
        ShowCommonWeaknesses();
    }

    private static void ShowFactorTypes()
    {
        Console.WriteLine("1) FACTOR TYPES");
        Console.WriteLine("- Knowledge: password or PIN");
        Console.WriteLine("- Possession: authenticator app, FIDO key");
        Console.WriteLine("- Inherence: biometric from trusted device");
        Console.WriteLine("- Prefer phishing-resistant options (passkeys/FIDO2)\n");
    }

    private static void ShowRiskBasedPrompting()
    {
        Console.WriteLine("2) RISK-BASED PROMPTING");

        var events = new[]
        {
            "new_device",
            "impossible_travel",
            "privileged_action"
        };

        Console.WriteLine($"- Step-up triggers: {string.Join(", ", events)}");
        Console.WriteLine("- Challenge only on higher-risk context to reduce friction\n");
    }

    private static void ShowRecoveryControls()
    {
        Console.WriteLine("3) RECOVERY CONTROLS");
        Console.WriteLine("- Issue single-use recovery codes at enrollment");
        Console.WriteLine("- Require identity proofing for factor reset");
        Console.WriteLine("- Add delay + alert on recovery method changes\n");
    }

    private static void ShowCommonWeaknesses()
    {
        Console.WriteLine("4) COMMON WEAKNESSES");
        Console.WriteLine("- SMS-only MFA for high-value accounts");
        Console.WriteLine("- Unlimited OTP retries without lockout");
        Console.WriteLine("- No audit trail for factor enrollment changes");
        Console.WriteLine("- Weak backup/recovery path bypassing MFA assurance\n");
    }
}
