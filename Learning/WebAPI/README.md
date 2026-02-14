# Web API & MVC Best Practices

## 📚 Overview

This folder contains comprehensive examples of building **Web APIs and MVC applications** in ASP.NET Core, covering three major approaches: **Minimal APIs**, **Controller-based APIs**, and **MVC with Views**. Each approach demonstrates **good vs bad practices** with extensive inline documentation.

## 🎯 What You'll Learn

- ✅ **Minimal APIs** - Modern, lightweight HTTP APIs (NET 6+)
- ✅ **Controller APIs** - Traditional class-based Web APIs
- ✅ **MVC Pattern** - Server-rendered web applications
- ✅ **Middleware Pipeline** - Request/response processing
- ✅ **Routing & HTTP verbs** - RESTful conventions
- ✅ **Model binding & validation** - Data Annotations and FluentValidation
- ✅ **Authentication & authorization** - Securing endpoints
- ✅ **Error handling** - Problem Details (RFC 7807)
- ✅ **Async/await patterns** - Scalable I/O operations
- ✅ **Dependency injection** - Service management

## 📂 Folder Structure

```
Learning/WebAPI/
├── MinimalAPI/
│   └── MinimalAPIBestPractices.cs        # ~17KB - 5 comprehensive examples
│       - Basic endpoint definition
│       - Dependency injection
│       - Request validation & error handling
│       - Async operations & cancellation
│       - Authentication & authorization
│
├── ControllerAPI/
│   └── WebAPIBestPractices.cs            # ~17KB - 5 comprehensive examples
│       - Controller structure & routing
│       - Model binding & validation
│       - Action filters & middleware
│       - Problem Details error handling
│       - Async actions & cancellation
│
├── MVC/
│   └── MVCBestPractices.cs                # ~16KB - 4 comprehensive examples
│       - Controller actions & views
│       - Form handling & validation
│       - Partial views & view components
│       - Filters & authorization
│
└── Middleware/
    └── MiddlewareBestPractices.cs        # ~20KB - 8 comprehensive examples
        - Middleware pipeline order (CRITICAL!)
        - Custom inline middleware (Use, Run, Map)
        - Custom middleware classes
        - Global exception handling
        - Request/response logging
        - CORS configuration
        - Rate limiting (.NET 7+)
        - Short-circuiting & branching
```

## 🔄 When to Use Which Approach

### Minimal APIs

**Best For:**

- Microservices (< 50 endpoints)
- Serverless functions (Azure Functions, AWS Lambda)
- Performance-critical scenarios
- Simple routing patterns
- Modern, functional programming style

**Advantages:**

- 15-20% faster than Controllers
- Less boilerplate code
- Simpler code organization
- Great for learning ASP.NET Core

**Example Use Cases:**

- Backend for mobile apps
- Internal microservices
- Webhook handlers
- Simple CRUD APIs

### Controller APIs

**Best For:**

- Large APIs (50+ endpoints)
- Complex routing scenarios
- Teams familiar with MVC pattern
- Extensive middleware/filter requirements
- Enterprise applications

**Advantages:**

- Rich action filter pipeline
- Better code organization for large apps
- Testing infrastructure maturity
- Built-in model binding & validation
- Clear separation of concerns

**Example Use Cases:**

- Corporate REST APIs
- Public APIs with versioning
- Applications with complex business logic
- APIs requiring extensive middleware

### MVC (Views)

**Best For:**

- Server-rendered web applications
- SEO-critical websites
- Traditional multi-page applications
- Forms-heavy applications
- Content management systems

**Advantages:**

- Server-side rendering for SEO
- Rich view helpers & tag helpers
- Partial views for reusability
- View components for complex UI
- Strong typing in views

**Example Use Cases:**

- E-commerce websites
- Content management systems
- Admin panels & dashboards
- Traditional web applications
- Public-facing marketing sites

## 💡 Key Concepts Covered

### 1. Middleware Pipeline

**Correct Order (CRITICAL!):**

```csharp
app.UseExceptionHandler("/error");  // 1. Exception handling FIRST
app.UseHttpsRedirection();           // 2. HTTPS redirect
app.UseStaticFiles();                // 3. Static files (short-circuit)
app.UseRouting();                    // 4. Routing (MUST be before CORS/Auth)
app.UseCors("MyPolicy");             // 5. CORS (after routing, before auth)
app.UseAuthentication();             // 6. Authentication (before authorization)
app.UseAuthorization();              // 7. Authorization (before endpoints)
app.UseRateLimiter();                // 8. Custom middleware
app.MapControllers();                // 9. Endpoints LAST
```

**Custom Middleware:**

```csharp
// Inline
app.Use(async (context, next) =>
{
    // Before: execute before next middleware
    await next();
    // After: execute after next middleware
});

// Class-based
public class TimingMiddleware
{
    private readonly RequestDelegate _next;
    public TimingMiddleware(RequestDelegate next) => _next = next;
    public async Task InvokeAsync(HttpContext context)
    {
        var start = DateTime.UtcNow;
        await _next(context);
        var duration = DateTime.UtcNow - start;
        // Log duration
    }
}
```

### 2. Routing Patterns

**Minimal API:**

```csharp
app.MapGet("/api/products/{id:int}", (int id) => GetProduct(id));
```

**Controller API:**

```csharp
[HttpGet("{id:int}")]
public ActionResult<Product> GetById(int id) => Ok(GetProduct(id));
```

**MVC:**

```csharp
[HttpGet]
public IActionResult Product(int id) => View(GetProductViewModel(id));
```

### 2. Dependency Injection

**Minimal API:**

```csharp
app.MapGet("/data", (IDataService service) => service.GetData());
```

**Controller/MVC:**

```csharp
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    public ProductsController(IProductService service) => _service = service;
}
```

### 3. Validation

**Minimal API:**

```csharp
app.MapPost("/users", (User user, IValidator<User> validator) =>
{
    var result = validator.Validate(user);
    return result.IsValid ? Results.Ok() : Results.ValidationProblem();
});
```

**Controller/MVC:**

```csharp
[HttpPost]
public ActionResult Create([FromBody] CreateDto dto)
{
    // [ApiController] validates ModelState automatically
    // Returns 400 with errors if invalid
}
```

### 4. Error Handling

**Minimal API:**

```csharp
app.MapGet("/users/{id}", (int id) =>
{
    var user = GetUser(id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});
```

**Controller/MVC:**

```csharp
public ActionResult<User> GetUser(int id)
{
    var user = _service.GetUser(id);
    return user != null ? Ok(user) : NotFound();
}
```

## 🚀 Quick Start

1. **Review the examples** - Each file has 4-5 comprehensive examples
2. **Study good vs bad patterns** - Every example shows ❌ bad and ✅ good approaches
3. **Read inline documentation** - THE PROBLEM / THE SOLUTION / WHY IT MATTERS format
4. **Run and experiment** - Copy examples to your own projects

## 📖 Learning Path

### Beginner

1. Start with **MinimalAPI** - Learn basic endpoint definition
2. Study **routing patterns** - Understand HTTP verbs and paths
3. Practice **model binding** - Map request data to C# objects

### Intermediate

1. Learn **Controller APIs** - Understand class-based organization
2. Study **validation patterns** - Data Annotations and FluentValidation
3. Implement **authentication** - Secure your endpoints

### Advanced

1. Master **action filters** - Reusable cross-cutting logic
2. Learn **MVC views** - Server-side rendering
3. Understand **view components** - Complex UI composition
4. Implement **API versioning** - Evolve APIs over time

## 🔍 Common Patterns

### CRUD Operations

| Operation | HTTP Verb | Route              | Status Code    |
| --------- | --------- | ------------------ | -------------- |
| Create    | POST      | /api/products      | 201 (Created)  |
| Read      | GET       | /api/products/{id} | 200 (OK) / 404 |
| Update    | PUT/PATCH | /api/products/{id} | 204 / 404      |
| Delete    | DELETE    | /api/products/{id} | 204 / 404      |
| List      | GET       | /api/products      | 200 (OK)       |

### Status Codes

| Code | Meaning      | When to Use              |
| ---- | ------------ | ------------------------ |
| 200  | OK           | Successful GET/PUT       |
| 201  | Created      | Successful POST          |
| 204  | No Content   | Successful DELETE/PUT    |
| 400  | Bad Request  | Validation errors        |
| 401  | Unauthorized | Missing/invalid auth     |
| 403  | Forbidden    | Insufficient permissions |
| 404  | Not Found    | Resource doesn't exist   |
| 500  | Server Error | Unexpected errors        |

## 🎨 Documentation Standards

All files follow the same comprehensive style:

1. **Large header** (30-40 lines) with purpose, why it matters, what you'll learn
2. **Comprehensive `/// <summary>` blocks** with THE PROBLEM / THE SOLUTION / WHY IT MATTERS
3. **Good ✅ vs Bad ❌ patterns** for every concept
4. **GOTCHA warnings** for common mistakes
5. **Performance metrics** where relevant
6. **Real-world scenarios** and when to use each approach
7. **Inline comments** explaining WHY, not just WHAT

## 🔗 Related Sections

- **[Testing](../Testing/)** - Unit/integration testing for APIs
- **[Entity Framework](../DataAccess/EntityFramework/)** - Data access patterns
- **[Async & Multithreading](../AsyncMultithreading/)** - Concurrent operations
- **[Design Patterns](../DesignPatterns/)** - Common architectural patterns

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core/)
- [Minimal APIs Overview](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis)
- [Web API Best Practices](https://learn.microsoft.com/aspnet/core/web-api/)
- [MVC Overview](https://learn.microsoft.com/aspnet/core/mvc/overview)
- [Problem Details RFC 7807](https://datatracker.ietf.org/doc/html/rfc7807)

## ✅ What's Complete

- ✅ **MinimalAPI/MinimalAPIBestPractices.cs** - 5 examples (~17KB)
- ✅ **ControllerAPI/WebAPIBestPractices.cs** - 5 examples (~17KB)
- ✅ **MVC/MVCBestPractices.cs** - 4 examples (~16KB)
- ✅ **Middleware/MiddlewareBestPractices.cs** - 8 examples (~20KB)
- ✅ **README.md** - Complete documentation

**Total**: 4 comprehensive files with 22 major patterns covered!
