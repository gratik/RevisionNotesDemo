// ==============================================================================
// Managed Identity & Service Principal Authentication
// ==============================================================================
// WHAT IS THIS?
// Managed Identity allows Azure resources (VMs, App Services, Functions) to authenticate with Azure services WITHOUT storing credentials. Azure handles the identity lifecycle.
//
// WHY IT MATTERS
// ✅ NO SECRETS: No credentials in code/config files | ✅ AUTO-ROTATION: Azure rotates tokens automatically | ✅ AUDIT: Every authentication logged | ✅ RBAC INTEGRATION: Use Azure roles for access control | ✅ REDUCES ATTACK SURFACE: No compromised secrets | ✅ ZERO CONFIGURATION: Works out of the box
//
// WHEN TO USE
// ✅ App Service connecting to Key Vault | ✅ Function App accessing storage | ✅ VM accessing databases | ✅ Managed Kubernetes pods | ✅ Any Azure-to-Azure communication
//
// WHEN NOT TO USE
// ❌ External services (GitHub, third-party APIs) | ❌ On-premises applications | ❌ Local development (use local credentials)
//
// REAL-WORLD EXAMPLE
// Web app: Code needs to access Key Vault for secrets. Without Managed Identity: store credential in config (security risk). With Managed Identity: App Service has identity, Azure grants it permission, access is automatic, audited, zero secrets.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Security;

public class ManagedIdentityAndAuthentication
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Managed Identity & Service Principals");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");

        Overview();
        ManagedIdentityTypes();
        CodeExample();
        ServicePrincipals();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Two identity types in Azure:");
        Console.WriteLine("  1. Managed Identity: Built into Azure resources (VMs, App Service)");
        Console.WriteLine("  2. Service Principal: Application identity in Azure AD\n");

        Console.WriteLine("Use case:");
        Console.WriteLine("  App Service needs to read secrets from Key Vault");
        Console.WriteLine("  Traditional: Store Key Vault credential in config (bad!)");
        Console.WriteLine("  Managed Identity: App Service HAS an identity, Azure grants access\n");
    }

    private static void ManagedIdentityTypes()
    {
        Console.WriteLine("🔏 MANAGED IDENTITY TYPES:\n");

        Console.WriteLine("1️⃣ SYSTEM-ASSIGNED:");
        Console.WriteLine("  • One identity per resource");
        Console.WriteLine("  • Lifecycle tied to resource (deleted when resource deleted)");
        Console.WriteLine("  • No extra setup required");
        Console.WriteLine("  • Usage: Web app, Function, VM (simple cases)\n");

        Console.WriteLine("2️⃣ USER-ASSIGNED:");
        Console.WriteLine("  • Standalone identity (reused across multiple resources)");
        Console.WriteLine("  • Lifecycle independent of resources");
        Console.WriteLine("  • More flexibility (multiple resources need same permissions)");
        Console.WriteLine("  • Usage: Multiple apps needing same access\n");

        Console.WriteLine("Comparison:");
        Console.WriteLine("  System-Assigned: Simple, 1:1 mapping, auto-cleanup");
        Console.WriteLine("  User-Assigned: Complex, 1:many mapping, manual cleanup\n");
    }

    private static void CodeExample()
    {
        Console.WriteLine("💻 CODE EXAMPLE:\n");

        Console.WriteLine("// No credentials needed! Azure handles it");
        Console.WriteLine("var credential = new DefaultAzureCredential();");
        Console.WriteLine("var client = new SecretClient(");
        Console.WriteLine("  vaultUri: new Uri(\"https://mykeyvault.vault.azure.net/\"),");
        Console.WriteLine("  credential: credential);");
        Console.WriteLine("KeyVaultSecret secret = await client.GetSecretAsync(\"database-password\");\n");

        Console.WriteLine("Under the hood:");
        Console.WriteLine("  1. App Service has managed identity");
        Console.WriteLine("  2. DefaultAzureCredential() detects environment");
        Console.WriteLine("  3. Gets token from Azure AD automatically");
        Console.WriteLine("  4. Sends token with request to Key Vault");
        Console.WriteLine("  5. Key Vault verifies identity, returns secret\n");

        Console.WriteLine("No username/password needed!");
        Console.WriteLine("Token auto-refreshes every hour");
        Console.WriteLine("All access logged in Azure Monitor\n");
    }

    private static void ServicePrincipals()
    {
        Console.WriteLine("🔑 SERVICE PRINCIPALS:\n");

        Console.WriteLine("What: Application identity in Azure AD (like a user account for apps)");
        Console.WriteLine("When: External services, CI/CD pipelines, scheduled jobs\n");

        Console.WriteLine("Types:");
        Console.WriteLine("  1. Application: Represents an app, single tenant");
        Console.WriteLine("  2. Managed Service Identity: For Azure resources");
        Console.WriteLine("  3. Legacy: Service principal without app registration\n");

        Console.WriteLine("Example (GitHub Actions CI/CD):");
        Console.WriteLine("  1. Register app in Azure AD");
        Console.WriteLine("  2. Create service principal");
        Console.WriteLine("  3. Grant permissions (RBAC role)");
        Console.WriteLine("  4. Store credentials in GitHub secrets");
        Console.WriteLine("  5. GitHub Actions uses credentials to deploy\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✅ BEST PRACTICES:\n");

        Console.WriteLine("Use Managed Identity when:");
        Console.WriteLine("  ✅ Azure resource → Azure resource");
        Console.WriteLine("  ✅ Want zero secrets in code");
        Console.WriteLine("  ✅ Need automatic token rotation");
        Console.WriteLine("  ✅ Want audit logging\n");

        Console.WriteLine("Use Service Principal when:");
        Console.WriteLine("  ✅ External service needs Azure access");
        Console.WriteLine("  ✅ CI/CD pipeline (GitHub Actions, Azure DevOps)");
        Console.WriteLine("  ✅ Scheduled job outside Azure");
        Console.WriteLine("  ✅ Local development (use with AzureKeyCredential)\n");

        Console.WriteLine("Security:");
        Console.WriteLine("  ✅ Always use DefaultAzureCredential (tries multiple methods)");
        Console.WriteLine("  ✅ Grant least privileged permissions (specific roles)");
        Console.WriteLine("  ✅ Rotate service principal credentials every 90 days");
        Console.WriteLine("  ✅ Monitor audit logs for suspicious access\n");
    }
}
