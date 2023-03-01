namespace Tolk.Domain;

public abstract class AggregateBase
{
    private readonly List<Event> _unsavedEvents = new();

    protected AggregateBase(Guid id, IEnumerable<IEvent> events)
    {
        Id = id;
        PartitionKey = $"{GetType().Name}-{Id}";
        Replay(events);
    }

    public Guid Id { get; }

    public string PartitionKey { get; }

    public int Version { get; private set; } = -1;

    private void Replay(IEnumerable<IEvent> events)
    {
        foreach (var @event in events)
        {
            ApplyEvent(@event);
            Version += 1;
        }
    }

    public List<Event> UnsavedEvents()
    {
        // todo clear
        return _unsavedEvents;
    }

    protected abstract void ApplyEvent(IEvent @event);

    private void StoreEvent(IEvent @event)
    {
        @event.Version = Version;
        @event.Aggregate = $"{GetType().Name}-{Id}";
        _unsavedEvents.Add((@event as Event)!); // todo fix hack
    }

    protected void ApplyAndStoreEvent(IEvent @event)
    {
        ApplyEvent(@event);
        Version += 1;
        StoreEvent(@event);
    }
}
