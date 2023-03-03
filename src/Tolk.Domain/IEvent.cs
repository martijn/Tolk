using System.Text.Json.Serialization;

namespace Tolk.Domain;

public interface IEvent
{
    [JsonPropertyName("id")]
    Guid Id { get; }

    string Type { get; }
    string? Aggregate { get; set; }
    int? Version { get; set; }
}
