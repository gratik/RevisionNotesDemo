# Structured Logging

> Subject: [Logging-Observability](../README.md)

## Structured Logging

### String Interpolation vs Templates

```csharp
// ❌ BAD: String interpolation (not structured)
_logger.LogInformation($"Order {orderId} processed in {elapsed}ms");
// Logged as: "Order 123 processed in 45ms" (just a string)

// ✅ GOOD: Template with parameters (structured)
_logger.LogInformation("Order {OrderId} processed in {ElapsedMs}ms", orderId, elapsed);
// Logged as: { Message: "Order...", OrderId: 123, ElapsedMs: 45 }
// Can query: WHERE OrderId = 123 OR WHERE ElapsedMs > 100
```

### Benefits of Structured Logging

**Queryable**: Search by specific field values
```sql
-- Find all logs for specific order
SELECT * FROM Logs WHERE OrderId = 123

-- Find slow operations
SELECT * FROM Logs WHERE ElapsedMs > 1000
```

**Performance**: No string concatenation
```csharp
// ✅ Template compiled once, parameters passed efficiently
_logger.LogInformation("Processing {Count} items", items.Count);
```

---


