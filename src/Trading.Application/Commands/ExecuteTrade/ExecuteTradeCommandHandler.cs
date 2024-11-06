namespace Trading.Application.Commands.ExecuteTrade;

using System.Text.Json;
using MediatR;
using Trading.Domain.Entities;
using Trading.Domain.Interfaces;
using Trading.Domain.SeedWork;

public class ExecuteTradeCommandHandler
{
    private readonly ITradeRepository tradeRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IOutboxRepository outboxRepository;

    public ExecuteTradeCommandHandler(ITradeRepository tradeRepository, IUnitOfWork unitOfWork, IOutboxRepository outboxRepository)
    {
        this.tradeRepository = tradeRepository;
        this.unitOfWork = unitOfWork;
        this.outboxRepository = outboxRepository;
    }

    public async Task<Guid> Handle(ExecuteTradeCommand request, CancellationToken cancellationToken)
    {
        var trade = new Trade(request.UserId, request.Asset, request.Quantity, request.Price);
        await this.tradeRepository.AddAsync(trade);

        // Create an outbox message for the trade event
        var outboxMessage = new OutboxMessage(
            eventType: "TradeExecuted",
            payload: JsonSerializer.Serialize(trade)
        );

        await this.outboxRepository.AddOutboxMessageAsync(outboxMessage);
        await this.unitOfWork.CommitAsync();

        return trade.Id;
    }
}