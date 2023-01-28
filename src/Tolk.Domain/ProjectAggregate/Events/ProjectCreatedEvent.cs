namespace Tolk.Domain.ProjectAggregate.Events;

public class ProjectCreatedEvent : Event
{
    public ProjectCreatedEvent(Guid id, string Name) : base(id)
    {
        this.Name = Name;
    }

    public string Name { get; }
}
