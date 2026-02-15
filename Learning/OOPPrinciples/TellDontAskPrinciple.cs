// ============================================================================
// TDA (Tell, Don't Ask)
// Reference: Revision Notes - Page 2
// ============================================================================
//
// WHAT IS TDA?
// ------------
// Tell objects what to do instead of asking for their state and making decisions
// externally. Behavior should live with the data it operates on.
//
// WHY IT MATTERS
// --------------
// - Improves encapsulation
// - Reduces coupling between objects
// - Keeps business logic in one place
// - Simplifies calling code
//
// WHEN TO USE
// -----------
// - YES: When an object owns data and rules for that data
// - YES: When client code calls multiple getters and makes decisions
// - YES: When domain logic is scattered across services
//
// WHEN NOT TO USE
// ---------------
// - NO: For simple DTOs or read-only projections
// - NO: When state must be exposed for reporting or serialization
//
// REAL-WORLD EXAMPLE
// ------------------
// Banking:
// - Bad: if (account.Balance >= amount) { account.Balance -= amount; }
// - Good: account.Withdraw(amount) and let the account enforce rules
// ============================================================================

namespace RevisionNotesDemo.OOPPrinciples;

// ❌ BAD EXAMPLE - Violates TDA (Ask pattern)
// We ask the object about its state and make decisions externally

public class BankAccountBad
{
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
}

public class BankingServiceBad
{
    public void ProcessWithdrawal(BankAccountBad account, decimal amount)
    {
        // Asking the object about its state (violation!)
        if (account.IsActive && account.Balance >= amount)
        {
            // Making decisions and modifying state externally
            account.Balance -= amount;
            Console.WriteLine($"Withdrawal successful. New balance: ${account.Balance}");
        }
        else
        {
            Console.WriteLine("Withdrawal failed");
        }
    }

    public void ProcessDeposit(BankAccountBad account, decimal amount)
    {
        // Again, asking about state and deciding externally
        if (account.IsActive && amount > 0)
        {
            account.Balance += amount;
            Console.WriteLine($"Deposit successful. New balance: ${account.Balance}");
        }
    }
}

// ✅ GOOD EXAMPLE - Follows TDA (Tell pattern)
// We tell the object what to do, and it handles its own state

public class BankAccount
{
    private decimal _balance;
    private bool _isActive;
    private List<string> _transactionHistory;

    public BankAccount(decimal initialBalance)
    {
        _balance = initialBalance;
        _isActive = true;
        _transactionHistory = new List<string>();
    }

    // Tell the account to withdraw - it decides and handles everything
    public bool Withdraw(decimal amount)
    {
        // Object makes its own decisions based on its state
        if (!_isActive)
        {
            Console.WriteLine("[TDA] Cannot withdraw - account is not active");
            return false;
        }

        if (amount <= 0)
        {
            Console.WriteLine("[TDA] Cannot withdraw - invalid amount");
            return false;
        }

        if (_balance < amount)
        {
            Console.WriteLine("[TDA] Cannot withdraw - insufficient funds");
            return false;
        }

        // Object manages its own state
        _balance -= amount;
        _transactionHistory.Add($"Withdrawal: ${amount} on {DateTime.Now:g}");
        Console.WriteLine($"[TDA] Withdrawal successful: ${amount}. New balance: ${_balance:F2}");
        return true;
    }

    // Tell the account to deposit
    public bool Deposit(decimal amount)
    {
        if (!_isActive)
        {
            Console.WriteLine("[TDA] Cannot deposit - account is not active");
            return false;
        }

        if (amount <= 0)
        {
            Console.WriteLine("[TDA] Cannot deposit - invalid amount");
            return false;
        }

        _balance += amount;
        _transactionHistory.Add($"Deposit: ${amount} on {DateTime.Now:g}");
        Console.WriteLine($"[TDA] Deposit successful: ${amount}. New balance: ${_balance:F2}");
        return true;
    }

    // Tell the account to close itself
    public void Close()
    {
        if (_balance > 0)
        {
            Console.WriteLine($"[TDA] Cannot close - account has balance of ${_balance:F2}");
            return;
        }

        _isActive = false;
        Console.WriteLine("[TDA] Account closed successfully");
    }

    // Tell the account to transfer to another account
    public bool TransferTo(BankAccount targetAccount, decimal amount)
    {
        if (Withdraw(amount))
        {
            if (targetAccount.Deposit(amount))
            {
                Console.WriteLine($"[TDA] Transfer successful: ${amount}");
                return true;
            }
            else
            {
                // Rollback withdrawal if deposit fails
                Deposit(amount);
                Console.WriteLine("[TDA] Transfer failed - deposit rejected");
                return false;
            }
        }

        Console.WriteLine("[TDA] Transfer failed - withdrawal rejected");
        return false;
    }

    public void PrintStatement()
    {
        Console.WriteLine($"\n[TDA] Account Statement:");
        Console.WriteLine($"[TDA] Current Balance: ${_balance:F2}");
        Console.WriteLine($"[TDA] Status: {(_isActive ? "Active" : "Closed")}");
        Console.WriteLine($"[TDA] Transaction History:");
        foreach (var transaction in _transactionHistory)
        {
            Console.WriteLine($"[TDA]   - {transaction}");
        }
        Console.WriteLine();
    }
}

// Another example: Order processing

// ❌ BAD - Ask pattern
public class OrderBad
{
    public List<string> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public bool IsPaid { get; set; }
    public bool IsShipped { get; set; }
}

// ✅ GOOD - Tell pattern
public class Order
{
    private List<string> _items = new();
    private decimal _totalAmount;
    private bool _isPaid;
    private bool _isShipped;
    private DateTime? _paidDate;
    private DateTime? _shippedDate;

    public Order(List<string> items, decimal totalAmount)
    {
        _items = new List<string>(items);
        _totalAmount = totalAmount;
    }

    // Tell the order to process payment
    public bool ProcessPayment(decimal amount)
    {
        if (_isPaid)
        {
            Console.WriteLine("[TDA] Order already paid");
            return false;
        }

        if (amount < _totalAmount)
        {
            Console.WriteLine("[TDA] Insufficient payment amount");
            return false;
        }

        _isPaid = true;
        _paidDate = DateTime.Now;
        Console.WriteLine($"[TDA] Payment processed successfully: ${amount:F2}");
        return true;
    }

    // Tell the order to ship itself
    public bool Ship()
    {
        if (!_isPaid)
        {
            Console.WriteLine("[TDA] Cannot ship - order not paid");
            return false;
        }

        if (_isShipped)
        {
            Console.WriteLine("[TDA] Order already shipped");
            return false;
        }

        _isShipped = true;
        _shippedDate = DateTime.Now;
        Console.WriteLine($"[TDA] Order shipped successfully");
        return true;
    }

    // Tell the order to cancel itself
    public bool Cancel()
    {
        if (_isShipped)
        {
            Console.WriteLine("[TDA] Cannot cancel - order already shipped");
            return false;
        }

        Console.WriteLine("[TDA] Order cancelled");
        _items.Clear();
        return true;
    }

    public void PrintOrderStatus()
    {
        Console.WriteLine($"\n[TDA] Order Status:");
        Console.WriteLine($"[TDA] Items: {string.Join(", ", _items)}");
        Console.WriteLine($"[TDA] Total: ${_totalAmount:F2}");
        Console.WriteLine($"[TDA] Paid: {(_isPaid ? $"Yes (on {_paidDate:g})" : "No")}");
        Console.WriteLine($"[TDA] Shipped: {(_isShipped ? $"Yes (on {_shippedDate:g})" : "No")}\n");
    }
}

// Usage demonstration
public class TDADemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== TELL, DON'T ASK PRINCIPLE DEMO ===\n");

        Console.WriteLine("--- Bank Account Example ---");
        var account1 = new BankAccount(1000m);
        var account2 = new BankAccount(500m);

        // Tell the account what to do (don't ask about its state)
        account1.Deposit(200m);
        account1.Withdraw(150m);
        account1.TransferTo(account2, 300m);

        account1.PrintStatement();
        account2.PrintStatement();

        Console.WriteLine("--- Order Processing Example ---");
        var order = new Order(
            new List<string> { "Laptop", "Mouse", "Keyboard" },
            1500m
        );

        order.PrintOrderStatus();

        // Tell the order what to do
        order.ProcessPayment(1500m);
        order.Ship();

        order.PrintOrderStatus();

        Console.WriteLine("\nBenefit: Objects encapsulate their behavior and state!");
        Console.WriteLine("Benefit: Better encapsulation and less coupling!");
        Console.WriteLine("Benefit: Object makes decisions about its own state!");

        Console.WriteLine("\nFrom Revision Notes:");
        Console.WriteLine("'Tell the object what to do' instead of 'asking about its state and deciding externally'");
    }
}
