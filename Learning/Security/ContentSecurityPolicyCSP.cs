// ============================================================================
// CONTENT SECURITY POLICY (CSP)
// ============================================================================
// WHAT IS THIS?
// -------------
// Browser security header that limits where scripts, styles, frames, and other
// resources can be loaded from.
//
// WHY IT MATTERS
// --------------
// ✅ Reduces impact of XSS and script injection
// ✅ Controls third-party resource execution
// ✅ Adds visibility through violation reports
//
// WHEN TO USE
// -----------
// ✅ Any web application rendering HTML in browsers
//
// WHEN NOT TO USE
// ---------------
// ❌ APIs returning only JSON (header still harmless but less relevant)
//
// REAL-WORLD EXAMPLE
// ------------------
// App starts with report-only mode, then moves to enforced policy with
// nonce-based scripts and restricted frame/src directives.
// ============================================================================

namespace RevisionNotesDemo.Security;

public static class ContentSecurityPolicyCSP
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Content Security Policy (CSP)                       ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        ShowPolicyShape();
        ShowNonceStrategy();
        ShowRolloutMode();
        ShowFrequentErrors();
    }

    private static void ShowPolicyShape()
    {
        Console.WriteLine("1) POLICY SHAPE");
        Console.WriteLine("- default-src 'self'");
        Console.WriteLine("- script-src 'self' 'nonce-<dynamic>'");
        Console.WriteLine("- object-src 'none'");
        Console.WriteLine("- frame-ancestors 'none' (or approved domains)\n");
    }

    private static void ShowNonceStrategy()
    {
        Console.WriteLine("2) NONCE STRATEGY");

        var nonceLength = 24;
        Console.WriteLine($"- Generate per-request nonce (min length: {nonceLength})");
        Console.WriteLine("- Attach nonce only to server-rendered trusted scripts");
        Console.WriteLine("- Do not reuse nonce across responses\n");
    }

    private static void ShowRolloutMode()
    {
        Console.WriteLine("3) ROLLOUT MODE");
        Console.WriteLine("- Start with Content-Security-Policy-Report-Only");
        Console.WriteLine("- Analyze violations and remove unsafe inline usage");
        Console.WriteLine("- Enforce policy after noise is reduced\n");
    }

    private static void ShowFrequentErrors()
    {
        Console.WriteLine("4) FREQUENT ERRORS");
        Console.WriteLine("- Using 'unsafe-inline' permanently");
        Console.WriteLine("- Wildcard domains for script-src in production");
        Console.WriteLine("- No reporting endpoint for policy violations");
        Console.WriteLine("- Forgetting CSP updates when adding legitimate dependencies\n");
    }
}
