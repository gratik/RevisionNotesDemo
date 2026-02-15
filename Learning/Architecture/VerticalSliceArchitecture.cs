// ============================================================================
// VERTICAL SLICE ARCHITECTURE
// ============================================================================
// WHAT IS THIS?
// -------------
// Organizing code by feature/use case ("slice") instead of technical layers.
// Each slice owns request model, validation, handler, and response mapping.
//
// WHY IT MATTERS
// --------------
// ✅ Improves local reasoning by keeping feature code together
// ✅ Reduces cross-layer churn for simple changes
// ✅ Aligns naturally with CQRS-style command/query handlers
//
// WHEN TO USE
// -----------
// ✅ API-heavy systems with many independent features
// ✅ Teams wanting fast feature delivery and low merge conflicts
//
// WHEN NOT TO USE
// ---------------
// ❌ Tiny projects where folders/abstractions are unnecessary overhead
//
// REAL-WORLD EXAMPLE
// ------------------
// "Approve Expense" slice with request validation, policy check, handler, and
// persistence in one cohesive unit.
// ============================================================================

namespace RevisionNotesDemo.Architecture;

public static class VerticalSliceArchitecture
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Vertical Slice Architecture                          ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        ShowSliceModel();
        ShowEndToEndSlice();
        ShowOperationalBenefits();
        ShowTradeoffs();
    }

    private static void ShowSliceModel()
    {
        Console.WriteLine("1) SLICE MODEL");
        Console.WriteLine("- One folder/namespace per feature (command/query + handler + tests)");
        Console.WriteLine("- Dependencies are explicit per feature, not global per layer");
        Console.WriteLine("- Slices can choose different persistence strategy when justified\n");
    }

    private static void ShowEndToEndSlice()
    {
        Console.WriteLine("2) END-TO-END SLICE DEMO");

        var repository = new VSliceInMemoryExpenseRepository();
        repository.Add(new VSliceExpense("exp-100", 240m, "Pending"));

        var policy = new VSliceApprovalPolicy(maxAutoApprovalAmount: 500m);
        var handler = new VSliceApproveExpenseHandler(repository, policy);

        var approved = handler.Handle(new VSliceApproveExpenseCommand("exp-100", "mgr-01"));
        var missing = handler.Handle(new VSliceApproveExpenseCommand("exp-404", "mgr-01"));

        Console.WriteLine($"- Existing expense result: {approved.Status}");
        Console.WriteLine($"- Missing expense result: {missing.Status}");
        Console.WriteLine($"- Repository count: {repository.Count}\n");
    }

    private static void ShowOperationalBenefits()
    {
        Console.WriteLine("3) OPERATIONAL BENEFITS");
        Console.WriteLine("- Roll out features independently with targeted tests");
        Console.WriteLine("- On-call debugging is faster because flow is localized");
        Console.WriteLine("- New engineers can own a slice without understanding whole system\n");
    }

    private static void ShowTradeoffs()
    {
        Console.WriteLine("4) TRADEOFFS");
        Console.WriteLine("- Shared logic can become duplicated if not curated");
        Console.WriteLine("- Conventions are required to prevent inconsistent slice quality");
        Console.WriteLine("- Large cross-feature workflows still need integration boundaries\n");
    }
}

public sealed record VSliceApproveExpenseCommand(string ExpenseId, string ApproverId);

public sealed record VSliceApproveExpenseResult(string ExpenseId, string Status);

public sealed class VSliceExpense
{
    public VSliceExpense(string id, decimal amount, string status)
    {
        Id = id;
        Amount = amount;
        Status = status;
    }

    public string Id { get; }

    public decimal Amount { get; }

    public string Status { get; private set; }

    public void Approve()
    {
        Status = "Approved";
    }
}

public interface IVSliceExpenseRepository
{
    VSliceExpense? FindById(string expenseId);
    void Save(VSliceExpense expense);
}

public sealed class VSliceApprovalPolicy
{
    private readonly decimal _maxAutoApprovalAmount;

    public VSliceApprovalPolicy(decimal maxAutoApprovalAmount)
    {
        _maxAutoApprovalAmount = maxAutoApprovalAmount;
    }

    public bool CanApprove(VSliceExpense expense)
    {
        return expense.Amount <= _maxAutoApprovalAmount;
    }
}

public sealed class VSliceApproveExpenseHandler
{
    private readonly IVSliceExpenseRepository _repository;
    private readonly VSliceApprovalPolicy _policy;

    public VSliceApproveExpenseHandler(IVSliceExpenseRepository repository, VSliceApprovalPolicy policy)
    {
        _repository = repository;
        _policy = policy;
    }

    public VSliceApproveExpenseResult Handle(VSliceApproveExpenseCommand command)
    {
        var expense = _repository.FindById(command.ExpenseId);
        if (expense is null)
        {
            return new VSliceApproveExpenseResult(command.ExpenseId, "NotFound");
        }

        if (!_policy.CanApprove(expense))
        {
            return new VSliceApproveExpenseResult(command.ExpenseId, "Escalated");
        }

        expense.Approve();
        _repository.Save(expense);

        return new VSliceApproveExpenseResult(command.ExpenseId, "Approved");
    }
}

public sealed class VSliceInMemoryExpenseRepository : IVSliceExpenseRepository
{
    private readonly Dictionary<string, VSliceExpense> _storage = [];

    public int Count => _storage.Count;

    public void Add(VSliceExpense expense)
    {
        _storage[expense.Id] = expense;
    }

    public VSliceExpense? FindById(string expenseId)
    {
        _storage.TryGetValue(expenseId, out var expense);
        return expense;
    }

    public void Save(VSliceExpense expense)
    {
        _storage[expense.Id] = expense;
    }
}
