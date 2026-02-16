# Password Hashing

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


