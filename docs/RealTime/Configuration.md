# Configuration

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [RealTime](../README.md)

## Configuration

### SignalR Options

`csharp
builder.Services.AddSignalR(options =>
{
    // ✅ Enable detailed errors (development only)
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    
    // ✅ Keep-alive interval
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    
    // ✅ Client timeout
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    
    // ✅ Maximum message size
    options.MaximumReceiveMessageSize = 32 * 1024;  // 32 KB
    
    // ✅ Handshake timeout
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
});
`

### Scaling with Redis

`csharp
// ✅ Scale across multiple servers with Redis backplane
builder.Services.AddSignalR()
    .AddStackExchangeRedis("localhost:6379", options =>
    {
        options.Configuration.ChannelPrefix = "MyApp";
    });
`

---

## Detailed Guidance

Realtime communication guidance focuses on contract evolution, latency budgets, and resilience under variable network conditions.

### Design Notes
- Define success criteria for Configuration before implementation work begins.
- Keep boundaries explicit so Configuration decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Configuration in production-facing code.
- When performance, correctness, or maintainability depends on consistent Configuration decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Configuration as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Configuration is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Configuration are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

