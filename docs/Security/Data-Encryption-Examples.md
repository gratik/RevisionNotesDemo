# Data Encryption Examples

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Security](../README.md)

## Data Encryption Examples

### AES Encryption (Symmetric)

```csharp
public (byte[] ciphertext, byte[] iv) Encrypt(string plaintext, byte[] key)
{
    using var aes = Aes.Create();
    aes.Key = key;  // 256-bit key
    aes.GenerateIV();  // ✅ Random IV for each encryption
    
    using var encryptor = aes.CreateEncryptor();
    var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
    var ciphertext = encryptor.TransformFinalBlock(
        plaintextBytes, 0, plaintextBytes.Length);
    
    return (ciphertext, aes.IV);  // ✅ Store IV with ciphertext
}

public string Decrypt(byte[] ciphertext, byte[] iv, byte[] key)
{
    using var aes = Aes.Create();
    aes.Key = key;
    aes.IV = iv;  // Use same IV from encryption
    
    using var decryptor = aes.CreateDecryptor();
    var decryptedBytes = decryptor.TransformFinalBlock(
        ciphertext, 0, ciphertext.Length);
    
    return Encoding.UTF8.GetString(decryptedBytes);
}
```

### ASP.NET Core Data Protection API

```csharp
// ✅ Built-in encryption/decryption with key rotation
public class SecureService
{
    private readonly IDataProtector _protector;
    
    public SecureService(IDataProtectionProvider provider)
    {
        // Purpose-specific protector
        _protector = provider.CreateProtector("MyApp.PII");
    }
    
    public string EncryptSensitiveData(string data)
    {
        return _protector.Protect(data);  // ✅ Automatic key management
    }
    
    public string DecryptSensitiveData(string encrypted)
    {
        return _protector.Unprotect(encrypted);
    }
}

// Configure in Program.cs
services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"c:\keys"))
    .SetApplicationName("MyApp");
```

---

## Detailed Guidance

Security guidance focuses on defensive defaults, threat-aware design, and verifiable protection of sensitive operations.

### Design Notes
- Define success criteria for Data Encryption Examples before implementation work begins.
- Keep boundaries explicit so Data Encryption Examples decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Data Encryption Examples in production-facing code.
- When performance, correctness, or maintainability depends on consistent Data Encryption Examples decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Data Encryption Examples as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Data Encryption Examples is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Data Encryption Examples are documented and reviewable.
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

