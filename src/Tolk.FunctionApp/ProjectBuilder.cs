using System.Text.Json;
using Tolk.Domain;
using Tolk.Domain.ProjectAggregate;

namespace Tolk.FunctionApp;

public static class ProjectBuilder
{
    public static Project FromJsonEvents(Guid id, IEnumerable<JsonElement> jsonEvents)
    {
        var events = new List<IEvent>();

        foreach (var jsonEvent in jsonEvents)
        {
            var eventTypeString = jsonEvent.GetProperty("Type").GetString();
            var eventType = Type.GetType($"Tolk.Domain.ProjectAggregate.Events.{eventTypeString}, Tolk.Domain");

            if (eventType is null) throw new Exception($"Can't locate eventType {eventTypeString}");
            events.Add((IEvent)jsonEvent.Deserialize(eventType)!);
        }

        return new Project(id, events);
    }
}
