// ==============================================================================
// SQL SERVER SECURITY PATTERNS
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class SqlSecurityPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n--- SQL SERVER SECURITY PATTERNS ---\n");
        ShowLeastPrivilegeModel();
        ShowDataProtectionPatterns();
        ShowBadAndGoodSecurityPractices();
    }

    private static void ShowLeastPrivilegeModel()
    {
        Console.WriteLine("Least privilege model:");
        Console.WriteLine("- Separate read/write/admin roles.");
        Console.WriteLine("- Grant execute on required procedures, not broad table access.");
        Console.WriteLine("- Avoid shared high-privilege logins across services.\n");
    }

    private static void ShowDataProtectionPatterns()
    {
        Console.WriteLine("Data protection patterns:");
        Console.WriteLine("- Row-Level Security for tenant/user scoping.");
        Console.WriteLine("- Dynamic Data Masking for low-trust read scenarios.");
        Console.WriteLine("- Always Encrypted for highly sensitive columns.");
        Console.WriteLine("- TLS enforced for data-in-transit.\n");
    }

    private static void ShowBadAndGoodSecurityPractices()
    {
        Console.WriteLine("❌ BAD:");
        Console.WriteLine("- App login is db_owner.");
        Console.WriteLine("- Plaintext secrets in connection strings checked into source.");
        Console.WriteLine("- No audit trail for privileged actions.\n");

        Console.WriteLine("✅ GOOD:");
        Console.WriteLine("- Scoped role per service with least privilege grants.");
        Console.WriteLine("- Secret manager/managed identity for credentials.");
        Console.WriteLine("- Auditable privileged operations and periodic permission reviews.\n");
    }
}

