// ==============================================================================
// CTE VS TEMP TABLE VS TABLE VARIABLE
// ==============================================================================
// WHAT IS THIS?
// Guidance for choosing CTEs, #temp tables, or @table variables in SQL Server.
//
// WHY IT MATTERS
// - Wrong intermediate structure can cause large regressions.
// - Query plan quality depends on cardinality visibility and indexing options.
// - Good choices improve maintainability and runtime predictability.
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class CteTempTableAndTableVariablePatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n--- CTE VS TEMP TABLE VS TABLE VARIABLE ---\n");
        ShowDecisionGuide();
        ShowBadAndGoodPatterns();
        ShowPracticalRules();
    }

    private static void ShowDecisionGuide()
    {
        Console.WriteLine("Decision guide:");
        Console.WriteLine("1. CTE:");
        Console.WriteLine("   - Best for readability and one-pass transformations.");
        Console.WriteLine("   - Not materialized by default; think of it as query rewrite scope.");
        Console.WriteLine("   - If reused many times, optimizer may repeat work.\n");

        Console.WriteLine("2. #Temp Table:");
        Console.WriteLine("   - Best when intermediate result is reused or large.");
        Console.WriteLine("   - Supports indexes/statistics; often better for complex multi-step logic.");
        Console.WriteLine("   - Cost: tempdb I/O and object creation overhead.\n");

        Console.WriteLine("3. @Table Variable:");
        Console.WriteLine("   - Best for small row counts and simple procedural steps.");
        Console.WriteLine("   - Limited statistics behavior compared with #temp tables.");
        Console.WriteLine("   - Risky for large/unknown cardinality joins.\n");
    }

    private static void ShowBadAndGoodPatterns()
    {
        Console.WriteLine("❌ BAD: repeatedly recompute expensive CTE for multiple joins");
        Console.WriteLine("  WITH RecentOrders AS (");
        Console.WriteLine("    SELECT * FROM dbo.Orders WHERE CreatedUtc >= DATEADD(day, -30, SYSUTCDATETIME())");
        Console.WriteLine("  )");
        Console.WriteLine("  SELECT ... FROM RecentOrders r JOIN ...");
        Console.WriteLine("  UNION ALL");
        Console.WriteLine("  SELECT ... FROM RecentOrders r JOIN ...\n");

        Console.WriteLine("✅ GOOD: materialize once into #temp when reused heavily");
        Console.WriteLine("  SELECT OrderId, CustomerId, Status, CreatedUtc");
        Console.WriteLine("  INTO #RecentOrders");
        Console.WriteLine("  FROM dbo.Orders");
        Console.WriteLine("  WHERE CreatedUtc >= DATEADD(day, -30, SYSUTCDATETIME());");
        Console.WriteLine("  CREATE INDEX IX_RecentOrders_CustomerId ON #RecentOrders(CustomerId);");
        Console.WriteLine("  SELECT ... FROM #RecentOrders r JOIN ...;\n");

        Console.WriteLine("❌ BAD: large joins through @table variable on unknown rowcount");
        Console.WriteLine("  DECLARE @Items TABLE (OrderId bigint, ProductId bigint, Qty int);");
        Console.WriteLine("  INSERT @Items ... -- thousands of rows");
        Console.WriteLine("  SELECT ... FROM @Items i JOIN dbo.Products p ON p.ProductId = i.ProductId;\n");

        Console.WriteLine("✅ GOOD: use @table variable for truly small sets");
        Console.WriteLine("  DECLARE @Keys TABLE (OrderId bigint PRIMARY KEY);");
        Console.WriteLine("  INSERT @Keys VALUES (101), (102), (103);");
        Console.WriteLine("  SELECT o.OrderId, o.Status FROM dbo.Orders o JOIN @Keys k ON k.OrderId = o.OrderId;\n");
    }

    private static void ShowPracticalRules()
    {
        Console.WriteLine("Practical rules:");
        Console.WriteLine("- Start with CTE for single-use readable transformations.");
        Console.WriteLine("- Switch to #temp when intermediate rows are reused or large.");
        Console.WriteLine("- Keep @table variables for small, bounded cardinality.");
        Console.WriteLine("- Benchmark using actual execution plans and logical reads.");
        Console.WriteLine("- Watch tempdb if #temp usage grows under concurrency.\n");
    }
}
