# Common Pitfalls

> Subject: [Security](../README.md)

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


