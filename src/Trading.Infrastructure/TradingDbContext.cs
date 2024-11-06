namespace Trading.Infrastructure;

using Trading.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Trading.Domain.Entities;

public class TradingDbContext : DbContext
{
    public TradingDbContext(DbContextOptions<TradingDbContext> options) : base(options) { }

    public DbSet<Trade?> Trades { get; set; }
}