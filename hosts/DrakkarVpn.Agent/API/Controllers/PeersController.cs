using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.DTOs;
using DrakkarVpn.Agent.Application.Peers.Commads.RegisterPeer;
using DrakkarVpn.Agent.Application.Peers.Commads.RevokePeer;
using DrakkarVpn.Agent.Application.Peers.Queries.Peers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Agent.API.Controllers;

[ApiController]
[Route("peers")]
public sealed class PeersController : ControllerBase
{
    private readonly IMediator _mediator;

    public PeersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> RegisterPeer(
        [FromBody] RegisterPeerRequestDto req,
        CancellationToken ct)
    {
        if (req.PeerUuid == Guid.Empty)
            return BadRequest(new { code = "bad_request", message = "peerUuid required" });

        await _mediator.Send(
            new RegisterPeerCommand(req.PeerUuid),
            ct);
        
        return NoContent();
    }

    
    [HttpDelete("{peerUuid:guid}")]
    public async Task<IActionResult> RevokePeer(Guid peerUuid, CancellationToken ct)
    {
        var success = await _mediator.Send(new RevokePeerCommand(peerUuid), ct);
        return success ? NoContent() : NotFound();
    }
    
    
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PeersResultDto>>> ListPeers(CancellationToken ct)
    {
        var peers = await _mediator.Send(new PeersQuery(), ct);
        return Ok(peers);
    }
}