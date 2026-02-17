# JWT (JSON Web Tokens)

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Security](../README.md)

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

