namespace Trading.Application.Queries.GetTradeById;

using MediatR;
using Trading.Application.DTO;

public class GetTradeByIdQuery : IRequest<TradeDto>
{
    public Guid Id { get; set; }
}