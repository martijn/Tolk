namespace Tolk.Domain;

public interface IEvent
{
    Guid Id { get; }
    string Type { get; }
    string? Aggregate { get; set; }
    int? Version { get; set;  }
}
