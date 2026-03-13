using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Peers.RevokePeer;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Peers.GetPeerDetails;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Peers.GetPeerHistory;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Admin.API;

[ApiController]
[Route("api/admin")]
public sealed class AdminPeersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminPeersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("peers/{peerId:guid}")]
    [ProducesResponseType(typeof(PeerDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PeerDetailsDto>> GetPeerDetails(
        Guid peerId,
        CancellationToken ct = default)
    {
        var dto = await _mediator.Send(new GetPeerDetailsQuery(peerId), ct);
        if (dto is null)
            return NotFound();

        return Ok(dto);
    }

    [HttpGet("peers/{peerId:guid}/history")]
    public async Task<ActionResult<IReadOnlyList<PeerMetricsHistoryDto>>> GetPeerHistory(
        Guid peerId,
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        CancellationToken ct = default)
    {
        var dto = await _mediator.Send(
            new GetPeerHistoryRequest(peerId, fromUtc, toUtc),
            ct);

        return Ok(dto);
    }

    [HttpPost("peers/{peerId:guid}/revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RevokePeer(
        [FromRoute] Guid peerId,
        [FromQuery] Guid serverId,
        CancellationToken ct = default)
    {
        var ok = await _mediator.Send(new RevokePeerRequest(peerId, serverId), ct);
        if (!ok)
            return NotFound();

        return NoContent();
    }
}