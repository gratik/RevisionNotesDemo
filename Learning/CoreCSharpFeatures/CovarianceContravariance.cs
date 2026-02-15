// ============================================================================
// COVARIANCE & CONTRAVARIANCE IN GENERICS
// Reference: Revision Notes - Page 7
// ============================================================================
// WHAT IS THIS?
// -------------
// Variance rules for generic type parameters (`out` and `in`).
//
// WHY IT MATTERS
// --------------
// ✅ Enables safe assignment across type hierarchies
// ✅ Makes APIs more flexible and reusable
//
// WHEN TO USE
// -----------
// ✅ Producer/consumer interfaces like `IEnumerable<T>` or `Action<T>`
// ✅ Delegate variance for callbacks
//
// WHEN NOT TO USE
// ---------------
// ❌ Invariant types that both read and write the same T
// ❌ Misusing variance leading to unsafe assumptions
//
// REAL-WORLD EXAMPLE
// ------------------
// Assign `IEnumerable<string>` to `IEnumerable<object>`.
// ============================================================================

namespace RevisionNotesDemo.CoreCSharpFeatures;

public class CovarianceContravarianceDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== COVARIANCE & CONTRAVARIANCE DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Page 7\n");

        // COVARIANCE (out): IEnumerable<Derived> -> IEnumerable<Base>
        Console.WriteLine("--- Covariance (out) ---");
        IEnumerable<string> strings = new List<string> { "a", "b", "c" };
        IEnumerable<object> objects = strings; // Covariance - OK!

        Console.WriteLine($"[COVAR] strings assigned to IEnumerable<object>: {string.Join(", ", objects)}");

        // CONTRAVARIANCE (in): Action<Base> -> Action<Derived>
        Console.WriteLine("\n--- Contravariance (in) ---");
        Action<object> consumeObject = o => Console.WriteLine($"[CONTRAVAR] Processing: {o}");
        Action<string> consumeString = consumeObject; // Contravariance - OK!
        consumeString("hello");

        Console.WriteLine("\n💡 From Revision Notes:");
        Console.WriteLine("   - Covariance (out): Return more derived types");
        Console.WriteLine("   - Contravariance (in): Accept less derived types");
    }
}
