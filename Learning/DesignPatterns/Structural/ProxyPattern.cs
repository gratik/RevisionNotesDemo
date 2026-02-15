// ============================================================================
// PROXY PATTERN - Control Access to Objects with Surrogate
// Reference: Revision Notes - Design Patterns (Structural) - Page 3
// ============================================================================
//
// WHAT IS THE PROXY PATTERN?
// ---------------------------
// Provides a surrogate or placeholder for another object to control access to it.
// The proxy has the same interface as the real object but adds additional
// functionality like lazy loading, access control, caching, or logging.
//
// Think of it as: "Credit card (proxy) for bank account (real object) - same
// purchasing power but adds security checks and logging"
//
// Core Concepts:
//   • Subject: Interface that both Proxy and RealSubject implement
//   • RealSubject: The actual object being proxied
//   • Proxy: Controls access to RealSubject
//   • Transparent: Client uses proxy exactly like real object
//   • Delegation: Proxy forwards requests to RealSubject (when appropriate)
//
// WHY IT MATTERS
// --------------
// ✅ LAZY INITIALIZATION: Create expensive objects only when needed
// ✅ ACCESS CONTROL: Check permissions before allowing operations
// ✅ CACHING: Store results to avoid repeated expensive operations
// ✅ LOGGING: Track all access to object for audit/debug
// ✅ REMOTE ACCESS: Represent remote objects locally (RPC, WCF)
// ✅ REFERENCE COUNTING: Track object usage for resource management
//
// TYPES OF PROXIES
// ----------------
// 1. VIRTUAL PROXY (Lazy Loading):
//    Delays creation of expensive object until first use
//    Example: Load 100MB image only when user scrolls to it
//
// 2. PROTECTION PROXY (Access Control):
//    Controls access based on permissions/roles
//    Example: Admin-only operations
//
// 3. REMOTE PROXY:
//    Represents object in different address space
//    Example: WCF service proxy, REST API client
//
// 4. CACHING PROXY:
//    Stores results of expensive operations
//    Example: Cache database query results
//
// 5. LOGGING PROXY:
//    Logs all method calls
//    Example: Audit trail for sensitive operations
//
// 6. SMART REFERENCE:
//    Additional actions when object is accessed
//    Example: Reference counting for garbage collection
//
// WHEN TO USE IT
// --------------
// ✅ Lazy initialization: Expensive object creation should be deferred
// ✅ Access control: Need to check permissions before operations
// ✅ Remote object: Object is in different process/machine
// ✅ Caching: Avoid repeated expensive operations
// ✅ Logging/Monitoring: Track object usage
// ✅ Resource management: Control lifecycle of expensive resources
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Object creation is cheap (unnecessary overhead)
// ❌ No need for access control, lazy loading, or caching
// ❌ Direct access is acceptable
// ❌ Proxy logic becomes more complex than real object
// ❌ Performance overhead not justified
//
// REAL-WORLD EXAMPLE - Image Gallery
// -----------------------------------
// Photo gallery app with 1000 high-res images:
//   • Each image: 10MB (10GB total!)
//   • User scrolls through thumbnails
//   • Loading all images upfront = app crash (out of memory)
//
// Without Proxy:
//   → var images = new List<RealImage>();
//   → for (int i = 0; i < 1000; i++) {
//         images.Add(new RealImage($"photo{i}.jpg")); // Loads 10MB immediately!
//     }
//   → 10GB loaded into memory
//   → App crashes or becomes unusably slow
//   → User only views 5-10 images per session
//   → Wasted 99% of memory on unseen images
//
// With Proxy (Virtual Proxy):
//   → var images = new List<IImage>();
//   → for (int i = 0; i < 1000; i++) {
//         images.Add(new ImageProxy($"photo{i}.jpg")); // Just filename, no load!
//     }
//   → ImageProxy.Display() checks if loaded:
//       - If not loaded: Load image from disk (10MB)
//       - If loaded: Use cached instance
//   → Only images actually viewed are loaded
//   → Memory usage: 50-100MB instead of 10GB
//   → Fast startup, responsive scrolling
//
// ANOTHER EXAMPLE - Database Access Control
// -----------------------------------------
// Banking system:
//   • CustomerRepository allows all operations
//   • Only admins should delete customers
//   • Regular users can only read
//
// Protection Proxy:
//   public class ProtectedCustomerRepository : ICustomerRepository
//   {
//       private readonly ICustomerRepository _realRepo;
//       private readonly IUser _currentUser;
//
//       public void Delete(int id)
//       {
//           if (!_currentUser.IsAdmin)
//               throw new UnauthorizedAccessException("Admin only!");
//           
//           _realRepo.Delete(id); // Only executes if authorized
//       }
//
//       public Customer Get(int id)
//       {
//           // Anyone can read
//           return _realRepo.Get(id);
//       }
//   }
//
// ANOTHER EXAMPLE - Caching Proxy
// -------------------------------
// Expensive web API calls:
//   public class CachingWeatherProxy : IWeatherService
//   {
//       private readonly IWeatherService _realService;
//       private readonly Dictionary<string, (Weather data, DateTime cached)> _cache;
//
//       public Weather GetWeather(string city)
//       {
//           if (_cache.TryGetValue(city, out var cached))
//           {
//               if (DateTime.Now - cached.cached < TimeSpan.FromHours(1))
//                   return cached.data; // Return cached data
//           }
//
//           var weather = _realService.GetWeather(city); // Expensive API call
//           _cache[city] = (weather, DateTime.Now);
//           return weather;
//       }
//   }
//
// .NET FRAMEWORK EXAMPLES
// -----------------------
// Proxy pattern used extensively in .NET:
//   • WCF Service Proxies: Represent remote services
//   • Entity Framework: DbContext uses proxies for lazy loading
//   • Lazy<T>: Virtual proxy for lazy initialization
//   • Dynamic proxies: Castle DynamicProxy, DispatchProxy
//
// PROXY VS SIMILAR PATTERNS
// -------------------------
// Proxy vs Decorator:
//   • Proxy: Same interface, controls access (protection, lazy loading)
//   • Decorator: Same interface, adds functionality
//
// Proxy vs Adapter:
//   • Proxy: Same interface as real object
//   • Adapter: Converts one interface to another
//
// Proxy vs Facade:
//   • Proxy: One-to-one relationship with real object
//   • Facade: Simplifies access to entire subsystem
//
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
