# Bulkhead Pattern

> Subject: [Resilience](../README.md)

## Bulkhead Pattern

```csharp
// ✅ Limit concurrent operations to 10
var bulkheadPolicy = Policy
    .BulkheadAsync(
        maxParallelization: 10,  // Max 10 concurrent
        maxQueuingActions: 5);   // Max 5 queued

// Prevents resource exhaustion
// Request 11-15: Queued
// Request 16+: BulkheadRejectedException
```

---


