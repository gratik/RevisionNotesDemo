// ==============================================================================
// Rate Limiting & Quota Management Patterns
// ==============================================================================
// WHAT IS THIS?
// Rate limiting restricts API request frequency per client to protect resources. Quotas track cumulative usage (daily/monthly). Prevent abuse, protect infrastructure, enable fair access.
//
// WHY IT MATTERS
// ✅ DDoS PROTECTION: Throttle attackers before service impact | ✅ FAIR USAGE: Prevent single client consuming all resources | ✅ COST CONTROL: Pay-per-request, limit overspending | ✅ SLA COMPLIANCE: Guarantee service for legitimate users | ✅ TIERED PRICING: Different limits per subscription tier
//
// WHEN TO USE
// ✅ Public APIs | ✅ Multi-tenant systems | ✅ Resource-constrained services | ✅ Paid API tiers
//
// WHEN NOT TO USE
// ❌ Internal APIs with trusted clients | ❌ No abuse/cost concerns
//
// REAL-WORLD EXAMPLE
// GitHub: Free tier 60 req/min, Pro 5000 req/hour, Enterprise unlimited. Each API call decrements quota. Hit limit? 429 Too Many Requests. Reset at hour boundary. Prevents runaway scripts, protects infrastructure.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.WebAPI;

public class RateLimitingQuotaPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Rate Limiting & Quota Management Patterns");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");
        
        Overview();
        AlgorithmComparison();
        HeaderExamples();
        GitHubExample();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Rate limiting vs Quotas:");
        Console.WriteLine("  Rate limit: 100 requests per minute (rolling)");
        Console.WriteLine("  Quota: 1,000,000 requests per month (fixed bucket)\n");
    }

    private static void AlgorithmComparison()
    {
        Console.WriteLine("⚙️  ALGORITHMS:\n");
        
        Console.WriteLine("1. TOKEN BUCKET (most common)");
        Console.WriteLine("   - Fill bucket: 100 tokens/min");
        Console.WriteLine("   - Each request costs 1 token");
        Console.WriteLine("   - Bucket size: max burst (e.g., 200 tokens)");
        Console.WriteLine("   - Benefit: Allow bursts\n");
        
        Console.WriteLine("2. SLIDING WINDOW");
        Console.WriteLine("   - Last 60 seconds: count requests");
        Console.WriteLine("   - Limit: 100 per minute");
        Console.WriteLine("   - Remove older than 60s");
        Console.WriteLine("   - Benefit: Accurate per-minute window\n");
        
        Console.WriteLine("3. LEAKY BUCKET");
        Console.WriteLine("   - Request flows into bucket");
        Console.WriteLine("   - Bucket leaks at constant rate");
        Console.WriteLine("   - Overflow = rejected");
        Console.WriteLine("   - Benefit: Smooth request flow\n");
    }

    private static void HeaderExamples()
    {
        Console.WriteLine("📨 RESPONSE HEADERS:\n");
        
        Console.WriteLine("X-RateLimit-Limit: 5000");
        Console.WriteLine("X-RateLimit-Remaining: 4999");
        Console.WriteLine("X-RateLimit-Reset: 1645392000  // Unix timestamp\n");
        
        Console.WriteLine("If exceeded:");
        Console.WriteLine("HTTP 429 Too Many Requests");
        Console.WriteLine("Retry-After: 60 // seconds to wait\n");
    }

    private static void GitHubExample()
    {
        Console.WriteLine("🌐 GITHUB API TIERS:\n");
        
        Console.WriteLine(" Tier               Per-Minute  Per-Hour");
        Console.WriteLine("──────────────────────────────────────────");
        Console.WriteLine("Unauthenticated    60          (implied)");
        Console.WriteLine("Free (auth)        0           (archived)");
        Console.WriteLine("Pro User           N/A         5,000");
        Console.WriteLine("Enterprise         N/A         Unlimited\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");
        
        Console.WriteLine("1. DISTRIBUTED RATE LIMITING");
        Console.WriteLine("   Use Redis for shared limit state");
        Console.WriteLine("   Atomic increment across servers\n");
        
        Console.WriteLine("2. CLIENT-SIDE EXPONENTIAL BACKOFF");
        Console.WriteLine("   On 429: wait 1s, 2s, 4s, 8s (exponential)\n");
        
        Console.WriteLine("3. WHITELIST TRUSTED CLIENTS");
        Console.WriteLine("   Skip rate limiting for internal services\n");
        
        Console.WriteLine("4. GRADUAL QUOTA RESET");
        Console.WriteLine("   Don't reset all at midnight (thundering herd)\n");
    }
}
