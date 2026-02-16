# Minimal API vs Controller API vs MVC

> Subject: [Web-API-MVC](../README.md)

## Minimal API vs Controller API vs MVC

| Feature | Minimal API | Controller API | MVC |
|---------|-------------|----------------|-----|
| **Introduced** | .NET 6 | .NET Core 1.0 | .NET Core 1.0 |
| **Style** | Functional | OOP | OOP + Views |
| **Boilerplate** | Minimal | Moderate | More |
| **Best For** | Simple APIs, microservices | Large APIs, complex logic | Server-rendered apps |
| **Filters** | Limited | ✅ Action/Resource filters | ✅ Full filter pipeline |
| **Routing** | Convention-based | Attribute-based | Convention/Attribute |
| **DI** | Parameter injection | Constructor injection | Constructor injection |
| **Testing** | Harder | Easier | Easier |

### When to Use Each

**Minimal API**: Small APIs, microservices, quick prototypes
**Controller API**: Large REST APIs, complex validation, reusable filters
**MVC**: Server-rendered applications with views (Razor Pages often better)

---


