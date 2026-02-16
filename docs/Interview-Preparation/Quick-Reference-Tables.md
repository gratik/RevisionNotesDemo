# Quick Reference Tables

> Subject: [Interview-Preparation](../README.md)

## Quick Reference Tables

### Do's and Don'ts

| Topic            | ✅ Do                               | ❌ Avoid                          |
| ---------------- | ----------------------------------- | --------------------------------- |
| **Async I/O**    | Use async/await end-to-end          | Blocking with .Result or .Wait()  |
| **CPU Work**     | Use Task.Run for expensive work     | Marking CPU methods as async      |
| **Logging**      | Use structured templates            | String interpolation in hot paths |
| **EF Core**      | Use AsNoTracking for reads          | Tracking every query by default   |
| **Caching**      | Set expirations and size limits     | Unbounded cache growth            |
| **DI Lifetimes** | Scoped for request services         | Singleton for per-request state   |
| **Exceptions**   | Use specific exception types        | Catching Exception (too broad)    |
| **Strings**      | Use StringBuilder for concatenation | += in loops                       |
| **Collections**  | Use appropriate collection type     | List<T> for everything            |
| **Nullability**  | Enable nullable reference types     | Ignoring null warnings            |

---


