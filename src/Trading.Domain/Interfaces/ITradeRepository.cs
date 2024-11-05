namespace Trading.Domain.Interfaces;

using Trading.Domain.Entities;

public interface ITradeRepository
{
    Task<Trade> GetByIdAsync(Guid id);
    Task AddAsync(Trade trade);

    Task<IEnumerable<Trade>> GetAllAsync();
}