// ==============================================================================
// SPAN AND MEMORY - Zero-Allocation Performance
// ==============================================================================
// WHAT IS THIS?
// -------------
// Span<T>/Memory<T> patterns for zero-allocation slicing and parsing.
//
// WHY IT MATTERS
// --------------
// ✅ Reduces GC pressure in hot paths
// ✅ Improves throughput for parsing and buffers
//
// WHEN TO USE
// -----------
// ✅ High-throughput parsing, networking, or file I/O
// ✅ Large buffer processing where allocations matter
//
// WHEN NOT TO USE
// ---------------
// ❌ Low-traffic CRUD apps where complexity outweighs gains
// ❌ Code where readability is more valuable than micro-optimizations
//
// REAL-WORLD EXAMPLE
// ------------------
// Parse CSV lines with ReadOnlySpan<char>.
// ==============================================================================

using System;
using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;

namespace RevisionNotesDemo.Performance;

/// <summary>
/// EXAMPLE 1: SPAN BASICS - Stack-Based Array Window
/// 
/// THE PROBLEM:
/// Array slicing with SubArray() creates new arrays - heap allocations.
/// string.Substring() creates new strings - heap allocations.
/// 
/// THE SOLUTION:
/// Span<T> provides a view into existing memory - zero allocations.
/// 
/// WHY IT MATTERS:
/// - No GC pressure
/// - Cache-friendly
/// - Bounds-checked (safe)
/// </summary>
public class SpanBasicsExamples
{
    // ❌ BAD: Traditional approach - allocations
    public void ProcessArray_Traditional(int[] data)
    {
        // Each slice creates a NEW array
        var firstHalf = data.Take(data.Length / 2).ToArray();   // ❌ Allocation
        var secondHalf = data.Skip(data.Length / 2).ToArray();  // ❌ Allocation

        var sum1 = firstHalf.Sum();   // ❌ Enumerate allocated array
        var sum2 = secondHalf.Sum();  // ❌ Enumerate allocated array
    }

    // ✅ GOOD: Span approach - zero allocations
    public void ProcessArray_Span(int[] data)
    {
        Span<int> dataSpan = data;  // ✅ No allocation, just a view

        // Slicing creates views, not copies
        Span<int> firstHalf = dataSpan[..(dataSpan.Length / 2)];    // ✅ Zero allocation
        Span<int> secondHalf = dataSpan[(dataSpan.Length / 2)..];   // ✅ Zero allocation

        var sum1 = Sum(firstHalf);   // ✅ Process view directly
        var sum2 = Sum(secondHalf);
    }

    private int Sum(Span<int> span)
    {
        int total = 0;
        foreach (var item in span)  // ✅ Efficient iteration
            total += item;
        return total;
    }

    // ✅ GOOD: String parsing without allocations
    public (int hours, int minutes) ParseTime_Span(string time)  // "14:30"
    {
        ReadOnlySpan<char> span = time;  // ✅ View into string

        int colonIndex = span.IndexOf(':');

        var hoursSpan = span[..colonIndex];        // ✅ Slice, no allocation
        var minutesSpan = span[(colonIndex + 1)..]; // ✅ Slice, no allocation

        int hours = int.Parse(hoursSpan);      // ✅ Parse span directly (C# 8+)
        int minutes = int.Parse(minutesSpan);

        return (hours, minutes);
    }

    // ❌ BAD: Traditional string parsing - allocations
    public (int hours, int minutes) ParseTime_Traditional(string time)
    {
        var parts = time.Split(':');  // ❌ Allocates array
        int hours = int.Parse(parts[0]);    // ❌ Each string is allocation
        int minutes = int.Parse(parts[1]);
        return (hours, minutes);
    }
}

/// <summary>
/// EXAMPLE 2: STACKALLOC - Ultra-Fast Temporary Buffers
/// 
/// THE PROBLEM:
/// Need temporary buffer, new[] allocates on heap.
/// 
/// THE SOLUTION:
/// stackalloc allocates on stack - instant allocation/cleanup.
/// 
/// WHY IT MATTERS:
/// - 100x faster allocation
/// - Zero GC impact
/// - Automatic cleanup
/// 
/// GOTCHA: Stack is limited (~1MB), only for small buffers.
/// </summary>
public class StackallocExamples
{
    // ✅ GOOD: Stackalloc for temporary buffers
    public string ProcessData_Stackalloc(ReadOnlySpan<byte> data)
    {
        const int MaxStackSize = 256;

        if (data.Length <= MaxStackSize)
        {
            // ✅ Small buffer - use stack
            Span<char> buffer = stackalloc char[data.Length];

            for (int i = 0; i < data.Length; i++)
                buffer[i] = (char)data[i];

            return new string(buffer);  // Convert to string once
        }
        else
        {
            // ✅ Large buffer - use ArrayPool
            var rented = ArrayPool<char>.Shared.Rent(data.Length);
            try
            {
                Span<char> buffer = rented.AsSpan(0, data.Length);

                for (int i = 0; i < data.Length; i++)
                    buffer[i] = (char)data[i];

                return new string(buffer);
            }
            finally
            {
                ArrayPool<char>.Shared.Return(rented);
            }
        }
    }

    // ✅ GOOD: StringBuilder with stackalloc
    public string BuildString_Efficient(int count)
    {
        Span<char> buffer = stackalloc char[20];  // ✅ Stack buffer for formatting

        var sb = new StringBuilder();
        for (int i = 0; i < count; i++)
        {
            if (i.TryFormat(buffer, out int written))  // ✅ Format to span
            {
                sb.Append(buffer[..written]);
                sb.Append(", ");
            }
        }

        return sb.ToString();
    }

    // TIP: C# 8+ allows stackalloc in expressions
    public void ModernStackalloc()
    {
        // ✅ Expression mode (C# 8+)
        Span<int> numbers = stackalloc int[] { 1, 2, 3, 4, 5 };

        // ✅ Pattern: Use span, no explicit array
        ProcessNumbers(stackalloc int[] { 10, 20, 30 });
    }

    private void ProcessNumbers(Span<int> numbers)
    {
        foreach (var num in numbers)
            Console.WriteLine(num);
    }
}

/// <summary>
/// EXAMPLE 3: MEMORY<T> - Async-Friendly Span
/// 
/// THE PROBLEM:
/// Span<T> is ref struct - cannot use in async methods or store in fields.
/// 
/// THE SOLUTION:
/// Memory<T> provides similar benefits but is async-safe.
/// 
/// WHY IT MATTERS:
/// - Async I/O operations
/// - Store in classes
/// - Slightly more overhead than Span
/// </summary>
public class MemoryExamples
{
    // ❌ WILL NOT COMPILE: Span in async method
    // public async Task ProcessAsync(Span<byte> data)  // ❌ ref struct not allowed
    // {
    //     await Task.Delay(100);  // ❌ Span can't survive await
    // }

    // ✅ GOOD: Memory<T> in async methods
    public async Task ProcessAsync_Memory(Memory<byte> data)
    {
        await Task.Delay(100);  // ✅ Memory survives await

        // Convert to Span when needed
        Span<byte> span = data.Span;  // ✅ Get span for synchronous work
        ProcessData(span);
    }

    private void ProcessData(Span<byte> data)
    {
        // Synchronous processing with Span
        for (int i = 0; i < data.Length; i++)
            data[i] = (byte)(data[i] * 2);
    }

    // ✅ GOOD: Store Memory in class field
    public class BufferedProcessor
    {
        private readonly Memory<byte> _buffer;  // ✅ OK to store Memory

        public BufferedProcessor(int size)
        {
            _buffer = new byte[size];
        }

        public async Task ProcessAsync(Stream stream)
        {
            // ✅ Read directly into Memory
            int bytesRead = await stream.ReadAsync(_buffer);

            // ✅ Process the data (convert to Span)
            ProcessChunk(_buffer.Span[..bytesRead]);
        }

        private void ProcessChunk(Span<byte> chunk)
        {
            // Process data
        }
    }
}

/// <summary>
/// EXAMPLE 4: STRING MANIPULATION - Zero-Allocation Parsing
/// 
/// THE PROBLEM:
/// String operations (Split, Substring, Trim) allocate heavily.
/// 
/// THE SOLUTION:
/// ReadOnlySpan<char> for parsing, MemoryExtensions methods.
/// </summary>
public class StringManipulationExamples
{
    // ❌ BAD: Traditional CSV parsing - many allocations
    public List<(string name, int age)> ParseCsv_Traditional(string csv)
    {
        var results = new List<(string, int)>();

        var lines = csv.Split('\n');  // ❌ Allocates array
        foreach (var line in lines)
        {
            var trimmed = line.Trim();  // ❌ New string
            var parts = trimmed.Split(',');  // ❌ Array + strings

            if (parts.Length == 2)
            {
                results.Add((parts[0].Trim(), int.Parse(parts[1].Trim())));  // ❌ More allocations
            }
        }

        return results;
    }

    // ✅ GOOD: Span-based parsing - minimal allocations
    public List<(string name, int age)> ParseCsv_Span(string csv)
    {
        var results = new List<(string, int)>();

        ReadOnlySpan<char> remaining = csv;

        while (remaining.Length > 0)
        {
            int newlineIndex = remaining.IndexOf('\n');
            ReadOnlySpan<char> line = newlineIndex >= 0
                ? remaining[..newlineIndex]
                : remaining;

            remaining = newlineIndex >= 0
                ? remaining[(newlineIndex + 1)..]
                : ReadOnlySpan<char>.Empty;

            line = line.Trim();  // ✅ Returns Span, no allocation

            int commaIndex = line.IndexOf(',');
            if (commaIndex > 0)
            {
                var name = line[..commaIndex].Trim();
                var ageSpan = line[(commaIndex + 1)..].Trim();

                if (int.TryParse(ageSpan, out int age))
                {
                    // Only allocation: converting name span to string for storage
                    results.Add((name.ToString(), age));
                }
            }
        }

        return results;
    }

    // ✅ GOOD: Path manipulation without allocations
    public ReadOnlySpan<char> GetFileName(ReadOnlySpan<char> path)
    {
        int lastSlash = path.LastIndexOfAny("/\\");
        return lastSlash >= 0 ? path[(lastSlash + 1)..] : path;
    }

    public ReadOnlySpan<char> GetExtension(ReadOnlySpan<char> path)
    {
        int lastDot = path.LastIndexOf('.');
        return lastDot >= 0 ? path[lastDot..] : ReadOnlySpan<char>.Empty;
    }

    // Usage:
    public void Example()
    {
        string fullPath = "C:\\Users\\Documents\\report.xlsx";

        var fileName = GetFileName(fullPath);        // ✅ "report.xlsx" (no allocation)
        var extension = GetExtension(fullPath);      // ✅ ".xlsx" (no allocation)

        Console.WriteLine($"File: {fileName}, Ext: {extension}");  // Convert at print time
    }
}

/// <summary>
/// EXAMPLE 5: SPAN CONVERSIONS AND MARSHALLING
/// 
/// THE PROBLEM:
/// Need to work with different memory representations.
/// 
/// THE SOLUTION:
/// MemoryMarshal for casting between types.
/// </summary>
public class SpanConversionExamples
{
    // ✅ GOOD: Cast span from bytes to ints (reinterpret memory)
    public void ReinterpretMemory()
    {
        Span<byte> bytes = stackalloc byte[16];

        // Fill with data
        for (int i = 0; i < bytes.Length; i++)
            bytes[i] = (byte)i;

        // ✅ Cast to int span (4 bytes = 1 int)
        Span<int> ints = MemoryMarshal.Cast<byte, int>(bytes);

        Console.WriteLine($"Bytes: {bytes.Length}, Ints: {ints.Length}");  // 16, 4

        // Modify ints affects bytes (same memory)
        ints[0] = 42;
        Console.WriteLine($"First 4 bytes: {bytes[0]}, {bytes[1]}, {bytes[2]}, {bytes[3]}");
    }

    // ✅ GOOD: String to span without allocation
    public int CountVowels(string text)
    {
        ReadOnlySpan<char> span = text.AsSpan();  // ✅ View into string
        ReadOnlySpan<char> vowels = "aeiouAEIOU".AsSpan();

        int count = 0;
        foreach (char c in span)
        {
            if (vowels.Contains(c))
                count++;
        }

        return count;
    }

    // ✅ GOOD: Format numbers to existing buffer
    public void FormatToBuffer()
    {
        Span<char> buffer = stackalloc char[50];
        int position = 0;

        // Format multiple values into same buffer
        if (DateTime.Now.TryFormat(buffer[position..], out int written1, "yyyy-MM-dd"))
        {
            position += written1;
            buffer[position++] = ' ';
        }

        if (123.456.TryFormat(buffer[position..], out int written2, "F2"))
        {
            position += written2;
        }

        var result = new string(buffer[..position]);  // "2024-01-15 123.46"
    }
}

/// <summary>
/// EXAMPLE 6: PERFORMANCE PATTERNS AND GOTCHAS
/// </summary>
public class PerformancePatternsAndGotchas
{
    // ✅ PATTERN: Span for synchronous, Memory for async
    public void SyncWork(Span<byte> data) { }  // ✅ Use Span
    public async Task AsyncWork(Memory<byte> data) { await Task.Yield(); }  // ✅ Use Memory

    // ✅ PATTERN: ArrayPool for large temporary buffers
    public void ProcessLargeData()
    {
        const int size = 1024 * 1024;  // 1 MB - too big for stackalloc

        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            Span<byte> span = buffer.AsSpan(0, size);
            // Use span...
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer, clearArray: true);
        }
    }

    // GOTCHA 1: Span cannot be stored in fields
    public class BadExample
    {
        // ❌ WILL NOT COMPILE
        // private Span<byte> _buffer;  // ref struct cannot be field

        // ✅ Use Memory instead
        private Memory<byte> _buffer;  // OK
    }

    // GOTCHA 2: Span cannot escape method
    public Span<int> BadReturn()
    {
        Span<int> local = stackalloc int[10];
        // return local;  // ❌ WILL NOT COMPILE - stack memory would be invalid

        // ✅ Return array-backed span if needed
        var array = new int[10];
        return array.AsSpan();
    }

    // GOTCHA 3: Stackalloc size limit
    public void StackallocLimit()
    {
        // ✅ Small - OK
        Span<int> small = stackalloc int[100];

        // ❌ Large - stack overflow risk
        // Span<int> huge = stackalloc int[1_000_000];  // Don't do this

        // ✅ Use threshold pattern
        const int MaxStackSize = 256;

        Span<int> buffer = stackalloc int[Math.Min(100, MaxStackSize)];
    }

    // TIP: Benchmark before optimizing
    // Span adds complexity - only use when perf matters
}

// SUMMARY - Span vs Memory vs Array:
//
// +-----------------+----------------+------------------+-------------------+
// | Feature         | Array          | Span<T>          | Memory<T>         |
// +-----------------+----------------+------------------+-------------------+
// | Heap allocation | Yes            | No (view)        | No (view)         |
// | Async methods   | Yes            | ❌ NO            | ✅ Yes            |
// | Store in field  | Yes            | ❌ NO            | ✅ Yes            |
// | Performance     | Baseline       | Fastest          | Fast (slight cost)|
// | Use case        | Storage        | Sync perf work   | Async perf work   |
// +-----------------+----------------+------------------+-------------------+
//
// DECISION TREE:
// - Do you need it in an async method? → Memory<T>
// - Do you need it in a field? → Memory<T>
// - Do you need synchronous perf? → Span<T>
// - Do you need to store long-term? → Array/List
//
// WHEN TO USE:
// ✅ High-throughput APIs
// ✅ Parsing/formatting hot paths
// ✅ Network/file I/O
// ✅ Large array processing
// ❌ Not useful for CRUD apps with low traffic
