namespace Trading.Infrastructure.Producer;

using System.Text.Json;
using Confluent.Kafka;
using Trading.Domain.Entities;
using Trading.Domain.Interfaces;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<Null, string> producer;

    public KafkaProducer(string bootstrapServers)
    {
        var config = new ProducerConfig { BootstrapServers = bootstrapServers };
        this.producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, Trade? trade)
    {
        var message = JsonSerializer.Serialize(trade);
        await this.producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
    }
}