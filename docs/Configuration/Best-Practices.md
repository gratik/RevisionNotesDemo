# Best Practices

> Subject: [Configuration](../README.md)

## Best Practices

### ✅ Configuration Management
- Use appsettings.json for non-sensitive defaults
- Use User Secrets for local development secrets
- Use environment variables for production secrets
- Use strongly-typed Options pattern (not IConfiguration directly)
- Validate configuration on startup
- Document required settings in README

### ✅ Security
- Never commit secrets to source control
- Use Azure Key Vault or AWS Secrets Manager in production
- Rotate secrets regularly
- Limit access to production configuration
- Use different connection strings per environment

### ✅ Environment Strategy
- Development: User Secrets + appsettings.Development.json
- Staging: Environment variables + appsettings.Staging.json
- Production: Environment variables + appsettings.Production.json

---


