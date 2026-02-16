# Covariance and Contravariance

> Subject: [Core-CSharp](../README.md)

## Covariance and Contravariance

### Covariance (out)

```csharp
// ✅ Covariant interface (output only)
public interface IProducer<out T>
{
    T Produce();
}

public class Animal { }
public class Dog : Animal { }

public class DogProducer : IProducer<Dog>
{
    public Dog Produce() => new Dog();
}

// ✅ Covariance allows this
IProducer<Dog> dogProducer = new DogProducer();
IProducer<Animal> animalProducer = dogProducer;  // ✅ OK!
```

### Contravariance (in)

```csharp
// ✅ Contravariant interface (input only)
public interface IConsumer<in T>
{
    void Consume(T item);
}

public class AnimalConsumer : IConsumer<Animal>
{
    public void Consume(Animal animal) { /* ... */ }
}

// ✅ Contravariance allows this
IConsumer<Animal> animalConsumer = new AnimalConsumer();
IConsumer<Dog> dogConsumer = animalConsumer;  // ✅ OK!
```

---


