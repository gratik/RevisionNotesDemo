// ============================================================================
// OBSERVER PATTERN
// Reference: Revision Notes - Design Patterns (Behavioral) - Page 3
// ============================================================================
// DEFINITION:
//   Define a one-to-many dependency between objects so that when one object
//   changes state, all its dependents are notified and updated automatically.
//
// PURPOSE:
//   Implements distributed event handling. A subject maintains a list of observers
//   and notifies them automatically of any state changes.
//
// EXAMPLE:
//   Stock price updates - when price changes, all displays/alerts are updated
//   Newsletter subscriptions - when article published, all subscribers notified
//   MVC - Model notifies View when data changes
//
// MODERN .NET ALTERNATIVES:
//   • event keyword (most common)
//   • IObservable<T> / IObserver<T> (Reactive Extensions)
//   • INotifyPropertyChanged (WPF, Xamarin)
//   • EventAggregator pattern
//
// NOTE FROM REVISION NOTES:
//   "Observer – now built-in via IObservable<T> or event streams" - Page 4
//
// WHEN TO USE:
//   • Event handling systems
//   • Model-View patterns (MVC, MVVM)
//   • Pub/sub systems
//   • Real-time updates (stock prices, notifications)
//   • Multiple objects need to react to state changes
//
// BENEFITS:
//   • Loose coupling between subject and observers
//   • Dynamic subscription at runtime
//   • Broadcast communication
//   • Open-Closed Principle (add observers without modifying subject)
//
// CAUTIONS:
//   • Memory leaks if observers don't unsubscribe
//   • Notification order is undefined
//   • Can lead to cascading updates
//   • Performance issues with many observers
//
// BEST PRACTICES:
//   • Always unsubscribe from events (use IDisposable)
//   • Use weak events for long-lived subjects
//   • Consider async observers for expensive operations
//   • Avoid circular dependencies
//   • Prefer C# events over manual implementation
// ============================================================================

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
