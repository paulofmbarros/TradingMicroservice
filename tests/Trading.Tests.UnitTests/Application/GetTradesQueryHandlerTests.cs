namespace Trading.Tests.Application;

using FluentAssertions;
using Moq;
using Trading.Application.Queries.GetTradesQuery;
using Trading.Domain.Entities;
using Trading.Domain.Interfaces;

public class GetTradesQueryHandlerTests
{
    private readonly Mock<ITradeRepository> tradeRepositoryMock;
    private readonly GetTradesQueryHandler handler;

    public GetTradesQueryHandlerTests()
    {
        this.tradeRepositoryMock = new Mock<ITradeRepository>();
        this.handler = new GetTradesQueryHandler(this.tradeRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllTrades()
    {
        // Arrange
        var trades = new[]
        {
            new Trade("user1", "AAPL", 10, 150m),
            new Trade("user2", "TSLA", 5, 800m)
        };
        this.tradeRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(trades);

        // Act
        var result = await this.handler.Handle(new GetTradesQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().HaveCount(2);
        result.Should().ContainEquivalentOf(new { UserId = "user1", Asset = "AAPL" });
        result.Should().ContainEquivalentOf(new { UserId = "user2", Asset = "TSLA" });
    }
}