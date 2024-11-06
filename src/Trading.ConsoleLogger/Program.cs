// See https://aka.ms/new-console-template for more information

using System;
using System.Threading;
using System.Threading.Tasks;
using Trading.ConsoleLogger.Consumer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


class Program
{
    static async Task Main(string[] args)
    {
        // Build configuration to read settings from appsettings.json or environment variables
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddEnvironmentVariables(); // Load environment variables in case they override appsettings.json
            })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IConfiguration>(context.Configuration);
            });

        var host = hostBuilder.Build();
        var config = host.Services.GetService<IConfiguration>();

        // Read Kafka settings from configuration (either from appsettings.json or environment variables)
        var bootstrapServers = config["Kafka:BootstrapServers"] ?? Environment.GetEnvironmentVariable("KAFKA_BROKER");
        var topic = config["Kafka:Topic"] ?? "trade-executed";

        if (string.IsNullOrEmpty(bootstrapServers))
        {
            Console.WriteLine("Error: Kafka bootstrap servers not configured.");
            return;
        }

        // Create and start the consumer
        var consumer = new TradeConsumer(bootstrapServers, topic);
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            Console.WriteLine("Shutting down...");
        };

        await Task.Run(() => consumer.StartConsuming());
    }
}