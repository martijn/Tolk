using Tolk.Domain;
using Tolk.Domain.ProjectAggregate;

namespace Tolk.FunctionApp;

public interface IProjectFactory
{
    Project Create(Guid id, IEnumerable<IEvent> events);
}

/// <summary>
///     ProjectFactory builds a new Project aggregate by replaying the supplied Domain events
///     that were loaded from the database. It should only be used by ProjectBuilder. To create a
///     new project call <see cref="Tolk.Domain.ProjectAggregate.Project.Create" />.
/// </summary>
public class ProjectFactory : IProjectFactory
{
    public Project Create(Guid id, IEnumerable<IEvent> events)
    {
        return new Project(id, events);
    }
}
