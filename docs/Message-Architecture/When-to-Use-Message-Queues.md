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


