// ==============================================================================
// MongoDB with .NET Implementation
// ==============================================================================
// WHAT IS THIS?
// MongoDB is a document-oriented NoSQL database where data is stored as JSON-like BSON documents. Unlike SQL tables with fixed schemas, MongoDB collections can store documents with flexible, different structures.
//
// WHY IT MATTERS
// ✅ FLEXIBLE SCHEMA: Add fields to documents without schema migration | ✅ NESTED DOCUMENTS: Embed related data (one document contains address object) | ✅ INDEXING: Fast queries with compound indexes | ✅ HORIZONTAL SCALING: Sharding distributes data across servers | ✅ REPLICATION: Automatic failover with replica sets
//
// WHEN TO USE
// ✅ Content management systems with varying content types | ✅ IoT applications with different sensor types | ✅ Real-time analytics where schema evolves | ✅ Rapid prototyping with changing requirements
//
// WHEN NOT TO USE
// ❌ Strict ACID transactions across many documents (use MongoDB 4.0+ transactions) | ❌ Complex multi-table JOINs | ❌ Guaranteed consistency over performance
//
// REAL-WORLD EXAMPLE
// E-commerce: User document embeds addresses and payment methods. Add loyalty points? Just add field to new documents. Product has variant specifications that vary by type (shoe sizes vs color counts)? No migration needed.
// ==============================================================================

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RevisionNotesDemo.Database;

public class MongoDBWithDotNet
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  MongoDB with .NET Implementation");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");

        Overview();
        DocumentStructure();
        DriverExample();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("MongoDB differs from SQL:\n");
        Console.WriteLine("  SQL:     Tables contain rows with fixed schema");
        Console.WriteLine("  MongoDB: Collections contain documents with flexible schema\n");
        Console.WriteLine("Document example (BSON):");
        Console.WriteLine("  {");
        Console.WriteLine("    \"_id\": ObjectId(...),");
        Console.WriteLine("    \"name\": \"John\",");
        Console.WriteLine("    \"email\": \"john@example.com\",");
        Console.WriteLine("    \"address\": {  // Embedded document");
        Console.WriteLine("      \"street\": \"123 Main\",");
        Console.WriteLine("      \"city\": \"NYC\"");
        Console.WriteLine("    },");
        Console.WriteLine("    \"tags\": [\"vip\", \"premium\"]  // Array field");
        Console.WriteLine("  }\n");
    }

    private static void DocumentStructure()
    {
        Console.WriteLine("📋 DOCUMENT STRUCTURE (Embedded vs. Referenced):\n");

        Console.WriteLine("✅ EMBEDDING (when data accessed together):");
        Console.WriteLine("  User document contains address and payment:");
        Console.WriteLine("    {");
        Console.WriteLine("      \"userId\": 1,");
        Console.WriteLine("      \"name\": \"Alice\",");
        Console.WriteLine("      \"primaryAddress\": {");
        Console.WriteLine("        \"street\": \"456 Oak\",");
        Console.WriteLine("        \"zip\": \"10001\"");
        Console.WriteLine("      },");
        Console.WriteLine("      \"paymentMethods\": [");
        Console.WriteLine("        { \"type\": \"credit\", \"last4\": \"4242\" }");
        Console.WriteLine("      ]");
        Console.WriteLine("    }\n");

        Console.WriteLine("✅ REFERENCING (when data accessed separately):");
        Console.WriteLine("  User references Orders (avoid embedding 1000s of orders):");
        Console.WriteLine("    User: { \"userId\": 1, \"name\": \"Alice\" }");
        Console.WriteLine("    Order: { \"orderId\": 1, \"userId\": 1, \"total\": 99 }\n");
    }

    private static void DriverExample()
    {
        Console.WriteLine("💻 C# DRIVER QUICK EXAMPLES:\n");

        Console.WriteLine("// Connect to MongoDB");
        Console.WriteLine("var client = new MongoClient(\"mongodb://localhost:27017\");");
        Console.WriteLine("var db = client.GetDatabase(\"myapp\");");
        Console.WriteLine("var users = db.GetCollection<User>(\"users\");\n");

        Console.WriteLine("// Insert user");
        Console.WriteLine("var user = new User { Name = \"John\", Email = \"john@example.com\" };");
        Console.WriteLine("await users.InsertOneAsync(user);\n");

        Console.WriteLine("// Find by query");
        Console.WriteLine("var docs = await users");
        Console.WriteLine("  .Find(u => u.Email == \"john@example.com\")");
        Console.WriteLine("  .ToListAsync();\n");

        Console.WriteLine("// Update");
        Console.WriteLine("var filter = Builders<User>.Filter.Eq(u => u.Id, userId);");
        Console.WriteLine("var update = Builders<User>.Update");
        Console.WriteLine("  .Set(u => u.Email, \"newemail@example.com\");");
        Console.WriteLine("await users.UpdateOneAsync(filter, update);\n");

        Console.WriteLine("// Delete");
        Console.WriteLine("await users.DeleteOneAsync(u => u.Id == userId);\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");
        Console.WriteLine("1️⃣  USE INDEXES FOR QUERIES");
        Console.WriteLine("   - Create index: db.users.createIndex({ email: 1 }) // for equality queries");
        Console.WriteLine("   - Create compound: db.users.createIndex({ email: 1, status: 1 })\n");

        Console.WriteLine("2️⃣  DESIGN FOR QUERIES YOU RUN");
        Console.WriteLine("   - If you fetch user + orders together → embed orders");
        Console.WriteLine("   - If you fetch users but not orders → reference (store userId only)\n");

        Console.WriteLine("3️⃣  DOCUMENT SIZE LIMITS");
        Console.WriteLine("   - Max document size: 16 MB");
        Console.WriteLine("   - Avoid massive arrays → use separate collection\n");

        Console.WriteLine("4️⃣  TRANSACTIONS (MongoDB 4.0+)");
        Console.WriteLine("   - Multi-document ACID transactions supported");
        Console.WriteLine("   - Enables consistency with flexibility\n");

        Console.WriteLine("5️⃣  SHARDING FOR SCALE");
        Console.WriteLine("   - Choose shard key by query patterns");
        Console.WriteLine("   - Bad key: \"country\" (uneven distribution)");
        Console.WriteLine("   - Good key: \"userId\" (high cardinality, evenly distributed)\n");
    }

    private class User
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
