// ==============================================================================
// Encryption at Rest & In Transit
// ==============================================================================
// WHAT IS THIS?
// Encryption at rest protects data stored on disk (databases, files). Encryption in transit protects data moving between systems (HTTPS, TLS). Together they form the foundation of data security.
//
// WHY IT MATTERS
// ✅ COMPLIANCE: GDPR, PCI-DSS, HIPAA require encryption | ✅ DATA BREACH PREVENTION: Encrypted data useless without keys | ✅ HTTPS REQUIRED: Modern browsers require TLS/SSL | ✅ CERTIFICATE PINNING: Prevent man-in-the-middle attacks | ✅ KEY ROTATION: Regularly update encryption keys | ✅ PERFORMANCE: Modern TLS adds <1ms latency
//
// WHEN TO USE
// ✅ Always for sensitive data (passwords, PII, health records) | ✅ All database fields with personal info | ✅ All network communication (HTTPS only) | ✅ Backup data | ✅ API keys and secrets
//
// WHEN NOT TO USE
// ❌ Public data (doesn't need encryption)
//
// REAL-WORLD EXAMPLE
// Healthcare database: Patient records encrypted at rest (AES-256). Patient accesses via HTTPS (TLS 1.3). Even if database stolen, encrypted. Even if network intercepted, TLS protects.
// ==============================================================================

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace RevisionNotesDemo.Security;

public class EncryptionAtRestAndInTransit
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Encryption at Rest & In Transit");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");

        Overview();
        EncryptionAtRest();
        EncryptionInTransit();
        KeyManagement();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Two critical encryption scenarios:");
        Console.WriteLine("  1. AT REST: Data on disk (database, files, backups)");
        Console.WriteLine("  2. IN TRANSIT: Data on network (APIs, databases, messages)\n");
    }

    private static void EncryptionAtRest()
    {
        Console.WriteLine("🔐 ENCRYPTION AT REST:\n");

        Console.WriteLine("Methods:");
        Console.WriteLine("  • Database-level: SQL Server TDE (Transparent Data Encryption)");
        Console.WriteLine("  • Column-level: Encrypt specific sensitive columns");
        Console.WriteLine("  • File-level: EFS (Windows) or LUKS (Linux)\n");

        Console.WriteLine("SQL Server TDE Example:");
        Console.WriteLine("  CREATE DATABASE master_key");
        Console.WriteLine("  CREATE CERTIFICATE tde_cert WITH SUBJECT = 'TDE';");
        Console.WriteLine("  CREATE DATABASE ENCRYPTION KEY");
        Console.WriteLine("    WITH ALGORITHM=AES_256 ENCRYPTION BY SERVER CERTIFICATE;\n");

        Console.WriteLine("Result: Database file on disk = encrypted");
        Console.WriteLine("Performance: <1% overhead\n");

        Console.WriteLine("Cosmos DB at rest:");
        Console.WriteLine("  By default: Encrypted with Microsoft-managed keys");
        Console.WriteLine("  Optional: Customer-managed keys (CMK) for compliance\n");
    }

    private static void EncryptionInTransit()
    {
        Console.WriteLine("🔒 ENCRYPTION IN TRANSIT:\n");

        Console.WriteLine("HTTPS/TLS Handshake:");
        Console.WriteLine("  1. Client → Server: Hello (TLS version, supported ciphers)");
        Console.WriteLine("  2. Server → Client: Certificate (public key)");
        Console.WriteLine("  3. Client verifies: Certificate valid? Signed by trusted CA?");
        Console.WriteLine("  4. Key exchange: Both derive symmetric session key");
        Console.WriteLine("  5. All data: Encrypted with symmetric key\n");

        Console.WriteLine("TLS Versions (use 1.3):");
        Console.WriteLine("  ❌ SSL 2.0, 3.0: Broken (don't use)");
        Console.WriteLine("  ❌ TLS 1.0, 1.1: Deprecated");
        Console.WriteLine("  ⚠️ TLS 1.2: Acceptable");
        Console.WriteLine("  ✅ TLS 1.3: Best (recommended)\n");

        Console.WriteLine("Certificate Pinning Example:");
        Console.WriteLine("  // Don't trust just any cert");
        Console.WriteLine("  // Pin specific APIs' certificates");
        Console.WriteLine("  var handler = new HttpClientHandler();");
        Console.WriteLine("  handler.ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) =>");
        Console.WriteLine("  {");
        Console.WriteLine("    string thumbprint = cert.GetCertHashString();");
        Console.WriteLine("    // Verify thumbprint matches known value");
        Console.WriteLine("    return knownThumbprints.Contains(thumbprint);");
        Console.WriteLine("  };\n");
    }

    private static void KeyManagement()
    {
        Console.WriteLine("🔑 KEY MANAGEMENT:\n");

        Console.WriteLine("Encryption Hierarchy:");
        Console.WriteLine("  Data Encryption Key (DEK)");
        Console.WriteLine("    ↓ Encrypted by");
        Console.WriteLine("  Key Encryption Key (KEK)");
        Console.WriteLine("    ↓ Encrypted by");
        Console.WriteLine("  Master Key (in Key Vault)\n");

        Console.WriteLine("Key Rotation:");
        Console.WriteLine("  Strategy: Rotate keys every 90 days");
        Console.WriteLine("  Process:");
        Console.WriteLine("    1. Generate new key");
        Console.WriteLine("    2. Re-encrypt data with new key");
        Console.WriteLine("    3. Keep old key for grace period (reading old data)");
        Console.WriteLine("    4. Retire old key\n");

        Console.WriteLine("Azure Key Vault:");
        Console.WriteLine("  ✅ Centralized key storage");
        Console.WriteLine("  ✅ Audit logging (who accessed what key)");
        Console.WriteLine("  ✅ HSM-backed (Hardware Security Module)");
        Console.WriteLine("  ✅ Managed identity support\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✅ BEST PRACTICES:\n");

        Console.WriteLine("Data Classification:");
        Console.WriteLine("  • Public: No encryption needed");
        Console.WriteLine("  • Internal: Encryption in transit only");
        Console.WriteLine("  • Confidential: Encrypt at rest + transit + key rotation");
        Console.WriteLine("  • Restricted: Above + additional audit logging\n");

        Console.WriteLine("Implementation:");
        Console.WriteLine("  ✅ Always use HTTPS (TLS 1.2+)");
        Console.WriteLine("  ✅ Never hardcode encryption keys");
        Console.WriteLine("  ✅ Use strong algorithm (AES-256, not DES)");
        Console.WriteLine("  ✅ Encrypt PII, health data, payment info");
        Console.WriteLine("  ✅ Rotate keys regularly");
        Console.WriteLine("  ✅ Hash passwords (never encrypt)\n");

        Console.WriteLine("Compliance:");
        Console.WriteLine("  • GDPR: Encryption required for data processing");
        Console.WriteLine("  • PCI-DSS: Strong encryption for cardholder data");
        Console.WriteLine("  • HIPAA: Encryption for protected health information\n");
    }
}
