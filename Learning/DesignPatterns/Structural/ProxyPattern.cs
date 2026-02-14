// ============================================================================
// PROXY PATTERN
// Reference: Revision Notes - Design Patterns (Structural) - Page 3
// ============================================================================
// PURPOSE: "Provides a placeholder for another object to control access to it."
// EXAMPLE: Lazy-loading large images in a gallery.
// ============================================================================

namespace RevisionNotesDemo.DesignPatterns.Structural;

// Subject interface
public interface IImage
{
    void Display();
    string GetMetadata();
}

// Real subject - expensive to create
public class RealImage : IImage
{
    private string _filename;
    private byte[] _imageData = Array.Empty<byte>();

    public RealImage(string filename)
    {
        _filename = filename;
        LoadFromDisk(); // Expensive operation
    }

    private void LoadFromDisk()
    {
        Console.WriteLine($"[PROXY] Loading image from disk: {_filename}");
        System.Threading.Thread.Sleep(1000); // Simulate expensive I/O
        _imageData = new byte[1024 * 1024]; // 1MB image
        Console.WriteLine($"[PROXY] Image loaded: {_filename} ({_imageData.Length / 1024}KB)");
    }

    public void Display()
    {
        Console.WriteLine($"[PROXY] Displaying image: {_filename}");
    }

    public string GetMetadata()
    {
        return $"{_filename} (1024x768, 1MB)";
    }
}

// Proxy - controls access to real subject
public class ImageProxy : IImage
{
    private string _filename;
    private RealImage? _realImage;

    public ImageProxy(string filename)
    {
        _filename = filename;
        Console.WriteLine($"[PROXY] ImageProxy created for: {filename} (not loaded yet)");
    }

    public void Display()
    {
        // Lazy loading - create real image only when needed
        if (_realImage == null)
        {
            Console.WriteLine("[PROXY] First access - loading real image...");
            _realImage = new RealImage(_filename);
        }

        _realImage.Display();
    }

    public string GetMetadata()
    {
        // Can return metadata without loading the full image
        return $"{_filename} (proxy - not loaded)";
    }
}

// Protection Proxy example - access control
public interface IDocument
{
    void Read();
    void Write(string content);
    void Delete();
}

public class SecureDocument : IDocument
{
    private string _content = "Secure document content";

    public void Read()
    {
        Console.WriteLine($"[PROXY] Reading document: {_content}");
    }

    public void Write(string content)
    {
        _content = content;
        Console.WriteLine($"[PROXY] Document written: {content}");
    }

    public void Delete()
    {
        Console.WriteLine("[PROXY] Document deleted");
        _content = string.Empty;
    }
}

public class DocumentProxy : IDocument
{
    private readonly SecureDocument _document;
    private readonly string _userRole;

    public DocumentProxy(SecureDocument document, string userRole)
    {
        _document = document;
        _userRole = userRole;
    }

    public void Read()
    {
        // Everyone can read
        Console.WriteLine($"[PROXY] User ({_userRole}) accessing document");
        _document.Read();
    }

    public void Write(string content)
    {
        // Only editors and admins can write
        if (_userRole == "Editor" || _userRole == "Admin")
        {
            Console.WriteLine($"[PROXY] Write access granted to {_userRole}");
            _document.Write(content);
        }
        else
        {
            Console.WriteLine($"[PROXY] Write access denied for {_userRole}");
        }
    }

    public void Delete()
    {
        // Only admins can delete
        if (_userRole == "Admin")
        {
            Console.WriteLine($"[PROXY] Delete access granted to {_userRole}");
            _document.Delete();
        }
        else
        {
            Console.WriteLine($"[PROXY] Delete access denied for {_userRole}");
        }
    }
}

// Caching Proxy example
public interface IDataService
{
    string GetData(string key);
}

public class RealDataService : IDataService
{
    public string GetData(string key)
    {
        Console.WriteLine($"[PROXY] Fetching data from database: {key}");
        System.Threading.Thread.Sleep(500); // Simulate database delay
        return $"Data for {key}";
    }
}

public class CachingDataServiceProxy : IDataService
{
    private readonly RealDataService _service;
    private readonly Dictionary<string, string> _cache = new();

    public CachingDataServiceProxy(RealDataService service)
    {
        _service = service;
    }

    public string GetData(string key)
    {
        if (_cache.TryGetValue(key, out var cachedData))
        {
            Console.WriteLine($"[PROXY] Returning cached data for: {key}");
            return cachedData;
        }

        Console.WriteLine($"[PROXY] Cache miss for: {key}");
        var data = _service.GetData(key);
        _cache[key] = data;
        return data;
    }
}

// Usage demonstration
public class ProxyDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== PROXY PATTERN DEMO ===\n");

        Console.WriteLine("--- Example 1: Virtual Proxy (Lazy Loading) ---");

        // Create proxies (cheap)
        var image1 = new ImageProxy("photo1.jpg");
        var image2 = new ImageProxy("photo2.jpg");
        var image3 = new ImageProxy("photo3.jpg");

        Console.WriteLine("\n[PROXY] Proxies created, but images not loaded yet");
        Console.WriteLine($"[PROXY] Image 1 metadata: {image1.GetMetadata()}");

        Console.WriteLine("\n[PROXY] Now displaying image 1 (triggers loading):");
        image1.Display();

        Console.WriteLine("\n[PROXY] Displaying image 1 again (already loaded):");
        image1.Display();

        Console.WriteLine("\n--- Example 2: Protection Proxy (Access Control) ---");

        var secureDoc = new SecureDocument();

        var readerProxy = new DocumentProxy(secureDoc, "Reader");
        readerProxy.Read(); // Allowed
        readerProxy.Write("New content"); // Denied
        readerProxy.Delete(); // Denied

        Console.WriteLine();
        var editorProxy = new DocumentProxy(secureDoc, "Editor");
        editorProxy.Read(); // Allowed
        editorProxy.Write("Editor's content"); // Allowed
        editorProxy.Delete(); // Denied

        Console.WriteLine();
        var adminProxy = new DocumentProxy(secureDoc, "Admin");
        adminProxy.Read(); // Allowed
        adminProxy.Write("Admin's content"); // Allowed
        adminProxy.Delete(); // Allowed

        Console.WriteLine("\n--- Example 3: Caching Proxy ---");

        var dataService = new CachingDataServiceProxy(new RealDataService());

        Console.WriteLine("\n[PROXY] First request (will hit database):");
        var data1 = dataService.GetData("user:123");

        Console.WriteLine("\n[PROXY] Second request for same key (cached):");
        var data2 = dataService.GetData("user:123");

        Console.WriteLine("\n[PROXY] Different key (will hit database):");
        var data3 = dataService.GetData("user:456");

        Console.WriteLine("\n💡 Benefit: Controls access to expensive or sensitive objects");
        Console.WriteLine("💡 Types: Virtual (lazy loading), Protection (access control), Caching");
        Console.WriteLine("💡 Use case: Large objects, security, performance optimization");
    }
}
