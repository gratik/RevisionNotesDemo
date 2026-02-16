// ==============================================================================
// ENCRYPTION & HASHING EXAMPLES - Data Protection
// ==============================================================================
// WHAT IS THIS?
// -------------
// Encryption, hashing, and data protection patterns for sensitive data.
//
// WHY IT MATTERS
// --------------
// ✅ Protects sensitive data at rest and in transit
// ✅ Supports compliance and breach mitigation
//
// WHEN TO USE
// -----------
// ✅ Password storage, tokens, and PII protection
// ✅ Integrity checks and secure data exchange
//
// WHEN NOT TO USE
// ---------------
// ❌ Non-sensitive data where plaintext is required
// ❌ Weak or homegrown crypto implementations
//
// REAL-WORLD EXAMPLE
// ------------------
// Hash passwords with PBKDF2 and protect tokens with AES.
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

    public string? Unprotect(string ciphertext)
    {
        try
        {
            return _protector.Unprotect(ciphertext);
        }
        catch (CryptographicException)
        {
            // Key expired or data corrupted
            return null;
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
/// <summary>
/// EXAMPLE 6: HMAC - MESSAGE AUTHENTICATION CODES
/// 
/// THE PROBLEM:
/// Need to verify data hasn't been tampered with.
/// Hashing alone can't prove authenticity (attacker can rehash).
/// 
/// THE SOLUTION:
/// Use HMAC (Hash-based Message Authentication Code) with secret key.
/// Combines hashing with symmetric key - proves both integrity and authenticity.
/// 
/// WHY IT MATTERS:
/// - Prevents data tampering
/// - Verifies message authenticity
/// - Protects against replay attacks (with timestamps)
/// - Used in JWT tokens, API signatures, webhooks
/// 
/// BEST FOR: API request signing, webhook verification, data integrity
/// ALGORITHMS: HMACSHA256 (common), HMACSHA512 (more secure)
/// 
/// REAL-WORLD: GitHub webhook signatures, AWS API signatures
/// </summary>
public class HmacAuthenticationService
{
    private readonly byte[] _secretKey;

    public HmacAuthenticationService(byte[] secretKey)
    {
        if (secretKey.Length < 32)
            throw new ArgumentException("Secret key must be at least 256 bits (32 bytes)");

        _secretKey = secretKey;
    }

    /// <summary>
    /// Generate HMAC signature for data
    /// </summary>
    public string GenerateSignature(string data)
    {
        using var hmac = new HMACSHA256(_secretKey);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        var signatureBytes = hmac.ComputeHash(dataBytes);
        return Convert.ToBase64String(signatureBytes);
    }

    /// <summary>
    /// Verify HMAC signature
    /// </summary>
    public bool VerifySignature(string data, string signature)
    {
        // ✅ GOOD: Constant-time comparison prevents timing attacks
        var expectedBytes = ComputeSignatureBytes(data);
        if (!TryDecodeBase64(signature, out var providedBytes))
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
    }

    /// <summary>
    /// Sign API request (with timestamp to prevent replay)
    /// </summary>
    public string SignApiRequest(string method, string url, string body, DateTimeOffset timestamp)
    {
        // ✅ GOOD: Include timestamp, method, URL in signature
        var message = $"{method}|{url}|{body}|{timestamp.ToUnixTimeSeconds()}";
        return GenerateSignature(message);
    }

    /// <summary>
    /// Verify API request signature (with replay protection)
    /// </summary>
    public bool VerifyApiRequest(
        string method, string url, string body,
        DateTimeOffset timestamp, string signature,
        TimeSpan maxAge)
    {
        var now = DateTimeOffset.UtcNow;

        // ✅ GOOD: Prevent replay attacks - reject old timestamps
        if (now - timestamp > maxAge)
        {
            return false;
        }

        // ✅ GOOD: Reject requests with far-future timestamps
        if (timestamp - now > TimeSpan.FromMinutes(1))
        {
            return false;
        }

        var message = $"{method}|{url}|{body}|{timestamp.ToUnixTimeSeconds()}";
        var expectedBytes = ComputeSignatureBytes(message);
        if (!TryDecodeBase64(signature, out var providedBytes))
        {
            return false;
        }

        return CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
    }

    private byte[] ComputeSignatureBytes(string data)
    {
        using var hmac = new HMACSHA256(_secretKey);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        return hmac.ComputeHash(dataBytes);
    }

    private static bool TryDecodeBase64(string value, out byte[] result)
    {
        try
        {
            result = Convert.FromBase64String(value);
            return true;
        }
        catch (FormatException)
        {
            result = [];
            return false;
        }
    }

    // ❌ BAD: Simple hash without key (can be forged)
    // public string BadSignature(string data) => 
    //     Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(data)));
}

/// <summary>
/// EXAMPLE 7: KEY DERIVATION FUNCTIONS (KDF)
/// 
/// THE PROBLEM:
/// Need to derive encryption keys from passwords or master keys.
/// Direct use of passwords as keys is insecure.
/// 
/// THE SOLUTION:
/// Use PBKDF2 or HKDF to derive strong keys from weak inputs.
/// 
/// WHY IT MATTERS:
/// - Transforms weak passwords into strong keys
/// - Derives multiple keys from single master key
/// - Adds computational cost (makes brute force harder)
/// - Standardized and proven
/// 
/// PBKDF2: Password → Key (with salt and iterations)
/// HKDF: Master Key → Multiple derived keys
/// </summary>
public static class KeyDerivationExamples
{
    /// <summary>
    /// Derive encryption key from password using PBKDF2
    /// </summary>
    public static byte[] DeriveKeyFromPassword(string password, byte[] salt, int keyLength = 32)
    {
        // ✅ GOOD: High iteration count slows brute force
        return Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations: 100_000,
            HashAlgorithmName.SHA256,
            keyLength);
    }

    /// <summary>
    /// Derive multiple keys from master key using HKDF
    /// </summary>
    public static (byte[] EncryptionKey, byte[] MacKey) DeriveKeysFromMaster(byte[] masterKey, byte[] salt)
    {
        // Derive encryption key
        var encryptionKey = HKDF.DeriveKey(
            HashAlgorithmName.SHA256,
            masterKey,
            32, // Key length
            salt,
            Encoding.UTF8.GetBytes("encryption") // Context/purpose
        );

        // Derive MAC key (different purpose = different key)
        var macKey = HKDF.DeriveKey(
            HashAlgorithmName.SHA256,
            masterKey,
            32,
            salt,
            Encoding.UTF8.GetBytes("authentication")
        );

        return (encryptionKey, macKey);
    }

    // ❌ BAD: Using password directly as encryption key
    // var key = Encoding.UTF8.GetBytes(password); // WEAK!
}

/// <summary>
/// EXAMPLE 8: END-TO-END ENCRYPTION PATTERN
/// 
/// THE PROBLEM:
/// Need to encrypt data on sender side, only receivable by intended recipient.
/// Server should not be able to read the data.
/// 
/// THE SOLUTION:
/// Use asymmetric encryption or shared key exchange.
/// Encrypt on client, decrypt on client - server just stores encrypted data.
/// 
/// WHY IT MATTERS:
/// - Maximum privacy (zero-trust architecture)
/// - Server breach doesn't expose data
/// - Compliance (GDPR, HIPAA)
/// - User trust
/// 
/// REAL-WORLD: WhatsApp, Signal, encrypted email
/// PATTERN: Hybrid encryption (RSA for key exchange + AES for data)
/// </summary>
public class EndToEndEncryptionService
{
    /// <summary>
    /// Sender: Encrypt message for recipient using their public key
    /// Uses hybrid encryption: RSA for AES key, AES for message
    /// </summary>
    public record EncryptedMessage(
        byte[] EncryptedAesKey,      // AES key encrypted with recipient's RSA public key
        byte[] EncryptedContent,      // Message encrypted with AES
        byte[] IV                     // AES initialization vector
    );

    public EncryptedMessage EncryptForRecipient(string message, string recipientPublicKeyXml)
    {
        // Step 1: Generate random AES key for this message
        using var aes = Aes.Create();
        aes.GenerateKey();
        aes.GenerateIV();

        // Step 2: Encrypt message with AES (fast for large data)
        using var encryptor = aes.CreateEncryptor();
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var encryptedContent = encryptor.TransformFinalBlock(messageBytes, 0, messageBytes.Length);

        // Step 3: Encrypt AES key with recipient's RSA public key
        using var rsa = RSA.Create();
        rsa.FromXmlString(recipientPublicKeyXml);
        var encryptedAesKey = rsa.Encrypt(aes.Key, RSAEncryptionPadding.OaepSHA256);

        return new EncryptedMessage(encryptedAesKey, encryptedContent, aes.IV);
    }

    /// <summary>
    /// Recipient: Decrypt message using their private key
    /// </summary>
    public string DecryptFromSender(EncryptedMessage encrypted, string recipientPrivateKeyXml)
    {
        // Step 1: Decrypt AES key with recipient's RSA private key
        using var rsa = RSA.Create();
        rsa.FromXmlString(recipientPrivateKeyXml);
        var aesKey = rsa.Decrypt(encrypted.EncryptedAesKey, RSAEncryptionPadding.OaepSHA256);

        // Step 2: Decrypt message with AES key
        using var aes = Aes.Create();
        aes.Key = aesKey;
        aes.IV = encrypted.IV;

        using var decryptor = aes.CreateDecryptor();
        var decryptedBytes = decryptor.TransformFinalBlock(
            encrypted.EncryptedContent, 0, encrypted.EncryptedContent.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}

/// <summary>
/// EXAMPLE 9: COMMON ENCRYPTION MISTAKES - WHAT NOT TO DO!
/// 
/// THE PROBLEM:
/// Many developers make critical encryption mistakes.
/// Even one mistake can completely compromise security.
/// 
/// WHY IT MATTERS:
/// - Weak encryption is worse than no encryption (false sense of security)
/// - These mistakes lead to real breaches
/// - Understanding mistakes prevents repeating them
/// </summary>
public static class EncryptionMistakes
{
    // ❌ MISTAKE 1: Using ECB mode (Electronic Codebook)
    public static class EcbModeMistake
    {
        // ECB encrypts each block identically - patterns in plaintext visible in ciphertext!
        // Famous ECB penguin image shows why this is bad
        public static byte[] DontUseEcb(string plaintext, byte[] key)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = CipherMode.ECB; // ❌ NEVER USE ECB!

            using var encryptor = aes.CreateEncryptor();
            var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            return encryptor.TransformFinalBlock(plaintextBytes, 0, plaintextBytes.Length);
        }

        // ✅ GOOD: Use CBC or GCM mode with random IV
        // aes.Mode = CipherMode.CBC;
        // aes.GenerateIV();
    }

    // ❌ MISTAKE 2: Hardcoding encryption keys
    public static class HardcodedKeysMistake
    {
        // Keys in source code will be compromised!
        private const string HardcodedKey = "MySecretKey12345"; // ❌ NEVER!

        // ✅ GOOD: Load keys from secure storage
        // - Azure Key Vault
        // - AWS KMS
        // - Environment variables (better than code, but not best)
        // - Hardware Security Module (HSM) for maximum security
    }

    // ❌ MISTAKE 3: Reusing the same IV (Initialization Vector)
    public static class IvReuseMistake
    {
        private static readonly byte[] StaticIV = new byte[16]; // ❌ NEVER REUSE!

        public static byte[] DontReuseIv(string plaintext, byte[] key)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = StaticIV; // ❌ Same IV for multiple encryptions = broken!

            // If IV is reused with same key, attacker can:
            // - Detect identical messages
            // - Break encryption with XOR attacks

            using var encryptor = aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(
                Encoding.UTF8.GetBytes(plaintext), 0, plaintext.Length);
        }

        // ✅ GOOD: Generate new random IV for each encryption
        // aes.GenerateIV();
    }

    // ❌ MISTAKE 4: Using weak or deprecated algorithms
    public static class WeakAlgorithmsMistake
    {
        // DES (56-bit key) - cracked in hours
        // RC4 - multiple known attacks
        // MD5 - collision attacks
        // SHA1 - deprecated for signatures

        public static byte[] DontUseDes(string plaintext, byte[] key)
        {
            // ❌ DES is completely broken!
            // Can be brute-forced in hours with GPU
            throw new NotSupportedException("DES is insecure - use AES!");
        }

        // ✅ GOOD: Use modern algorithms
        // - AES-256 for symmetric encryption
        // - RSA-2048+ for asymmetric encryption
        // - SHA-256 or SHA-512 for hashing
        // - HMACSHA256 for MAC
    }

    // ❌ MISTAKE 5: Not authenticating encrypted data (encryption without integrity)
    public static class NoAuthenticationMistake
    {
        // Encryption alone doesn't prevent tampering!
        // Attacker can modify ciphertext and cause:
        // - Padding oracle attacks
        // - Bit flipping attacks

        // ❌ BAD: Encrypt without MAC
        public static byte[] EncryptOnly(string plaintext, byte[] key)
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(
                Encoding.UTF8.GetBytes(plaintext), 0, plaintext.Length);
            // No integrity check! Attacker can modify ciphertext
        }

        // ✅ GOOD: Use Authenticated Encryption (AEAD)
        // - AES-GCM (Galois/Counter Mode)
        // - ChaCha20-Poly1305
        // - Or: Encrypt-then-MAC (AES-CBC + HMAC)
    }

    // ❌ MISTAKE 6: Rolling your own crypto
    public static class CustomCryptoMistake
    {
        // "I'll just XOR with a password" - famous last words!
        public static byte[] DontRollYourOwn(string plaintext, string password)
        {
            // ❌ Custom encryption almost always has fatal flaws
            var result = new byte[plaintext.Length];
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            for (int i = 0; i < plaintext.Length; i++)
            {
                // Simple XOR - broken by frequency analysis!
                result[i] = (byte)(plaintext[i] ^ passwordBytes[i % passwordBytes.Length]);
            }

            return result; // Absolutely insecure!
        }

        // ✅ GOOD: Use established cryptography libraries
        // - .NET's built-in crypto classes
        // - Libsodium (NaCl)
        // - BouncyCastle
        // "Don't roll your own crypto" - Security Proverb #1
    }

    // ❌ MISTAKE 7: Storing passwords in plaintext or weakly hashed
    public static class PasswordStorageMistake
    {
        // ❌ NEVER store passwords like this:
        // 1. Plaintext: "password123" - instant breach
        // 2. Simple hash: SHA256(password) - rainbow table attack
        // 3. MD5: Broken, fast to crack

        // ✅ GOOD: Use password hashing algorithms:
        // - Argon2id (BEST - winner of password hashing competition)
        // - bcrypt (GOOD - widely used, proven)
        // - scrypt (GOOD - memory-hard)
        // - PBKDF2 (ACCEPTABLE - built into .NET, use 100k+ iterations)

        // All require:
        // - Unique salt per password
        // - High computational cost (iterations/memory)
        // - Comparison must be constant-time
    }

    // ❌ MISTAKE 8: Not handling keys securely in memory
    public static class KeyMemoryMistake
    {
        // Keys as strings stay in memory until garbage collected
        // Can be read from memory dumps or swap files

        public static void DontUseStringForKeys()
        {
            string key = "my-secret-key"; // ❌ String is immutable, stays in memory!
            // Use the key...
            // String lingers in memory even after method returns
        }

        // ✅ GOOD: Use SecureString or byte[] and zero after use
        public static void SecureKeyHandling()
        {
            byte[] key = RandomNumberGenerator.GetBytes(32);
            try
            {
                // Use key...
            }
            finally
            {
                // ✅ Zero out key in memory
                Array.Clear(key, 0, key.Length);
            }
        }
    }
}

/// <summary>
/// DEMONSTRATION RUNNER
/// Shows all encryption examples
/// </summary>
public static class EncryptionExamplesRunner
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  ENCRYPTION & HASHING - Best Practices");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");

        RunSymmetricEncryption();
        RunPasswordHashing();
        RunAsymmetricEncryption();
        RunHmacAuthentication();
        RunKeyDerivation();
        RunEndToEndEncryption();
        ShowCommonMistakes();
        ShowBestPracticesSummary();
    }

    private static void RunSymmetricEncryption()
    {
        Console.WriteLine("🔐 1. SYMMETRIC ENCRYPTION (AES-256):\n");

        var service = new AesEncryptionService();
        var key = AesEncryptionService.GenerateKey();

        var plaintext = "Sensitive data: Credit card 4532-1234-5678-9010";
        var encrypted = service.Encrypt(plaintext, key);
        var decrypted = service.Decrypt(encrypted, key);

        Console.WriteLine($"  Original:  {plaintext}");
        Console.WriteLine($"  Encrypted: {Convert.ToBase64String(encrypted.Ciphertext)}");
        Console.WriteLine($"  IV:        {Convert.ToBase64String(encrypted.IV)}");
        Console.WriteLine($"  Decrypted: {decrypted}");
        Console.WriteLine($"  ✅ Match: {plaintext == decrypted}\n");
    }

    private static void RunPasswordHashing()
    {
        Console.WriteLine("🔑 2. PASSWORD HASHING (PBKDF2):\n");

        var service = new PasswordHashingService();
        var password = "SecureP@ssw0rd!";

        var hash1 = service.HashPassword(password);
        var hash2 = service.HashPassword(password); // Different hash due to random salt

        Console.WriteLine($"  Password:      {password}");
        Console.WriteLine($"  Hash 1:        {hash1[..50]}...");
        Console.WriteLine($"  Hash 2:        {hash2[..50]}...");
        Console.WriteLine($"  Hashes differ: {hash1 != hash2} (random salt)");
        Console.WriteLine($"  Password OK:   {service.VerifyPassword(password, hash1)}");
        Console.WriteLine($"  Wrong password:{service.VerifyPassword("WrongPassword", hash1)}\n");
    }

    private static void RunAsymmetricEncryption()
    {
        Console.WriteLine("🔒 3. ASYMMETRIC ENCRYPTION (RSA-2048):\n");

        var service = new RsaEncryptionService();
        var keyPair = service.GenerateKeyPair(2048);

        var message = "Confidential message";
        var encrypted = service.Encrypt(message, keyPair.PublicKeyXml);
        var decrypted = service.Decrypt(encrypted, keyPair.PrivateKeyXml);

        Console.WriteLine($"  Message:   {message}");
        Console.WriteLine($"  Encrypted: {Convert.ToBase64String(encrypted)}");
        Console.WriteLine($"  Decrypted: {decrypted}");
        Console.WriteLine($"  ✅ Match:  {message == decrypted}");

        // Digital signature
        var signature = service.SignData(message, keyPair.PrivateKeyXml);
        var isValid = service.VerifySignature(message, signature, keyPair.PublicKeyXml);

        Console.WriteLine($"\n  Signature: {Convert.ToBase64String(signature)[..40]}...");
        Console.WriteLine($"  Valid:     {isValid}\n");
    }

    private static void RunHmacAuthentication()
    {
        Console.WriteLine("✍️  4. HMAC AUTHENTICATION:\n");

        var secretKey = RandomNumberGenerator.GetBytes(32);
        var service = new HmacAuthenticationService(secretKey);

        var data = "{\"userId\":123,\"action\":\"transfer\",\"amount\":1000}";
        var signature = service.GenerateSignature(data);
        var isValid = service.VerifySignature(data, signature);

        Console.WriteLine($"  Data:      {data}");
        Console.WriteLine($"  Signature: {signature}");
        Console.WriteLine($"  Valid:     {isValid}");

        // Tampering detection
        var tamperedData = data.Replace("1000", "9999");
        var stillValid = service.VerifySignature(tamperedData, signature);
        Console.WriteLine($"\n  Tampered:  {tamperedData}");
        Console.WriteLine($"  Valid:     {stillValid} ❌ (tampering detected!)\n");
    }

    private static void RunKeyDerivation()
    {
        Console.WriteLine("🔑 5. KEY DERIVATION:\n");

        var password = "user-password";
        var salt = RandomNumberGenerator.GetBytes(32);

        var derivedKey = KeyDerivationExamples.DeriveKeyFromPassword(password, salt);
        Console.WriteLine($"  Password:     {password}");
        Console.WriteLine($"  Salt:         {Convert.ToBase64String(salt)[..40]}...");
        Console.WriteLine($"  Derived Key:  {Convert.ToBase64String(derivedKey)}");

        // Multiple keys from master
        var masterKey = RandomNumberGenerator.GetBytes(32);
        var (encKey, macKey) = KeyDerivationExamples.DeriveKeysFromMaster(masterKey, salt);

        Console.WriteLine($"\n  Master Key:   {Convert.ToBase64String(masterKey)[..40]}...");
        Console.WriteLine($"  Encrypt Key:  {Convert.ToBase64String(encKey)[..40]}...");
        Console.WriteLine($"  MAC Key:      {Convert.ToBase64String(macKey)[..40]}...\n");
    }

    private static void RunEndToEndEncryption()
    {
        Console.WriteLine("🔐 6. END-TO-END ENCRYPTION:\n");

        // Alice and Bob scenario
        var rsaService = new RsaEncryptionService();
        var bobKeys = rsaService.GenerateKeyPair(2048);

        var e2eService = new EndToEndEncryptionService();
        var message = "Secret message from Alice to Bob";

        // Alice encrypts for Bob using Bob's public key
        var encrypted = e2eService.EncryptForRecipient(message, bobKeys.PublicKeyXml);

        Console.WriteLine($"  Alice sends: {message}");
        Console.WriteLine($"  Encrypted:   {Convert.ToBase64String(encrypted.EncryptedContent)[..40]}...");
        Console.WriteLine($"  AES Key (encrypted): {Convert.ToBase64String(encrypted.EncryptedAesKey)[..40]}...");

        // Bob decrypts with his private key
        var decrypted = e2eService.DecryptFromSender(encrypted, bobKeys.PrivateKeyXml);

        Console.WriteLine($"  Bob receives:{decrypted}");
        Console.WriteLine($"  ✅ Secure end-to-end communication!\n");
    }

    private static void ShowCommonMistakes()
    {
        Console.WriteLine("❌ 7. COMMON MISTAKES TO AVOID:\n");
        Console.WriteLine("  1. Using ECB mode (patterns leak)");
        Console.WriteLine("  2. Hardcoding encryption keys in source code");
        Console.WriteLine("  3. Reusing the same IV (Initialization Vector)");
        Console.WriteLine("  4. Using weak algorithms (DES, RC4, MD5)");
        Console.WriteLine("  5. Encryption without authentication (no MAC)");
        Console.WriteLine("  6. Rolling your own crypto");
        Console.WriteLine("  7. Storing passwords in plaintext or simple hash");
        Console.WriteLine("  8. Not clearing keys from memory\n");
    }

    private static void ShowBestPracticesSummary()
    {
        Console.WriteLine("✅ BEST PRACTICES SUMMARY:\n");
        Console.WriteLine("  ENCRYPTION:");
        Console.WriteLine("    • Use AES-256 for symmetric encryption");
        Console.WriteLine("    • Use RSA-2048+ for asymmetric encryption");
        Console.WriteLine("    • Always use random IV for each encryption");
        Console.WriteLine("    • Use authenticated encryption (AES-GCM or Encrypt-then-MAC)");
        Console.WriteLine("    • Never use ECB mode\n");

        Console.WriteLine("  HASHING:");
        Console.WriteLine("    • Use SHA-256 or SHA-512 for hashing");
        Console.WriteLine("    • Use Argon2, bcrypt, or PBKDF2 for passwords");
        Console.WriteLine("    • Always use unique random salt per password");
        Console.WriteLine("    • Use HMAC for message authentication\n");

        Console.WriteLine("  KEY MANAGEMENT:");
        Console.WriteLine("    • Never hardcode keys in source code");
        Console.WriteLine("    • Use Azure Key Vault, AWS KMS, or HSM");
        Console.WriteLine("    • Rotate keys regularly");
        Console.WriteLine("    • Clear keys from memory after use");
        Console.WriteLine("    • Use key derivation for passwords\n");

        Console.WriteLine("  GENERAL:");
        Console.WriteLine("    • Don't roll your own crypto");
        Console.WriteLine("    • Use established libraries (.NET Crypto, Libsodium)");
        Console.WriteLine("    • Keep crypto libraries updated");
        Console.WriteLine("    • Use constant-time comparisons");
        Console.WriteLine("    • Include timestamps to prevent replay attacks\n");
    }
}
