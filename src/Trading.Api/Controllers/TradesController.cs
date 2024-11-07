namespace Trading.Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Trading.Application.Commands.ExecuteTrade;
using Trading.Application.Queries.GetTradeById;
using Trading.Application.Queries.GetTradesQuery;

[ApiController]
[Route("api/[controller]")]
public class TradesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    public async Task<IActionResult> ExecuteTrade([FromBody] ExecuteTradeCommand command)
    {
        var tradeId = await mediator.Send(command);
        return CreatedAtAction(nameof(GetTradeById), new { id = tradeId }, null);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTradeById(Guid id)
    {
        var query = new GetTradeByIdQuery { Id = id };
        var trade = await mediator.Send(query);

        return Ok(trade);
    }

    [HttpGet]
    public async Task<IActionResult> GetTrades()
    {
        var result = await mediator.Send(new GetTradesQuery());
        return Ok(result);
    }

}