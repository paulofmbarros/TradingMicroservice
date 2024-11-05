namespace Trading.Domain.Interfaces;

using Trading.Domain.Entities;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, Trade trade);
}