# Authentication, Authorization, and Encryption

> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../../README.md)

## Overview

Security is non-negotiable in modern applications. This guide covers authentication (verifying identity),
authorization (checking permissions), encryption (protecting data), and secure coding practices. The examples
focus on ASP.NET Core patterns, JWT tokens, password hashing, and data protection.

---

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

## Authentication Methods Comparison

| Method | Use Case | Stateful? | Cross-Domain | Mobile-Friendly |
|--------|----------|-----------|--------------|------------------|
| **Cookie** | Traditional web apps | Yes (session) | ❌ No | ❌ Limited |
| **JWT** | APIs, SPAs, mobile | No (stateless) | ✅ Yes | ✅ Yes |
| **OAuth 2.0** | Third-party login | Depends | ✅ Yes | ✅ Yes |
| **API Key** | Service-to-service | No | ✅ Yes | ✅ Yes |
| **Certificate** | Enterprise/devices | No | ✅ Yes | ⚠️ Complex |

---

## JWT (JSON Web Tokens)

### Structure: Header.Payload.Signature

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.  ← Header (algorithm, type)
eyJzdWIiOiIxMjM0IiwibmFtZSI6IkpvaG4ifQ.  ← Payload (claims)
SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c   ← Signature (verification)
```

### Generating JWT Tokens

```csharp
public string GenerateToken(string userId, string username, string[] roles)
{
    // ✅ Build claims (user info)
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Name, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };
    
    // Add roles
    foreach (var role in roles)
        claims.Add(new Claim(ClaimTypes.Role, role));
    
    // ✅ Sign with secret key
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    
    // ✅ Create token
    var token = new JwtSecurityToken(
        issuer: "MyApp",
        audience: "MyApp-Users",
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),  // Short-lived
        signingCredentials: credentials
    );
    
    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

### Validating JWT Tokens

```csharp
public ClaimsPrincipal? ValidateToken(string token)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(_secretKey);
    
    try
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,  // ✅ Check signature
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,            // ✅ Check issuer
            ValidIssuer = "MyApp",
            ValidateAudience = true,          // ✅ Check audience
            ValidAudience = "MyApp-Users",
            ValidateLifetime = true,          // ✅ Check expiration
            ClockSkew = TimeSpan.FromMinutes(5)
        };
        
        return tokenHandler.ValidateToken(token, validationParameters, out _);
    }
    catch
    {
        return null;  // Invalid token
    }
}
```

### JWT Best Practices

**✅ DO:**
- Store secret keys in configuration/Key Vault (never in code)
- Use HTTPS only (JWT in HTTP = credentials exposed)
- Keep tokens short-lived (1 hour ideal)
- Use refresh tokens for long sessions
- Include minimal claims (no sensitive data)
- Validate all token fields (issuer, audience, expiration)

**❌ DON'T:**
- Store sensitive data in tokens (they're not encrypted, just signed)
- Use weak secrets (min 256 bits)
- Accept tokens without validation
- Store tokens in localStorage (XSS vulnerable) - use httpOnly cookies

---

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

## Secure Coding Practices

### Input Validation

```csharp
// ❌ BAD: No validation (SQL injection, XSS, path traversal)
public IActionResult GetFile(string filename)
{
    return File(filename, "text/plain");  // ❌ Path traversal!
}

// ✅ GOOD: Validate and sanitize
public IActionResult GetFile(string filename)
{
    // Whitelist validation
    if (!Regex.IsMatch(filename, @"^[a-zA-Z0-9_-]+\.txt$"))
        return BadRequest("Invalid filename");
    
    var safePath = Path.Combine(_basePath, filename);
    
    // Prevent path traversal
    if (!safePath.StartsWith(_basePath))
        return BadRequest("Invalid path");
    
    return File(safePath, "text/plain");
}
```

### SQL Injection Prevention

```csharp
// ❌ DANGEROUS: SQL injection vulnerability
var sql = $"SELECT * FROM Users WHERE Username = '{username}'";
// Input: '; DROP TABLE Users; --

// ✅ SAFE: Parameterized queries
var sql = "SELECT * FROM Users WHERE Username = @Username";
var user = await connection.QueryFirstOrDefaultAsync<User>(
    sql, new { Username = username });
```

### XSS Prevention

```csharp
// ❌ BAD: Unencoded user input in HTML
@Model.Comment  // ❌ XSS if Comment contains <script>alert(1)</script>

// ✅ GOOD: Razor automatically encodes
@Model.Comment  // ✅ <script> becomes &lt;script&gt;

// ✅ Sanitize if you need HTML
var sanitizer = new HtmlSanitizer();
var clean = sanitizer.Sanitize(userHtml);
```

### Security Headers

```csharp
// Add security headers in middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Strict-Transport-Security", 
        "max-age=31536000; includeSubDomains");
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self'");
    
    await next();
});
```

---

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

## Related Files

- [Security/AuthenticationExamples.cs](../Security/AuthenticationExamples.cs) - JWT, Cookie, OAuth patterns
- [Security/EncryptionExamples.cs](../Security/EncryptionExamples.cs) - AES, RSA, password hashing, Data Protection API
- [Security/SecureCodingPractices.cs](../Security/SecureCodingPractices.cs) - Input validation, SQL injection prevention
- [Security/HashingAndSigningExamples.cs](../Security/HashingAndSigningExamples.cs) - HMAC, digital signatures

---

## See Also

- [Web API and MVC](Web-API-MVC.md) - Authentication middleware and authorization policies
- [Data Access](Data-Access.md) - SQL injection prevention
- [Logging and Observability](Logging-Observability.md) - Security event logging
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14
