// ============================================================================
// FACADE PATTERN - Simplify Complex Subsystems with Unified Interface
// Reference: Revision Notes - Design Patterns (Structural) - Page 3
// ============================================================================
//
// WHAT IS THE FACADE PATTERN?
// ----------------------------
// Provides a simplified, unified interface to a complex subsystem. Hides the
// complexity of multiple classes/APIs behind a single, easy-to-use interface.
// Acts as a "front desk" that handles interactions with complex backend systems.
//
// Think of it as: "Restaurant waiter (facade) - you order from simple menu,
// waiter coordinates kitchen, bar, dishes without you knowing details"
//
// Core Concepts:
//   • Facade: Simplified interface to complex subsystem
//   • Subsystem Classes: Complex components doing actual work
//   • Client: Uses facade instead of subsystem directly
//   • Delegation: Facade delegates work to subsystem classes
//   • Simplification: Reduces learning curve and coupling
//
// WHY IT MATTERS
// --------------
// ✅ SIMPLICITY: Easy-to-use interface over complex system
// ✅ DECOUPLING: Clients don't depend on subsystem internals
// ✅ LAYERING: Clear separation between high-level and low-level code
// ✅ REDUCED LEARNING CURVE: New developers learn one facade, not 20 classes
// ✅ FLEXIBILITY: Can change subsystem without breaking client code
// ✅ COMMON OPERATIONS: Provides convenient methods for frequent tasks
//
// WHEN TO USE IT
// --------------
// ✅ Complex subsystem with many interconnected classes
// ✅ Want to provide simple interface for common use cases
// ✅ Reduce dependencies between client and subsystem
// ✅ Layer your application (presentation → facade → business → data)
// ✅ Need to wrap poorly designed or legacy API
// ✅ Multiple subsystems need coordination for simple task
//
// WHEN NOT TO USE IT
// ------------------
// ❌ System is already simple (unnecessary layer)
// ❌ Clients need fine-grained control over subsystem
// ❌ Facade becomes a "god object" doing too much
// ❌ Over-simplification hides needed functionality
// ❌ Just wrapping without adding value
//
// REAL-WORLD EXAMPLE
// ------------------
// Imagine smart home automation system:
//   • Dozens of devices: Lights, thermostat, locks, cameras, alarm, garage
//   • Each has complex API: LightingSystem.SetZone(1).SetBrightness(80).SetColor(RGB)...
//   • User wants simple: "Goodnight mode" or "Movie mode"
//
// Without Facade:
//   → livingRoomLights.SetBrightness(0);
//   → bedroomLights.SetBrightness(0);
//   → kitchenLights.SetBrightness(0);
//   → thermostat.SetTemperature(68);
//   → thermostat.SetMode("Sleep");
//   → securitySystem.Arm();
//   → garageDoor.CheckClosed();
//   → if (!garageDoor.IsClosed()) garageDoor.Close();
//   → frontDoorLock.Engage();
//   → backDoorLock.Engage();
//   → 15+ lines of code for one action!
//   → User must remember all steps
//   → If new device added, change code everywhere
//
// With Facade:
//   → smartHomeFacade.ActivateGoodnightMode();
//   → ONE line, facade handles all complexity internally
//   → Facade coordinates all subsystems
//   → Easy to add "Movie mode", "Away mode", "Morning mode"
//   → Adding new device = update facade only, client code unchanged
//
// ANOTHER EXAMPLE - Order Processing
// ----------------------------------
// E-commerce: Place order involves many subsystems:
//   • InventoryService.CheckStock()
//   • PaymentGateway.AuthorizeCard()
//   • ShippingService.CalculateRate()
//   • ShippingService.CreateLabel()
//   • EmailService.SendConfirmation()
//   • LoyaltyService.AddPoints()
//   • AnalyticsService.TrackPurchase()
//
// OrderFacade.PlaceOrder(order) handles all of this internally:
//   public class OrderFacade
//   {
//       public async Task<OrderResult> PlaceOrder(Order order)
//       {
//           if (!await _inventory.CheckStock(order.Items)) 
//               return OrderResult.OutOfStock;
//           
//           if (!await _payment.Authorize(order.Payment))
//               return OrderResult.PaymentFailed;
//           
//           await _shipping.CreateShipment(order);
//           await _email.SendConfirmation(order);
//           await _loyalty.AddPoints(order.Customer, order.Total);
//           await _analytics.Track("Purchase", order);
//           
//           return OrderResult.Success;
//       }
//   }
//
// Client just calls: var result = await orderFacade.PlaceOrder(order);
//
// .NET FRAMEWORK EXAMPLES
// -----------------------
// .NET uses Facade pattern in several places:
//   • DbContext (EF Core): Facade over complex query/tracking/cache systems
//   • HttpClient: Facade over complex HTTP stack
//   • IConfiguration: Facade over multiple config sources
//   • ILogger: Facade over complex logging infrastructure
//
// BEST PRACTICES
// --------------
// ✅ Keep facade simple - delegate complexity to subsystem
// ✅ Facade should not contain business logic
// ✅ Don't prevent advanced users from accessing subsystem directly
// ✅ One facade per subsystem (don't create mega-facade)
// ✅ Use dependency injection for subsystem components
//
// ============================================================================

namespace RevisionNotesDemo.DesignPatterns.Structural;

// Complex subsystem classes
public class VideoCodec
{
    public void Initialize()
    {
        Console.WriteLine("[FACADE] VideoCodec: Initializing codec");
    }

    public void Decode(string filename)
    {
        Console.WriteLine($"[FACADE] VideoCodec: Decoding {filename}");
    }
}

public class AudioMixer
{
    public void Setup()
    {
        Console.WriteLine("[FACADE] AudioMixer: Setting up audio mixer");
    }

    public void AdjustVolume(int level)
    {
        Console.WriteLine($"[FACADE] AudioMixer: Adjusting volume to {level}%");
    }

    public void Mix()
    {
        Console.WriteLine("[FACADE] AudioMixer: Mixing audio tracks");
    }
}

public class VideoRenderer
{
    public void Initialize()
    {
        Console.WriteLine("[FACADE] VideoRenderer: Initializing renderer");
    }

    public void Render(string filename)
    {
        Console.WriteLine($"[FACADE] VideoRenderer: Rendering {filename}");
    }
}

public class SubtitleEngine
{
    public void Load(string subtitleFile)
    {
        Console.WriteLine($"[FACADE] SubtitleEngine: Loading subtitles from {subtitleFile}");
    }

    public void Synchronize()
    {
        Console.WriteLine("[FACADE] SubtitleEngine: Synchronizing subtitles with video");
    }
}

// Facade - provides a simple interface
public class MultimediaFacade
{
    private readonly VideoCodec _codec;
    private readonly AudioMixer _audioMixer;
    private readonly VideoRenderer _renderer;
    private readonly SubtitleEngine _subtitles;

    public MultimediaFacade()
    {
        _codec = new VideoCodec();
        _audioMixer = new AudioMixer();
        _renderer = new VideoRenderer();
        _subtitles = new SubtitleEngine();

        Console.WriteLine("[FACADE] Multimedia facade initialized\n");
    }

    // Simplified interface - hides complexity
    public void PlayVideo(string filename, string subtitleFile = "", int volume = 50)
    {
        Console.WriteLine($"[FACADE] === Playing video: {filename} ===");

        // Complex initialization handled internally
        _codec.Initialize();
        _renderer.Initialize();
        _audioMixer.Setup();

        // Setup video
        _codec.Decode(filename);
        _renderer.Render(filename);

        // Setup audio
        _audioMixer.AdjustVolume(volume);
        _audioMixer.Mix();

        // Setup subtitles if provided
        if (!string.IsNullOrEmpty(subtitleFile))
        {
            _subtitles.Load(subtitleFile);
            _subtitles.Synchronize();
        }

        Console.WriteLine($"[FACADE] === Now playing: {filename} ===\n");
    }

    public void ConvertVideo(string inputFile, string outputFile)
    {
        Console.WriteLine($"[FACADE] === Converting video: {inputFile} -> {outputFile} ===");
        _codec.Initialize();
        _codec.Decode(inputFile);
        _renderer.Initialize();
        _renderer.Render(outputFile);
        Console.WriteLine("[FACADE] === Conversion complete ===\n");
    }
}

// Another example: Home Theater Facade
public class DVDPlayer
{
    public void On() => Console.WriteLine("[FACADE] DVD Player ON");
    public void Play(string movie) => Console.WriteLine($"[FACADE] Playing: {movie}");
    public void Off() => Console.WriteLine("[FACADE] DVD Player OFF");
}

public class Projector
{
    public void On() => Console.WriteLine("[FACADE] Projector ON");
    public void WideScreenMode() => Console.WriteLine("[FACADE] Projector: Widescreen mode");
    public void Off() => Console.WriteLine("[FACADE] Projector OFF");
}

public class SoundSystem
{
    public void On() => Console.WriteLine("[FACADE] Sound System ON");
    public void SetSurroundSound() => Console.WriteLine("[FACADE] 5.1 Surround Sound activated");
    public void SetVolume(int level) => Console.WriteLine($"[FACADE] Volume set to {level}");
    public void Off() => Console.WriteLine("[FACADE] Sound System OFF");
}

public class Lights
{
    public void Dim(int level) => Console.WriteLine($"[FACADE] Lights dimmed to {level}%");
}

// Home Theater Facade
public class HomeTheaterFacade
{
    private readonly DVDPlayer _dvd;
    private readonly Projector _projector;
    private readonly SoundSystem _soundSystem;
    private readonly Lights _lights;

    public HomeTheaterFacade(DVDPlayer dvd, Projector projector, SoundSystem soundSystem, Lights lights)
    {
        _dvd = dvd;
        _projector = projector;
        _soundSystem = soundSystem;
        _lights = lights;
    }

    public void WatchMovie(string movie)
    {
        Console.WriteLine($"[FACADE] === Starting movie: {movie} ===");
        _lights.Dim(10);
        _projector.On();
        _projector.WideScreenMode();
        _soundSystem.On();
        _soundSystem.SetSurroundSound();
        _soundSystem.SetVolume(50);
        _dvd.On();
        _dvd.Play(movie);
        Console.WriteLine("[FACADE] === Enjoy the movie! ===\n");
    }

    public void EndMovie()
    {
        Console.WriteLine("[FACADE] === Shutting down home theater ===");
        _dvd.Off();
        _soundSystem.Off();
        _projector.Off();
        _lights.Dim(100);
        Console.WriteLine("[FACADE] === Shutdown complete ===\n");
    }
}

// Usage demonstration
public class FacadeDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== FACADE PATTERN DEMO ===\n");

        Console.WriteLine("--- Example 1: Multimedia Facade ---");
        var multimediaFacade = new MultimediaFacade();

        // Simple interface hides complex subsystem
        multimediaFacade.PlayVideo("movie.mp4", "subtitles.srt", volume: 75);
        multimediaFacade.ConvertVideo("input.avi", "output.mp4");

        Console.WriteLine("--- Example 2: Home Theater Facade ---");
        var homeTheater = new HomeTheaterFacade(
            new DVDPlayer(),
            new Projector(),
            new SoundSystem(),
            new Lights()
        );

        // One simple method instead of managing many components
        homeTheater.WatchMovie("The Matrix");
        homeTheater.EndMovie();

        Console.WriteLine("💡 Benefit: Simplifies complex subsystems with a unified interface");
        Console.WriteLine("💡 Benefit: Reduces coupling between client and subsystem");
        Console.WriteLine("💡 Benefit: Provides a higher-level interface that's easier to use");
    }
}
