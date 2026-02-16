# Authentication vs Authorization

> Subject: [Security](../README.md)

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


