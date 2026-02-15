// ==============================================================================
// Azure Key Vault for Secrets & Certificate Management
// ==============================================================================
// WHAT IS THIS?
// Azure Key Vault is a cloud service for securely managing secrets, 
// cryptographic keys, and certificates with audit logging, access policies,
// and automatic rotation capabilities. Prevents secrets from code/configs.
//
// WHY IT MATTERS
// ✅ SECURE: Hardware security module (HSM) backing available
// ✅ AUDIT: Every access logged for compliance
// ✅ ROTATION: Automatic certificate and secret renewal
// ✅ IDENTITY: Managed Identity (no passwords needed)
// ✅ SEPARATION: Secrets never appear in code or configs
//
// WHEN TO USE
// ✅ Database connection strings
// ✅ API keys and authentication tokens
// ✅ SSL certificates for HTTPS endpoints
// ✅ Encryption keys for customer data
// ✅ Compliance with PCI-DSS, HIPAA, SOC2
//
// WHEN NOT TO USE
// ❌ Performance-critical paths <10ms (HSM adds latency)
// ❌ Development with frequent secret changes
//
// REAL-WORLD EXAMPLE
// SaaS application: Database password in Key Vault, Function App uses
// Managed Identity (no password in config), Key Vault rotates password
// monthly, old connections gracefully timeout, compliance audit shows
// all access history, zero secrets in source code.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Cloud;

public class AzureKeyVaultSecrets
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Azure Key Vault Management");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");
        
        Overview();
        SecretTypes();
        AccessControl();
        RotationPatterns();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Key Vault provides a secure location for storing secrets,");
        Console.WriteLine("eliminating the need to pass credentials through code or");
        Console.WriteLine("configuration files.\n");
    }

    private static void SecretTypes()
    {
        Console.WriteLine("🔑 TYPES OF SECRETS:\n");
        Console.WriteLine("  • Secrets: Database passwords, API keys, tokens");
        Console.WriteLine("  • Keys: Cryptographic keys for encryption");
        Console.WriteLine("  • Certificates: SSL/TLS certificates");
        Console.WriteLine("  • Storage Accounts: Azure Storage credentials\n");
    }

    private static void AccessControl()
    {
        Console.WriteLine("🔐 ACCESS CONTROL:\n");
        Console.WriteLine("  • RBAC: Role-based access control");
        Console.WriteLine("  • Managed Identity: App services authenticate without secrets");
        Console.WriteLine("  • VNet: Restrict access to specific networks");
        Console.WriteLine("  • Audit: Log all access for compliance\n");
    }

    private static void RotationPatterns()
    {
        Console.WriteLine("🔄 SECRET ROTATION:\n");
        Console.WriteLine("  • Automatic: Scheduled rotation via Azure Functions");
        Console.WriteLine("  • Graceful: Old secrets remain valid during transition");
        Console.WriteLine("  • Versioning: Multiple versions of secret available");
        Console.WriteLine("  • Monitoring: Alerts for expiration approaching\n");
    }
}
