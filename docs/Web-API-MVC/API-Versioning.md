# API Versioning

> Subject: [Web-API-MVC](../README.md)

## API Versioning

### URL Versioning

```csharp
// ✅ Version in URL (most common)
[ApiController]
[Route("api/v{version:apiVersion}/users")]
[ApiVersion("1.0")]
public class UsersV1Controller : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Version 1");
}

[ApiController]
[Route("api/v{version:apiVersion}/users")]
[ApiVersion("2.0")]
public class UsersV2Controller : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Version 2");
}

// GET /api/v1/users → Version 1
// GET /api/v2/users → Version 2
```

### Header Versioning

```csharp
// ✅ Version in header
[ApiController]
[Route("api/users")]
[ApiVersion("1.0")]
public class UsersController : ControllerBase
{
    // Request: GET /api/users
    // Header: api-version: 1.0
}
```

### Query String Versioning

```csharp
// ✅ Version in query string
[HttpGet]
public IActionResult Get([FromQuery] string version)
{
    // GET /api/users?version=1.0
}
```

---


