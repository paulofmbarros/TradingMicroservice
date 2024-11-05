namespace Trading.Infrastructure.UnitOfWork;

using Trading.Domain.Interfaces;

public class UnitOfWork : IUnitOfWork
{
    private readonly TradingDbContext context;

    public UnitOfWork(TradingDbContext context)
    {
        this.context = context;
    }

    public async Task<bool> CommitAsync() => await this.context.SaveChangesAsync() > 0;

    public void Dispose()
    {
        this.context.Dispose();
    }
}