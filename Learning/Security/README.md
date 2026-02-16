# Security Guide

## Learning goals

- Understand the main concepts covered in Security.
- Compare baseline and recommended implementation approaches.
- Apply the patterns in runnable project examples.

## Prerequisites

- Authentication and authorization basics
- HTTP and web fundamentals

## Runnable examples

- AuthenticationExamples.cs - Topic implementation and demonstration code
- AuthorizationExamples.cs - Topic implementation and demonstration code
- BiometricAndMFAPatterns.cs - Topic implementation and demonstration code
- CertificateManagementAndTLS.cs - Topic implementation and demonstration code
- ContentSecurityPolicyCSP.cs - Topic implementation and demonstration code
- CookieSessionAndTokenManagement.cs - Topic implementation and demonstration code
- EncryptionAtRestAndInTransit.cs - Topic implementation and demonstration code
- EncryptionExamples.cs - Topic implementation and demonstration code
- IdentityServerAndOpenIDConnect.cs - Topic implementation and demonstration code
- ManagedIdentityAndAuthentication.cs - Topic implementation and demonstration code
- MultiTenantAuthentication.cs - Topic implementation and demonstration code
- OAuth2FlowsInDepth.cs - Topic implementation and demonstration code
- OWASPTop10WithExamples.cs - Topic implementation and demonstration code
- SecretRotationAndVaultPatterns.cs - Topic implementation and demonstration code
- SecureAPIDesignPatterns.cs - Topic implementation and demonstration code

Run examples from the project root:

```bash
dotnet run
```

## Additional reference examples

- AuthenticationExamples.cs - Deep-dive authentication patterns and code snippets
- AuthorizationExamples.cs - Deep-dive authorization patterns and code snippets
- SecureCodingPractices.cs - Defensive secure-coding examples and anti-patterns

## Bad vs good examples summary

- Bad: brittle or overly coupled approach that reduces maintainability.
- Good: clear, testable, and production-oriented implementation pattern.
- Why it matters: consistent patterns improve readability, reliability, and onboarding speed.

## Related docs

- [Primary](../docs/Security.md)
- [Related](../docs/Web-API-MVC.md)
