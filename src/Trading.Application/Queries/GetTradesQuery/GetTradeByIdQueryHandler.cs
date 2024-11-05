namespace Trading.Application.Queries.GetTradesQuery;

using MediatR;
using Trading.Application.DTO;
using Trading.Domain.Interfaces;

public class GetTradesQueryHandler : IRequestHandler<GetTradesQuery, IEnumerable<TradeDto>>
{
    private readonly ITradeRepository tradeRepository;

    public GetTradesQueryHandler(ITradeRepository tradeRepository)
    {
        this.tradeRepository = tradeRepository;
    }

    public async Task<IEnumerable<TradeDto>> Handle(GetTradesQuery request, CancellationToken cancellationToken)
    {
        var trades = await this.tradeRepository.GetAllAsync();

        return trades.Select(trade => new TradeDto
        {
            Id = trade.Id,
            UserId = trade.UserId,
            Asset = trade.Asset,
            Quantity = trade.Quantity,
            Price = trade.Price,
            Timestamp = trade.Timestamp
        });
    }
}