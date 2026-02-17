# Strongly-Typed Hubs

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: WebSocket/SignalR basics, auth context propagation, and scaling fundamentals.
- Related examples: docs/RealTime/README.md
> Subject: [RealTime](../README.md)

## Strongly-Typed Hubs

### Type-Safe Client Methods

`csharp
// ✅ Define client interface
public interface IChatClient
{
    Task ReceiveMessage(string user, string message);
    Task UserJoined(string connectionId);
    Task UserLeft(string connectionId);
}

// ✅ Use strongly-typed hub
public class ChatHub : Hub<IChatClient>
{
    public async Task SendMessage(string user, string message)
    {
        // ✅ IntelliSense and compile-time checking
        await Clients.All.ReceiveMessage(user, message);
    }
    
    public override async Task OnConnectedAsync()
    {
        await Clients.Others.UserJoined(Context.ConnectionId);
        await base.OnConnectedAsync();
    }
}
`

---

## Detailed Guidance

Realtime communication guidance focuses on contract evolution, latency budgets, and resilience under variable network conditions.

### Design Notes
- Define success criteria for Strongly-Typed Hubs before implementation work begins.
- Keep boundaries explicit so Strongly-Typed Hubs decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Strongly-Typed Hubs in production-facing code.
- When performance, correctness, or maintainability depends on consistent Strongly-Typed Hubs decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Strongly-Typed Hubs as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Strongly-Typed Hubs is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Strongly-Typed Hubs are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Strongly-Typed Hubs is about stateful real-time communication patterns. It matters because real-time paths amplify scale and connection-lifecycle concerns.
- Use it when broadcasting live updates to connected clients safely.

2-minute answer:
- Start with the problem Strongly-Typed Hubs solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: low-latency delivery vs connection/session management overhead.
- Close with one failure mode and mitigation: assuming connection permanence and ignoring reconnection flows.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Strongly-Typed Hubs but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Strongly-Typed Hubs, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Strongly-Typed Hubs and map it to one concrete implementation in this module.
- 3 minutes: compare Strongly-Typed Hubs with an alternative, then walk through one failure mode and mitigation.