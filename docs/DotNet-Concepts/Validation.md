# Validation

> Subject: [DotNet-Concepts](../README.md)

## Validation

### Validate Services on Startup

`csharp
// ✅ Validate DI configuration on startup (Development only)
if (app.Environment.IsDevelopment())
{
    var serviceProvider = app.Services;
    
    // Throws exception if any service can't be resolved
    using var scope = serviceProvider.CreateScope();
    
    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
}

// ✅ Better: Use ValidateOnBuild()
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IEmailService, EmailService>();
// ... register all services

if (builder.Environment.IsDevelopment())
{
    builder.Host.UseDefaultServiceProvider(options =>
    {
        options.ValidateScopes = true;  // ✅ Detect scope issues
        options.ValidateOnBuild = true;  // ✅ Validate on startup
    });
}
`

---


