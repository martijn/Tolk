namespace Tolk.Domain.ProjectAggregate.Events;

public class ProjectArchivedEvent : Event
{
    public ProjectArchivedEvent(Guid id) : base(id)
    {
    }
}
