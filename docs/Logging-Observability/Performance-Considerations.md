# Performance Considerations

> Subject: [Logging-Observability](../README.md)

## Performance Considerations

### Log Guards

```csharp
// ❌ BAD: Expensive operation even if logging disabled
_logger.LogDebug($"User data: {SerializeToJson(user)}");
// SerializeToJson runs even if Debug logging is off!

// ✅ GOOD: Check if enabled first
if (_logger.IsEnabled(LogLevel.Debug))
{
    _logger.LogDebug("User data: {UserJson}", SerializeToJson(user));
}

// ✅ BETTER: Use LoggerMessage for high-performance logging
private static readonly Action<ILogger, string, Exception?> _logUserData =
    LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(1, "UserData"),
        "User data: {UserJson}");

public void LogUserData(string json)
{
    _logUserData(_logger, json, null);
}
```

### High-Performance Logging with LoggerMessage

```csharp
public static class LogMessages
{
    // ✅ Compiled once, reused (zero allocations)
    private static readonly Action<ILogger, int, int, Exception?> _orderProcessed =
        LoggerMessage.Define<int, int>(
            LogLevel.Information,
            new EventId(1001, "OrderProcessed"),
            "Order {OrderId} processed in {ElapsedMs}ms");
    
    public static void LogOrderProcessed(this ILogger logger, int orderId, int elapsedMs)
    {
        _orderProcessed(logger, orderId, elapsedMs, null);
    }
}

// Usage
_logger.LogOrderProcessed(123, 45);
// 10x faster than regular logging in hot paths
```

---


