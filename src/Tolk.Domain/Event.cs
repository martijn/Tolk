using System.Text.Json.Serialization;

namespace Tolk.Domain;

public class Event : IEvent
{
    protected Event(Guid _id)
    {
        Id = _id;
        Type = GetType().Name;
    }

    [JsonPropertyName("id")]
    public Guid Id { get; }

    public string Type { get; }
    public string? Aggregate { get; set; } // todo internal set?
    public int? Version { get; set; } // todo internal set?
}
