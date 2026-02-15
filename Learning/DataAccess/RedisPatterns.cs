// ==============================================================================
// Redis: In-Memory Data Structure Store for Caching and Real-Time
// ==============================================================================
// WHAT IS THIS?
// Redis is an in-memory data structure store supporting strings, hashes, lists, sets, sorted sets, and streams, enabling fast caching, session storage, pub/sub messaging, and real-time analytics with optional persistence.
//
// WHY IT MATTERS
// ✅ SPEED: <1ms latency for most operations (vs 10-50ms for disk) | ✅ VERSATILE: Strings, hashes, lists, sets, sorted sets enable diverse patterns | ✅ ATOMIC: Operations are atomic, safe concurrent access | ✅ PERSISTENCE: Optional AOF/RDB for durability without losing speed | ✅ CLUSTERING: Replication and sharding for high availability and scale
//
// WHEN TO USE
// ✅ Session storage (user login state with TTL) | ✅ Rate limiting and quota tracking | ✅ Real-time leaderboards | ✅ Message queues and job processing | ✅ Cache layer for database query results | ✅ Real-time counters and analytics
//
// WHEN NOT TO USE
// ❌ Complex transactions across keys | ❌ Data too large for RAM | ❌ Data durability critical without AOF (power loss risk) | ❌ Complex joins/queries (SQL more suitable) | ❌ Team needs ACID guarantees
//
// REAL-WORLD EXAMPLE
// Gaming platform: Store player session in Redis with expiration (auto cleanup), maintain leaderboard using sorted set (score + player ID, auto-ranked), use pub/sub for real-time chat messages, atomic operations ensure consistency (INCR for score increment is atomic), if node fails, replicas take over. Handles millions of concurrent players.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.DataAccess;

public class RedisPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Redis: In-Memory Data Structure Store");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝\n");

        Overview();
        DataStructures();
        UsePatterns();
        PerformanceComparison();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Redis stores data in RAM organized by data structure");
        Console.WriteLine("  Key difference from Memcached: Rich data structures, not just strings\n");
        Console.WriteLine("Common comparisons:");
        Console.WriteLine("  Memcached: Simple key-value, fast but limited");
        Console.WriteLine("  Redis: Rich structures, even faster, more flexible");
        Console.WriteLine("  Database: Persistent but slow, complex queries\n");
    }

    private static void DataStructures()
    {
        Console.WriteLine("📚 DATA STRUCTURES:");

        Console.WriteLine("\n1. STRINGS (simplest):");
        Console.WriteLine("  SET user:1:name \"John\"");
        Console.WriteLine("  GET user:1:name → \"John\"");
        Console.WriteLine("  INCR counter → atomic increment\n");

        Console.WriteLine("2. HASHES (object-like):");
        Console.WriteLine("  HSET user:1 name \"John\" age 30 email \"john@...\"");
        Console.WriteLine("  HGET user:1 name → \"John\"");
        Console.WriteLine("  HGETALL user:1 → all fields\n");

        Console.WriteLine("3. LISTS (ordered sequences):");
        Console.WriteLine("  LPUSH messages \"msg1\" \"msg2\" \"msg3\"");
        Console.WriteLine("  LRANGE messages 0 -1 → [\"msg3\", \"msg2\", \"msg1\"]");
        Console.WriteLine("  LPOP messages → \"msg3\" (queue/stack operations)\n");

        Console.WriteLine("4. SETS (unique members):");
        Console.WriteLine("  SADD users:online user1 user2 user3");
        Console.WriteLine("  SMEMBERS users:online → all members");
        Console.WriteLine("  SISMEMBER users:online user1 → true (fast check)\n");

        Console.WriteLine("5. SORTED SETS (ordered by score):");
        Console.WriteLine("  ZADD leaderboard 1000 player1 500 player2 2000 player3");
        Console.WriteLine("  ZRANGE leaderboard 0 -1 WITHSCORES → [player2, 500], [player1, 1000], [player3, 2000]");
        Console.WriteLine("  ZRANK leaderboard player1 → 2 (position in ranking)\n");

        Console.WriteLine("6. STREAMS (append-only):");
        Console.WriteLine("  XADD events * field1 value1 field2 value2");
        Console.WriteLine("  XRANGE events - + → all events");
        Console.WriteLine("  Consumer groups for distributed message processing\n");
    }

    private static void UsePatterns()
    {
        Console.WriteLine("🔧 REAL-WORLD PATTERNS:\n");

        Console.WriteLine("1. SESSION STORAGE:");
        Console.WriteLine("  SET session:{sessionId} {jsonData} EX 3600");
        Console.WriteLine("  Expires after 1 hour, fast lookup");
        Console.WriteLine("  Alternative to database (much faster)\n");

        Console.WriteLine("2. CACHING:");
        Console.WriteLine("  GET cache:{query_key}");
        Console.WriteLine("  If miss: fetch from DB, SET cache:{query_key} {data} EX 300");
        Console.WriteLine("  Typical: Reduce DB queries 90%+\n");

        Console.WriteLine("3. RATE LIMITING:");
        Console.WriteLine("  INCR rate_limit:{user_id}:{hour}");
        Console.WriteLine("  EXPIRE rate_limit:{user_id}:{hour} 3600");
        Console.WriteLine("  Check if count > 1000 per hour, reject if exceeded\n");

        Console.WriteLine("4. LEADERBOARD:");
        Console.WriteLine("  ZADD leaderboard {score} {player_id}");
        Console.WriteLine("  ZREVRANGE leaderboard 0 99 → top 100 players");
        Console.WriteLine("  ZREVRANK leaderboard {player_id} → player's rank\n");

        Console.WriteLine("5. PUB/SUB (messaging):");
        Console.WriteLine("  Channel: \"chat:room:123\"");
        Console.WriteLine("  PUBLISH chat:room:123 {message}");
        Console.WriteLine("  All subscribers receive instantly\n");

        Console.WriteLine("6. JOB QUEUE:");
        Console.WriteLine("  LPUSH queue:tasks {job_data}");
        Console.WriteLine("  Worker: RPOP queue:tasks, process, repeat");
        Console.WriteLine("  Simple FIFO queue, atomic operations\n");
    }

    private static void PerformanceComparison()
    {
        Console.WriteLine("⚡ PERFORMANCE:\n");

        Console.WriteLine("Operation latencies:");
        Console.WriteLine("  Redis GET/SET: <0.5ms");
        Console.WriteLine("  SQL query: 10-50ms");
        Console.WriteLine("  Network RTT: 1ms (local)");
        Console.WriteLine("  Speedup: 20-100x compared to database\n");

        Console.WriteLine("Throughput:");
        Console.WriteLine("  Single Redis server: 100,000+ operations per second");
        Console.WriteLine("  Cluster: millions of ops/sec across shards\n");

        Console.WriteLine("Memory usage:");
        Console.WriteLine("  1GB RAM: Cache millions of small objects");
        Console.WriteLine("  100GB RAM: Cache large datasets");
        Console.WriteLine("  Typically cheaper than adding DB servers\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✅ BEST PRACTICES:\n");

        Console.WriteLine("1. DESIGN KEYS CAREFULLY:");
        Console.WriteLine("  ✓ Namespaced: \"user:{id}:profile\"");
        Console.WriteLine("  ✓ Hierarchical: \"cache:product:{id}:details\"");
        Console.WriteLine("  ✓ Expiration: Always set TTL (EX 3600)\n");

        Console.WriteLine("2. PERSISTENCE:");
        Console.WriteLine("  ✓ AOF (Append-Only File) for safety");
        Console.WriteLine("  ✓ RDB (snapshots) for efficiency");
        Console.WriteLine("  ✓ Cache-like data: no persistence needed");
        Console.WriteLine("  ✓ Important data: enable AOF\n");

        Console.WriteLine("3. EVICTION POLICIES:");
        Console.WriteLine("  ✓ maxmemory-policy: What to do when full?");
        Console.WriteLine("  ✓ allkeys-lru: Evict least recently used");
        Console.WriteLine("  ✓ volatile-ttl: Evict expiring first");
        Console.WriteLine("  ✓ noeviction: Reject writes when full\n");

        Console.WriteLine("4. HIGH AVAILABILITY:");
        Console.WriteLine("  ✓ Replication (master-slave)");
        Console.WriteLine("  ✓ Clustering (sharding across nodes)");
        Console.WriteLine("  ✓ Sentinel (automatic failover)");
        Console.WriteLine("  ✓ Monitor replication lag\n");

        Console.WriteLine("5. MONITORING:");
        Console.WriteLine("  ✓ Monitor memory usage");
        Console.WriteLine("  ✓ Track hit rate (cache effectiveness)");
        Console.WriteLine("  ✓ Watch for slow commands (KEYS *)");
        Console.WriteLine("  ✓ Alert on disconnections\n");
    }
}
