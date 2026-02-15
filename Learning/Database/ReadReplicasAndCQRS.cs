// ==============================================================================
// Read Replicas and CQRS (Command-Query Responsibility Segregation)
// ==============================================================================
// WHAT IS THIS?
// Separate the database into write-optimized (ACID, normalized) and read-optimized (denormalized, fast) models. Reads don't go to the primary write database but to specialized read replicas.
//
// WHY IT MATTERS
// ✅ PERFORMANCE: Denormalized reads 10x faster | ✅ FLEXIBILITY: Read model optimized for specific queries (e.g., pre-counted likes) | ✅ SCALABILITY: 1000 replicas for reads, 1 primary for writes | ✅ EVENTUAL CONSISTENCY: Accept <100ms lag for huge performance gain | ✅ SEPARATION OF CONCERNS: Write logic independent from read logic
//
// WHEN TO USE
// ✅ Read-heavy workloads (100:1 read-to-write ratio) | ✅ Complex queries that benefit from denormalization | ✅ Multiple client types needing different data shapes | ✅ Performance is critical
//
// WHEN NOT TO USE
// ❌ Strong consistency required (ACID) | ❌ Read-write balanced | ❌ Simple CRUD
//
// REAL-WORLD EXAMPLE
// LinkedIn: Write model (normalized) stores profile changes, experience, education separately. Read model (denormalized) has profile with pre-joined experience/education/recommendations. View profile ~10x faster. Updates eventual (appear in 1-2 seconds).
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Database;

public class ReadReplicasAndCQRS
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Read Replicas and CQRS");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");
        
        Overview();
        WriteModel();
        ReadModel();
        DataFlowExample();
        ConsistencyOptions();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Traditional: One database (same schema for writes and reads)");
        Console.WriteLine("CQRS: Two databases");
        Console.WriteLine("  Command:  Optimized for WRITES (normalized, ACID)");
        Console.WriteLine("  Query:    Optimized for READS (denormalized, eventual consistency)\n");
    }

    private static void WriteModel()
    {
        Console.WriteLine("✍️  WRITE MODEL (ACID, Normalized):\n");
        Console.WriteLine("  users table:");
        Console.WriteLine("    id, username, email, profile_picture_url\n");
        
        Console.WriteLine("  user_experience table:");
        Console.WriteLine("    id, user_id, company, title, start_date, end_date\n");
        
        Console.WriteLine("  user_followers table:");
        Console.WriteLine("    follower_id, following_id, created_at\n");
        
        Console.WriteLine("Update user_experience:");
        Console.WriteLine("  INSERT INTO user_experience VALUES (123, \"Google\", \"SWE\", ...)\n");
        
        Console.WriteLine("Advantages:");
        Console.WriteLine("  - Single source of truth");
        Console.WriteLine("  - No data duplication");
        Console.WriteLine("  - ACID consistency\n");
    }

    private static void ReadModel()
    {
        Console.WriteLine("📖 READ MODEL (Denormalized, Fast):\n");
        Console.WriteLine("  user_profile_view table:");
        Console.WriteLine("    id, username, email, profile_picture,");
        Console.WriteLine("    experience (array of structs), follower_count");
        Console.WriteLine("    SINGLE ROW per user with ALL data joined\n");
        
        Console.WriteLine("  Example document:");
        Console.WriteLine("    {");
        Console.WriteLine("      \"id\": 123,");
        Console.WriteLine("      \"username\": \"alice\",");
        Console.WriteLine("      \"follower_count\": 15000,  // Pre-counted!");
        Console.WriteLine("      \"experience\": [");
        Console.WriteLine("        { \"company\": \"Google\", \"title\": \"SWE\" },");
        Console.WriteLine("        { \"company\": \"Meta\", \"title\": \"Senior SWE\" }");
        Console.WriteLine("      ]");
        Console.WriteLine("    }\n");
        
        Console.WriteLine("Advantages:");
        Console.WriteLine("  - Single query to fetch all data");
        Console.WriteLine("  - No JOINs");
        Console.WriteLine("  - Pre-computed values (like): instant\n");
    }

    private static void DataFlowExample()
    {
        Console.WriteLine("🔄 DATA FLOW:\n");
        
        Console.WriteLine("1. User updates profile:");
        Console.WriteLine("   POST /profile → API → WRITE to users table (primary)\n");
        
        Console.WriteLine("2. Write succeeds");
        Console.WriteLine("   User sees success immediately (optimistic update)\n");
        
        Console.WriteLine("3. Event published:");
        Console.WriteLine("   ProfileUpdatedEvent → message queue (RabbitMQ/Kafka)\n");
        
        Console.WriteLine("4. Subscribers process event:");
        Console.WriteLine("   ProfileUpdateProjection reads event");
        Console.WriteLine("   UPDATE user_profile_view SET ... WHERE id = 123\n");
        
        Console.WriteLine("5. Read propagation (~100ms lag):");
        Console.WriteLine("   GET /profile → Read from user_profile_view");
        Console.WriteLine("   Returns latest (or slightly stale) data\n");
    }

    private static void ConsistencyOptions()
    {
        Console.WriteLine("⚙️  CONSISTENCY OPTIONS:\n");
        
        Console.WriteLine("Option 1: EVENTUAL Consistency (default)");
        Console.WriteLine("  Lag: 100-1000ms");
        Console.WriteLine("  Risk: Read stale data briefly");
        Console.WriteLine("  Benefit: Maximum performance\n");
        
        Console.WriteLine("Option 2: Read-After-Write Consistency");
        Console.WriteLine("  After write, query primary for updates");
        Console.WriteLine("  Risk: One extra query to primary");
        Console.WriteLine("  Benefit: Guaranteed see your own writes\n");
        
        Console.WriteLine("Option 3: Session Consistency");
        Console.WriteLine("  Within session, see all previous writes");
        Console.WriteLine("  Risk: Other sessions see eventual");
        Console.WriteLine("  Benefit: Good UX, reasonable consistency\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");
        
        Console.WriteLine("1. PUBLISH EVENTS FROM WRITES");
        Console.WriteLine("   Every write triggers event for read model to consume\n");
        
        Console.WriteLine("2. VERSION READ MODELS");
        Console.WriteLine("   If schema changes, migrate old read models gradually\n");
        
        Console.WriteLine("3. HANDLE FAILURES");
        Console.WriteLine("   If projection fails, queue for retry");
        Console.WriteLine("   Circuit breaker pattern\n");
        
        Console.WriteLine("4. MONITOR LAG");
        Console.WriteLine("   Track write→read propagation delay");
        Console.WriteLine("   Alert if > 5 seconds\n");
        
        Console.WriteLine("5. START SIMPLE");
        Console.WriteLine("   Add CQRS only if read performance is problem\n");
    }
}
