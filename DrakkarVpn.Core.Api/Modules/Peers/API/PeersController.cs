using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.CreatePeer;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.DeletePeer;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Commands.RevokePeer;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetActivePeersCount;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeerById;
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
    
    [HttpDelete("{serverId:guid}/{peerId:guid}")]
    public async Task<IActionResult> RevokePeer(Guid serverId, Guid peerId, CancellationToken ct)
    {
        var req = new RevokePeerRequest(peerId, serverId);
        var result = await _mediator.Send(req, ct);
        return result ? Ok() : NotFound();
    }
    
    [HttpDelete("{id:guid}/hard")]
    public async Task<IActionResult> HardDelete(Guid id, CancellationToken ct)
    {
        var ok = await _mediator.Send(new DeletePeerRequest(id), ct);
        return ok ? NoContent() : NotFound();
    }
    
    
    [HttpGet("{peerId:guid}")]
    public async Task<IActionResult> GetPeerById(Guid peerId, CancellationToken ct)
    {
        var req = new GetPeerByIdRequest(peerId);
        var result = await _mediator.Send(req, ct);
        return result is null ? NotFound() : Ok(result);
    }
    
    
    [HttpGet("by-server/{serverId:guid}/active-count")]
    public async Task<IActionResult> GetActivePeersCount(Guid serverId, CancellationToken ct)
    {
        var req = new GetActivePeersCountRequest(serverId);
        var result = await _mediator.Send(req, ct);
        return Ok(result);
    }
}