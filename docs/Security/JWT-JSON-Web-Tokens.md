# JWT (JSON Web Tokens)

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: AuthN/AuthZ basics, secret management, and secure coding fundamentals.
- Related examples: docs/Security/README.md
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
- JWT (JSON Web Tokens) is about application and API security controls. It matters because security failures are high-impact and expensive to remediate.
- Use it when implementing defense-in-depth across authentication and authorization.

2-minute answer:
- Start with the problem JWT (JSON Web Tokens) solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: security strictness vs developer and user ergonomics.
- Close with one failure mode and mitigation: treating security as a one-time feature instead of an ongoing practice.
## Interview Bad vs Strong Answer
Bad answer:
- Defines JWT (JSON Web Tokens) but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose JWT (JSON Web Tokens), what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define JWT (JSON Web Tokens) and map it to one concrete implementation in this module.
- 3 minutes: compare JWT (JSON Web Tokens) with an alternative, then walk through one failure mode and mitigation.