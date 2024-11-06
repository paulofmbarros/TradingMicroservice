namespace Trading.Tests.OutboxProcessor;

using System.Text.Json;
using FluentAssertions;
using Moq;
using Trading.Api.BackgroundServices;
using Trading.Domain.Entities;
using Trading.Domain.Interfaces;

public class OutboxProcessorTests
{
    private readonly Mock<IOutboxRepository> outboxRepositoryMock;
    private readonly Mock<IKafkaProducer> kafkaProducerMock;
    private readonly OutboxProcessor processor;

    public OutboxProcessorTests()
    {
        this.outboxRepositoryMock = new Mock<IOutboxRepository>();
        this.kafkaProducerMock = new Mock<IKafkaProducer>();
        this.processor = new OutboxProcessor(this.outboxRepositoryMock.Object, this.kafkaProducerMock.Object);
    }

    [Fact]
    public async Task ProcessOutboxMessagesAsync_ShouldProduceMessagesAndMarkAsProcessed()
    {
        // Arrange
        var trade = new Trade("user123", "AAPL", 10, 150m);
        var outboxMessage = new OutboxMessage("TradeExecuted", JsonSerializer.Serialize(trade));
        var messages = new List<OutboxMessage> { outboxMessage };

        this.outboxRepositoryMock.Setup(x => x.GetUnprocessedMessagesAsync()).ReturnsAsync(messages);
        this.kafkaProducerMock
            .Setup(x => x.ProduceAsync(It.Is<string>(t => t == "trade-executed"), It.Is<Trade>(t => t.UserId == trade.UserId)))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await this.processor.ProcessOutboxMessagesAsync();

        // Assert
        this.kafkaProducerMock.Verify(x => x.ProduceAsync("trade-executed", It.Is<Trade>(t => t.UserId == "user123")), Times.Once);
        this.outboxRepositoryMock.Verify(x => x.MarkMessagesAsProcessedAsync(It.Is<List<OutboxMessage>>(m => m.Count == 1 && m[0].Processed)), Times.Once);
        outboxMessage.Processed.Should().BeTrue();
    }

}