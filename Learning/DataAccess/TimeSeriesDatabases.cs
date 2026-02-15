// ==============================================================================
// Time-Series Data Access Patterns with ADO.NET & EF Core
// ==============================================================================
// WHAT IS THIS?
// Accessing time-series data efficiently in .NET using ADO.NET, Dapper, and Entity Framework Core. Time-series data (metrics, events with timestamps) has unique access patterns and optimization requirements.
//
// WHY IT MATTERS
// ✅ BILLION-RECORD QUERIES: Efficient time-range queries | ✅ COMPRESSION: Store 10x more data in same space | ✅ PARTITIONING: Query last 7 days (not 10 years) | ✅ AGGREGATION: Group by time buckets (hourly, daily) | ✅ RETENTION: Archive old data efficiently | ✅ PERFORMANCE: <1s queries on 1B+ records
//
// WHEN TO USE
// ✅ Metrics/monitoring (CPU, memory, requests) | ✅ Stock prices, currency rates | ✅ Event logs, audit trails | ✅ IoT sensor data | ✅ Application performance data
//
// WHEN NOT TO USE
// ❌ Transactional data (user accounts, orders) | ❌ Frequently updated records | ❌ Complex JOINs
//
// REAL-WORLD EXAMPLE
// Monitoring system: Log 1M metrics/hour (86B/day). ADO.NET batch inserts 1000-row chunks → 100x faster. Queries: Last hour's data using index + compression. Archive older than 30 days to cold storage.
// ==============================================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace RevisionNotesDemo.DataAccess;

public class TimeSeriesDatabases
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Time-Series Data Access & Optimization");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");

        Overview();
        SchemaOptimization();
        BulkInsertPattern();
        QueryOptimization();
        PartitioningStrategy();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Time-series data characteristics:");
        Console.WriteLine("  • Timestamp-ordered (insert order = time order)");
        Console.WriteLine("  • Immutable (rarely updated)");
        Console.WriteLine("  • Append-only (always new data)");
        Console.WriteLine("  • Huge volume (~1M records/hour)\n");

        Console.WriteLine("Optimization techniques:");
        Console.WriteLine("  1. Clustering index on (TimeStamp) DESC");
        Console.WriteLine("  2. Partitioning by date (daily partition)\n");
    }

    private static void SchemaOptimization()
    {
        Console.WriteLine("🗄️ OPTIMAL SCHEMA FOR TIME-SERIES:\n");

        Console.WriteLine("❌ BAD (normalized, slow):");
        Console.WriteLine("  CREATE TABLE Metrics");
        Console.WriteLine("  {");
        Console.WriteLine("    MetricId BIGINT PRIMARY KEY,");
        Console.WriteLine("    MetricTypeId INT FOREIGN KEY, -- JOIN required");
        Console.WriteLine("    MetricNameId INT FOREIGN KEY, -- JOIN required");
        Console.WriteLine("    Value DECIMAL(10,2),");
        Console.WriteLine("    Timestamp DATETIME");
        Console.WriteLine("  };\n");

        Console.WriteLine("✅ GOOD (denormalized, fast):");
        Console.WriteLine("  CREATE TABLE Metrics");
        Console.WriteLine("  (");
        Console.WriteLine("    Timestamp DATETIME NOT NULL,");
        Console.WriteLine("    MetricType VARCHAR(50) NOT NULL,  // Denormalized (no join)");
        Console.WriteLine("    MetricName VARCHAR(100) NOT NULL,");
        Console.WriteLine("    Host VARCHAR(50) NOT NULL,");
        Console.WriteLine("    Value FLOAT NOT NULL,");
        Console.WriteLine("    CLUSTERED INDEX (Timestamp DESC)");
        Console.WriteLine("  }\n");

        Console.WriteLine("Index strategy:");
        Console.WriteLine("  • Primary index: (Timestamp DESC) - fastest for \"last hour\"");
        Console.WriteLine("  • Secondary: (Host, Timestamp DESC) - fastest for \"host X last week\"\n");
    }

    private static void BulkInsertPattern()
    {
        Console.WriteLine("⚡ BULK INSERT (1000x faster than individual INSERT):\n");

        Console.WriteLine("❌ SLOW (individual inserts):");
        Console.WriteLine("  foreach (var metric in metrics)");
        Console.WriteLine("  {");
        Console.WriteLine("    cmd.CommandText = \"INSERT INTO Metrics ...\";");
        Console.WriteLine("    cmd.ExecuteNonQuery(); // 1M iterations = slow!");
        Console.WriteLine("  }\n");

        Console.WriteLine("✅ FAST (bulk insert):");
        Console.WriteLine("  using (SqlBulkCopy bulk = new SqlBulkCopy(connection))");
        Console.WriteLine("  {");
        Console.WriteLine("    bulk.DestinationTableName = \"Metrics\";");
        Console.WriteLine("    bulk.BatchSize = 1000;");
        Console.WriteLine("    bulk.WriteToServer(dataTable);");
        Console.WriteLine("    // 1M rows inserted in seconds!");
        Console.WriteLine("  }\n");

        Console.WriteLine("Performance: 100-1000x faster than row-by-row inserts\n");
    }

    private static void QueryOptimization()
    {
        Console.WriteLine("🔍 EFFICIENT TIME-RANGE QUERIES:\n");

        Console.WriteLine("Query: Last hour's metrics for host 'server-01'");
        Console.WriteLine("  SELECT Timestamp, MetricType, Value");
        Console.WriteLine("  FROM Metrics");
        Console.WriteLine("  WHERE Timestamp >= DATEADD(HOUR, -1, GETUTCDATE())");
        Console.WriteLine("    AND Host = 'server-01'");
        Console.WriteLine("  ORDER BY Timestamp DESC;\n");

        Console.WriteLine("C# with Dapper:");
        Console.WriteLine("  var metrics = connection.Query<MetricRecord>(");
        Console.WriteLine("    @\"SELECT * FROM Metrics");
        Console.WriteLine("       WHERE Timestamp >= @startTime AND Host = @host");
        Console.WriteLine("       ORDER BY Timestamp DESC\",");
        Console.WriteLine("    new { startTime = DateTime.UtcNow.AddHours(-1), host = \"server-01\" }");
        Console.WriteLine("  );\n");

        Console.WriteLine("Index usage:");
        Console.WriteLine("  • Query uses (Host, Timestamp DESC) index");
        Console.WriteLine("  • Seeks to host='server-01', timestamp >= 1 hour ago");
        Console.WriteLine("  • Returns results without table scan\n");
    }

    private static void PartitioningStrategy()
    {
        Console.WriteLine("📂 PARTITION BY DATE:\n");

        Console.WriteLine("Daily partitions:");
        Console.WriteLine("  Metrics_2026_02_10  (2026-02-10 data)");
        Console.WriteLine("  Metrics_2026_02_11  (2026-02-11 data)");
        Console.WriteLine("  Metrics_2026_02_12  (2026-02-12 data)\n");

        Console.WriteLine("Benefits:");
        Console.WriteLine("  ✅ Query '7-day range' uses only 7 tables (not 10 years)");
        Console.WriteLine("  ✅ Archive old partitions (move Metrics_2025_01_* to cold storage)");
        Console.WriteLine("  ✅ Add new partition daily (programmatically)\n");

        Console.WriteLine("Implementation:");
        Console.WriteLine("  // Archive old data (move to archive table)");
        Console.WriteLine("  INSERT INTO Metrics_Archive");
        Console.WriteLine("  SELECT * FROM Metrics WHERE Timestamp < @30DaysAgo;\\n");
        Console.WriteLine("  DELETE FROM Metrics WHERE Timestamp < @30DaysAgo;\\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✅ BEST PRACTICES:\n");

        Console.WriteLine("Insert patterns:");
        Console.WriteLine("  ✅ Use SqlBulkCopy for batches (1000+ rows)");
        Console.WriteLine("  ✅ Batch size: 1000-5000 rows");
        Console.WriteLine("  ✅ Disable indexes during bulk load, rebuild after\n");

        Console.WriteLine("Index strategy:");
        Console.WriteLine("  ✅ Clustered index: (Timestamp DESC)");
        Console.WriteLine("  ✅ Secondary: (Host, Timestamp DESC)");
        Console.WriteLine("  ✅ Avoid: (MetricType, Timestamp) - time is primary filter\n");

        Console.WriteLine("Data retention:");
        Console.WriteLine("  ✅ Archive data > 30 days");
        Console.WriteLine("  ✅ Delete old partitions");
        Console.WriteLine("  ✅ Compress archived data\n");

        Console.WriteLine("Query patterns:");
        Console.WriteLine("  ✅ Always include timestamp in WHERE (uses index)");
        Console.WriteLine("  ✅ Prefer time ranges over specific times");
        Console.WriteLine("  ✅ Use aggregation (GROUP BY time(1h)) instead of raw points\n");
    }
}
