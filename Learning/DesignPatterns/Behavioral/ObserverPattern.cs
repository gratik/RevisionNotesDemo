// ==============================================================================
// OBSERVER PATTERN - Publish-Subscribe for Event-Driven Systems
// Reference: Revision Notes - Design Patterns (Behavioral) - Page 3
// ==============================================================================
//
// WHAT IS THE OBSERVER PATTERN?
// ------------------------------
// Defines a one-to-many dependency between objects so that when one object (Subject)
// changes state, all its dependents (Observers) are notified and updated automatically.
// Implements distributed event handling with loose coupling.
//
// Think of it as: "YouTube subscriptions - when a channel uploads a video,
// all subscribers get notified automatically"
//
// Core Concepts:
//   • Subject: The observable object that maintains state and notifies observers
//   • Observer: Objects that want to be notified of subject's state changes
//   • Subscribe/Unsubscribe: Dynamic registration mechanism
//   • Notify: Subject broadcasts changes to all registered observers
//   • Loose Coupling: Subject doesn't know concrete observer types
//
// WHY IT MATTERS
// --------------
// ✅ LOOSE COUPLING: Subject and observers are independent, interact via interface
// ✅ DYNAMIC RELATIONSHIPS: Add/remove observers at runtime
// ✅ BROADCAST COMMUNICATION: One change notifies many objects
// ✅ OPEN/CLOSED PRINCIPLE: Add observers without modifying subject
// ✅ SEPARATION OF CONCERNS: Subject manages state, observers react to changes
// ✅ EVENT-DRIVEN ARCHITECTURE: Foundation for reactive systems
//
// WHEN TO USE IT
// --------------
// ✅ Change in one object requires updating multiple other objects
// ✅ Don't know in advance how many objects need to be updated
// ✅ Object should notify others without knowing who they are
// ✅ Event handling systems (UI events, notifications)
// ✅ Model-View architecture (MVC, MVVM, MVP)
// ✅ Real-time updates (stock prices, sports scores, social media feeds)
// ✅ Pub/sub messaging systems
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Only one observer will ever exist (use direct reference)
// ❌ Performance critical (observer notification has overhead)
// ❌ Observers need to modify the subject during notification (circular dependencies)
// ❌ Simple one-to-one relationships (overkill)
//
// REAL-WORLD EXAMPLE - Stock Trading Platform
// -------------------------------------------
// Bloomberg Terminal / Trading App:
//   • 1 Stock (AAPL) monitored by multiple observers
//   • Price changes from $150.00 → $150.25
//   • All observers notified simultaneously:
//     - Dashboard widget updates price display
//     - Alert system checks if price > $150.20 threshold → sends SMS
//     - Trading bot checks strategy → executes buy order
//     - Chart component redraws candlestick
//     - Portfolio calculator updates total value
//     - Logger records price change to database
//
// Without Observer:
//   ❌ Stock class coupled to Dashboard, AlertSystem, TradingBot, etc.
//   ❌ Adding new display requires modifying Stock class
//   ❌ Can't add/remove displays at runtime
//   ❌ 50 observers = 50 direct dependencies!
//
// With Observer:
//   ✅ Stock only knows IStockObserver interface
//   ✅ Add new display: implement interface, subscribe
//   ✅ Users subscribe/unsubscribe at runtime
//   ✅ Stock.SetPrice() → NotifyObservers() → all observers updated
//   ✅ Zero coupling between Stock and concrete observers
//
// Code structure:
//   interface IStockObserver { void Update(Stock stock, decimal newPrice); }
//   class Stock {
//       List<IStockObserver> _observers;
//       void Subscribe(IStockObserver obs) => _observers.Add(obs);
//       void SetPrice(decimal price) { _price = price; NotifyObservers(); }
//       void NotifyObservers() => _observers.ForEach(o => o.Update(this, _price));
//   }
//
// ANOTHER EXAMPLE - Social Media Notifications
// --------------------------------------------
// Twitter/Instagram post notifications:
//   • Celebrity posts a tweet
//   • 10 million followers notified
//   • Different observers react differently:
//     - Mobile app: Push notification
//     - Email service: Digest email
//     - Analytics: Track engagement
//     - Content moderation: Check for violations
//     - Trending algorithm: Update trends
//
// ANOTHER EXAMPLE - Weather Station
// ---------------------------------
// IoT weather station:
//   • Temperature sensor (subject) reads 35°C
//   • Observers notified:
//     - Display shows current temp
//     - StatisticsDisplay calculates avg/min/max
//     - ForecastDisplay predicts conditions
//     - SmartThermostat adjusts AC
//     - AlertSystem sends "Heat warning" if > 32°C
//
// MODERN .NET ALTERNATIVES
// ------------------------
// Observer pattern is built into .NET:
//   • **event keyword** (most common, simplest)
//     public event EventHandler<PriceChangedEventArgs> PriceChanged;
//   • **IObservable<T> / IObserver<T>** (Reactive Extensions - Rx.NET)
//     IObservable<StockPrice> stockStream = ...;
//     stockStream.Subscribe(price => Console.WriteLine(price));
//   • **INotifyPropertyChanged** (WPF/MAUI data binding)
//     public event PropertyChangedEventHandler PropertyChanged;
//   • **EventAggregator** (Prism, MediatR)
//   • **Channels** (System.Threading.Channels)
//
// NOTE FROM REVISION NOTES:
//   "Observer – now built-in via IObservable<T> or event streams" - Page 4
//   Translation: Use C# events or Rx.NET instead of manual implementation
//
// PUSH VS PULL MODEL
// ------------------
// PUSH (pass data in notification):
//   ✅ void Update(decimal newPrice) - Observer gets all data immediately
//   ❌ Subject must know what data observers need
//   
// PULL (observer queries subject):
//   ✅ void Update(Stock stock) - Observer pulls what it needs: stock.GetPrice()
//   ✅ More flexible - observers get what they want
//   ❌ Extra method calls (performance)
//
// MEMORY LEAK WARNING!
// --------------------
// ⚠️ CRITICAL: Observers hold subject reference → Subject holds observer reference
//   → If observer doesn't unsubscribe, MEMORY LEAK!
//
// Solution:
//   • Always unsubscribe: stock.Unsubscribe(observer) or use IDisposable
//   • Weak events: WeakEventManager (WPF) - doesn't prevent GC
//   • Use 'using' statement:
//     using (var subscription = observable.Subscribe(observer))
//     { /* observer active */ }
//
// BEST PRACTICES
// --------------
// ✅ Use IDisposable for subscriptions (unsubscribe in Dispose)
// ✅ Prefer C# events over manual implementation
// ✅ Use weak events for long-lived subjects + short-lived observers
// ✅ Make notification thread-safe (lock or immutable data)
// ✅ Consider async observers for I/O operations
// ✅ Avoid circular dependencies (A observes B, B observes A)
// ✅ Notification order is undefined - don't rely on it
//
// OBSERVER VS SIMILAR PATTERNS
// ----------------------------
// Observer vs Mediator:
//   • Observer: One-to-many, subject notifies observers directly
//   • Mediator: Many-to-many, all communication through central mediator
//
// Observer vs Event Bus:
//   • Observer: Direct subscription to specific subject
//   • Event Bus: Global pub/sub, decoupled via message types
//
// Observer vs Chain of Responsibility:
//   • Observer: All observers notified
//   • Chain: Stop at first handler that processes
//
// ==============================================================================

namespace RevisionNotesDemo.DesignPatterns.Behavioral;

// Subject interface
public interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify();
}

// Observer interface
public interface IObserver
{
    void Update(string message);
}

// Concrete Subject - Stock
public class Stock : ISubject
{
    private readonly List<IObserver> _observers = new();
    private string _symbol;
    private decimal _price;

    public Stock(string symbol, decimal initialPrice)
    {
        _symbol = symbol;
        _price = initialPrice;
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                _price = value;
                Console.WriteLine($"[OBSERVER] Stock {_symbol} price changed to ${_price:F2}");
                Notify();
            }
        }
    }

    public void Attach(IObserver observer)
    {
        _observers.Add(observer);
        Console.WriteLine($"[OBSERVER] Observer attached to {_symbol}");
    }

    public void Detach(IObserver observer)
    {
        _observers.Remove(observer);
        Console.WriteLine($"[OBSERVER] Observer detached from {_symbol}");
    }

    public void Notify()
    {
        Console.WriteLine($"[OBSERVER] Notifying {_observers.Count} observers...");
        foreach (var observer in _observers)
        {
            observer.Update($"{_symbol} is now ${_price:F2}");
        }
    }
}

// Concrete Observers
public class MobileApp : IObserver
{
    private readonly string _userName;

    public MobileApp(string userName)
    {
        _userName = userName;
    }

    public void Update(string message)
    {
        Console.WriteLine($"[OBSERVER] 📱 Mobile notification for {_userName}: {message}");
    }
}

public class EmailAlert : IObserver
{
    private readonly string _email;

    public EmailAlert(string email)
    {
        _email = email;
    }

    public void Update(string message)
    {
        Console.WriteLine($"[OBSERVER] 📧 Email sent to {_email}: {message}");
    }
}

public class Dashboard : IObserver
{
    public void Update(string message)
    {
        Console.WriteLine($"[OBSERVER] 📊 Dashboard updated: {message}");
    }
}

// Modern .NET approach using events
public class StockModern
{
    private string _symbol;
    private decimal _price;

    // Built-in event pattern (modern alternative to Observer)
    public event EventHandler<StockPriceChangedArgs>? PriceChanged;

    public StockModern(string symbol, decimal initialPrice)
    {
        _symbol = symbol;
        _price = initialPrice;
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (_price != value)
            {
                var oldPrice = _price;
                _price = value;
                OnPriceChanged(new StockPriceChangedArgs(_symbol, oldPrice, _price));
            }
        }
    }

    protected virtual void OnPriceChanged(StockPriceChangedArgs e)
    {
        PriceChanged?.Invoke(this, e);
    }
}

public class StockPriceChangedArgs : EventArgs
{
    public string Symbol { get; }
    public decimal OldPrice { get; }
    public decimal NewPrice { get; }

    public StockPriceChangedArgs(string symbol, decimal oldPrice, decimal newPrice)
    {
        Symbol = symbol;
        OldPrice = oldPrice;
        NewPrice = newPrice;
    }
}

// Usage demonstration
public class ObserverDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== OBSERVER PATTERN DEMO ===\n");

        Console.WriteLine("--- Classic Observer Pattern ---\n");

        var appleStock = new Stock("AAPL", 150.00m);

        // Create observers
        var mobileApp = new MobileApp("John");
        var emailAlert = new EmailAlert("john@example.com");
        var dashboard = new Dashboard();

        // Attach observers
        appleStock.Attach(mobileApp);
        appleStock.Attach(emailAlert);
        appleStock.Attach(dashboard);

        Console.WriteLine("\n[OBSERVER] Changing stock price...\n");
        appleStock.Price = 155.50m; // All observers notified

        Console.WriteLine("\n[OBSERVER] Changing stock price again...\n");
        appleStock.Price = 152.75m;

        Console.WriteLine("\n[OBSERVER] Detaching email alert...\n");
        appleStock.Detach(emailAlert);

        Console.WriteLine("\n[OBSERVER] Changing stock price after detachment...\n");
        appleStock.Price = 160.00m; // Only 2 observers notified

        Console.WriteLine("\n--- Modern .NET Events Approach ---\n");

        var googleStock = new StockModern("GOOGL", 2800.00m);

        // Subscribe using events (modern approach)
        googleStock.PriceChanged += (sender, e) =>
        {
            Console.WriteLine($"[OBSERVER] 📱 Event: {e.Symbol} changed from ${e.OldPrice:F2} to ${e.NewPrice:F2}");
        };

        googleStock.PriceChanged += (sender, e) =>
        {
            if (e.NewPrice > e.OldPrice)
            {
                Console.WriteLine($"[OBSERVER] 📈 Alert: {e.Symbol} increased by ${e.NewPrice - e.OldPrice:F2}");
            }
            else
            {
                Console.WriteLine($"[OBSERVER] 📉 Alert: {e.Symbol} decreased by ${e.OldPrice - e.NewPrice:F2}");
            }
        };

        Console.WriteLine("\n[OBSERVER] Changing Google stock price...\n");
        googleStock.Price = 2850.00m;

        Console.WriteLine("\n[OBSERVER] Changing Google stock price again...\n");
        googleStock.Price = 2825.00m;

        Console.WriteLine("\n💡 Benefit: Loose coupling between subject and observers");
        Console.WriteLine("💡 Benefit: Dynamic subscription/unsubscription");
        Console.WriteLine("💡 From Revision Notes: Now built-in via IObservable<T> or event streams in .NET");
    }
}
