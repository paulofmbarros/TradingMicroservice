namespace Trading.Application.Commands.ExecuteTrade;

using MediatR;

public class ExecuteTradeCommand : IRequest<Guid>
{
    public string UserId { get; set; }
    public string Asset { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}