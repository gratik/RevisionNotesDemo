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


