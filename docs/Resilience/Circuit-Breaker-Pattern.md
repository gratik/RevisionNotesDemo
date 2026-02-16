# Circuit Breaker Pattern

> Subject: [Resilience](../README.md)

## Circuit Breaker Pattern

### The Three States

**CLOSED** (Normal): Requests flow through, failures counted
**OPEN** (Tripped): Requests fail immediately, no calls made  
**HALF-OPEN** (Testing): Allow one request to test recovery

### Basic Circuit Breaker

```csharp
var circuitBreakerPolicy = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(
        exceptionsAllowedBeforeBreaking: 3,  // Break after 3 failures
        durationOfBreak: TimeSpan.FromSeconds(30));  // Stay open 30s

// Flow:
// Attempt 1: Fails → Count = 1 (CLOSED)
// Attempt 2: Fails → Count = 2 (CLOSED)
// Attempt 3: Fails → Circuit OPENS
// Attempt 4+: BrokenCircuitException (immediate)
// After 30s: Circuit moves to HALF-OPEN
// Next: Success → CLOSED | Failure → OPEN again
```

### Advanced Circuit Breaker

```csharp
// ✅ Based on failure rate, not consecutive failures
var circuitBreakerPolicy = Policy
    .Handle<HttpRequestException>()
    .AdvancedCircuitBreakerAsync(
        failureThreshold: 0.5,  // 50% failure rate
        samplingDuration: TimeSpan.FromSeconds(10),  // Over 10s window
        minimumThroughput: 10,  // Minrequests before evaluating
        durationOfBreak: TimeSpan.FromSeconds(30));
```

---


