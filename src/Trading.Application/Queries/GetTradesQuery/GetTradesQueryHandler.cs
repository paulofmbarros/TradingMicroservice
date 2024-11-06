namespace Trading.Application.Queries.GetTradesQuery;

using MediatR;
using Trading.Application.DTO;
using Trading.Domain.Entities;
using Trading.Domain.Interfaces;
using Trading.Domain.SeedWork;

public class GetTradesQueryHandler(ITradeRepository tradeRepository)
    : IRequestHandler<GetTradesQuery, IEnumerable<TradeDto>>
{
    public async Task<IEnumerable<TradeDto>> Handle(GetTradesQuery request, CancellationToken cancellationToken)
    {
        var trades = await tradeRepository.GetAllAsync();

        return trades.Select(trade => new TradeDto
        {
            UserId = trade.UserId,
            Asset = trade.Asset,
            Quantity = trade.Quantity,
            Price = trade.Price,
            Timestamp = trade.Timestamp
        });
    }
}