using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.Metrics.Queries.AgentMetrics;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Agent.Controllers;

[ApiController]
[Route("metrics")]
public class MetricsController : ControllerBase
{
    private readonly IMediator _mediator;
    public MetricsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetAgentMetricsQuery(), ct);
        return Ok(dto);
    }
    
}