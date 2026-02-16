# User Secrets (Development)

> Subject: [Configuration](../README.md)

## User Secrets (Development)

### Why User Secrets?

**Problem**: Don't commit sensitive data (passwords, API keys) to source control  
**Solution**: User Secrets store settings outside project directory

`ash
# Initialize user secrets
dotnet user-secrets init

# Set a secret
dotnet user-secrets set "EmailSettings:Password" "mypassword"
dotnet user-secrets set "ApiKeys:Stripe" "sk_test_123456"

# List secrets
dotnet user-secrets list

# Remove secret
dotnet user-secrets remove "ApiKeys:Stripe"

# Clear all
dotnet user-secrets clear
`

### Using User Secrets

`csharp
// ✅ Automatically loaded in Development environment
public class EmailService
{
    private readonly EmailSettings _settings;
    
    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
        // Password comes from user secrets in dev
        // Comes from environment variables in production
    }
}
`

---


