using System.Text.Json;
using Tolk.Domain.ProjectAggregate;

namespace Tolk.FunctionApp;

public interface IProjectBuilder
{
    Project FromJsonEvents(Guid id, IEnumerable<JsonElement> jsonEvents);
}
