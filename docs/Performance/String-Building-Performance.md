# String Building Performance

> Subject: [Performance](../README.md)

## String Building Performance

### Comparison

```csharp
// ❌ WORST: String concatenation (1000x slower)
public string BuildMessage_Bad(int count)
{
    string result = "";
    for (int i = 0; i < count; i++)
        result += $"Item {i},";  // ❌ New string each iteration!
    return result;
}

// ✅ GOOD: StringBuilder (100x faster)
public string BuildMessage_Better(int count)
{
    var sb = new StringBuilder(count * 10);  // ✅ Preallocate
    for (int i = 0; i < count; i++)
        sb.Append($"Item {i},");
    return sb.ToString();
}

// ✅ BEST: Span + stackalloc (zero allocations until final string)
public string BuildMessage_Best(int count)
{
    var chars = ArrayPool<char>.Shared.Rent(count * 10);
    try
    {
        Span<char> buffer = chars;
        int position = 0;
        
        for (int i = 0; i < count; i++)
        {
            // Write directly to buffer
            if (!i.TryFormat(buffer[position..], out int written))
                break;
            position += written;
            buffer[position++] = ',';
        }
        
        return new string(buffer[..position]);
    }
    finally
    {
        ArrayPool<char>.Shared.Return(chars);
    }
}
```

---


