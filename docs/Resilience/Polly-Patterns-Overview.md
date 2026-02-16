# Polly Patterns Overview

> Subject: [Resilience](../README.md)

## Polly Patterns Overview

| Pattern | Purpose | When to Use |
|---------|---------|-------------|
| **Retry** | Try again after failure | Transient errors (network blips) |
| **Circuit Breaker** | Stop calling failing service | Persistent failures (service down) |
| **Timeout** | Bound operation duration | Prevent hanging |
| **Bulkhead** | Limit concurrent operations | Prevent resource exhaustion |
| **Fallback** | Return default on failure | Provide degraded experience |

---


