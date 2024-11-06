namespace Trading.Application.Commands.ExecuteTrade;

using MediatR;
using Trading.Domain.Entities;
using Trading.Domain.Interfaces;
using Trading.Domain.SeedWork;

public class ExecuteTradeCommandHandler : IRequestHandler<ExecuteTradeCommand, Guid>
{
    private readonly ITradeRepository tradeRepository;
    private readonly IKafkaProducer kafkaProducer;
    private readonly IUnitOfWork unitOfWork;

    public ExecuteTradeCommandHandler(ITradeRepository tradeRepository, IUnitOfWork unitOfWork, IKafkaProducer kafkaProducer)
    {
        this.tradeRepository = tradeRepository;
        this.unitOfWork = unitOfWork;
        this.kafkaProducer = kafkaProducer;
    }

    public async Task<Guid> Handle(ExecuteTradeCommand request, CancellationToken cancellationToken)
    {
        var trade = new Trade(request.UserId, request.Asset, request.Quantity, request.Price);
        await this.tradeRepository.AddAsync(trade);
        await this.unitOfWork.CommitAsync();
        await this.kafkaProducer.ProduceAsync("trade-executed", trade);
        return ((EntityBase)trade).Id;
    }
}