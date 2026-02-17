# Password Hashing

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: AuthN/AuthZ basics, secret management, and secure coding fundamentals.
- Related examples: docs/Security/README.md
> Subject: [Security](../README.md)

## Password Hashing

### The Right Way: PBKDF2, bcrypt, or Argon2

```csharp
public string HashPassword(string password)
{
    // ✅ Generate random salt (unique per password)
    byte[] salt = RandomNumberGenerator.GetBytes(32);
    
    // ✅ Slow hashing with iterations (defeats brute force)
    byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
        password,
        salt,
        iterations: 100_000,  // Adjust based on hardware
        HashAlgorithmName.SHA256,
        32  // Hash size
    );
    
    // ✅ Store hash + salt together
    return Convert.ToBase64String(hash) + "." + Convert.ToBase64String(salt);
}

public bool VerifyPassword(string password, string storedHash)
{
    var parts = storedHash.Split('.');
    var hash = Convert.FromBase64String(parts[0]);
    var salt = Convert.FromBase64String(parts[1]);
    
    // ✅ Hash input password with stored salt
    var testHash = Rfc2898DeriveBytes.Pbkdf2(
        password, salt, 100_000, HashAlgorithmName.SHA256, 32);
    
    // ✅ Constant-time comparison (prevents timing attacks)
    return CryptographicOperations.FixedTimeEquals(hash, testHash);
}
```

### Why This Matters

**Salt**: Defeats rainbow table attacks (precomputed hashes)
- Same password + different salt = different hash
- Must be random and unique per password

**Iterations**: Slows down brute force attacks
- 100,000 iterations = 100,000x slower to crack
- Adjust based on acceptable performance (target: ~100ms)

**Constant-Time Comparison**: Prevents timing attacks
- Variable-time comparison leaks information
- `FixedTimeEquals` always takes same time

---

## Detailed Guidance

Security guidance focuses on defensive defaults, threat-aware design, and verifiable protection of sensitive operations.

### Design Notes
- Define success criteria for Password Hashing before implementation work begins.
- Keep boundaries explicit so Password Hashing decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Password Hashing in production-facing code.
- When performance, correctness, or maintainability depends on consistent Password Hashing decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Password Hashing as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Password Hashing is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Password Hashing are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Password Hashing is about application and API security controls. It matters because security failures are high-impact and expensive to remediate.
- Use it when implementing defense-in-depth across authentication and authorization.

2-minute answer:
- Start with the problem Password Hashing solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: security strictness vs developer and user ergonomics.
- Close with one failure mode and mitigation: treating security as a one-time feature instead of an ongoing practice.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Password Hashing but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Password Hashing, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Password Hashing and map it to one concrete implementation in this module.
- 3 minutes: compare Password Hashing with an alternative, then walk through one failure mode and mitigation.