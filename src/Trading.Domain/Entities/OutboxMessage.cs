namespace Trading.Domain.Entities;

public class OutboxMessage(string eventType, string payload)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string EventType { get; private set; } = eventType;
    public string Payload { get; private set; } = payload;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public bool Processed { get; set; } = false;
}