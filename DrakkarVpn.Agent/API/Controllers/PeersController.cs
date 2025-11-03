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
    public async Task<ActionResult<RegisterPeerResponseDto>> RegisterPeer(CancellationToken ct)
    {
        var result = await _mediator.Send(new RegisterPeerCommand(), ct);
        return Ok(result);
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