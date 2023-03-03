using System.Text.Json;
using Tolk.Domain;
using Tolk.Domain.ProjectAggregate;

namespace Tolk.FunctionApp;

public interface IProjectBuilder
{
    Project FromJsonEvents(Guid id, IEnumerable<JsonElement> jsonEvents);
}

/// <summary>
///     ProjectBuilder rehydrates a Project aggregate from a list of events received from CosmosDB, in the
///     original JSON format.
/// </summary>
public sealed class ProjectBuilder : IProjectBuilder
{
    private readonly IProjectFactory _projectFactory;

    public ProjectBuilder(IProjectFactory projectFactory)
    {
        _projectFactory = projectFactory;
    }

    public Project FromJsonEvents(Guid id, IEnumerable<JsonElement> jsonEvents)
    {
        var events = new List<IEvent>();

        foreach (var jsonEvent in jsonEvents)
        {
            var eventTypeString = jsonEvent.GetProperty("Type").GetString();
            var eventType = Type.GetType($"Tolk.Domain.ProjectAggregate.Events.{eventTypeString}, Tolk.Domain");

            if (eventType is null) throw new Exception($"Can't locate eventType {eventTypeString}");
            // TODO safe cast to IEvent
            events.Add((IEvent)jsonEvent.Deserialize(eventType)!);
        }

        return _projectFactory.Create(id, events);
    }
}
