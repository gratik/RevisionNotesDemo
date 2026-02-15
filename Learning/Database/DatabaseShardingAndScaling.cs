// ==============================================================================
// Database Sharding and Horizontal Scaling
// ==============================================================================
// WHAT IS THIS?
// Sharding is partitioning data across multiple database servers where each server holds a subset. Unlike replication (all servers have all data), sharding divides data so no server is a bottleneck.
//
// WHY IT MATTERS
// ✅ HORIZONTAL SCALING: 1000s of servers, exabytes of data | ✅ NO SINGLE BOTTLENECK: Data distributed evenly | ✅ INDEPENDENT FAILURES: One shard failure doesn't affect others | ✅ PARALLEL QUERIES: Each server handles its shard | ✅ COST: Commodity hardware instead of mega-server
//
// WHEN TO USE
// ✅ Data exceeds single server capacity (terabytes+) | ✅ Write throughput exceeds single server (1M writes/sec) | ✅ Geographic distribution required | ✅ Workload is read/write heavy at scale
//
// WHEN NOT TO USE
// ❌ Data fits on one server | ❌ Complex joins across shards | ❌ Transactions spanning shards
//
// REAL-WORLD EXAMPLE
// WhatsApp: 2 billion users, 100+ terabytes. Single database can't handle. Shard by user_id % 100 = shard number. User 1 goes to shard 1, User 101 goes to shard 1, User 2 goes to shard 2. Each shard is independent PostgreSQL server.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Database;

public class DatabaseShardingAndScaling
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Database Sharding and Horizontal Scaling");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");
        
        Overview();
        ShardingStrategy();
        RoutingExample();
        Challenges();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Replication vs Sharding:");
        Console.WriteLine("  Replication: All servers store SAME full data");
        Console.WriteLine("    Pro: HA, read scaling");
        Console.WriteLine("    Con: Write bottleneck, storage 1GB × 10 servers = 10GB\n");
        
        Console.WriteLine("  Sharding: Each server stores DIFFERENT subset");
        Console.WriteLine("    Pro: Horizontal scaling, huge capacity");
        Console.WriteLine("    Con: Complex, risk of hot shards\n");
    }

    private static void ShardingStrategy()
    {
        Console.WriteLine("🔀 SHARDING STRATEGIES:\n");
        
        Console.WriteLine("1. RANGE SHARDING (user_id ranges)");
        Console.WriteLine("   Shard 1: user_id 1-1,000,000");
        Console.WriteLine("   Shard 2: user_id 1,000,001-2,000,000");
        Console.WriteLine("   Risk: Uneven if growth concentrated in new ranges\n");
        
        Console.WriteLine("2. HASH SHARDING (most common)");
        Console.WriteLine("   shard_number = user_id % total_shards");
        Console.WriteLine("   user_id: 1     → 1 % 100 = shard 1");
        Console.WriteLine("   user_id: 101   → 101 % 100 = shard 1");
        Console.WriteLine("   user_id: 2     → 2 % 100 = shard 2");
        Console.WriteLine("   Pro: Even distribution");
        Console.WriteLine("   Con: Re-sharding hard (changing divisor)\n");
        
        Console.WriteLine("3. CONSISTENT HASHING");
        Console.WriteLine("   Use when servers added/removed frequently");
        Console.WriteLine("   Risk: Only affects nearby shards, not all\n");
        
        Console.WriteLine("4. DIRECTORY-BASED");
        Console.WriteLine("   Lookup table: user_id → shard_id");
        Console.WriteLine("   Pro: Total flexibility");
        Console.WriteLine("   Con: Lookup overhead, another system to maintain\n");
    }

    private static void RoutingExample()
    {
        Console.WriteLine("📍 ROUTING EXAMPLE:\n");
        
        Console.WriteLine("Application:");
        Console.WriteLine("  1. Get user_id from request: 123456");
        Console.WriteLine("  2. Calculate shard: shard_id = 123456 % 100 = 56");
        Console.WriteLine("  3. Connect to server[56]: database-56.internal:5432");
        Console.WriteLine("  4. Run query on that shard only\n");
        
        Console.WriteLine("Pseudocode:");
        Console.WriteLine("  function getUser(userId) {");
        Console.WriteLine("    shardId = userId % totalShards;");
        Console.WriteLine("    server = shardServers[shardId];  // db-56.internal");
        Console.WriteLine("    return server.query('SELECT * FROM users WHERE id = ?', userId);");
        Console.WriteLine("  }\n");
    }

    private static void Challenges()
    {
        Console.WriteLine("⚠️  SHARDING CHALLENGES:\n");
        
        Console.WriteLine("1. RE-SHARDING");
        Console.WriteLine("   Adding new shard (100→101): formula changes");
        Console.WriteLine("   All data must move! Requires downtime or complex migration\n");
        
        Console.WriteLine("2. CROSS-SHARD QUERIES");
        Console.WriteLine("   Query: \"Get all users created today\"");
        Console.WriteLine("   Must query all 100 shards, merge results → slower\n");
        
        Console.WriteLine("3. TRANSACTIONS");
        Console.WriteLine("   Update user_1 and user_2 (different shards)?");
        Console.WriteLine("   Two-phase commit complex, often not supported\n");
        
        Console.WriteLine("4. SHARD EXHAUSTION");
        Console.WriteLine("   One shard grows faster → becomes hot spot");
        Console.WriteLine("   Need monitoring and re-distribution strategy\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");
        
        Console.WriteLine("1. CHOOSE SHARD KEY CAREFULLY");
        Console.WriteLine("   High cardinality: user_id (billions of values)");
        Console.WriteLine("   Avoid: month (12 values, uneven distribution)\n");
        
        Console.WriteLine("2. MONITOR SHARD HEALTH");
        Console.WriteLine("   Track data/query distribution");
        Console.WriteLine("   Alert if shard becomes hot\n");
        
        Console.WriteLine("3. PLAN RE-SHARDING");
        Console.WriteLine("   Use directory-based sharding to make re-sharding easier");
        Console.WriteLine("   Or accept downtime window\n");
        
        Console.WriteLine("4. START WITH REPLICATION");
        Console.WriteLine("   Only shard when single server exhausted\n");
    }
}
