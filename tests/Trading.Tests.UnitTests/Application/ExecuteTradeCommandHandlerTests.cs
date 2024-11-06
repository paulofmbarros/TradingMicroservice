namespace Trading.Tests.Application;

using FluentAssertions;
using Moq;
using Trading.Application.Commands.ExecuteTrade;
using Trading.Application.Queries.GetTradeById;
using Trading.Application.Queries.GetTradesQuery;
using Trading.Domain.Entities;
using Trading.Domain.Interfaces;

public class ExecuteTradeCommandHandlerTests
{
    private readonly Mock<ITradeRepository> tradeRepositoryMock;
    private readonly Mock<IUnitOfWork> unitOfWorkMock;
    private readonly Mock<IOutboxRepository> outboxRepositoryMock;
    private readonly ExecuteTradeCommandHandler handler;

    public ExecuteTradeCommandHandlerTests()
    {
        this.tradeRepositoryMock = new Mock<ITradeRepository>();
        this.unitOfWorkMock = new Mock<IUnitOfWork>();
        this.outboxRepositoryMock = new Mock<IOutboxRepository>();

        this.handler = new ExecuteTradeCommandHandler(
            this.tradeRepositoryMock.Object,
            this.unitOfWorkMock.Object,
            this.outboxRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldAddTradeAndOutboxMessageAndCompleteUnitOfWork_WhenCommandIsValid()
    {
        // Arrange
        var command = new ExecuteTradeCommand
        {
            UserId = "user123",
            Asset = "AAPL",
            Quantity = 10,
            Price = 150.00m
        };
        this.unitOfWorkMock.Setup(x => x.CommitAsync()).ReturnsAsync(true);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        this.tradeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Trade>()), Times.Once);
        this.outboxRepositoryMock.Verify(x => x.AddOutboxMessageAsync(It.Is<OutboxMessage>(
            m => m.EventType == "TradeExecuted" && m.Payload.Contains("user123"))), Times.Once);
        this.unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenUnitOfWorkFails()
    {
        // Arrange
        var command = new ExecuteTradeCommand
        {
            UserId = "user123",
            Asset = "AAPL",
            Quantity = 10,
            Price = 150.00m
        };
        this.unitOfWorkMock.Setup(x => x.CommitAsync()).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await this.handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Failed to complete the trade transaction.");
        this.tradeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Trade>()), Times.Once);
        this.outboxRepositoryMock.Verify(x => x.AddOutboxMessageAsync(It.IsAny<OutboxMessage>()), Times.Once);
    }
}