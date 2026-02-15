// ==============================================================================
// Time-Series Databases and Optimization
// ==============================================================================
// WHAT IS THIS?
// Time-series databases (TSDB) are optimized for storing metrics with timestamps: CPU usage, stock prices, sensor readings. Data is immutable (append-only) and queried by time ranges and aggregations.
//
// WHY IT MATTERS
// ✅ COMPRESSION: 200MB/day data becomes 20MB (10x reduction) via compression + downsampling | ✅ FAST QUERIES: Aggregate billions of points in seconds | ✅ IMMUTABLE: No UPDATEs, only appends | ✅ PARTITIONING: Automatic time-based partitions | ✅ DOWNSAMPLING: Summarize old data (1-minute → 1-hour)
//
// WHEN TO USE
// ✅ Metrics/monitoring (Prometheus, InfluxDB, Datadog) | ✅ Stock prices, currency rates | ✅ Sensor data from IoT devices | ✅ Application performance monitoring | ✅ Billing and usage tracking
//
// WHEN NOT TO USE
// ❌ Update existing data frequently | ❌ Non-timestamped data | ❌ Complex JOINs between datasets
//
// REAL-WORLD EXAMPLE
// Stock trading: 100M price updates daily (1 per millisecond × 100K symbols). Regular database = 200GB/day. InfluxDB with downsampling = 20GB/day (10x compression). Prices queried by time range + aggregations (high/low/avg).
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Database;

public class TimeSeriesDatabases
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Time-Series Databases and Optimization");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");
        
        Overview();
        CompressionStrategy();
        QueryExamples();
        DownsamplingStrategy();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("TSDB optimizations:");
        Console.WriteLine("  1. IMMUTABLE: No UPDATE, only INSERT → simpler structure");
        Console.WriteLine("  2. TIME PARTITIONING: Daily/monthly tables → delete old data easily");
        Console.WriteLine("  3. COMPRESSION: Detect repeating values, compress\n");
    }

    private static void CompressionStrategy()
    {
        Console.WriteLine("📊 COMPRESSION EXAMPLE:\n");
        
        Console.WriteLine("Raw data (1 day, 100 symbols, 1/ms):");
        Console.WriteLine("  100 × 86,400 samples × 1,000ms = 100M points");
        Console.WriteLine("  Avg point: 8 bytes (timestamp) + 8 bytes (price) = 16 bytes");
        Console.WriteLine("  Total: 100M × 16 = 1.6 billion bytes = 160 MB\n");
        
        Console.WriteLine("❌ SQL Storage (uncompressed):");
        Console.WriteLine("  timestamp DATETIME (8B) + price DECIMAL (16B) + symbol (10B)");
        Console.WriteLine("  Per day: 200 MB × symbols load = bloat\n");
        
        Console.WriteLine("✅ InfluxDB (compressed):");
        Console.WriteLine("  Delta-of-delta compression: Store price difference instead");
        Console.WriteLine("  If price stays $150 for 1000ms: 1 value compressed");
        Console.WriteLine("  Result: 20 MB per day (10x reduction)\n");
        
        Console.WriteLine("✅ Gorilla compression (Facebook):");
        Console.WriteLine("  XOR compression + variable-length encoding");
        Console.WriteLine("  Average: 1.37 bytes per point (16→1.37 = 92% reduction)\n");
    }

    private static void QueryExamples()
    {
        Console.WriteLine("🔍 QUERY EXAMPLES (InfluxQL):\n");
        
        Console.WriteLine("// Get average CPU for last hour");
        Console.WriteLine("SELECT MEAN(cpu_usage) FROM metrics");
        Console.WriteLine("  WHERE time > now() - 1h GROUP BY time(1m)\n");
        
        Console.WriteLine("// Get 95th percentile latency");
        Console.WriteLine("SELECT PERCENTILE(latency, 95) FROM api_calls");
        Console.WriteLine("  WHERE time > now() - 1d\n");
        
        Console.WriteLine("// Get highest price per hour, all symbols");
        Console.WriteLine("SELECT MAX(price) FROM stock_prices");
        Console.WriteLine("  WHERE time > now() - 7d GROUP BY time(1h), symbol\n");
    }

    private static void DownsamplingStrategy()
    {
        Console.WriteLine("⬇️  DOWNSAMPLING STRATEGY:\n");
        
        Console.WriteLine("Level 1 (Raw): 1-minute granularity");
        Console.WriteLine("  Keep 2 days (2,880 points per symbol)\n");
        
        Console.WriteLine("Level 2 (1-hour summary): 1-hour aggregation");
        Console.WriteLine("  Keep 1 year (8,760 points per symbol)");
        Console.WriteLine("  Store: high, low, open, close, volume\n");
        
        Console.WriteLine("Level 3 (1-day summary): 1-day aggregation");
        Console.WriteLine("  Keep 10 years (3,650 points per symbol)");
        Console.WriteLine("  Store: high, low, open, close, volume\n");
        
        Console.WriteLine("Storage savings:");
        Console.WriteLine("  1-min data: 2 days × 1,440 = 2,880 points");
        Console.WriteLine("  1-hour data: 365 days × 24 = 8,760 points");
        Console.WriteLine("  1-day data: 10 years × 365 = 3,650 points");
        Console.WriteLine("  Total → 99.9% reduction vs keeping raw forever\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");
        
        Console.WriteLine("1. BATCH WRITES");
        Console.WriteLine("   Write 1000s of points in batch (not one-by-one)\n");
        
        Console.WriteLine("2. USE TAGS FOR COMMON QUERIES");
        Console.WriteLine("   Tags: indexed, can filter on immediately");
        Console.WriteLine("   Fields: values you aggregate (SUM, AVG, MAX)\n");
        
        Console.WriteLine("3. RETENTION POLICIES");
        Console.WriteLine("   Raw data: 30 days retention");
        Console.WriteLine("   Hourly aggregates: 1 year retention");
        Console.WriteLine("   Daily aggregates: 10 year retention\n");
        
        Console.WriteLine("4. CHOOSE TSDB FOR SCALE");
        Console.WriteLine("   Prometheus: Built-in Kubernetes support");
        Console.WriteLine("   InfluxDB: Easier scaling");
        Console.WriteLine("   TimescaleDB: PostgreSQL extension (SQL familiarity)\n");
    }
}
