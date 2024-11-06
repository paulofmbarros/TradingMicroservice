namespace Trading.Api.BackgroundServices;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Trading.Domain.Entities;
using Trading.Domain.Interfaces;
using Trading.Infrastructure;

public class OutboxBackgroundService : BackgroundService
{
    private readonly IServiceProvider serviceProvider;

    public OutboxBackgroundService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = this.serviceProvider.CreateScope())
            {
                var processor = scope.ServiceProvider.GetRequiredService<OutboxProcessor>();
                await processor.ProcessOutboxMessagesAsync();
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Run every 10 seconds
        }
    }
}