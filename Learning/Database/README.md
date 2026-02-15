# 💾 Database & Data Patterns

**Section 15 - Data Storage, NoSQL, and Database Scaling**

This folder contains patterns for modern data storage, including SQL, NoSQL, time-series, and graph databases.

## 📂 Contents

- **MongoDBWithDotNet.cs** - Document-oriented NoSQL, flexible schema, C# driver examples
- **RedisPatterns.cs** - In-memory caching, data structures, leaderboards, sessions
- **TimeSeriesDatabases.cs** - Compression, downsampling, metrics collection, InfluxDB patterns
- **GraphDatabasePatterns.cs** - Relationship queries, Cypher patterns, recommendations
- **DatabaseShardingAndScaling.cs** - Horizontal scaling, shard distribution, re-sharding challenges
- **ReadReplicasAndCQRS.cs** - Read/write separation, eventual consistency, CQRS pattern

## 🎯 Key Concepts

### What You'll Learn
- Database selection by use case
- Performance optimization techniques
- Scaling databases horizontally
- NoSQL patterns and tradeoffs
- Caching strategies
- Event-driven architectures with data

### Real-World Scenarios
- E-commerce product catalog caching
- Stock price time-series data ingestion
- Social network friend recommendations
- WhatsApp-scale user data sharding

## 💡 Usage

Each file demonstrates:
- Performance comparisons (latency, throughput, storage)
- Code examples with multiple programming approaches
- Production considerations
- Failure scenarios and recovery

Run demonstrations:
```bash
dotnet run
# Select Database patterns from menu
```

## 🔗 Related Sections
- [Microservices](../Microservices/README.md) - Data patterns in distributed systems
- [Observability](../Observability/README.md) - Monitoring database performance

---
_Updated: February 15, 2026_
