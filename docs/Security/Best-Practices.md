# Best Practices

> Subject: [Security](../README.md)

## Best Practices

### ✅ Authentication
- Use industry-standard protocols (OAuth, OpenID Connect)
- Enforce strong passwords (length > complexity)
- Implement MFA for sensitive operations
- Use HTTPS everywhere (no exceptions)
- Implement account lockout (prevent brute force)
- Log authentication failures

### ✅ Password Security
- Never store passwords in plain text
- Use bcrypt, Argon2, or PBKDF2 (not MD5/SHA1)
- Salt every password (unique, random)
- Use slow hashing (100,000+ iterations)
- Implement password reset securely (time-limited tokens)

### ✅ Encryption
- Use AES-256 for symmetric encryption
- Use RSA-2048+ for asymmetric encryption
- Generate random IV for each encryption
- Store keys in Key Vault (Azure, AWS, HashiCorp)
- Rotate keys regularly
- Never hardcode keys in source code

### ✅ API Security
- Validate all input (whitelist, not blacklist)
- Use parameterized queries (prevent SQL injection)
- Implement rate limiting
- Return generic error messages (don't leak info)
- Use security headers (CSP, X-Frame-Options, etc.)

---

## Detailed Guidance

Security guidance focuses on defensive defaults, threat-aware design, and verifiable protection of sensitive operations.

### Design Notes
- Define success criteria for Best Practices before implementation work begins.
- Keep boundaries explicit so Best Practices decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Best Practices in production-facing code.
- When performance, correctness, or maintainability depends on consistent Best Practices decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Best Practices as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Best Practices is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Best Practices are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

