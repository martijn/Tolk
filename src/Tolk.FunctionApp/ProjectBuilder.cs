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
        var events = jsonEvents.Select(jsonEvent =>
        {
            var eventTypeString = jsonEvent.TryGetProperty("Type", out var typeElement)
                ? typeElement.GetString()
                : throw new InvalidEventException("Event does not have a 'Type' attribute");

            var eventType = Type.GetType($"Tolk.Domain.ProjectAggregate.Events.{eventTypeString}, Tolk.Domain")
                            ?? throw new InvalidEventException($"Unknown eventType '{eventTypeString}'");

            if (jsonEvent.Deserialize(eventType) is not IEvent deserializedEvent)
                throw new InvalidEventException($"Could not cast {eventType} to IEvent");

            return deserializedEvent;
        });

        return _projectFactory.Create(id, events);
    }

    public class InvalidEventException : Exception
    {
        public InvalidEventException(string? message) : base(message)
        {
        }
    }
}
