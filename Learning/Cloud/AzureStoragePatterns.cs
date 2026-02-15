// ==============================================================================
// Azure Storage Patterns & Services
// ==============================================================================
// WHAT IS THIS?
// Azure Storage provides four main services for storing massive amounts of data:
// Blob (files), Queue (messages), Table (NoSQL records), and File (SMB shares),
// all with durability, availability, and security built-in at scale.
//
// WHY IT MATTERS
// ✅ MASSIVE SCALE: Store exabytes of data cost-effectively
// ✅ DURABILITY: 99.999999999% (11 nines) availability guaranteed
// ✅ REDUNDANCY: Automatic geo-replication across regions
// ✅ TIERS: Hot/Cool/Archive tiers for cost optimization
// ✅ SECURITY: Encryption at rest, SAS tokens, Managed Identity
//
// WHEN TO USE
// ✅ User uploads (avatars, documents, videos, images)
// ✅ Backup and archival (comply with retention policies)
// ✅ Asynchronous job queuing (decouple services)
// ✅ CDN origin for static assets (billions of requests)
// ✅ Data lakes for analytics and ML
//
// WHEN NOT TO USE
// ❌ Structured data (use databases instead)
// ❌ Real-time sync (eventual consistency only)
// ❌ Complex queries (not a search engine)
//
// REAL-WORLD EXAMPLE
// Media library: Store millions of video files in Blob Storage, move old
// files to cool/archive tiers (90% cost reduction), serve via Azure CDN for
// <50ms worldwide access, use SAS tokens for temporary access instead of
// storing credentials.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Cloud;

public class AzureStoragePatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Azure Storage Patterns & Services");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");
        
        Overview();
        StorageTypes();
        AccessPatterns();
        CostOptimization();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Azure Storage is a massively scalable, secure cloud storage");
        Console.WriteLine("service for unstructured data. Supports billions of objects");
        Console.WriteLine("with 11 nines durability.\n");
    }

    private static void StorageTypes()
    {
        Console.WriteLine("📦 STORAGE TYPES:\n");
        Console.WriteLine("  • Blob: Large files (videos, backups, logs)");
        Console.WriteLine("  • Queue: Messages for async processing");
        Console.WriteLine("  • Table: NoSQL records (partitioned by key)");
        Console.WriteLine("  • File: SMB shares for legacy applications\n");
    }

    private static void AccessPatterns()
    {
        Console.WriteLine("🔐 ACCESS PATTERNS:\n");
        Console.WriteLine("  • Account Key: Full access (for trusted apps)");
        Console.WriteLine("  • SAS Token: Scoped, time-limited access");
        Console.WriteLine("  • Managed Identity: Zero secrets in code");
        Console.WriteLine("  • Public: Allow anonymous read (CDN origin)\n");
    }

    private static void CostOptimization()
    {
        Console.WriteLine("💰 COST OPTIMIZATION:\n");
        Console.WriteLine("  • Hot: Real-time access (most expensive)");
        Console.WriteLine("  • Cool: Accessed <30 days (50% cheaper)");
        Console.WriteLine("  • Archive: Long-term retention (90% cheaper)");
        Console.WriteLine("  • Tier by Age: Automatically move data to cheaper tiers\n");
    }
}
