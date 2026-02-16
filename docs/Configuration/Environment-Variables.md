# Environment Variables

> Subject: [Configuration](../README.md)

## Environment Variables

### Reading Environment Variables

`csharp
// ✅ Environment variables override appsettings.json
var value = _configuration["MySetting"];  // Checks env vars first

// ✅ Explicit environment variable read
var envValue = Environment.GetEnvironmentVariable("MY_SETTING");

// ✅ Hierarchical with double underscore
// Environment variable: EmailSettings__SmtpServer
// Maps to: EmailSettings:SmtpServer
`

### Setting Environment Variables

`ash
# Windows
$env:EmailSettings__SmtpServer="smtp.gmail.com"
$env:ConnectionStrings__DefaultConnection="Server=prod;Database=MyDb"

# Linux/Mac
export EmailSettings__SmtpServer=smtp.gmail.com
export ConnectionStrings__DefaultConnection="Server=prod;Database=MyDb"

# Docker
docker run -e EmailSettings__SmtpServer=smtp.gmail.com myapp

# Kubernetes
env:
  - name: EmailSettings__SmtpServer
    value: "smtp.gmail.com"
`

---


