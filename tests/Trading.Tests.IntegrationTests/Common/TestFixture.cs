namespace Trading.Tests.IntegrationTests.Common;


using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Trading.Application.Commands.ExecuteTrade;
using Trading.Application.Queries.GetTradeById;
using Trading.Application.Queries.GetTradesQuery;
using Trading.Domain.Interfaces;
using Trading.Infrastructure;
using Trading.Infrastructure.Repositories;
using Trading.Infrastructure.UnitOfWork;
using Xunit;

public class TestFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; private set; }

    public TestFixture()
    {
        var services = new ServiceCollection();

        // Add DbContext with in-memory database
        services.AddDbContext<TradingDbContext>(options =>
            options.UseInMemoryDatabase(databaseName: "TradingDb"));

        // Register repositories, handlers, and UnitOfWork
        services.AddScoped<ITradeRepository, TradeRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IKafkaProducer, FakeKafkaProducer>(); // Using a fake Kafka producer for testing
        services.AddScoped<IOutboxRepository, OutboxRepository>(); // Using a fake Kafka producer for testing
        services.AddScoped<ExecuteTradeCommandHandler>();
        services.AddScoped<GetTradesQueryHandler>();
        services.AddScoped<GetTradeByIdQueryHandler>();

        ServiceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        // Cleanup
    }
}