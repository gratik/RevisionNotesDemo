# Secure Coding Practices

> Subject: [Security](../README.md)

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


