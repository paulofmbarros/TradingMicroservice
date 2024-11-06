namespace Trading.Application.Queries.GetTradeById;

using MediatR;
using Trading.Application.DTO;
using Trading.Domain.Interfaces;
using Trading.Domain.SeedWork;

public class GetTradeByIdQueryHandler : IRequestHandler<GetTradeByIdQuery, TradeDto>
{
    private readonly ITradeRepository tradeRepository;

    public GetTradeByIdQueryHandler(ITradeRepository tradeRepository)
    {
        this.tradeRepository = tradeRepository;
    }

    public async Task<TradeDto> Handle(GetTradeByIdQuery request, CancellationToken cancellationToken)
    {
        var trade = await this.tradeRepository.GetByIdAsync(request.Id);

        if (trade is null)
        {
            return null;
        }

        return new TradeDto
        {
            UserId = trade.UserId,
            Asset = trade.Asset,
            Quantity = trade.Quantity,
            Price = trade.Price,
            Timestamp = trade.Timestamp
        };
    }
}