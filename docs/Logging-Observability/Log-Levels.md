# Log Levels

> Subject: [Logging-Observability](../README.md)

## Log Levels

| Level | Value | When to Use | Examples |
|-------|-------|-------------|----------|
| **Trace** | 0 | Detailed diagnostic info | Method entry/exit, variable values |
| **Debug** | 1 | Developer debugging | Query parameters, cache hits |
| **Information** | 2 | General flow | Request started, order processed |
| **Warning** | 3 | Unexpected but handled | Retry attempted, fallback used |
| **Error** | 4 | Failure in operation | Exception caught, operation failed |
| **Critical** | 5 | Application crash | Unrecoverable error, data corruption |

### Choosing the Right Level

```csharp
// ✅ Trace: Very detailed (disabled in production)
_logger.LogTrace("Entering GetUser method with userId={UserId}", userId);

// ✅ Debug: Helpful for debugging (disabled in production)
_logger.LogDebug("Cache hit for key {CacheKey}", key);

// ✅ Information: General flow (enabled in production)
_logger.LogInformation("User {UserId} logged in successfully", userId);

// ✅ Warning: Something unexpected but handled
_logger.LogWarning("API rate limit reached, using fallback data");

// ✅ Error: Operation failed
_logger.LogError(ex, "Failed to save order {OrderId}", orderId);

// ✅ Critical: System is unusable
_logger.LogCritical(ex, "Database connection pool exhausted");
```

---


