# Content Negotiation

> Subject: [Web-API-MVC](../README.md)

## Content Negotiation

```csharp
// ✅ Return JSON or XML based on Accept header
[HttpGet]
[Produces("application/json", "application/xml")]
public ActionResult<User> Get()
{
    var user = new User { Name = "Alice" };
    return Ok(user);
}

// Request: Accept: application/json → Returns JSON
// Request: Accept: application/xml → Returns XML
```

---


