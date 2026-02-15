// ==============================================================================
// MongoDB: Document-Oriented Database for Flexible Schemas
// ==============================================================================
// WHAT IS THIS?
// MongoDB is a document-oriented NoSQL database storing JSON-like documents, offering flexible schemas, horizontal scaling through sharding, and rich query language with aggregation pipelines.
//
// WHY IT MATTERS
// ✅ FLEXIBLE SCHEMA: Add fields without migration (electronics have specs, clothing have colors/sizes) | ✅ NATIVE JSON: Maps directly to C# objects with BSON serialization | ✅ HORIZONTAL SCALING: Shard by key ranges seamlessly | ✅ TRANSACTIONS: ACID transactions in 4.0+ for consistency | ✅ AGGREGATIONS: Complex analytics in database (no data transfer)
//
// WHEN TO USE
// ✅ Rapid schema evolution (startups, MVPs) | ✅ Varied data structures (products with different attributes) | ✅ Nested data structures (orders with embedded line items) | ✅ Large-scale data (sharding built-in) | ✅ Content management systems with flexible content types
//
// WHEN NOT TO USE
// ❌ Requires strict schema with foreign key constraints | ❌ Complex multi-document transactions critical | ❌ Team only experienced with SQL | ❌ Simple tabular data (SQL more efficient) | ❌ Real-time strong consistency required
//
// REAL-WORLD EXAMPLE
// E-commerce platform: Store product documents with flexible attributes (electronics have specs like resolution, clothing have colors/sizes, books have ISBN/pages), add new attributes without migration, query by attributes using aggregation pipelines, scale to millions of products across shards by product ID. AWS Marketplace uses MongoDB for flexible product catalog.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.DataAccess;

public class MongoDBWithDotNet
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  MongoDB: Document-Oriented Database");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝\n");

        Overview();
        DocumentModel();
        QueryPatterns();
        SchemaDesign();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("MongoDB stores data as documents in BSON format (JSON superset)\n");
        Console.WriteLine("SQL vs MongoDB concept mapping:");
        Console.WriteLine("  SQL: Database → Collections → Documents");
        Console.WriteLine("  SQL: Table → Rows → Columns");
        Console.WriteLine("  Equivalent but different philosophy:\n");
        Console.WriteLine("  SQL: Normalized rows with strict schema");
        Console.WriteLine("  MongoDB: Self-contained documents with flexible schema\n");
    }

    private static void DocumentModel()
    {
        Console.WriteLine("📄 DOCUMENT MODEL:\n");

        Console.WriteLine("Example product document (no strict schema):");
        Console.WriteLine("  {");
        Console.WriteLine("    _id: ObjectId(...),");
        Console.WriteLine("    name: \"Samsung TV\",");
        Console.WriteLine("    // Electronics have specs");
        Console.WriteLine("    specs: {");
        Console.WriteLine("      resolution: \"4K\",");
        Console.WriteLine("      refresh_rate: 120");
        Console.WriteLine("    },");
        Console.WriteLine("    price: 999.99");
        Console.WriteLine("  }");
        Console.WriteLine("  {");
        Console.WriteLine("    name: \"Cotton T-Shirt\",");
        Console.WriteLine("    // Clothing has colors/sizes (different fields!)");
        Console.WriteLine("    variants: [");
        Console.WriteLine("      { color: \"red\", sizes: [\"S\", \"M\", \"L\"] },");
        Console.WriteLine("      { color: \"blue\", sizes: [\"S\", \"M\"] }");
        Console.WriteLine("    ],");
        Console.WriteLine("    price: 19.99");
        Console.WriteLine("  }");
        Console.WriteLine("  Same collection, completely different structures!\n");

        Console.WriteLine("✅ ADD NEW FIELDS: New document type can have different fields");
        Console.WriteLine("❌ NO MIGRATION: Existing documents don't need schema change\n");
    }

    private static void QueryPatterns()
    {
        Console.WriteLine("🔍 QUERY PATTERNS:\n");

        Console.WriteLine("Simple find (by property):");
        Console.WriteLine("  db.products.find({ price: { \"$gt\": 100 } })");
        Console.WriteLine("  Find products over $100\n");

        Console.WriteLine("Array queries:");
        Console.WriteLine("  db.products.find({ variants.color: \"red\" })");
        Console.WriteLine("  Find products available in red variant\n");

        Console.WriteLine("Aggregation pipeline (complex analytics):");
        Console.WriteLine("  db.products.aggregate([");
        Console.WriteLine("    { \"$match\": { price: { \"$gt\": 50 } } },");
        Console.WriteLine("    { \"$group\": { _id: \"$category\", avgPrice: { \"$avg\": \"$price\" } } },");
        Console.WriteLine("    { \"$sort\": { avgPrice: -1 } }");
        Console.WriteLine("  ])");
        Console.WriteLine("  Group products by category, calculate average price, sort descending\n");

        Console.WriteLine("Text search:");
        Console.WriteLine("  db.products.find({ \"$text\": { \"$search\": \"black smart tv\" } })");
        Console.WriteLine("  Full-text search across indexed fields\n");
    }

    private static void SchemaDesign()
    {
        Console.WriteLine("🏗️ SCHEMA DESIGN STRATEGIES:\n");

        Console.WriteLine("1. EMBEDDING (Denormalization):");
        Console.WriteLine("  Order with embedded line items:");
        Console.WriteLine("  {");
        Console.WriteLine("    order_id: 123,");
        Console.WriteLine("    items: [");
        Console.WriteLine("      { product_id: 1, qty: 2, price: 10 },");
        Console.WriteLine("      { product_id: 2, qty: 1, price: 20 }");
        Console.WriteLine("    ]");
        Console.WriteLine("  }");
        Console.WriteLine("  Pros: Single document read, atomic updates");
        Console.WriteLine("  Cons: Data duplication, grows large\n");

        Console.WriteLine("2. REFERENCES (Normalization):");
        Console.WriteLine("  Order with product references:");
        Console.WriteLine("  {");
        Console.WriteLine("    order_id: 123,");
        Console.WriteLine("    product_ids: [1, 2, 3]");
        Console.WriteLine("  }");
        Console.WriteLine("  Then query products separately");
        Console.WriteLine("  Pros: Less data duplication");
        Console.WriteLine("  Cons: Multiple queries needed\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✅ BEST PRACTICES:\n");

        Console.WriteLine("1. LEVERAGE SCHEMA FLEXIBILITY:");
        Console.WriteLine("  ✓ Different document types in same collection OK");
        Console.WriteLine("  ✓ Add fields gradually (no downtime)");
        Console.WriteLine("  ✓ Documents can have different depths\n");

        Console.WriteLine("2. BALANCE EMBEDDING VS REFERENCING:");
        Console.WriteLine("  ✓ Embed if accessed together always");
        Console.WriteLine("  ✓ Reference if accessed sometimes");
        Console.WriteLine("  ✓ Limit embedding size (MongoDB doc limit 16MB)\n");

        Console.WriteLine("3. DESIGN FOR SHARDING:");
        Console.WriteLine("  ✓ Choose shard key carefully (user_id, tenant_id)");
        Console.WriteLine("  ✓ Even distribution (high cardinality)");
        Console.WriteLine("  ✓ Cannot change shard key later\n");

        Console.WriteLine("4. INDEXING STRATEGY:");
        Console.WriteLine("  ✓ Index commonly queried fields");
        Console.WriteLine("  ✓ Compound indexes (field1, field2)");
        Console.WriteLine("  ✓ Sparse indexes for optional fields");
        Console.WriteLine("  ❌ Index every field (slows writes)\n");

        Console.WriteLine("5. TRANSACTIONS (4.0+):");
        Console.WriteLine("  ✓ Single document: atomic by default");
        Console.WriteLine("  ✓ Multi-document: use transactions for consistency");
        Console.WriteLine("  ✓ Eventual consistency acceptable: no transaction needed\n");
    }
}
