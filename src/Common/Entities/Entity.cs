using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Entities;

public abstract class Entity
{
    public Guid Id { get; set; }
    private readonly List<BaseEvent> _domainEvents = new();

    [NotMapped] 
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public string Payload  { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime? ProcessedOnUtc { get; set; }
    public string Status { get; set; } 
    public int RetryCount { get; set; }
    public string? Error { get; set; }
}
public abstract record BaseEvent
{
}