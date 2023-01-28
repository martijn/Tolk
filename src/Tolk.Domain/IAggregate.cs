namespace Tolk.Domain;

public interface IAggregate
{
    Guid Id { get; }
    string PartitionKey { get; }

    void ApplyEvent(IEvent @event);
}
