// ==============================================================================
// ENCRYPTION & HASHING EXAMPLES - Data Protection
// ==============================================================================
// PURPOSE:
//   Demonstrate encryption, hashing, and secure data protection patterns
//   for protecting sensitive data at rest and in transit.
//
// WHY ENCRYPTION:
//   - Protect sensitive data (passwords, PII, financial)
//   - Comply with regulations (GDPR, HIPAA, PCI-DSS)
//   - Prevent data breaches
//   - Secure communication  
//   - Digital signatures and verification
//
// WHAT YOU'LL LEARN:
//   1. Symmetric encryption (AES)
//   2. Asymmetric encryption (RSA)
//   3. Hashing (SHA256, bcrypt)
//   4. Password hashing best practices
//   5. Data Protection API
//   6. HMAC for message authentication
//   7. Secure random generation
//
// KEY CONCEPTS:
//   - Encryption: Reversible (can decrypt)
//   - Hashing: One-way (cannot reverse)
//   - Symmetric: Same key for encrypt/decrypt (fast, shared secret)
//   - Asymmetric: Public/private key pair (slow, no shared secret needed)
//
// ==============================================================================

using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

namespace RevisionNotesDemo.Security;

/// <summary>
/// EXAMPLE 1: SYMMETRIC ENCRYPTION (AES)
/// 
/// THE PROBLEM:
/// Need to encrypt data for storage or transmission.
/// Must be able to decrypt later with same key.
/// 
/// THE SOLUTION:
/// Use AES (Advanced Encryption Standard) - industry standard symmetric encryption.
/// Generate random key and IV (Initialization Vector).
/// 
/// WHY IT MATTERS:
/// - Fast encryption/decryption
/// - Industry standard (NIST approved)
/// - Secure when used correctly
/// - Suitable for large data
/// 
/// BEST FOR: Database encryption, file encryption, secure storage
/// KEY SIZE: 256-bit for maximum security
/// 
/// GOTCHA: Key management is critical! Store keys securely (Key Vault, not in code)
/// SECURITY: Always use random IV for each encryption operation!
/// </summary>
public class AesEncryptionService
{
    public record EncryptedData(byte[] Ciphertext, byte[] IV);
    
    /// <summary>
    /// Encrypt data using AES-256
    /// </summary>
    public EncryptedData Encrypt(string plaintext, byte[] key)
    {
        if (key.Length != 32) throw new ArgumentException("Key must be 256 bits (32 bytes)");
        
        using var aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV(); // ✅ GOOD: Generate random IV for each encryption
        
        using var encryptor = aes.CreateEncryptor();
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var ciphertext = encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);
        
        return new EncryptedData(ciphertext, aes.IV);
    }
    
    /// <summary>
    /// Decrypt data using AES-256
    /// </summary>
    public string Decrypt(EncryptedData encryptedData, byte[] key)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = encryptedData.IV; // Use same IV that was used for encryption
        
        using var decryptor = aes.CreateDecryptor();
        var decryptedBytes = decryptor.TransformFinalBlock(
            encryptedData.Ciphertext, 0, encryptedData.Ciphertext.Length);
        
        return Encoding.UTF8.GetString(decryptedBytes);
    }
    
    /// <summary>
    /// Generate secure random 256-bit key
    /// </summary>
    public static byte[] GenerateKey()
    {
        // ✅ GOOD: Cryptographically secure random key
        using var aes = Aes.Create();
        aes.GenerateKey();
        return aes.Key;
    }
    
    // ❌ BAD: Reusing same IV
    // private static byte[] _staticIV = new byte[16];
    // aes.IV = _staticIV; // NEVER do this!
    
    // ❌ BAD: Hardcoded key in code
    // private const string HardcodedKey = "my-secret-key-123";
}

/// <summary>
/// EXAMPLE 2: PASSWORD HASHING WITH PBKDF2
/// 
/// THE PROBLEM:
/// Need to store passwords securely.
/// Plain text or simple hashing (MD5, SHA1) is insecure.
/// 
/// THE SOLUTION:
/// Use PBKDF2 (Password-Based Key Derivation Function 2) with salt and iterations.
/// Slow, salted, iterated hashing defeats rainbow tables and brute force.
/// 
/// WHY IT MATTERS:
/// - Protects passwords even if database is compromised
/// - Defeats rainbow table attacks (salt)
/// - Slows brute force attacks (iterations)
/// - Industry standard
/// 
/// BEST PRACTICE: Use bcrypt or Argon2 in production (PBKDF2 is built-in alternative)
/// ITERATIONS: 100,000+ (adjust based on performance requirements)
/// SALT: Random, unique per password
/// </summary>
public class PasswordHashingService
{
    private const int SaltSize = 32; // 256 bits
    private const int HashSize = 32; // 256 bits
    private const int Iterations = 100_000;
    
    /// <summary>
    /// Hash password with salt and iterations
    /// </summary>
    public string HashPassword(string password)
    {
        // ✅ GOOD: Generate random salt
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        
        // ✅ GOOD: Use PBKDF2 with high iteration count
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);
        
        // ✅ GOOD: Store hash and salt together (salt is not secret)
        var hashWithSalt = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashWithSalt, 0, SaltSize);
        Array.Copy(hash, 0, hashWithSalt, SaltSize, HashSize);
        
        return Convert.ToBase64String(hashWithSalt);
    }
    
    /// <summary>
    /// Verify password matches hash
    /// </summary>
    public bool VerifyPassword(string password, string hashedPassword)
    {
        var hashWithSalt = Convert.FromBase64String(hashedPassword);
        
        // Extract salt
        var salt = new byte[SaltSize];
        Array.Copy(hashWithSalt, 0, salt, 0, SaltSize);
        
        // Extract stored hash
        var storedHash = new byte[HashSize];
        Array.Copy(hashWithSalt, SaltSize, storedHash, 0, HashSize);
        
        // Hash provided password with same salt
        var computedHash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);
        
        // ✅ GOOD: Constant-time comparison (prevents timing attacks)
        return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
    }
    
    // ❌ BAD: Storing plain text passwords
    // public void SavePassword(string password) => _db.Save(password);
    
    // ❌ BAD: Simple hashing without salt
    // public string WeakHash(string password) => SHA256.HashData(Encoding.UTF8.GetBytes(password));
}

/// <summary>
/// EXAMPLE 3: ASYMMETRIC ENCRYPTION (RSA)
/// 
/// THE PROBLEM:
/// Need to encrypt data where sharing a secret key is impractical.
/// Want digital signatures to verify authenticity.
/// 
/// THE SOLUTION:
/// Use RSA asymmetric encryption with public/private key pair.
/// Public key encrypts, private key decrypts.
/// Private key signs, public key verifies.
/// 
/// WHY IT MATTERS:
/// - No shared secret needed
/// - Public key can be freely distributed
/// - Enables digital signatures
/// - Foundation of SSL/TLS
/// 
/// BEST FOR: Key exchange, digital signatures, certificate-based encryption
/// NOT FOR: Large data (slow) - use RSA to encrypt AES key, then AES for data
/// 
/// KEY SIZE: 2048-bit minimum, 4096-bit for high security
/// </summary>
public class RsaEncryptionService
{
    public record RsaKeyPair(string PublicKeyXml, string PrivateKeyXml);
    
    /// <summary>
    /// Generate RSA key pair
    /// </summary>
    public RsaKeyPair GenerateKeyPair(int keySize = 2048)
    {
        using var rsa = RSA.Create(keySize);
        return new RsaKeyPair(
            rsa.ToXmlString(false), // Public key
            rsa.ToXmlString(true)   // Private key
        );
    }
    
    /// <summary>
    /// Encrypt data with public key
    /// </summary>
    public byte[] Encrypt(string plaintext, string publicKeyXml)
    {
        using var rsa = RSA.Create();
        rsa.FromXmlString(publicKeyXml);
        
        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        
        // ✅ GOOD: Use OAEP padding (secure)
        return rsa.Encrypt(plaintextBytes, RSAEncryptionPadding.OaepSHA256);
    }
    
    /// <summary>
    /// Decrypt data with private key
    /// </summary>
    public string Decrypt(byte[] ciphertext, string privateKeyXml)
    {
        using var rsa = RSA.Create();
        rsa.FromXmlString(privateKeyXml);
        
        var decryptedBytes = rsa.Decrypt(ciphertext, RSAEncryptionPadding.OaepSHA256);
        return Encoding.UTF8.GetString(decryptedBytes);
    }
    
    /// <summary>
    /// Sign data with private key (proves authenticity)
    /// </summary>
    public byte[] SignData(string data, string privateKeyXml)
    {
        using var rsa = RSA.Create();
        rsa.FromXmlString(privateKeyXml);
        
        var dataBytes = Encoding.UTF8.GetBytes(data);
        return rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
    
    /// <summary>
    /// Verify signature with public key
    /// </summary>
    public bool VerifySignature(string data, byte[] signature, string publicKeyXml)
    {
        using var rsa = RSA.Create();
        rsa.FromXmlString(publicKeyXml);
        
        var dataBytes = Encoding.UTF8.GetBytes(data);
        return rsa.VerifyData(dataBytes, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}

/// <summary>
/// EXAMPLE 4: DATA PROTECTION API (ASP. NET Core)
/// 
/// THE PROBLEM:
/// Need to encrypt sensitive data with automatic key management.
/// Don't want to manage encryption keys manually.
/// 
/// THE SOLUTION:
/// Use ASP.NET Core Data Protection API - handles keys, rotation, encryption.
/// Purpose strings prevent cross-application decryption.
/// 
/// WHY IT MATTERS:
/// - Automatic key management and rotation
/// - Built-in lifetime limits
/// - Purpose isolation (encrypted for purpose X can't be decrypted for purpose Y)
/// - Enterprise-ready (Azure Key Vault integration)
/// 
/// BEST FOR: Cookie encryption, anti-forgery tokens, temp data, auth tickets
/// </summary>
public class DataProtectionExample
{
    private readonly IDataProtector _protector;
    
    public DataProtectionExample(IDataProtectionProvider provider)
    {
        // ✅ GOOD: Create protector with purpose string
        _protector = provider.CreateProtector("MyApp.UserData");
    }
    
    public string Protect(string plaintext)
    {
        // ✅ GOOD: Encrypt with automatic key management
        return _protector.Protect(plaintext);
    }
    
    public string Unprotect(string ciphertext)
    {
        try
        {
            return _protector.Unprotect(ciphertext);
        }
        catch (CryptographicException)
        {
            // Key expired or data corrupted
            return null!;
        }
    }
    
    // ✅ GOOD: Time-limited protection
    public string ProtectWithExpiration(string plaintext, TimeSpan lifetime)
    {
        var timeLimitedProtector = _protector.ToTimeLimitedDataProtector();
        return timeLimitedProtector.Protect(plaintext, lifetime);
    }
}

/// <summary>
/// EXAMPLE 5: SECURE RANDOM NUMBER GENERATION
/// 
/// THE PROBLEM:
/// Need cryptographically secure random values.
/// System.Random is NOT secure (predictable).
/// 
/// THE SOLUTION:
/// Use RandomNumberGenerator for security-critical randomness.
/// 
/// WHY IT MATTERS:
/// - Session tokens must be unpredictable
/// - API keys must be random
/// - Salts must be unique
/// - IVs must be random
/// 
/// SECURITY: NEVER use System.Random for security!
/// </summary>
public static class SecureRandomExamples
{
    /// <summary>
    /// Generate secure random bytes
    /// </summary>
    public static byte[] GenerateRandomBytes(int length)
    {
        // ✅ GOOD: Cryptographically secure random
        return RandomNumberGenerator.GetBytes(length);
    }
    
    /// <summary>
    /// Generate secure random token (for API keys, session IDs)
    /// </summary>
    public static string GenerateSecureToken(int byteLength = 32)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(byteLength);
        return Convert.ToBase64String(randomBytes)
            .TrimEnd('=') // Remove padding
            .Replace('+', '-') // URL-safe
            .Replace('/', '_'); // URL-safe
    }
    
    // ❌ BAD: Using System.Random for security
    // var random = new Random();
    // var token = random.Next().ToString(); // PREDICTABLE!
}
