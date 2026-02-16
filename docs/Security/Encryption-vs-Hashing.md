# Encryption vs Hashing

> Subject: [Security](../README.md)

## Encryption vs Hashing

### Comparison

| Feature | Encryption | Hashing |
|---------|------------|----------|
| **Reversible?** | ✅ Yes (with key) | ❌ No (one-way) |
| **Purpose** | Protect data (decrypt later) | Verify integrity, store passwords |
| **Examples** | AES, RSA | SHA-256, bcrypt, PBKDF2 |
| **Use Cases** | Database encryption, file encryption | Passwords, checksums, fingerprints |
| **Output** | Same input = different output (with IV) | Same input = same output |

### Symmetric vs Asymmetric Encryption

| Aspect | Symmetric (AES) | Asymmetric (RSA) |
|--------|-----------------|------------------|
| **Keys** | Same key encrypts & decrypts | Public key encrypts, private decrypts |
| **Speed** | Fast | Slow (100x slower) |
| **Key Exchange** | Requires secure channel | No shared secret needed |
| **Use Case** | Bulk data encryption | Key exchange, digital signatures |
| **Key Size** | 128/256 bits | 2048/4096 bits |

---

## Detailed Guidance

Security guidance focuses on defensive defaults, threat-aware design, and verifiable protection of sensitive operations.

### Design Notes
- Define success criteria for Encryption vs Hashing before implementation work begins.
- Keep boundaries explicit so Encryption vs Hashing decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Encryption vs Hashing in production-facing code.
- When performance, correctness, or maintainability depends on consistent Encryption vs Hashing decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Encryption vs Hashing as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Encryption vs Hashing is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Encryption vs Hashing are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

