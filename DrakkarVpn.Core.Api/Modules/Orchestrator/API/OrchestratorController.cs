using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.AllocatePeer;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.RenewDuePeers;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetUserPeers;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.API;

[ApiController]
[Route("api/orchestrator")]
public sealed class OrchestratorController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrchestratorController(IMediator mediator) => _mediator = mediator;
    
    [HttpPost("allocate")]
    public async Task<ActionResult<PeerRegisterResponseDto>> Allocate(
        [FromBody] AllocatePeerRequest body,
        CancellationToken ct)
    {
        var peer = await _mediator.Send(body, ct);
        return Ok(peer);
    }
    
    [HttpPost("renew-due")]
    public async Task<IActionResult> RenewDue(CancellationToken ct)
    {
        var count = await _mediator.Send(new RenewDuePeersRequest(), ct);
        return Ok(new { revoked = count });
    }
    
    [HttpGet("users/{userId:guid}/peers")]
    public async Task<ActionResult<IReadOnlyList<GetUserPeerDto>>> GetUserPeers(
        [FromRoute] Guid userId,
        CancellationToken ct)
    {
        var peers = await _mediator.Send(new GetUserPeersRequest(userId), ct);
        return Ok(peers);
    }
    

}