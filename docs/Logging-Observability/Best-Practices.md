# Best Practices

> Subject: [Logging-Observability](../README.md)

## Best Practices

### ✅ Logging Guidelines
- Use structured logging (templates, not interpolation)
- Include relevant context in log messages
- Use appropriate log levels
- Add correlation IDs for request tracing
- Use log scopes for grouping related logs
- Include timing information for operations

### ✅ Performance
- Use LoggerMessage for hot paths
- Check IsEnabled before expensive operations
- Avoid logging in tight loops
- Don't serialize large objects unless necessary
- Use async logging providers where available

### ✅ Security
- Never log passwords or secrets
- Sanitize PII (personally identifiable information)
- Be careful with sensitive data in exceptions
- Use separate logs for security events

---


