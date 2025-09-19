using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.CreatePeer;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.RenewPeer;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.RevokePeer;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetActivePeersCount;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeerById;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersByServer;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersByUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Peers.API;

[ApiController]
[Route("api/[controller]")]
public sealed class PeersController : ControllerBase
{
    private readonly IMediator _mediator;

    public PeersController(IMediator mediator) => _mediator = mediator;
    
    [HttpPost]
    public async Task<IActionResult> CreatePeer([FromBody] RegisterPeerRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(request, ct);
        return Ok(result);
    }

    [HttpPost("{peerId:guid}/renew")]
    public async Task<IActionResult> RenewPeer(Guid peerId, [FromBody] DateTime newExpiresAt, CancellationToken ct)
    {
        var req = new RenewPeerRequest(peerId, newExpiresAt);
        var result = await _mediator.Send(req, ct);
        return result ? Ok() : BadRequest();
    }

    [HttpDelete("{serverId:guid}/{peerId:guid}")]
    public async Task<IActionResult> RevokePeer(Guid serverId, Guid peerId, CancellationToken ct)
    {
        var req = new RevokePeerRequest(peerId, serverId);
        var result = await _mediator.Send(req, ct);
        return result ? Ok() : NotFound();
    }


    
    
    [HttpGet("{peerId:guid}")]
    public async Task<IActionResult> GetPeerById(Guid peerId, CancellationToken ct)
    {
        var req = new GetPeerByIdRequest(peerId);
        var result = await _mediator.Send(req, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("by-user/{userId:guid}")]
    public async Task<IActionResult> GetPeersByUser(Guid userId, CancellationToken ct)
    {
        var req = new GetPeersByUserRequest(userId);
        var result = await _mediator.Send(req, ct);
        return Ok(result);
    }

    [HttpGet("by-server/{serverId:guid}")]
    public async Task<IActionResult> GetPeersByServer(Guid serverId, CancellationToken ct)
    {
        var req = new GetPeersByServerRequest(serverId);
        var result = await _mediator.Send(req, ct);
        return Ok(result);
    }

    [HttpGet("by-server/{serverId:guid}/active-count")]
    public async Task<IActionResult> GetActivePeersCount(Guid serverId, CancellationToken ct)
    {
        var req = new GetActivePeersCountRequest(serverId);
        var result = await _mediator.Send(req, ct);
        return Ok(result);
    }
}