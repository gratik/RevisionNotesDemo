// ==============================================================================
// Redis Patterns and Use Cases
// ==============================================================================
// WHAT IS THIS?
// Redis is an in-memory data structure store providing <1ms latency for get/set operations (compared to database 50ms). Supports strings, lists, sets, hashes, sorted sets, streams.
//
// WHY IT MATTERS
// ✅ EXTREME SPEED: In-memory, <1ms latency (50x faster than DB) | ✅ FLEXIBLE TYPES: More than key-value (sorted sets, streams) | ✅ ATOMIC OPERATIONS: Lists/sets operations atomic | ✅ PUBLISH/SUBSCRIBE: Real-time messaging | ✅ PERSISTENCE: Snapshots and AOF options
//
// WHEN TO USE
// ✅ Session store (invalidates in cache, login redirects) | ✅ Leaderboards (ZSET for scores) | ✅ Rate limiting counters | ✅ Job queues | ✅ Cache hot data
//
// WHEN NOT TO USE
// ❌ Primary data storage (no ACID, max ~256GB per instance) | ❌ Complex queries | ❌ Persistent transactions
//
// REAL-WORLD EXAMPLE
// Gaming leaderboard: ZADD leaderboard 1000 player1, ZADD leaderboard 1500 player2. ZREVRANGE leaderboard 0 10 returns top 10 instantly (<1ms) for 1 million players.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Database;

public class RedisPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Redis Patterns and Use Cases");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");
        
        Overview();
        DataStructures();
        PerformanceComparison();
        UseCases();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Redis is NOT a replacement for databases but a complement:");
        Console.WriteLine("  Database (MySQL/PostgreSQL): Persistent, durability, [[ACID, complex queries");
        Console.WriteLine("  Redis: In-memory, blazing fast, simple operations, data loss risk\n");
        Console.WriteLine("Common architecture: Application reads from Redis, misses go to DB and cache\n");
    }

    private static void DataStructures()
    {
        Console.WriteLine("🗂️  DATA STRUCTURES:\n");
        
        Console.WriteLine("1. STRING - Simple key-value");
        Console.WriteLine("   SET counter 100 | GET counter | INCR counter\n");
        
        Console.WriteLine("2. LIST - Ordered collection, supports push/pop");
        Console.WriteLine("   LPUSH queue job1 | LPUSH queue job2 | RPOP queue\n");
        
        Console.WriteLine("3. SET - Unique unordered values");
        Console.WriteLine("   SADD tags nodejs | SADD tags python | SMEMBERS tags\n");
        
        Console.WriteLine("4. HASH - Map of field-value");
        Console.WriteLine("   HSET user:1 name Alice email alice@example.com | HGETALL user:1\n");
        
        Console.WriteLine("5. SORTED SET (ZSET) - Ordered by score");
        Console.WriteLine("   ZADD leaderboard 1000 player1 | ZADD leaderboard 1500 player2");
        Console.WriteLine("   ZREVRANGE leaderboard 0 10  // Top 10 by score\n");
        
        Console.WriteLine("6. STREAM - Log-like append-only");
        Console.WriteLine("   XADD events * user alice action login | Message broker pattern\n");
    }

    private static void PerformanceComparison()
    {
        Console.WriteLine("⚡ PERFORMANCE COMPARISON:\n");
        
        Console.WriteLine("Operation                    Latency         Speed Improvement");
        Console.WriteLine("────────────────────────────────────────────────────────────");
        Console.WriteLine("Database SELECT (MySQL)      50ms            Baseline");
        Console.WriteLine("Redis GET (in-memory)        0.1ms           500x faster");
        Console.WriteLine("SSD disk read                5ms             10x faster");
        Console.WriteLine("Network round-trip           1ms             50x faster\n");
        
        Console.WriteLine("Example: 1 million user sessions");
        Console.WriteLine("  Database: 50,000 requests/sec max");
        Console.WriteLine("  Redis: 100,000+ requests/sec (in-memory advantage)\n");
    }

    private static void UseCases()
    {
        Console.WriteLine("💡 USE CASES:\n");
        
        Console.WriteLine("1. SESSION STORE");
        Console.WriteLine("   User logs in → store session token in Redis → expires in 30 min");
        Console.WriteLine("   Benefits: O(1) lookup, automatic expiration\n");
        
        Console.WriteLine("2. LEADERBOARD");
        Console.WriteLine("   ZADD leaderboard score playerName");
        Console.WriteLine("   ZREVRANGE leaderboard 0 10 WITHSCORES → Top 10\n");
        
        Console.WriteLine("3. RATE LIMITING");
        Console.WriteLine("   INCR rate:user:123 → increment counter");
        Console.WriteLine("   EXPIRE rate:user:123 60 → reset every minute");
        Console.WriteLine("   Check if counter > limit (100 per minute)\n");
        
        Console.WriteLine("4. JOB QUEUE");
        Console.WriteLine("   LPUSH job_queue { job data } → push job");
        Console.WriteLine("   RPOP job_queue → worker fetches\n");
        
        Console.WriteLine("5. CACHE HOT DATA");
        Console.WriteLine("   Popular product details cached in Redis");
        Console.WriteLine("   TTL 1 hour → automatic refresh\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");
        
        Console.WriteLine("1. SET EXPIRATION");
        Console.WriteLine("   EXPIRE key 3600 // or SET key value EX 3600\n");
        
        Console.WriteLine("2. USE NAMESPACING");
        Console.WriteLine("   session:user:123, leaderboard:game:2024\n");
        
        Console.WriteLine("3. PIPELINE COMMANDS");
        Console.WriteLine("   Reduce round trips: pipeline 100 INCR operations\n");
        
        Console.WriteLine("4. MONITOR MEMORY");
        Console.WriteLine("   Redis holds all data in RAM → monitor size");
        Console.WriteLine("   INFO memory → used_memory_human\n");
        
        Console.WriteLine("5. REPLICATION");
        Console.WriteLine("   Master-replica for HA (read scaling)");
        Console.WriteLine("   Sentinel for automatic failover\n");
    }
}
