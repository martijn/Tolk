namespace Tolk.Domain;

public abstract class AggregateBase
{
    protected AggregateBase(Guid id, IEnumerable<IEvent> events)
    {
        Id = id;
        PartitionKey = $"{GetType().Name}-{Id}";
        Replay(events);
    }

    public Guid Id { get; }

    public string PartitionKey { get; }
    // TODO Unsaved events

    public int Version { get; protected set; } = -1;
   
    protected bool _isReplaying;
    protected List<IEvent> _unsavedEvents = new List<IEvent>();

    private void Replay(IEnumerable<IEvent> events)
    {
        _isReplaying = true;
        foreach (var @event in events) ApplyEvent(@event);
        _isReplaying = false;
    }

    public List<IEvent> UnsavedEvents()
    {
        // todo clear
        return _unsavedEvents;
    }

    public abstract void ApplyEvent(IEvent @event);
}
