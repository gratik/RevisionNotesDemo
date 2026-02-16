# Authentication vs Authorization

> Subject: [Security](../README.md)

## Authentication vs Authorization

**Authentication**: WHO you are (identity verification)
- "Prove you're John Doe"
- Login credentials, tokens, biometrics
- Answers: "Are you who you claim to be?"

**Authorization**: WHAT you can access (permission check)
- "Can John Doe access this resource?"
- Roles, claims, policies
- Answers: "Are you allowed to do this?"

```csharp
// Authentication: Verify identity
[Authorize]  // ✅ User must be logged in
public IActionResult ViewProfile() { }

// Authorization: Check permissions
[Authorize(Roles = "Admin")]  // ✅ User must be Admin
public IActionResult DeleteUser() { }

[Authorize(Policy = "MinimumAge21")]  // ✅ Custom policy
public IActionResult BuyAlcohol() { }
```

---

## Detailed Guidance

Security guidance focuses on defensive defaults, threat-aware design, and verifiable protection of sensitive operations.

### Design Notes
- Define success criteria for Authentication vs Authorization before implementation work begins.
- Keep boundaries explicit so Authentication vs Authorization decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Authentication vs Authorization in production-facing code.
- When performance, correctness, or maintainability depends on consistent Authentication vs Authorization decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Authentication vs Authorization as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Authentication vs Authorization is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Authentication vs Authorization are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

