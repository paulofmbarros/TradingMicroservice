namespace Trading.Tests.IntegrationTests.Outbox;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Trading.Domain.Entities;
using Trading.Domain.Interfaces;
using Trading.Infrastructure;
using Trading.Infrastructure.Repositories;

public class OutboxRepositoryTests
{
    private readonly TradingDbContext dbContext;
    private readonly IOutboxRepository outboxRepository;

    public OutboxRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TradingDbContext>()
            .UseInMemoryDatabase(databaseName: "OutboxTestDb")
            .Options;

        this.dbContext = new TradingDbContext(options);
        this.outboxRepository = new OutboxRepository(this.dbContext);
    }

    [Fact]
    public async Task AddOutboxMessageAsync_ShouldAddMessageToDatabase()
    {
        // Arrange
        var message = new OutboxMessage("TestEvent", "{\"data\":\"test\"}");

        // Act
        await this.outboxRepository.AddOutboxMessageAsync(message);

        // Assert
        var messages = await this.dbContext.OutboxMessages.ToListAsync();
        messages.Should().ContainSingle(m => m.EventType == "TestEvent");
    }

    [Fact]
    public async Task GetUnprocessedMessagesAsync_ShouldReturnUnprocessedMessages()
    {
        // Arrange
        var message = new OutboxMessage("TestEvent", "{\"data\":\"test\"}");
        await this.outboxRepository.AddOutboxMessageAsync(message);

        // Act
        var unprocessedMessages = await this.outboxRepository.GetUnprocessedMessagesAsync();

        // Assert
        unprocessedMessages.Should().ContainSingle(m => m.EventType == "TestEvent");
    }

    [Fact]
    public async Task MarkMessagesAsProcessedAsync_ShouldUpdateMessageStatus()
    {
        // Arrange
        var message = new OutboxMessage("TestEvent", "{\"data\":\"test\"}");
        await this.outboxRepository.AddOutboxMessageAsync(message);

        // Act
        message.Processed = true;
        await this.outboxRepository.MarkMessagesAsProcessedAsync(new List<OutboxMessage> { message });

        // Assert
        var processedMessage = await this.dbContext.OutboxMessages.FirstAsync();
        processedMessage.Processed.Should().BeTrue();
    }
}