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


