// ==============================================================================
// RELATIONAL DATA MODELING PATTERNS
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class RelationalDataModelingPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n--- RELATIONAL DATA MODELING PATTERNS ---\n");
        ShowSoftDeletePattern();
        ShowAuditAndTemporalPattern();
        ShowMultiTenantPatterns();
    }

    private static void ShowSoftDeletePattern()
    {
        Console.WriteLine("Soft delete pattern:");
        Console.WriteLine("- Use IsDeleted bit + DeletedUtc datetime2.");
        Console.WriteLine("- Pair with filtered index for active rows.");
        Console.WriteLine("  CREATE INDEX IX_Orders_Active ON dbo.Orders(CustomerId) WHERE IsDeleted = 0;\n");
    }

    private static void ShowAuditAndTemporalPattern()
    {
        Console.WriteLine("Audit and temporal pattern:");
        Console.WriteLine("- Track CreatedUtc/CreatedBy and UpdatedUtc/UpdatedBy.");
        Console.WriteLine("- Use temporal tables for point-in-time history when required.");
        Console.WriteLine("- Keep audit writes lightweight on hot paths.\n");
    }

    private static void ShowMultiTenantPatterns()
    {
        Console.WriteLine("Multi-tenant modeling options:");
        Console.WriteLine("- Shared schema + TenantId discriminator (common).");
        Console.WriteLine("- Schema-per-tenant (stronger isolation, higher ops complexity).");
        Console.WriteLine("- Database-per-tenant (strongest isolation, highest cost/ops).\n");
    }
}

