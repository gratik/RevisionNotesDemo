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

## Detailed Guidance

Distributed systems guidance focuses on idempotent workflows, eventual consistency, and replay-safe failure handling.

### Design Notes
- Define success criteria for Best Practices before implementation work begins.
- Keep boundaries explicit so Best Practices decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Best Practices in production-facing code.
- When performance, correctness, or maintainability depends on consistent Best Practices decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Best Practices as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Best Practices is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Best Practices are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

