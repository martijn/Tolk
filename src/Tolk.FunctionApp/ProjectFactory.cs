using Tolk.Domain;
using Tolk.Domain.ProjectAggregate;

namespace Tolk.FunctionApp;

public interface IProjectFactory
{
    Project Create(Guid id, IEnumerable<IEvent> events);
}

/// <summary>
///     ProjectFactory creates a new Project aggregate from the supplied Domain events.
/// </summary>
public class ProjectFactory : IProjectFactory
{
    public Project Create(Guid id, IEnumerable<IEvent> events)
    {
        return new Project(id, events);
    }
}
