namespace Trading.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Trading.Domain.Entities;
using Trading.Domain.Interfaces;

public class OutboxRepository : IOutboxRepository
{
    private readonly TradingDbContext dbContext;

    public OutboxRepository(TradingDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<OutboxMessage>> GetUnprocessedMessagesAsync()
    {
        return await this.dbContext.OutboxMessages
            .Where(m => !m.Processed)
            .ToListAsync();
    }

    public async Task MarkMessagesAsProcessedAsync(IEnumerable<OutboxMessage> messages)
    {
        this.dbContext.OutboxMessages.UpdateRange(messages);
        await this.dbContext.SaveChangesAsync();
    }

    public async Task AddOutboxMessageAsync(OutboxMessage message)
    {
        await this.dbContext.OutboxMessages.AddAsync(message);
        await this.dbContext.SaveChangesAsync();
    }
}