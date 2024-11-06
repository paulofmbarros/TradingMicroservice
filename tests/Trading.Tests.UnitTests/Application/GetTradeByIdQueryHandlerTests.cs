namespace Trading.Tests.Application;

using FluentAssertions;
using Moq;
using Trading.Application.Queries.GetTradeById;
using Trading.Domain.Entities;
using Trading.Domain.Interfaces;
using Trading.Domain.SeedWork;

public class GetTradeByIdQueryHandlerTests
{
    private readonly Mock<ITradeRepository> tradeRepositoryMock;
    private readonly GetTradeByIdQueryHandler handler;

    public GetTradeByIdQueryHandlerTests()
    {
        this.tradeRepositoryMock = new Mock<ITradeRepository>();
        this.handler = new GetTradeByIdQueryHandler(this.tradeRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrade_WhenTradeExists()
    {
        // Arrange
        var trade = new Trade("userId", "AAPL", 10, 150m);
        this.tradeRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(trade);

        // Act
        var result = await this.handler.Handle(new GetTradeByIdQuery { Id = Guid.NewGuid() }, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(trade.UserId);
        result.Asset.Should().Be(trade.Asset);
        result.Quantity.Should().Be(trade.Quantity);
        result.Price.Should().Be(trade.Price);
        result.Timestamp.Should().Be(trade.Timestamp);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenTradeDoesNotExist()
    {
        // Arrange
        this.tradeRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Trade)null);

        // Act
        var result = await this.handler.Handle(new GetTradeByIdQuery { Id = Guid.NewGuid() }, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}