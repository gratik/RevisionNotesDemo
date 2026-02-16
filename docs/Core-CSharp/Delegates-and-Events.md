# Delegates and Events

> Subject: [Core-CSharp](../README.md)

## Delegates and Events

### Delegates (Type-Safe Function Pointers)

```csharp
// ✅ Delegate declaration
public delegate int BinaryOperation(int a, int b);

public class Calculator
{
    public int Execute(BinaryOperation operation, int a, int b)
    {
        return operation(a, b);
    }
}

// Usage
int Add(int a, int b) => a + b;
int Multiply(int a, int b) => a * b;

var calc = new Calculator();
var result1 = calc.Execute(Add, 3, 5);       // 8
var result2 = calc.Execute(Multiply, 3, 5);  // 15
var result3 = calc.Execute((a, b) => a - b, 10, 3);  // 7 (lambda)
```

### Built-in Delegates: Func, Action, Predicate

```csharp
// ✅ Func<T, TResult> - returns a value
Func<int, int, int> add = (a, b) => a + b;
int sum = add(3, 5);  // 8

// ✅ Action<T> - returns void
Action<string> log = message => Console.WriteLine(message);
log("Hello");  // Prints "Hello"

// ✅ Predicate<T> - returns bool
Predicate<int> isEven = n => n % 2 == 0;
bool result = isEven(4);  // true
```

### Events

```csharp
// ✅ Publisher
public class Order
{
    // Event declaration
    public event EventHandler<OrderEventArgs>? OrderPlaced;
    
    public void PlaceOrder()
    {
        // Process order...
        
        // Raise event
        OrderPlaced?.Invoke(this, new OrderEventArgs { OrderId = 123 });
    }
}

public class OrderEventArgs : EventArgs
{
    public int OrderId { get; set; }
}

// ✅ Subscriber
public class EmailService
{
    public void Subscribe(Order order)
    {
        order.OrderPlaced += OnOrderPlaced;
    }
    
    private void OnOrderPlaced(object? sender, OrderEventArgs e)
    {
        Console.WriteLine($"Sending email for order {e.OrderId}");
    }
}

// Usage
var order = new Order();
var emailService = new EmailService();
emailService.Subscribe(order);
order.PlaceOrder();  // Triggers email
```

---


