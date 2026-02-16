# Best Practices

> Subject: [Message-Architecture](../README.md)

## Best Practices

### Message Design

- ✅ **Idempotent** - Safe to process multiple times
- ✅ **Self-contained** - Include all needed data
- ✅ **Versioned** - Support schema evolution
- ✅ **Small** - Only essential data
- ✅ **Immutable** - Don't modify after creation

### Error Handling

- ✅ **Retry with exponential backoff**
- ✅ **Dead letter queue** for failed messages
- ✅ **Logging** for troubleshooting
- ✅ **Alerts** for DLQ growth
- ✅ **Idempotency** to handle duplicates

### Performance

- ✅ **Batch sending** when possible
- ✅ **Concurrent consumers** for throughput
- ✅ **Message prefetching** (but not too much)
- ✅ **Connection pooling**
- ✅ **Monitor queue depth**

### Reliability

- ✅ **Persistent messages** (survive restart)
- ✅ **Manual acknowledgment** (not auto)
- ✅ **Transaction support** when needed
- ✅ **Message TTL** (time-to-live)
- ✅ **Duplicate detection**

---


