// ==============================================================================
// Database Sharding and Horizontal Scaling
// ==============================================================================
// WHAT IS THIS?
// Database sharding horizontally partitions data by shard key, distributing load across multiple database instances to handle data exceeding single machine capacity and increase throughput.
//
// WHY IT MATTERS
// ✅ CAPACITY: Store exabytes across thousands of machines | ✅ THROUGHPUT: Distribute operations across shards, handle 100K+ ops/sec | ✅ LATENCY: Queries hit single shard, not all data | ✅ INDEPENDENCE: Shard failures affect only that shard | ✅ GROWTH: Add shards as data grows | ✅ COST: Scale cheaply with commodity hardware
//
// WHEN TO USE
// ✅ Data exceeds single machine (>1TB) | ✅ Throughput exceeds single machine (>10K ops/sec) | ✅ Geographic distribution (regional shards) | ✅ User isolation (each tenant on own shard) | ✅ Growing datasets requiring future scaling
//
// WHEN NOT TO USE
// ❌ Data fits comfortably on single machine (<100GB) | ❌ Simple CRUD app with low traffic | ❌ Complex cross-shard joins required | ❌ Team unfamiliar with distributed databases | ❌ Strong ACID across shards critical
//
// REAL-WORLD EXAMPLE
// Facebook: User data sharded by user ID (0-999999 on shard 0, 1000000-1999999 on shard 1), 2+ billion users across 10000+ shards, each shard replicated for HA. When single shard failure occurs, 0.0001% of users affected. Re-sharding happens transparently by splitting shard ranges.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.DataAccess;

public class DatabaseShardingAndScaling
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Database Sharding and Horizontal Scaling");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝\n");

        Overview();
        ShardingStrategies();
        PracticalImplementation();
        ScalingMath();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Sharding horizontally partitions data by shard key\n");
        Console.WriteLine("Without sharding:\n");
        Console.WriteLine("  Database: Users 1-2,000,000,000\n");
        Console.WriteLine("With sharding (10,000 shards):\n");
        Console.WriteLine("  Shard 0: Users 1-200,000");
        Console.WriteLine("  Shard 1: Users 200,001-400,000");
        Console.WriteLine("  Shard N: Users distributed evenly\n");
        Console.WriteLine("Each shard is independent database instance\n");
    }

    private static void ShardingStrategies()
    {
        Console.WriteLine("🎯 SHARDING STRATEGIES:\n");

        Console.WriteLine("1️⃣ RANGE-BASED SHARDING:");
        Console.WriteLine("  Shard by key range (User IDs 1-1M, 1M-2M, etc.)");
        Console.WriteLine("  Pros: Simple, easy re-sharding");
        Console.WriteLine("  Cons: Hot shards if data skewed (all active users on shard 0)\n");

        Console.WriteLine("2️⃣ HASH-BASED SHARDING:");
        Console.WriteLine("  Shard = hash(user_id) % num_shards");
        Console.WriteLine("  Pros: Data distributed evenly, no hot shards");
        Console.WriteLine("  Cons: Re-sharding requires rehashing all data\n");

        Console.WriteLine("3️⃣ DIRECTORY-BASED SHARDING:");
        Console.WriteLine("  Lookup table: User ID → Shard mapping");
        Console.WriteLine("  Pros: Most flexible, enables smart sharding");
        Console.WriteLine("  Cons: Lookup overhead, directory must scale\n");

        Console.WriteLine("4️⃣ GEOGRAPHIC SHARDING:");
        Console.WriteLine("  Shard by region (North America on Shard A, Europe on B)");
        Console.WriteLine("  Pros: Low latency, data residency compliance");
        Console.WriteLine("  Cons: Uneven distribution, cross-shard queries slower\n");
    }

    private static void PracticalImplementation()
    {
        Console.WriteLine("⚙️ PRACTICAL IMPLEMENTATION:\n");

        Console.WriteLine("Code example (hash-based):");
        Console.WriteLine("  int shard_id = hash(user_id) % 100;  // 100 shards");
        Console.WriteLine("  connection_string = GetShardConnection(shard_id);");
        Console.WriteLine("  user = db.Users.Where(u => u.Id == user_id).First();\n");

        Console.WriteLine("Cross-shard query (broadcast):");
        Console.WriteLine("  Find all users created yesterday:");
        Console.WriteLine("  Query all 100 shards in parallel");
        Console.WriteLine("  Merge results from all shards");
        Console.WriteLine("  Total time: ~100-200ms (shard latency)\n");

        Console.WriteLine("Re-sharding (splitting shard):");
        Console.WriteLine("  Old: Hash mod 10 (shards 0-9)");
        Console.WriteLine("  New: Hash mod 20 (shards 0-19)");
        Console.WriteLine("  Migration: Move affected data to new shards");
        Console.WriteLine("  Double-write during transition");
        Console.WriteLine("  Cutover: Redirect traffic to new shards\n");
    }

    private static void ScalingMath()
    {
        Console.WriteLine("📊 SCALING MATHEMATICS:\n");

        Console.WriteLine("Single database baseline:");
        Console.WriteLine("  Storage: 1,000 TB (1 PB)");
        Console.WriteLine("  Throughput: 10,000 ops/sec");
        Console.WriteLine("  Shards needed: 1\n");

        Console.WriteLine("With 100 shards:");
        Console.WriteLine("  Storage per shard: 1,000 TB / 100 = 10 TB (manageable)");
        Console.WriteLine("  Throughput per shard: 10,000 / 100 = 100 ops/sec");
        Console.WriteLine("  Total throughput: 100 * 100 = 10,000 ops/sec ✓\n");

        Console.WriteLine("With 10,000 shards (Facebook scale):");
        Console.WriteLine("  Storage per shard: 1,000 TB / 10,000 = 100 GB (SSD comfortable)");
        Console.WriteLine("  Throughput per shard: 10,000 / 10,000 = 1 op/sec");
        Console.WriteLine("  Total throughput: 1 * 10,000 = 10,000 ops/sec ✓\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✅ BEST PRACTICES:\n");

        Console.WriteLine("1. CHOOSE SHARD KEY CAREFULLY:");
        Console.WriteLine("  ✓ High cardinality (user_id good, status bad)");
        Console.WriteLine("  ✓ Even distribution (immutable hash)");
        Console.WriteLine("  ✓ Supports access patterns (query by shard key)");
        Console.WriteLine("  ❌ Mutable keys (user email can change)\n");

        Console.WriteLine("2. PLAN FOR RESHARDING:");
        Console.WriteLine("  ✓ Use consistent hashing (adds/removes shards smoothly)");
        Console.WriteLine("  ✓ Allocate shard ranges generously (future growth)");
        Console.WriteLine("  ✓ Double-write during transition");
        Console.WriteLine("  ❌ Assume shard count fixed forever\n");

        Console.WriteLine("3. HANDLE CROSS-SHARD OPERATIONS:");
        Console.WriteLine("  ✓ Broadcast to all shards in parallel");
        Console.WriteLine("  ✓ Set reasonable timeouts (slow shards don't block)");
        Console.WriteLine("  ✓ Cache results if data not changing");
        Console.WriteLine("  ❌ Synchronous sequential queries across shards\n");

        Console.WriteLine("4. REPLICATE EACH SHARD:");
        Console.WriteLine("  ✓ Master-slave replication (read scaling)");
        Console.WriteLine("  ✓ Failover to replica if master down");
        Console.WriteLine("  ✓ Geographically distributed for disaster recovery");
        Console.WriteLine("  ❌ Single-shard with no replication\n");
    }
}
