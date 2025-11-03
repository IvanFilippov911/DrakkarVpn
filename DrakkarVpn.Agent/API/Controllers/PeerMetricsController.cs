using DrakkarVpn.Agent.Application.Peers.Queries.PeerMetrics;
using DrakkarVpn.Shared.Peers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Agent.API.Controllers;

[ApiController]
[Route("metrics/peers")]
public class PeerMetricsController : ControllerBase
{
    private readonly IMediator _mediator;
    public PeerMetricsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PeerMetricsDto>>> Get(CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetPeerMetricsQuery(), ct);
        return Ok(dto);
    }
}