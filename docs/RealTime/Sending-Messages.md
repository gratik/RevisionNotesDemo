# Sending Messages

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: WebSocket/SignalR basics, auth context propagation, and scaling fundamentals.
- Related examples: docs/RealTime/README.md
> Subject: [RealTime](../README.md)

## Sending Messages

### Broadcast to All Clients

`csharp
public class NotificationHub : Hub
{
    // ✅ Send to everyone
    public async Task BroadcastNotification(string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", message);
    }
}
`

### Send to Specific Client

`csharp
public class ChatHub : Hub
{
    // ✅ Send to specific connection
    public async Task SendPrivateMessage(string connectionId, string message)
    {
        await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
    }
    
    // ✅ Send to everyone except caller
    public async Task Broadcast(string message)
    {
        await Clients.Others.SendAsync("ReceiveMessage", message);
    }
    
    // ✅ Send to caller only
    public async Task Echo(string message)
    {
        await Clients.Caller.SendAsync("ReceiveMessage", message);
    }
}
`

### Send to Multiple Clients

`csharp
public class ChatHub : Hub
{
    // ✅ Send to list of connections
    public async Task SendToUsers(List<string> connectionIds, string message)
    {
        await Clients.Clients(connectionIds).SendAsync("ReceiveMessage", message);
    }
    
    // ✅ Send to all except specific connections
    public async Task SendToAllExcept(List<string> excludedConnectionIds, string message)
    {
        await Clients.AllExcept(excludedConnectionIds).SendAsync("ReceiveMessage", message);
    }
}
`

---

## Detailed Guidance

Realtime communication guidance focuses on contract evolution, latency budgets, and resilience under variable network conditions.

### Design Notes
- Define success criteria for Sending Messages before implementation work begins.
- Keep boundaries explicit so Sending Messages decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Sending Messages in production-facing code.
- When performance, correctness, or maintainability depends on consistent Sending Messages decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Sending Messages as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Sending Messages is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Sending Messages are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Sending Messages is about stateful real-time communication patterns. It matters because real-time paths amplify scale and connection-lifecycle concerns.
- Use it when broadcasting live updates to connected clients safely.

2-minute answer:
- Start with the problem Sending Messages solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: low-latency delivery vs connection/session management overhead.
- Close with one failure mode and mitigation: assuming connection permanence and ignoring reconnection flows.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Sending Messages but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Sending Messages, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Sending Messages and map it to one concrete implementation in this module.
- 3 minutes: compare Sending Messages with an alternative, then walk through one failure mode and mitigation.