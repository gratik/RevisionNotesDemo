# Data Encryption Examples

> Subject: [Security](../README.md)

## Data Encryption Examples

### AES Encryption (Symmetric)

```csharp
public (byte[] ciphertext, byte[] iv) Encrypt(string plaintext, byte[] key)
{
    using var aes = Aes.Create();
    aes.Key = key;  // 256-bit key
    aes.GenerateIV();  // ✅ Random IV for each encryption
    
    using var encryptor = aes.CreateEncryptor();
    var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
    var ciphertext = encryptor.TransformFinalBlock(
        plaintextBytes, 0, plaintextBytes.Length);
    
    return (ciphertext, aes.IV);  // ✅ Store IV with ciphertext
}

public string Decrypt(byte[] ciphertext, byte[] iv, byte[] key)
{
    using var aes = Aes.Create();
    aes.Key = key;
    aes.IV = iv;  // Use same IV from encryption
    
    using var decryptor = aes.CreateDecryptor();
    var decryptedBytes = decryptor.TransformFinalBlock(
        ciphertext, 0, ciphertext.Length);
    
    return Encoding.UTF8.GetString(decryptedBytes);
}
```

### ASP.NET Core Data Protection API

```csharp
// ✅ Built-in encryption/decryption with key rotation
public class SecureService
{
    private readonly IDataProtector _protector;
    
    public SecureService(IDataProtectionProvider provider)
    {
        // Purpose-specific protector
        _protector = provider.CreateProtector("MyApp.PII");
    }
    
    public string EncryptSensitiveData(string data)
    {
        return _protector.Protect(data);  // ✅ Automatic key management
    }
    
    public string DecryptSensitiveData(string encrypted)
    {
        return _protector.Unprotect(encrypted);
    }
}

// Configure in Program.cs
services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"c:\keys"))
    .SetApplicationName("MyApp");
```

---


