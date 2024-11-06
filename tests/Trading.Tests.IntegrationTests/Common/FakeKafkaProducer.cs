namespace Trading.Tests.IntegrationTests.Common;

using Trading.Domain.Entities;
using Trading.Domain.Interfaces;

public class FakeKafkaProducer : IKafkaProducer
{
    public Task ProduceAsync(string topic, Trade? trade) =>
        Task.CompletedTask;
}