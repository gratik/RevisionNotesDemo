# Key Concepts

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: HTTP/2 basics, protobuf awareness, and service contract versioning.
- Related examples: docs/gRPC/README.md
> Subject: [gRPC](../README.md)

## Key Concepts

### Protocol Buffers (.proto files)

Define service contracts in `.proto` files:

```protobuf
syntax = "proto3";

service UserService {
  rpc GetUser (GetUserRequest) returns (UserResponse);
  rpc ListUsers (ListUsersRequest) returns (stream UserResponse);
}

message GetUserRequest {
  int32 id = 1;
}

message UserResponse {
  int32 id = 1;
  string name = 2;
  string email = 3;
}
```

### Service Implementation

Implement generated service base class:

```csharp
public class UserService : UserServiceBase
{
    public override async Task<UserResponse> GetUser(
        GetUserRequest request,
        ServerCallContext context)
    {
        var user = await _repo.GetAsync(request.Id);
        if (user == null)
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        
        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}
```

### Streaming Types

1. **Server Streaming**: Server sends multiple responses
2. **Client Streaming**: Client sends multiple requests
3. **Bi-directional Streaming**: Both send multiple messages

---

## Detailed Guidance

Realtime communication guidance focuses on contract evolution, latency budgets, and resilience under variable network conditions.

### Design Notes
- Define success criteria for Key Concepts before implementation work begins.
- Keep boundaries explicit so Key Concepts decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Key Concepts in production-facing code.
- When performance, correctness, or maintainability depends on consistent Key Concepts decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Key Concepts as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Key Concepts is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Key Concepts are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Key Concepts is about contract-first RPC communication. It matters because it optimizes service-to-service communication and typed contracts.
- Use it when high-throughput internal service calls with strict schemas.

2-minute answer:
- Start with the problem Key Concepts solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: performance and typing vs ecosystem/browser constraints.
- Close with one failure mode and mitigation: choosing gRPC for scenarios where REST interoperability is required.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Key Concepts but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Key Concepts, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Key Concepts and map it to one concrete implementation in this module.
- 3 minutes: compare Key Concepts with an alternative, then walk through one failure mode and mitigation.