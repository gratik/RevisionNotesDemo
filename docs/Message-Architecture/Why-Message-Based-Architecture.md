# Why Message-Based Architecture?

> Subject: [Message-Architecture](../README.md)

## Why Message-Based Architecture?

**Traditional Request-Response Problems**:

- Tight coupling between services
- Synchronous = slow (wait for response)
- Failures cascade (service1 down → service2 fails)
- Hard to scale independently
- No retry mechanism

**Message-Based Solutions**:

- ✅ Loose coupling (services don't know about each other)
- ✅ Asynchronous processing (don't wait for response)
- ✅ Fault tolerance (retry failed messages)
- ✅ Independent scaling (scale consumers separately)
- ✅ Load leveling (process at your own pace)

---


