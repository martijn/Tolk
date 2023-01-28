namespace Tolk.Domain.ProjectAggregate.Events;

public class SomePropertyChangedEvent : Event
{
    public SomePropertyChangedEvent(Guid eventId, string newValue) : base(eventId)
    {
        EventId = eventId;
        NewValue = newValue;
    }

    public Guid EventId { get; }
    public string NewValue { get; }
}
