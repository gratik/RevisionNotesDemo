# When to Use Message Queues

> Subject: [Message-Architecture](../README.md)

## When to Use Message Queues

### ✅ **Use When**

1. **Long-running operations**

   ```
   User uploads file → Queue → Background worker processes
   ```

2. **High traffic spikes**

   ```
   Black Friday orders → Queue → Process at sustainable rate
   ```

3. **Decoupling services**

   ```
   OrderService → Queue → EmailService, InventoryService, ShippingService
   ```

4. **Retry logic needed**

   ```
   Payment fails → Retry 3 times → Move to DLQ if still failing
   ```

5. **Multiple consumers**
   ```
   Image upload → Queue → Thumbnail worker, Watermark worker, Analysis worker
   ```

### ❌ **Don't Use When**

1. **Need immediate response**

   ```
   User login → Need instant success/failure response
   ```

2. **Simple CRUD operations**

   ```
   GET /api/users → Just query database directly
   ```

3. **Real-time requirements**

   ```
   Chat messages → Use SignalR/WebSockets instead
   ```

4. **Low complexity, low scale**
   ```
   Small apps with < 100 users → Overkill
   ```

---

## Detailed Guidance

Distributed systems guidance focuses on idempotent workflows, eventual consistency, and replay-safe failure handling.

### Design Notes
- Define success criteria for When to Use Message Queues before implementation work begins.
- Keep boundaries explicit so When to Use Message Queues decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring When to Use Message Queues in production-facing code.
- When performance, correctness, or maintainability depends on consistent When to Use Message Queues decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying When to Use Message Queues as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where When to Use Message Queues is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for When to Use Message Queues are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

