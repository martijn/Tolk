namespace Tolk.Domain.ProjectAggregate.Events;

public class PhraseCreatedEvent : Event
{
    public PhraseCreatedEvent(Guid id, string name) : base(id)
    {
        Name = name;
    }

    public string Name { get; }
}
