# DbContext Configuration

> Subject: [Entity-Framework](../README.md)

## DbContext Configuration

### DbContext Lifetime

```csharp
// ✅ GOOD: Scoped lifetime (default in ASP.NET Core)
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// ❌ BAD: Don't create DbContext manually in production
using var context = new AppDbContext();  // ❌ Not recommended

// ✅ GOOD: Inject DbContext
public class UserService
{
    private readonly AppDbContext _context;
    
    public UserService(AppDbContext context)
    {
        _context = context;
    }
}
```

### Connection String Management

```csharp
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyDb;Trusted_Connection=true"
  }
}

// ✅ Configure in Program.cs
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

---


