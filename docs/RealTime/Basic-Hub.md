# Basic Hub

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: WebSocket/SignalR basics, auth context propagation, and scaling fundamentals.
- Related examples: docs/RealTime/README.md
> Subject: [RealTime](../README.md)

## Basic Hub

### Creating a Hub

`csharp
// ✅ Simple chat hub
public class ChatHub : Hub
{
    // ✅ Clients can invoke this method
    public async Task SendMessage(string user, string message)
    {
        // ✅ Broadcast to ALL connected clients
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}

// ✅ Register SignalR
builder.Services.AddSignalR();

// ✅ Map hub endpoint
app.MapHub<ChatHub>("/chatHub");
`

### JavaScript Client

`javascript
// ✅ Connect to hub
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();

// ✅ Listen for messages from server
connection.on("ReceiveMessage", (user, message) => {
    console.log(${user}: );
    // Update UI
});

// ✅ Start connection
await connection.start();
console.log("Connected!");

// ✅ Invoke server method
await connection.invoke("SendMessage", "Alice", "Hello World!");

// ✅ Stop connection
await connection.stop();
`

---

## Detailed Guidance

Realtime communication guidance focuses on contract evolution, latency budgets, and resilience under variable network conditions.

### Design Notes
- Define success criteria for Basic Hub before implementation work begins.
- Keep boundaries explicit so Basic Hub decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Basic Hub in production-facing code.
- When performance, correctness, or maintainability depends on consistent Basic Hub decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Basic Hub as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Basic Hub is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Basic Hub are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Basic Hub is about stateful real-time communication patterns. It matters because real-time paths amplify scale and connection-lifecycle concerns.
- Use it when broadcasting live updates to connected clients safely.

2-minute answer:
- Start with the problem Basic Hub solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: low-latency delivery vs connection/session management overhead.
- Close with one failure mode and mitigation: assuming connection permanence and ignoring reconnection flows.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Basic Hub but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Basic Hub, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Basic Hub and map it to one concrete implementation in this module.
- 3 minutes: compare Basic Hub with an alternative, then walk through one failure mode and mitigation.