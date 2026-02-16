using System.Collections.Concurrent;

namespace RevisionNotes.Ddd.ModularMonolith.Modules.Shared;

public interface IModularEvent
{
    string Name { get; }
    DateTimeOffset OccurredAtUtc { get; }
}

public interface IModularEventBus
{
    void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class, IModularEvent;
    Task PublishAsync<TEvent>(TEvent modularEvent, CancellationToken cancellationToken) where TEvent : class, IModularEvent;
    IReadOnlyList<IModularEvent> GetPublishedEvents();
}

public sealed class InMemoryModularEventBus : IModularEventBus
{
    private readonly ConcurrentDictionary<Type, List<Func<IModularEvent, Task>>> _handlers = new();
    private readonly ConcurrentQueue<IModularEvent> _published = new();

    public void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class, IModularEvent
    {
        var list = _handlers.GetOrAdd(typeof(TEvent), _ => []);
        lock (list)
        {
            list.Add(evt => handler((TEvent)evt));
        }
    }

    public async Task PublishAsync<TEvent>(TEvent modularEvent, CancellationToken cancellationToken) where TEvent : class, IModularEvent
    {
        _published.Enqueue(modularEvent);

        if (!_handlers.TryGetValue(typeof(TEvent), out var handlers))
        {
            return;
        }

        List<Func<IModularEvent, Task>> copy;
        lock (handlers)
        {
            copy = [.. handlers];
        }

        foreach (var handler in copy)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await handler(modularEvent);
        }
    }

    public IReadOnlyList<IModularEvent> GetPublishedEvents() => _published.ToArray();
}
