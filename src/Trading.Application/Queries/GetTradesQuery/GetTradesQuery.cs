namespace Trading.Application.Queries.GetTradesQuery;

using MediatR;
using Trading.Application.DTO;

public class GetTradesQuery : IRequest<IEnumerable<TradeDto>>
{
}