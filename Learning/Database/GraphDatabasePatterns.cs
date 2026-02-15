// ==============================================================================
// Graph Databases and Relationship Queries
// ==============================================================================
// WHAT IS THIS?
// Graph databases store data as nodes (entities) and edges (relationships). They excel at queries like "find friends of friends" or "product recommendations based on purchase history."
//
// WHY IT MATTERS
// ✅ RELATIONSHIP QUERIES: O(1) hop traversal vs exponential SQL JOINs | ✅ REAL-TIME INSIGHTS: Recommendation engine queries in <100ms | ✅ PATTERN DETECTION: Fraud ring detection, influencer networks | ✅ FLEXIBLE MODELING: Nodes and edges can have arbitrary properties
//
// WHEN TO USE
// ✅ Social networks (friends, followers) | ✅ Recommendations (people who bought X also bought Y) | ✅ Knowledge graphs | ✅ Identity and access management | ✅ Network topology/IT operations
//
// WHEN NOT TO USE
// ❌ Simple CRUD applications | ❌ No relationship depth | ❌ Performance not critical
//
// REAL-WORLD EXAMPLE
// Amazon recommendations: Node = Product, Edge = "purchased_by" and "often_bought_with". Query: Start from iPhone 15, find products bought by same customers → rank by frequency → recommend top 5. Graph queries: instant. SQL: 5 JOINs across orders table.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Database;

public class GraphDatabasePatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Graph Databases and Relationship Queries");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");
        
        Overview();
        NodesAndEdges();
        CypherQueryExamples();
        UseCases();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Traditional SQL with relationships:");
        Console.WriteLine("  SELECT * FROM products p");
        Console.WriteLine("  JOIN orders o ON p.id = o.product_id");
        Console.WriteLine("  JOIN customers c ON o.customer_id = c.id");
        Console.WriteLine("  JOIN customers c2 ON ... (5+ more JOINs for \"friends of friends\")\n");
        
        Console.WriteLine("Graph database (Neo4j Cypher):");
        Console.WriteLine("  MATCH (p:Product)-[:BOUGHT_BY]-(c:Customer)-[:FRIEND]-(friend)");
        Console.WriteLine("  RETURN p, friend  // Single query, automatic path finding\n");
    }

    private static void NodesAndEdges()
    {
        Console.WriteLine("🔗 NODES, EDGES, AND PROPERTIES:\n");
        
        Console.WriteLine("Node example:");
        Console.WriteLine("  :User { username: \"alice\", email: \"alice@example.com\" }\n");
        
        Console.WriteLine("Edge example:");
        Console.WriteLine("  (alice)-[:FRIEND]-(bob)");
        Console.WriteLine("  (alice)-[:FOLLOWS { since: 2023 }]-(influencer)");
        Console.WriteLine("  (product)-[:BOUGHT_BY { quantity: 2, price: 29.99 }]-(customer)\n");
        
        Console.WriteLine("Properties can be on edges (relationships):");
        Console.WriteLine("  Helpful for temporal queries, strength of relationships, etc.\n");
    }

    private static void CypherQueryExamples()
    {
        Console.WriteLine("🔍 CYPHER QUERY EXAMPLES (Neo4j):\n");
        
        Console.WriteLine("// Get friends of alice");
        Console.WriteLine("MATCH (alice:User {username: \"alice\"})-[:FRIEND]-(friend)");
        Console.WriteLine("RETURN friend.username\n");
        
        Console.WriteLine("// Get friends of friends (2 hops)");
        Console.WriteLine("MATCH (alice:User {username: \"alice\"})-[:FRIEND*2]-(fof)");
        Console.WriteLine("RETURN DISTINCT fof.username\n");
        
        Console.WriteLine("// Find products bought by people who know alice");
        Console.WriteLine("MATCH (alice:User {username: \"alice\"})-[:FRIEND]-(friend)");
        Console.WriteLine("         -[:BOUGHT]->(product:Product)");
        Console.WriteLine("RETURN DISTINCT product.name, COUNT(*) as popularity");
        Console.WriteLine("ORDER BY popularity DESC\n");
        
        Console.WriteLine("// Shortest path between two users (degrees of separation)");
        Console.WriteLine("MATCH path = shortestPath((alice:User)-[:FRIEND*]-(bob:User))");
        Console.WriteLine("RETURN length(path) as degrees\n");
    }

    private static void UseCases()
    {
        Console.WriteLine("💡 USE CASES:\n");
        
        Console.WriteLine("1. SOCIAL NETWORKS");
        Console.WriteLine("   Friends, followers, messaging groups → graph structure natural\n");
        
        Console.WriteLine("2. RECOMMENDATIONS");
        Console.WriteLine("   \"People who bought X also bought Y\"");
        Console.WriteLine("   Graph query: fast, real-time\n");
        
        Console.WriteLine("3. FRAUD DETECTION");
        Console.WriteLine("   Find densely connected suspicious accounts");
        Console.WriteLine("   Pattern: many transfers between same group\n");
        
        Console.WriteLine("4. KNOWLEDGE GRAPHS");
        Console.WriteLine("   Google Knowledge Graph: (Donald Trump)-[:PRESIDENT_OF]-(USA)");
        Console.WriteLine("   Query relationships between any entities\n");
        
        Console.WriteLine("5. IT OPERATIONS");
        Console.WriteLine("   Visualize network topology, trace data flow\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");
        
        Console.WriteLine("1. CREATE INDEXES ON FREQUENTLY QUERIED PROPERTIES");
        Console.WriteLine("   CREATE INDEX ON :User(username)\n");
        
        Console.WriteLine("2. USE RELATIONSHIP TYPES MEANINGFULLY");
        Console.WriteLine("   :FRIEND is symmetric (bidirectional)");
        Console.WriteLine("   :FOLLOWS is directional (asymmetric)\n");
        
        Console.WriteLine("3. AVOID DEEP TRAVERSALS");
        Console.WriteLine("   5+ hops can be slow, add limits");
        Console.WriteLine("   MATCH ...-[:REL*1..3]-... (depth 1 to 3)\n");
        
        Console.WriteLine("4. USE ALGORITHMS FOR INSIGHTS");
        Console.WriteLine("   PageRank: importance in network");
        Console.WriteLine("   Community Detection: clustering users\n");
    }
}
