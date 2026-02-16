// ==============================================================================
// STORED PROCEDURE DESIGN STANDARDS
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class StoredProcedureDesignStandards
{
    public static void RunAll()
    {
        Console.WriteLine("\n--- STORED PROCEDURE DESIGN STANDARDS ---\n");
        ShowStructurePattern();
        ShowErrorHandlingPattern();
        ShowBadAndGoodExamples();
    }

    private static void ShowStructurePattern()
    {
        Console.WriteLine("Procedure structure pattern:");
        Console.WriteLine("- SET NOCOUNT ON");
        Console.WriteLine("- Validate inputs early");
        Console.WriteLine("- Keep deterministic output contract");
        Console.WriteLine("- Keep transaction scope minimal\n");
    }

    private static void ShowErrorHandlingPattern()
    {
        Console.WriteLine("Error handling pattern:");
        Console.WriteLine("- Use TRY/CATCH and THROW to preserve error semantics.");
        Console.WriteLine("- Return typed error codes only when business flow requires it.");
        Console.WriteLine("- Log critical context at app boundary, not ad hoc print/debug output.\n");
    }

    private static void ShowBadAndGoodExamples()
    {
        Console.WriteLine("❌ BAD:");
        Console.WriteLine("  - Accepts unconstrained nvarchar(max) inputs");
        Console.WriteLine("  - No validation, no transaction boundary clarity");
        Console.WriteLine("  - Silent failure and ambiguous output\n");

        Console.WriteLine("✅ GOOD:");
        Console.WriteLine("  - Validates parameters and expected state");
        Console.WriteLine("  - Uses set-based operations with clear output contract");
        Console.WriteLine("  - Emits deterministic success/failure behavior\n");
    }
}

