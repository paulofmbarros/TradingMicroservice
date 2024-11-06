namespace Trading.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Trading.Domain.Entities;
using Trading.Domain.Interfaces;

public class TradeRepository(TradingDbContext context) : ITradeRepository
{
    public async Task AddAsync(Trade? trade) => await context.Trades.AddAsync(trade);

    public async Task<IEnumerable<Trade?>> GetAllAsync() => await context.Trades.ToListAsync();

    public async Task<Trade?> GetByIdAsync(Guid id) => await context.Trades.FirstOrDefaultAsync(t => t.Id == id);
}