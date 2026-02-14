# Resilience Patterns with Polly

Handle transient failures gracefully with retry, circuit breaker, and timeout patterns.

## Files

- **PollyRetryPatterns.cs** - Retry policies with exponential backoff
- **CircuitBreakerPattern.cs** - Circuit breaker to prevent cascading failures  
- **TimeoutAndBulkhead.cs** - Timeout and bulkhead isolation patterns

## Key Concepts

- Transient vs persistent failures
- Exponential backoff with jitter
- Circuit breaker states (Closed, Open, Half-Open)
- Bulkhead isolation
- Policy wrapping
