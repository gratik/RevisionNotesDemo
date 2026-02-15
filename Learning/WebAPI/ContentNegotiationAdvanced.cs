// ==============================================================================
// Content Negotiation and Response Format Selection
// ==============================================================================
// WHAT IS THIS?
// Content negotiation allows clients to specify desired response format (JSON, XML, Protocol Buffers, MessagePack) via Accept header. Server responds with best match.
//
// WHY IT MATTERS
// ✅ BANDWIDTH EFFICIENCY: Protobuf 20 bytes vs JSON 45 bytes (56% smaller) | ✅ PERFORMANCE: Binary formats faster to parse | ✅ FLEXIBILITY: Different clients need different formats | ✅ MOBILE OPTIMIZATION: Light payloads | ✅ ENTERPRISE: Some systems require XML
//
// WHEN TO USE
// ✅ Mobile APIs (size matters) | ✅ High-throughput APIs | ✅ Supporting legacy systems | ✅ Performance-critical applications
//
// WHEN NOT TO USE
// ❌ Simple internal APIs (JSON default fine) | ❌ Debugging (text formats easier)
//
// REAL-WORLD EXAMPLE
// Netflix API: JSON (browsers), Protobuf (mobile app). Mobile uses Protobuf, saves millions in bandwidth. Browser gets JSON for debugging. Accept header routes to handler.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.WebAPI;

public class ContentNegotiationAdvanced
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Content Negotiation & Response Format Selection");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");

        Overview();
        FormatComparison();
        RequestExample();
        ImplementationPatterns();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Client specifies format via Accept header:");
        Console.WriteLine("  GET /products");
        Console.WriteLine("  Accept: application/json");
        Console.WriteLine("  → Server returns JSON\n");
    }

    private static void FormatComparison()
    {
        Console.WriteLine("📊 FORMAT SIZE COMPARISON:\n");

        var product = new { id = 123, name = "Laptop", price = 999.99, stock = 50 };

        Console.WriteLine("Product: { id: 123, name: \\\"Laptop\\\", price: 999.99, stock: 50 }\\n");

        Console.WriteLine("Format               Size    Ratio vs JSON   Language Support");
        Console.WriteLine("─────────────────────────────────────────────────────────────");
        Console.WriteLine("JSON                 45B     baseline        All");
        Console.WriteLine("XML                  75B     +66%            Mostly");
        Console.WriteLine("MessagePack          35B     -22%            Popular");
        Console.WriteLine("Protobuf             20B     -56%            Generated code");
        Console.WriteLine("CBOR                 28B     -38%            Emerging\n");
    }

    private static void RequestExample()
    {
        Console.WriteLine("📨 REQUEST HEADERS:\n");

        Console.WriteLine("Request 1: JSON");
        Console.WriteLine("  GET /api/products");
        Console.WriteLine("  Accept: application/json\n");

        Console.WriteLine("Request 2: Protobuf (binary)");
        Console.WriteLine("  GET /api/products");
        Console.WriteLine("   Accept: application/protobuf\n");

        Console.WriteLine("Request 3: Multiple (server picks first supported)");
        Console.WriteLine("  GET /api/products");
        Console.WriteLine("  Accept: application/protobuf, application/json;q=0.9\n");
    }

    private static void ImplementationPatterns()
    {
        Console.WriteLine("🔧 IMPLEMENTATION:\n");

        Console.WriteLine("ASP.NET Core:");
        Console.WriteLine("  [ApiController]");
        Console.WriteLine("  public class ProductsController");
        Console.WriteLine("  {");
        Console.WriteLine("    [Produces(\"application/json\", \"application/protobuf\")]");
        Console.WriteLine("    public IActionResult GetProducts()");
        Console.WriteLine("    {");
        Console.WriteLine("      // Framework auto-selects formatter based on Accept");
        Console.WriteLine("    }");
        Console.WriteLine("  }\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");

        Console.WriteLine("1. MOBILE APIs");
        Console.WriteLine("   Default: Protobuf (fast, small)");
        Console.WriteLine("   Fallback: JSON if client doesn't support\n");

        Console.WriteLine("2. CACHING HEADERS");
        Console.WriteLine("   Include format in cache key");
        Console.WriteLine("   Vary: Accept\n");

        Console.WriteLine("3. DOCUMENT SUPPORTED FORMATS");
        Console.WriteLine("   OpenAPI spec lists: application/json, application/protobuf\n");

        Console.WriteLine("4. GZIP STILL APPLIES");
        Console.WriteLine("   Protobuf + gzip = smallest payload\n");
    }
}
