// ==============================================================================
// STRATEGY PATTERN - Interchangeable Algorithms at Runtime
// Reference: Revision Notes - Design Patterns (Behavioral) - Page 3
// ==============================================================================
//
// WHAT IS THE STRATEGY PATTERN?
// ------------------------------
// Defines a family of algorithms, encapsulates each one behind a common interface,
// and makes them interchangeable. Strategy lets the algorithm vary independently
// from clients that use it. Eliminates conditional logic by replacing it with
// composition and polymorphism.
//
// Think of it as: "GPS navigation - same destination, different routes (fastest,
// shortest, avoid tolls) - picked at runtime based on preference"
//
// Core Concepts:
//   • Strategy Interface: Common contract for all algorithms
//   • Concrete Strategies: Different algorithm implementations
//   • Context: Object that uses a strategy (has-a relationship)
//   • Runtime Selection: Switch algorithms dynamically
//   • Composition over Inheritance: No subclassing needed
//
// WHY IT MATTERS
// --------------
// ✅ ELIMINATE CONDITIONALS: Replace if/else or switch with polymorphism
// ✅ RUNTIME FLEXIBILITY: Change algorithm on the fly
// ✅ OPEN/CLOSED PRINCIPLE: Add new strategies without modifying context
// ✅ SINGLE RESPONSIBILITY: Each strategy focused on one algorithm
// ✅ TESTABILITY: Easy to unit test each strategy in isolation
// ✅ COMPOSITION OVER INHERITANCE: Avoid rigid class hierarchies
//
// WHEN TO USE IT
// --------------
// ✅ Multiple algorithms for a specific task (sorting, compression, routing)
// ✅ Many related classes differ only in behavior
// ✅ Need to switch algorithms at runtime
// ✅ Want to hide complex algorithm implementation details
// ✅ Class has massive conditional logic (if/else, switch) for different behaviors
// ✅ Different variants of an algorithm (e.g., different pricing strategies)
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Only 2-3 simple conditions (overkill, just use if/else)
// ❌ Algorithm never changes
// ❌ Algorithm is trivial (one line of code)
// ❌ Clients don't care about algorithm details
//
// REAL-WORLD EXAMPLE - E-Commerce Payment Processing
// --------------------------------------------------
// Amazon checkout with multiple payment methods:
//   • Customer has order total: $127.50
//   • Payment options available:
//     1. **Credit Card Strategy**: Charge $127.50 + validate CVV + fraud check
//     2. **PayPal Strategy**: Redirect to PayPal + OAuth + charge $127.50
//     3. **Cryptocurrency Strategy**: Generate BTC invoice + wait for confirmations
//     4. **Gift Card Strategy**: Validate balance ≥ $127.50 + deduct amount
//     5. **Buy Now Pay Later Strategy**: Credit check + create installment plan
//
// WITHOUT STRATEGY (Code Smell):
//   ❌ if (paymentMethod == "CreditCard") {
//         // 50 lines of credit card logic
//     } else if (paymentMethod == "PayPal") {
//         // 40 lines of PayPal logic
//     } else if (paymentMethod == "Crypto") {
//         // 60 lines of crypto logic
//     } else if ... // 5 more payment methods
//   ❌ 300-line method with nested ifs
//   ❌ Adding Apple Pay = modify existing code (violates Open/Closed)
//   ❌ Can't test payment methods independently
//   ❌ Can't switch payment method during checkout
//
// WITH STRATEGY:
//   ✅ interface IPaymentStrategy { PaymentResult Process(decimal amount); }
//   ✅ class CreditCardStrategy : IPaymentStrategy { ... }
//   ✅ class PayPalStrategy : IPaymentStrategy { ... }
//   ✅ class ShoppingCart { 
//         private IPaymentStrategy _payment;
//         void SetPaymentMethod(IPaymentStrategy strategy) => _payment = strategy;
//         void Checkout() => _payment.Process(total);
//     }
//   ✅ cart.SetPaymentMethod(new CreditCardStrategy()); // Runtime selection
//   ✅ Adding Apple Pay: Create ApplePayStrategy (no existing code changes)
//   ✅ Each strategy tested independently
//
// ANOTHER EXAMPLE - Shipping Cost Calculation
// -------------------------------------------
// FedEx / UPS shipping calculator:
//   • Package: 5 lbs, Los Angeles → New York
//   • Strategies:
//     - **Standard Ground** (7-10 days): $12.50 (weight × $2.50)
//     - **2-Day Express**: $35.00 (weight × $7.00)
//     - **Overnight**: $75.00 (weight × $15.00)
//     - **International**: $125.00 (weight × $25.00, customs fee)
//     - **Free Shipping** (orders > $50): $0.00
//
// User selects strategy at checkout:
//   IShippingStrategy strategy = user.IsPrime 
//       ? new FreeShippingStrategy() 
//       : new StandardShippingStrategy();
//   decimal cost = strategy.CalculateCost(package);
//
// ANOTHER EXAMPLE - Data Compression
// ----------------------------------
// File backup system:
//   • Large file: 500 MB database dump
//   • Compression strategies:
//     - **None**: 500 MB, instant (0 CPU)
//     - **GZip**: 250 MB, 10 seconds (medium CPU)
//     - **BZip2**: 200 MB, 30 seconds (high CPU)
//     - **LZMA**: 180 MB, 60 seconds (very high CPU)
//
// Choose based on scenario:
//   • Fast backup over local network → NoCompressionStrategy
//   • Upload to cloud (bandwidth limited) → LZMAStrategy (smallest)
//   • Balanced → GZipStrategy
//
// REAL CODE EXAMPLE - Sorting Algorithms
// --------------------------------------
// Sorting service with different algorithms:
//   interface ISortStrategy<T> { void Sort(List<T> list); }
//   
//   class QuickSortStrategy<T> : ISortStrategy<T> { ... }  // O(n log n) avg
//   class MergeSortStrategy<T> : ISortStrategy<T> { ... }  // O(n log n) worst
//   class BubbleSortStrategy<T> : ISortStrategy<T> { ... } // O(n²) - small lists
//   
//   class DataProcessor<T> {
//       ISortStrategy<T> _sortStrategy;
//       void SetSortStrategy(ISortStrategy<T> strategy) => _sortStrategy = strategy;
//       void ProcessData(List<T> data) {
//           _sortStrategy.Sort(data); // Algorithm chosen at runtime
//       }
//   }
//   
//   // Usage:
//   var processor = new DataProcessor<int>();
//   processor.SetSortStrategy(list.Count < 100 
//       ? new BubbleSortStrategy<int>()  // Small list
//       : new QuickSortStrategy<int>()); // Large list
//
// .NET FRAMEWORK EXAMPLES
// -----------------------
// Strategy pattern used in .NET:
//   • LINQ sorting: OrderBy, OrderByDescending (IComparer<T> strategy)
//   • Stream classes: FileStream, MemoryStream, NetworkStream (same interface)
//   • Logging: ILogger with different providers (Console, File, Azure)
//   • Validation: IValidator implementations (FluentValidation)
//   • HttpClient handlers: DelegatingHandler pipeline
//
// STRATEGY + DEPENDENCY INJECTION
// -------------------------------
// Modern approach using DI:
//   services.AddScoped<IPaymentStrategy, CreditCardStrategy>();
//   
//   class OrderService {
//       private readonly IPaymentStrategy _paymentStrategy;
//       public OrderService(IPaymentStrategy paymentStrategy) {
//           _paymentStrategy = paymentStrategy; // Injected
//       }
//   }
//
// Or with Factory:
//   interface IPaymentStrategyFactory {
//       IPaymentStrategy GetStrategy(PaymentType type);
//   }
//
// BEST PRACTICES
// --------------
// ✅ Use dependency injection to provide strategies
// ✅ Keep strategies stateless when possible (easier to reuse)
// ✅ Name strategies by what they do: FastShipping, not Strategy1
// ✅ Consider Factory pattern to create strategies
// ✅ Strategies should be interchangeable (Liskov Substitution Principle)
// ✅ Document performance characteristics of each strategy
//
// STRATEGY VS SIMILAR PATTERNS
// ----------------------------
// Strategy vs State:
//   • Strategy: Client chooses algorithm, context doesn't change state
//   • State: Context changes its own state, different behavior per state
//
// Strategy vs Template Method:
//   • Strategy: Composition (has-a), change entire algorithm
//   • Template Method: Inheritance (is-a), change steps of algorithm
//
// Strategy vs Command:
//   • Strategy: How to perform operation (algorithm)
//   • Command: What operation to perform (encapsulate request)
//
// ==============================================================================

namespace RevisionNotesDemo.DesignPatterns.Behavioral;

// Strategy interface
public interface IPaymentStrategy
{
    bool Pay(decimal amount);
    string GetPaymentMethod();
}

// Concrete strategies
public class CreditCardPayment : IPaymentStrategy
{
    private readonly string _cardNumber;
    private readonly string _cvv;

    public CreditCardPayment(string cardNumber, string cvv)
    {
        _cardNumber = cardNumber;
        _cvv = cvv;
    }

    public bool Pay(decimal amount)
    {
        Console.WriteLine($"[STRATEGY] Processing credit card payment: ${amount:F2}");
        Console.WriteLine($"[STRATEGY] Card: ****{_cardNumber.Substring(_cardNumber.Length - 4)}");
        return true;
    }

    public string GetPaymentMethod() => "Credit Card";
}

public class PayPalPayment : IPaymentStrategy
{
    private readonly string _email;

    public PayPalPayment(string email)
    {
        _email = email;
    }

    public bool Pay(decimal amount)
    {
        Console.WriteLine($"[STRATEGY] Processing PayPal payment: ${amount:F2}");
        Console.WriteLine($"[STRATEGY] Account: {_email}");
        return true;
    }

    public string GetPaymentMethod() => "PayPal";
}

public class CryptoPayment : IPaymentStrategy
{
    private readonly string _walletAddress;

    public CryptoPayment(string walletAddress)
    {
        _walletAddress = walletAddress;
    }

    public bool Pay(decimal amount)
    {
        Console.WriteLine($"[STRATEGY] Processing cryptocurrency payment: ${amount:F2}");
        Console.WriteLine($"[STRATEGY] Wallet: {_walletAddress}");
        return true;
    }

    public string GetPaymentMethod() => "Cryptocurrency";
}

// Context
public class ShoppingCart
{
    private IPaymentStrategy? _paymentStrategy;
    private readonly List<(string item, decimal price)> _items = new();

    public void AddItem(string item, decimal price)
    {
        _items.Add((item, price));
        Console.WriteLine($"[STRATEGY] Added to cart: {item} - ${price:F2}");
    }

    public void SetPaymentStrategy(IPaymentStrategy strategy)
    {
        _paymentStrategy = strategy;
        Console.WriteLine($"[STRATEGY] Payment method set to: {strategy.GetPaymentMethod()}");
    }

    public decimal GetTotal() => _items.Sum(item => item.price);

    public bool Checkout()
    {
        if (_paymentStrategy == null)
        {
            Console.WriteLine("[STRATEGY] Error: No payment method selected");
            return false;
        }

        var total = GetTotal();
        Console.WriteLine($"\n[STRATEGY] === Checkout ===");
        Console.WriteLine($"[STRATEGY] Items: {_items.Count}");
        Console.WriteLine($"[STRATEGY] Total: ${total:F2}");

        return _paymentStrategy.Pay(total);
    }
}

// Another example: Sorting strategies
public interface ISortStrategy<T>
{
    void Sort(List<T> list);
    string GetStrategyName();
}

public class BubbleSortStrategy : ISortStrategy<int>
{
    public void Sort(List<int> list)
    {
        Console.WriteLine("[STRATEGY] Sorting using Bubble Sort");
        // Simplified bubble sort
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list.Count - 1; j++)
            {
                if (list[j] > list[j + 1])
                {
                    (list[j], list[j + 1]) = (list[j + 1], list[j]);
                }
            }
        }
    }

    public string GetStrategyName() => "Bubble Sort";
}

public class QuickSortStrategy : ISortStrategy<int>
{
    public void Sort(List<int> list)
    {
        Console.WriteLine("[STRATEGY] Sorting using Quick Sort");
        QuickSort(list, 0, list.Count - 1);
    }

    private void QuickSort(List<int> list, int low, int high)
    {
        if (low < high)
        {
            int pi = Partition(list, low, high);
            QuickSort(list, low, pi - 1);
            QuickSort(list, pi + 1, high);
        }
    }

    private int Partition(List<int> list, int low, int high)
    {
        int pivot = list[high];
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            if (list[j] < pivot)
            {
                i++;
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
        (list[i + 1], list[high]) = (list[high], list[i + 1]);
        return i + 1;
    }

    public string GetStrategyName() => "Quick Sort";
}

public class Sorter<T>
{
    private ISortStrategy<T>? _strategy;

    public void SetStrategy(ISortStrategy<T> strategy)
    {
        _strategy = strategy;
    }

    public void Sort(List<T> list)
    {
        if (_strategy == null)
        {
            Console.WriteLine("[STRATEGY] No sorting strategy set");
            return;
        }

        Console.WriteLine($"[STRATEGY] Using: {_strategy.GetStrategyName()}");
        _strategy.Sort(list);
    }
}

// Usage demonstration
public class StrategyDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== STRATEGY PATTERN DEMO ===\n");

        Console.WriteLine("--- Example 1: Payment Processing ---\n");

        var cart = new ShoppingCart();
        cart.AddItem("Laptop", 999.99m);
        cart.AddItem("Mouse", 29.99m);
        cart.AddItem("Keyboard", 79.99m);

        // Try different payment strategies
        Console.WriteLine("\n[STRATEGY] Paying with Credit Card:");
        cart.SetPaymentStrategy(new CreditCardPayment("1234567890123456", "123"));
        cart.Checkout();

        Console.WriteLine("\n[STRATEGY] Changing to PayPal:");
        cart.SetPaymentStrategy(new PayPalPayment("user@example.com"));
        cart.Checkout();

        Console.WriteLine("\n[STRATEGY] Changing to Cryptocurrency:");
        cart.SetPaymentStrategy(new CryptoPayment("1A2B3C4D5E6F7G8H9I0J"));
        cart.Checkout();

        Console.WriteLine("\n--- Example 2: Sorting Strategies ---\n");

        var sorter = new Sorter<int>();

        var list1 = new List<int> { 64, 34, 25, 12, 22, 11, 90 };
        Console.WriteLine($"[STRATEGY] Original: {string.Join(", ", list1)}");
        sorter.SetStrategy(new BubbleSortStrategy());
        sorter.Sort(list1);
        Console.WriteLine($"[STRATEGY] Sorted: {string.Join(", ", list1)}\n");

        var list2 = new List<int> { 10, 7, 8, 9, 1, 5 };
        Console.WriteLine($"[STRATEGY] Original: {string.Join(", ", list2)}");
        sorter.SetStrategy(new QuickSortStrategy());
        sorter.Sort(list2);
        Console.WriteLine($"[STRATEGY] Sorted: {string.Join(", ", list2)}");

        Console.WriteLine("\n💡 Benefit: Algorithms can be swapped at runtime");
        Console.WriteLine("💡 Benefit: Follows Open-Closed Principle");
        Console.WriteLine("💡 Benefit: Eliminates conditional statements for algorithm selection");
    }
}
