namespace Trading.Tests.IntegrationTests.Trade;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Trading.Application.Commands.ExecuteTrade;
using Trading.Application.Queries.GetTradeById;
using Trading.Application.Queries.GetTradesQuery;
using Trading.Domain.Entities;
using Trading.Infrastructure;
using Trading.Tests.IntegrationTests.Common;

public class TradeIntegrationTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    private readonly IServiceProvider serviceProvider = fixture.ServiceProvider;

    [Fact]
    public async Task ExecuteTrade_ShouldPersistTradeAndProduceMessage()
    {
        // Arrange
        using var scope = this.serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ExecuteTradeCommandHandler>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TradingDbContext>();

        var command = new ExecuteTradeCommand
        {
            UserId = "user123",
            Asset = "AAPL",
            Quantity = 10,
            Price = 150.00m
        };

        // Act
        var tradeId = await handler.Handle(command, default);

        // Assert
        var trade = await dbContext.Trades.FirstOrDefaultAsync(t => t.Id == tradeId);
        trade.Should().NotBeNull();
        trade.UserId.Should().Be(command.UserId);
        trade.Asset.Should().Be(command.Asset);
        trade.Quantity.Should().Be(command.Quantity);
        trade.Price.Should().Be(command.Price);
    }

    [Fact]
    public async Task GetTrades_ShouldReturnAllTrades()
    {
        // Arrange
        using var scope = this.serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<GetTradesQueryHandler>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TradingDbContext>();

        // Clear existing data to ensure consistency between test runs
        dbContext.Trades.RemoveRange(dbContext.Trades);
        await dbContext.SaveChangesAsync();

        dbContext.Trades.AddRange(
            new Trade("user1", "AAPL", 10, 150m),
            new Trade("user2", "TSLA", 5, 800m)
        );
        await dbContext.SaveChangesAsync();

        // Act
        var trades = await handler.Handle(new GetTradesQuery(), default);

        // Assert
        trades.Should().NotBeNullOrEmpty();
        trades.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTradeById_ShouldReturnCorrectTrade()
    {
        // Arrange
        using var scope = this.serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<GetTradeByIdQueryHandler>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TradingDbContext>();

        // Clear existing data to ensure consistency between test runs
        dbContext.Trades.RemoveRange(dbContext.Trades);
        await dbContext.SaveChangesAsync();

        var trade = new Trade("user123", "AAPL", 10, 150m);
        dbContext.Trades.Add(trade);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await handler.Handle(new GetTradeByIdQuery { Id = trade.Id }, default);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(trade.UserId);
        result.Asset.Should().Be(trade.Asset);
        result.Quantity.Should().Be(trade.Quantity);
        result.Price.Should().Be(trade.Price);

    }
}