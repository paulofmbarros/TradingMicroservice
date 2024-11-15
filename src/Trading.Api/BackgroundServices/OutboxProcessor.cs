﻿namespace Trading.Api.BackgroundServices;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Trading.Domain.Entities;
using Trading.Domain.Interfaces;
using Trading.Infrastructure;

public class OutboxProcessor
{
    private readonly IOutboxRepository outboxRepository;
    private readonly IKafkaProducer kafkaProducer;
    private readonly string kafkaBroker;


    public OutboxProcessor(IOutboxRepository outboxRepository, IKafkaProducer kafkaProducer, string kafkaBroker)
    {
        this.outboxRepository = outboxRepository;
        this.kafkaProducer = kafkaProducer;
        this.kafkaBroker = kafkaBroker;
    }

    public async Task ProcessOutboxMessagesAsync()
    {
        var messages = await this.outboxRepository.GetUnprocessedMessagesAsync();

        foreach (var message in messages)
        {
            try
            {
                if (message.EventType == "TradeExecuted")
                {
                    var trade = JsonSerializer.Deserialize<Trade>(message.Payload);
                    await this.kafkaProducer.ProduceAsync("trade-executed", trade);

                    // Mark the message as processed after a successful send
                    message.Processed = true;
                }
            }
            catch (Exception ex)
            {
                // Log error, but do not remove the message from the outbox
                Console.WriteLine($"Error processing outbox message {message.Id}: {ex.Message}");
            }
        }

        // Save the state of the processed messages
        await this.outboxRepository.MarkMessagesAsProcessedAsync(messages);
    }
}