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
        private readonly Mock<IKafkaProducer> kafkaProducerMock;
        private readonly ExecuteTradeCommandHandler handler;

        public ExecuteTradeCommandHandlerTests()
        {
            this.tradeRepositoryMock = new Mock<ITradeRepository>();
            this.unitOfWorkMock = new Mock<IUnitOfWork>();
            this.kafkaProducerMock = new Mock<IKafkaProducer>();
            this.handler = new ExecuteTradeCommandHandler(this.tradeRepositoryMock.Object, this.unitOfWorkMock.Object, this.kafkaProducerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldAddTradeAndCompleteUnitOfWork_WhenCommandIsValid()
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
            this.unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
            this.kafkaProducerMock.Verify(x => x.ProduceAsync(It.IsAny<string>(), It.IsAny<Trade>()), Times.Once);
        }
    }