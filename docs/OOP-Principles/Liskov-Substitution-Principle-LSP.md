# Liskov Substitution Principle (LSP)

> Subject: [OOP-Principles](../README.md)

## Liskov Substitution Principle (LSP)

**"Objects of a superclass should be replaceable with objects of a subclass without breaking the application."**

### ❌ Violation

```csharp
// ❌ BAD: Square violates the Rectangle contract
public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }
    
    public int CalculateArea() => Width * Height;
}

public class Square : Rectangle
{
    public override int Width
    {
        get => base.Width;
        set { base.Width = value; base.Height = value; }  // ❌ Side effect!
    }
    
    public override int Height
    {
        get => base.Height;
        set { base.Width = value; base.Height = value; }  // ❌ Side effect!
    }
}

// Test
Rectangle rect = new Square();
rect.Width = 5;
rect.Height = 10;
Console.WriteLine(rect.CalculateArea());  // Expected 50, got 100!
// Square is NOT a valid substitute for Rectangle
```

### ✅ Solution

```csharp
// ✅ GOOD: Separate hierarchies or use composition
public interface IShape
{
    int CalculateArea();
}

public class Rectangle : IShape
{
    public int Width { get; set; }
    public int Height { get; set; }
    
    public int CalculateArea() => Width * Height;
}

public class Square : IShape
{
    public int Side { get; set; }
    
    public int CalculateArea() => Side * Side;
}
// Both implement the same contract correctly
```

---


