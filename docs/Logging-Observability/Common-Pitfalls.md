# Common Pitfalls

> Subject: [Logging-Observability](../README.md)

## Common Pitfalls

### ❌ String Interpolation

```csharp
// ❌ BAD: Not structured, always allocates string
_logger.LogInformation($"Processing order {orderId}");

// ✅ GOOD: Structured, efficient
_logger.LogInformation("Processing order {OrderId}", orderId);
```

### ❌ Logging Sensitive Data

```csharp
// ❌ DANGER: Logging password!
_logger.LogDebug("Login attempt: {Username}, {Password}", username, password);

// ✅ SAFE: Don't log sensitive data
_logger.LogDebug("Login attempt: {Username}", username);
```

### ❌ Over-Logging

```csharp
// ❌ BAD: Logs in loop (millions of logs)
foreach (var item in items)
{
    _logger.LogDebug("Processing item {ItemId}", item.Id);
}

// ✅ GOOD: Log summary
_logger.LogInformation("Processing {ItemCount} items", items.Count);
```

### ❌ Wrong Log Level

```csharp
// ❌ BAD: Using Error for normal flow
if (user == null)
{
    _logger.LogError("User not found");  // ❌ Not an error!
    return NotFound();
}

// ✅ GOOD: Use Warning or Information
if (user == null)
{
    _logger.LogWarning("User {UserId} not found", userId);
    return NotFound();
}
```

---


