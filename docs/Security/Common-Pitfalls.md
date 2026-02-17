# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: AuthN/AuthZ basics, secret management, and secure coding fundamentals.
- Related examples: docs/Security/README.md
> Subject: [Security](../README.md)

## Common Pitfalls

### ❌ Weak JWT Secrets

```csharp
// ❌ BAD: Weak secret
var key = "secret123";  // ❌ Too short, guessable

// ✅ GOOD: Strong secret (256+ bits)
var key = "XQRv8F3K2mP9nL7bD5wJ1cY6hG4sA0tU...";  // 32+ chars
```

### ❌ Storing Passwords as Plain MD5/SHA1

```csharp
// ❌ BAD: Fast hashing (rainbow tables work)
var hash = SHA1.HashData(Encoding.UTF8.GetBytes(password));

// ✅ GOOD: Slow, salted hashing
var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, ...);
```

### ❌ Reusing IV in AES

```csharp
// ❌ BAD: Same IV for all encryptions (pattern leakage)
private static byte[] _staticIV = new byte[16];
aes.IV = _staticIV;

// ✅ GOOD: Random IV per encryption
aes.GenerateIV();
```

### ❌ Trusting Client Input

```csharp
// ❌ BAD: No validation
public void DeleteUser(int userId)
{
    _db.Execute("DELETE FROM Users WHERE Id = @Id", new { Id = userId });
    // ❌ What if userId belongs to admin?
}

// ✅ GOOD: Check permissions
[Authorize]
public void DeleteUser(int userId)
{
    var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
    if (userId != int.Parse(currentUserId) && !User.IsInRole("Admin"))
        throw new UnauthorizedAccessException();
    
    _db.Execute("DELETE FROM Users WHERE Id = @Id", new { Id = userId });
}
```

---

## Detailed Guidance

Security guidance focuses on defensive defaults, threat-aware design, and verifiable protection of sensitive operations.

### Design Notes
- Define success criteria for Common Pitfalls before implementation work begins.
- Keep boundaries explicit so Common Pitfalls decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Common Pitfalls in production-facing code.
- When performance, correctness, or maintainability depends on consistent Common Pitfalls decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Common Pitfalls as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Common Pitfalls is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Common Pitfalls are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Common Pitfalls is about application and API security controls. It matters because security failures are high-impact and expensive to remediate.
- Use it when implementing defense-in-depth across authentication and authorization.

2-minute answer:
- Start with the problem Common Pitfalls solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: security strictness vs developer and user ergonomics.
- Close with one failure mode and mitigation: treating security as a one-time feature instead of an ongoing practice.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Common Pitfalls but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Common Pitfalls, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Common Pitfalls and map it to one concrete implementation in this module.
- 3 minutes: compare Common Pitfalls with an alternative, then walk through one failure mode and mitigation.