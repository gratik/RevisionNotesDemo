// ============================================================================
// STRATEGY PATTERN
// Reference: Revision Notes - Design Patterns (Behavioral) - Page 3
// ============================================================================
// DEFINITION:
//   Define a family of algorithms, encapsulate each one, and make them interchangeable.
//   Strategy lets the algorithm vary independently from clients that use it.
//
// PURPOSE:
//   Enable selecting an algorithm at runtime. Defines a family of algorithms,
//   encapsulates each one, and makes them interchangeable without changing clients.
//
// EXAMPLE:
//   Payment methods: Credit Card, PayPal, Cryptocurrency - same interface, different logic
//   Shipping methods: Standard, Express, Overnight - different cost calculations
//   Sorting algorithms: QuickSort, MergeSort, BubbleSort - same interface, different implementation
//
// WHEN TO USE:
//   • Multiple algorithms for a specific task
//   • Avoiding conditional logic (if/else, switch)
//   • Algorithm selection at runtime
//   • Similar classes differing only in behavior
//
// BENEFITS:
//   • Easy to switch algorithms at runtime
//   • Isolates algorithm implementation from client code
//   • Open-Closed Principle (add new strategies without modifying context)
//   • Replaces inheritance with composition
//   • Eliminates conditional statements
//
// REAL-WORLD USES:
//   • Payment processing (credit card, PayPal, crypto)
//   • Shipping calculation (standard, express, overnight)
//   • Validation rules
//   • Compression algorithms
//   • Route finding algorithms
//
// CAUTIONS:
//   • Client must be aware of different strategies
//   • Increases number of classes/objects
//   • Overkill if you only have 2-3 simple conditions
//
// BEST PRACTICES:
//   • Use dependency injection to provide strategies
//   • Keep strategies stateless when possible
//   • Consider using Factory pattern to create strategies
//   • Name strategies clearly (what they do, not how)
// ============================================================================

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
