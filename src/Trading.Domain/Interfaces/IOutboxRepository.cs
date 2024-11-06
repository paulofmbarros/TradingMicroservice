namespace Trading.Domain.Interfaces;

using Trading.Domain.Entities;

public interface IOutboxRepository
{
    Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync();
    Task MarkMessagesAsProcessedAsync(IEnumerable<OutboxMessage> messages);

    Task AddOutboxMessageAsync(OutboxMessage message);
}