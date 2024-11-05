namespace Trading.Domain.SeedWork;

public class EntityBase
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; private set; }

    protected EntityBase()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}