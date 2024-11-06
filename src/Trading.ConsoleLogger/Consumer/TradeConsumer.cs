namespace Trading.ConsoleLogger.Consumer;
using Confluent.Kafka;

public class TradeConsumer
{
    private readonly IConsumer<Null, string> consumer;
    private readonly string topic = "trade-executed";

    public TradeConsumer(string bootstrapServers, string topic)
    {
        this.topic = topic;
        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = "trade-consumers",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        this.consumer = new ConsumerBuilder<Null, string>(config).Build();
    }

    public void StartConsuming()
    {
        this.consumer.Subscribe(this.topic);
        Console.WriteLine($"Subscribed to topic: {this.topic}");

        try
        {
            while (true)
            {
                var consumeResult = this.consumer.Consume();
                Console.WriteLine($"Trade Executed: {consumeResult.Message.Value}");
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Consumer loop was canceled.");
        }
        finally
        {
            this.consumer.Close();
        }
    }
}