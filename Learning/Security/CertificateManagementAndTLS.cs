// ============================================================================
// CERTIFICATE MANAGEMENT AND TLS
// ============================================================================
// WHAT IS THIS?
// -------------
// Practices for issuing, rotating, and validating certificates to secure
// transport channels with modern TLS settings.
//
// WHY IT MATTERS
// --------------
// ✅ Protects confidentiality and integrity in transit
// ✅ Prevents outages from certificate expiry
// ✅ Supports mTLS service identity in zero-trust networks
//
// WHEN TO USE
// -----------
// ✅ Any internet-facing or internal service communication
// ✅ Service-to-service traffic requiring strong identity
//
// WHEN NOT TO USE
// ---------------
// ❌ Never disable TLS in production environments
//
// REAL-WORLD EXAMPLE
// ------------------
// API gateway and backend services use automated cert renewal and mTLS with
// short-lived certs issued by internal PKI.
// ============================================================================

namespace RevisionNotesDemo.Security;

public static class CertificateManagementAndTLS
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Certificate Management and TLS                      ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        ShowTlsBaseline();
        ShowCertificateLifecycle();
        ShowMutualTlsPattern();
        ShowMisconfigurations();
    }

    private static void ShowTlsBaseline()
    {
        Console.WriteLine("1) TLS BASELINE");
        Console.WriteLine("- Enforce TLS 1.2+ (prefer TLS 1.3)");
        Console.WriteLine("- Disable weak ciphers and legacy protocols");
        Console.WriteLine("- Use HSTS for browser-facing endpoints\n");
    }

    private static void ShowCertificateLifecycle()
    {
        Console.WriteLine("2) CERTIFICATE LIFECYCLE");

        var daysUntilExpiry = 21;
        Console.WriteLine($"- Current cert expires in {daysUntilExpiry} days");
        Console.WriteLine("- Alert at <= 30 days, auto-rotate before <= 14 days");
        Console.WriteLine("- Track owner and renewal source for every cert\n");
    }

    private static void ShowMutualTlsPattern()
    {
        Console.WriteLine("3) MUTUAL TLS PATTERN");
        Console.WriteLine("- Client cert authenticates calling service identity");
        Console.WriteLine("- Server cert authenticates destination service");
        Console.WriteLine("- Authorize based on SAN/subject mapping\n");
    }

    private static void ShowMisconfigurations()
    {
        Console.WriteLine("4) COMMON MISCONFIGURATIONS");
        Console.WriteLine("- Self-signed certs in production without pinning controls");
        Console.WriteLine("- Skipping cert validation in HTTP clients");
        Console.WriteLine("- Long-lived certs with no rotation policy");
        Console.WriteLine("- Missing revocation/compromise response playbook\n");
    }
}
