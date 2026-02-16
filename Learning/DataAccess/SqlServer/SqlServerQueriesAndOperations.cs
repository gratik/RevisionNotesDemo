// ==============================================================================
// SQL SERVER QUERIES, OPERATIONS, PERFORMANCE, AND TROUBLESHOOTING
// ==============================================================================
// WHAT IS THIS?
// SQL Server practical guidance: query patterns, bad-vs-good practices,
// procedures/functions, keys/indexes, custom types, monitoring, and troubleshooting.
//
// WHY IT MATTERS
// ✅ PERFORMANCE: Correct query/index design removes major latency bottlenecks
// ✅ RELIABILITY: Strong key/constraint strategy prevents data corruption
// ✅ OPERATIONS: Monitoring and diagnostics reduce incident resolution time
// ✅ PORTABILITY: Deployment model differences affect features and architecture
//
// WHEN TO USE
// ✅ Building or maintaining production SQL Server-backed systems
// ✅ Performance tuning, incident response, and schema design work
//
// WHEN NOT TO USE
// ❌ As a substitute for workload-specific benchmarking
// ❌ As a replacement for environment-specific runbooks and SLOs
//
// REAL-WORLD EXAMPLE
// Checkout API moved from wildcard SELECT + missing indexes to targeted projections,
// composite indexes, and query-store-driven tuning. p95 latency dropped from 900ms
// to 120ms and deadlocks became rare after lock-ordering and retry fixes.
// ==============================================================================

namespace RevisionNotesDemo.DataAccess.SqlServer;

public static class SqlServerQueriesAndOperations
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  SQL Server Queries and Operations");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝\n");

        ShowQueryPatterns();
        ShowProceduresAndFunctions();
        ShowKeysIndexesAndCustomTypes();
        ShowPerformanceTuningWorkflow();
        ShowMonitoringAndTroubleshooting();
        ShowDeploymentModelDifferences();
        TableDesignFundamentals.RunAll();
        NormalizationAndDenormalizationPatterns.RunAll();
        RelationalDataModelingPatterns.RunAll();
        PartitioningAndDataLifecyclePatterns.RunAll();
        IndexArchitecturePatterns.RunAll();
        StatisticsAndCardinalityPatterns.RunAll();
        ExecutionPlanAnalysisLab.RunAll();
        StoredProcedureDesignStandards.RunAll();
        ConcurrencyConsistencyPatterns.RunAll();
        BulkIngestionPatterns.RunAll();
        OperationalMonitoringRunbook.RunAll();
        SqlSecurityPatterns.RunAll();
        CteTempTableAndTableVariablePatterns.RunAll();
        MergeAndUpsertPatterns.RunAll();
        SqlGraphDataTransferPatterns.RunAll();
        TransactionAndIsolationPatterns.RunAll();
        ShowPracticalChecklist();
    }

    private static void ShowQueryPatterns()
    {
        Console.WriteLine("1) QUERY PATTERNS: BAD VS GOOD\n");

        Console.WriteLine("❌ BAD: SELECT * on hot path");
        Console.WriteLine("   SELECT * FROM Orders WHERE CustomerId = @CustomerId;");
        Console.WriteLine("✅ GOOD: project only required columns");
        Console.WriteLine("   SELECT OrderId, Status, TotalAmount");
        Console.WriteLine("   FROM Orders WHERE CustomerId = @CustomerId;\n");

        Console.WriteLine("❌ BAD: non-SARGable predicate (index cannot be used efficiently)");
        Console.WriteLine("   WHERE YEAR(CreatedUtc) = 2026");
        Console.WriteLine("✅ GOOD: range predicate");
        Console.WriteLine("   WHERE CreatedUtc >= '2026-01-01' AND CreatedUtc < '2027-01-01'\n");

        Console.WriteLine("❌ BAD: scalar function in WHERE on each row");
        Console.WriteLine("   WHERE dbo.NormalizePhone(Phone) = @Phone");
        Console.WriteLine("✅ GOOD: persisted normalized column + index");
        Console.WriteLine("   WHERE PhoneNormalized = @Phone\n");

        Console.WriteLine("✅ GOOD defaults:");
        Console.WriteLine("  - Parameterized queries");
        Console.WriteLine("  - Explicit transaction boundaries");
        Console.WriteLine("  - Short-lived transactions to reduce lock duration\n");
    }

    private static void ShowProceduresAndFunctions()
    {
        Console.WriteLine("2) STORED PROCEDURES AND FUNCTIONS\n");

        Console.WriteLine("Stored procedure (command boundary, validation, and controlled writes):");
        Console.WriteLine("  CREATE PROCEDURE dbo.UpdateOrderStatus");
        Console.WriteLine("    @OrderId bigint, @NewStatus nvarchar(32)");
        Console.WriteLine("  AS");
        Console.WriteLine("  BEGIN");
        Console.WriteLine("    SET NOCOUNT ON;");
        Console.WriteLine("    UPDATE dbo.Orders SET Status = @NewStatus WHERE OrderId = @OrderId;");
        Console.WriteLine("  END;\n");

        Console.WriteLine("Inline table-valued function (optimizer-friendly reusable logic):");
        Console.WriteLine("  CREATE FUNCTION dbo.ActiveOrdersForCustomer(@CustomerId bigint)");
        Console.WriteLine("  RETURNS TABLE");
        Console.WriteLine("  AS RETURN");
        Console.WriteLine("  (SELECT OrderId, TotalAmount FROM dbo.Orders");
        Console.WriteLine("   WHERE CustomerId = @CustomerId AND Status IN ('Pending','Paid'));\n");

        Console.WriteLine("Function guidance:");
        Console.WriteLine("  - Prefer inline TVFs over multi-statement TVFs on hot paths");
        Console.WriteLine("  - Avoid scalar UDFs in WHERE/JOIN for large result sets");
        Console.WriteLine("  - Keep routines deterministic when possible for better plans\n");
    }

    private static void ShowKeysIndexesAndCustomTypes()
    {
        Console.WriteLine("3) KEYS, INDEXES, AND CUSTOM TYPES\n");

        Console.WriteLine("Key strategy:");
        Console.WriteLine("  - Primary key on every table");
        Console.WriteLine("  - Foreign keys for referential integrity");
        Console.WriteLine("  - Unique constraints for natural keys (Email, ExternalId)\n");

        Console.WriteLine("Index strategy:");
        Console.WriteLine("  - Clustered index: stable, narrow key");
        Console.WriteLine("  - Nonclustered indexes for frequent predicates");
        Console.WriteLine("  - Include columns for covering hot queries");
        Console.WriteLine("  - Filtered indexes for sparse predicates\n");

        Console.WriteLine("Custom types (useful and common):");
        Console.WriteLine("  - Alias types (e.g., dbo.EmailAddress)");
        Console.WriteLine("  - User-defined table types (TVPs for bulk-like parameter input)");
        Console.WriteLine("  - CLR UDTs are niche; avoid unless strongly justified\n");

        Console.WriteLine("TVP example:");
        Console.WriteLine("  CREATE TYPE dbo.OrderLineTableType AS TABLE");
        Console.WriteLine("  (ProductId bigint, Quantity int, UnitPrice decimal(18,2));\n");
    }

    private static void ShowPerformanceTuningWorkflow()
    {
        Console.WriteLine("4) PERFORMANCE TUNING WORKFLOW\n");

        Console.WriteLine("Step 1: Identify high-cost queries");
        Console.WriteLine("  - Query Store top resource consumers");
        Console.WriteLine("  - Wait stats (CPU, I/O, locking, memory grants)\n");

        Console.WriteLine("Step 2: Inspect execution plans");
        Console.WriteLine("  - Key lookups, scans, spills, missing indexes");
        Console.WriteLine("  - Parameter sniffing symptoms (plan unstable per value)\n");

        Console.WriteLine("Step 3: Apply targeted changes");
        Console.WriteLine("  - Rewrite predicates for SARGability");
        Console.WriteLine("  - Add/adjust indexes");
        Console.WriteLine("  - Split large transactions");
        Console.WriteLine("  - Evaluate partitioning for very large tables\n");

        Console.WriteLine("Step 4: Re-measure");
        Console.WriteLine("  - p95/p99 query duration");
        Console.WriteLine("  - CPU, logical reads, tempdb usage");
        Console.WriteLine("  - Lock wait and deadlock trend\n");
    }

    private static void ShowMonitoringAndTroubleshooting()
    {
        Console.WriteLine("5) MONITORING AND TROUBLESHOOTING\n");

        Console.WriteLine("Core telemetry:");
        Console.WriteLine("  - Query Store (plan regressions, top queries)");
        Console.WriteLine("  - Extended Events (deadlocks, timeouts, severe errors)");
        Console.WriteLine("  - DMVs: sys.dm_exec_query_stats, sys.dm_db_index_usage_stats");
        Console.WriteLine("  - Backup/restore job outcomes and RPO/RTO checks\n");

        Console.WriteLine("Common incidents and first checks:");
        Console.WriteLine("  - Timeouts: blocked sessions, long-running scans, resource saturation");
        Console.WriteLine("  - Deadlocks: lock order mismatch, over-wide transactions");
        Console.WriteLine("  - Sudden slowdown: plan regression, stale stats, parameter sniffing");
        Console.WriteLine("  - High DTU/vCore: missing indexes, chatty app patterns, hot partitions\n");

        Console.WriteLine("Troubleshooting sequence:");
        Console.WriteLine("  1. Confirm symptom window and affected workload");
        Console.WriteLine("  2. Capture top waits and top queries");
        Console.WriteLine("  3. Compare current vs prior execution plan");
        Console.WriteLine("  4. Apply minimal reversible fix and verify impact\n");
    }

    private static void ShowDeploymentModelDifferences()
    {
        Console.WriteLine("6) DEPLOYMENT MODELS: SQL SERVER VS HOSTED INSTANCE VS AZURE\n");

        Console.WriteLine("A) Standard Microsoft SQL Server (self-managed on-prem or VM):");
        Console.WriteLine("  - Full engine surface area and OS-level control");
        Console.WriteLine("  - You own patching, backups, HA/DR, and maintenance windows");
        Console.WriteLine("  - Best when you need maximum control/customization\n");

        Console.WriteLine("B) Hosted SQL Server instance (IaaS VM hosted by cloud/provider):");
        Console.WriteLine("  - Near-full SQL Server compatibility");
        Console.WriteLine("  - Infrastructure managed by host, SQL operations often still yours");
        Console.WriteLine("  - Good lift-and-shift model with fewer app changes\n");

        Console.WriteLine("C) Azure SQL offerings:");
        Console.WriteLine("  - Azure SQL Managed Instance:");
        Console.WriteLine("    • High SQL Server compatibility with managed service operations");
        Console.WriteLine("    • Fewer OS/instance-level controls than self-managed SQL Server");
        Console.WriteLine("  - Azure SQL Database (single/elastic pools):");
        Console.WriteLine("    • PaaS-first model with strongest managed operations");
        Console.WriteLine("    • More feature restrictions than full instance-level SQL Server");
        Console.WriteLine("    • Design for per-database boundaries and PaaS constraints\n");

        Console.WriteLine("Typical restriction differences to validate during migration:");
        Console.WriteLine("  - SQL Agent jobs and operational automation model");
        Console.WriteLine("  - Cross-database/instance dependencies and linked-server patterns");
        Console.WriteLine("  - CLR/extensibility, filesystem access, and server-level settings");
        Console.WriteLine("  - Backup/restore workflow and point-in-time restore model\n");
    }

    private static void ShowPracticalChecklist()
    {
        Console.WriteLine("7) PRACTICAL CHECKLIST\n");

        Console.WriteLine("Design:");
        Console.WriteLine("  - Model keys and constraints first");
        Console.WriteLine("  - Keep data types consistent across join columns");
        Console.WriteLine("  - Use UTC timestamps for cross-region systems\n");

        Console.WriteLine("Query quality:");
        Console.WriteLine("  - No SELECT * in API hot paths");
        Console.WriteLine("  - No non-SARGable predicates on large tables");
        Console.WriteLine("  - Parameterize all external input\n");

        Console.WriteLine("Operations:");
        Console.WriteLine("  - Baseline top queries and waits");
        Console.WriteLine("  - Track index usage and fragmentation policy");
        Console.WriteLine("  - Test restore runbooks regularly");
        Console.WriteLine("  - Define escalation triggers for p95/p99 regressions\n");
    }
}
