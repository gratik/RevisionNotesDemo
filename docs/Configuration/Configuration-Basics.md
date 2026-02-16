# Configuration Basics

> Subject: [Configuration](../README.md)

## Configuration Basics

### appsettings.json Structure

`json
{
  "AppName": "MyApplication",
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyDb"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.example.com",
    "Port": 587,
    "FromAddress": "noreply@example.com"
  },
  "Features": {
    "EnableCache": true,
    "MaxItemsPerPage": 50
  }
}
`

### Reading Configuration

`csharp
// ✅ Inject IConfiguration
public class MyService
{
    private readonly IConfiguration _configuration;
    
    public MyService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void ReadSettings()
    {
        // ✅ Simple values
        string appName = _configuration["AppName"];  // "MyApplication"
        
        // ✅ Nested values with colon notation
        string logLevel = _configuration["Logging:LogLevel:Default"];  // "Information"
        
        // ✅ Strongly-typed with GetValue<T>
        int maxItems = _configuration.GetValue<int>("Features:MaxItemsPerPage");  // 50
        bool cacheEnabled = _configuration.GetValue<bool>("Features:EnableCache");  // true
        
        // ✅ With default values
        int pageSize = _configuration.GetValue<int>("PageSize", 20);  // 20 if not found
        
        // ✅ Connection strings
        string connString = _configuration.GetConnectionString("DefaultConnection");
    }
}
`

---


