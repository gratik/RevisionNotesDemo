// ==============================================================================
// TABLE DESIGN FUNDAMENTALS
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class TableDesignFundamentals
{
    public static void RunAll()
    {
        Console.WriteLine("\n--- TABLE DESIGN FUNDAMENTALS ---\n");
        ShowColumnTypeGuidance();
        ShowKeyGuidance();
        ShowConstraintGuidance();
    }

    private static void ShowColumnTypeGuidance()
    {
        Console.WriteLine("Column type guidance:");
        Console.WriteLine("- Use the smallest practical type for join and key columns.");
        Console.WriteLine("- Prefer datetime2 over datetime for precision and range.");
        Console.WriteLine("- Prefer decimal(p,s) for money-like values; avoid float for currency.");
        Console.WriteLine("- Size nvarchar columns intentionally; avoid nvarchar(max) by default.\n");

        Console.WriteLine("❌ BAD:");
        Console.WriteLine("  CustomerName nvarchar(max), Created datetime, Price float");
        Console.WriteLine("✅ GOOD:");
        Console.WriteLine("  CustomerName nvarchar(200), CreatedUtc datetime2(3), Price decimal(18,2)\n");
    }

    private static void ShowKeyGuidance()
    {
        Console.WriteLine("Primary/foreign key guidance:");
        Console.WriteLine("- Every table gets a primary key.");
        Console.WriteLine("- Enforce foreign keys unless there is a measured reason not to.");
        Console.WriteLine("- Use unique constraints for natural identifiers (Email, ExternalId).\n");
    }

    private static void ShowConstraintGuidance()
    {
        Console.WriteLine("Constraint guidance:");
        Console.WriteLine("- Add CHECK constraints for domain safety.");
        Console.WriteLine("- Add defaults for system-generated fields.");
        Console.WriteLine("- Keep nullability intentional and explicit.\n");
    }
}

