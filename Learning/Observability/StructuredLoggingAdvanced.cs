// ==============================================================================
// Structured Logging - Searchable, Queryable Logs
// ==============================================================================
// WHAT IS THIS?
// Structured logging captures log events as JSON with fields (timestamp, level, userId, requestId, message), enabling powerful search instead of regex parsing unstructured text.
//
// WHY IT MATTERS
// ✅ SEARCHABLE: Find "all errors for userId=123" without grep | ✅ PERFORMANCE: JSON parsing faster than regex | ✅ CONTEXT: Every log includes metadata (correlationId, version) | ✅ AGGREGATION: Sum errors by service, track patterns | ✅ ALERTING: Alert on field values, not text matching
//
// WHEN TO USE
// ✅ Production systems | ✅ Multi-service architectures | ✅ Need troubleshooting | ✅ Compliance/auditing
//
// WHEN NOT TO USE
// ❌ Single-service simple app (text logs OK) | ❌ Logs are write-once, never searched
//
// REAL-WORLD EXAMPLE
// Production issue: Payment processing slow. Query: "filter requests where service=PaymentService AND duration > 5000ms AND timestamp > now()-1h". Returns 500 slow transactions. Immediately identify database bug.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Observability;

public class StructuredLoggingAdvanced
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Structured Logging - Searchable Logs");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");
        
        Overview();
        CompareTechniques();
        JSONLogExample();
        ImplementationShowcase();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Unstructured: \"Payment processed for user 123 in 500ms\"");
        Console.WriteLine("  Problem: Must parse text, hard to search\n");
        
        Console.WriteLine("Structured: { \\\"timestamp\\\": \\\"2024-02-15T10:30:00Z\\\", \\\"level\\\": \\\"info\\\", \\\"userId\\\": 123, \\\"service\\\": \\\"payment\\\", \\\"duration_ms\\\": 500 }");
        Console.WriteLine("  Benefit: Query by any field, instant parsing\n");
    }

    private static void CompareTechniques()
    {
        Console.WriteLine("📊 TECHNIQUE COMPARISON:\n");
        
        Console.WriteLine("Unstructured (plain text):");
        Console.WriteLine("  [2024-02-15 10:30:00] Payment for user 123, duration 500ms");
        Console.WriteLine("  Search: grep 'Payment' | grep 'user 123' (regex, slow)\n");
        
        Console.WriteLine("Structured (JSON):");
        Console.WriteLine("  { \\\"service\\\": \\\"payment\\\", \\\"userId\\\": 123, \\\"duration_ms\\\": 500 }");
        Console.WriteLine("  Search: service=payment AND userId=123 (instant)\n");
    }

    private static void JSONLogExample()
    {
        Console.WriteLine("📝 EXAMPLE LOG ENTRY (Serilog JSON):\n");
        
        Console.WriteLine("{");
        Console.WriteLine("  \\\"@timestamp\\\": \\\"2024-02-15T10:30:00.123Z\\\",");
        Console.WriteLine("  \\\"level\\\": \\\"Information\\\",");
        Console.WriteLine("  \\\"userId\\\": 123,");
        Console.WriteLine("  \\\"correlationId\\\": \\\"req-456\\\",");
        Console.WriteLine("  \\\"service\\\": \\\"PaymentService\\\",");
        Console.WriteLine("  \\\"operation\\\": \\\"ProcessPayment\\\",");
        Console.WriteLine("  \\\"duration_ms\\\": 500,");
        Console.WriteLine("  \\\"status\\\": \\\"success\\\",");
        Console.WriteLine("  \\\"amount\\\": 99.99,");
        Console.WriteLine("  \\\"message\\\": \\\"Payment processed successfully\\\"");
        Console.WriteLine("}\n");
    }

    private static void ImplementationShowcase()
    {
        Console.WriteLine("💻 C# IMPLEMENTATION (Serilog):\n");
        
        Console.WriteLine("// Configure output to JSON, write to file");
        Console.WriteLine("var log = new LoggerConfiguration()");
        Console.WriteLine("  .WriteTo.File(");
        Console.WriteLine("    \\\"logs/app.json\\\",");
        Console.WriteLine("    new JsonFormatter())");
        Console.WriteLine("  .CreateLogger();\n");
        
        Console.WriteLine("// Log with enrichment");
        Console.WriteLine("LogContext.PushProperty(\\\"userId\\\", 123);");
        Console.WriteLine("LogContext.PushProperty(\\\"correlationId\\\", \\\"req-456\\\");");
        Console.WriteLine("log.Information(\\\"Payment processed {@Amount}\\\", new { amount = 99.99 });\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");
        
        Console.WriteLine("1. CORRELATION ID");
        Console.WriteLine("   Same ID through entire request chain");
        Console.WriteLine("   Track: API Gateway → Service-A → Service-B\n");
        
        Console.WriteLine("2. LOG LEVELS");
        Console.WriteLine("   Debug: Variables, function calls");
        Console.WriteLine("   Info: Important events (order created)");
        Console.WriteLine("   Warn: Unexpected but recoverable");
        Console.WriteLine("   Error: Failures requiring attention\n");
        
        Console.WriteLine("3. AVOID LOGGING SECRETS");
        Console.WriteLine("   Never log passwords, API keys, tokens\n");
        
        Console.WriteLine("4. STRUCTURE FIELDS CONSISTENTLY");
        Console.WriteLine("   Always include: timestamp, level, userId, service\n");
    }
}
