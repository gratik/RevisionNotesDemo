// ============================================================================
// COVARIANCE & CONTRAVARIANCE IN GENERICS
// Reference: Revision Notes - Page 7
// ============================================================================
// Covariance (out): Return more derived type (IEnumerable<Derived> -> IEnumerable<Base>)
// Contravariance (in): Accept parameters of less derived types
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
