using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.AllocatePeer;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.RenewDuePeers;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetUserPeers;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetRegions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.Register;
using DrakkarVpn.Shared.Users;
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
    
    [HttpGet("peers")]
    public async Task<ActionResult<IReadOnlyList<GetTgPeersDto>>> GetUserPeersByTelegram(
        [FromQuery] long telegramId,
        CancellationToken ct)
    {
        var peers = await _mediator.Send(new GetUserPeersRequest(telegramId), ct);
        return Ok(peers);
    }

    
    [HttpPost("register-or-get")]
    public async Task<ActionResult<RegisterUserResponse>> RegisterOrGet([FromBody] RegisterUserRequest req, CancellationToken ct)
    {
        var isNew= await _mediator.Send(new RegisterRequest(req), ct);
        return Ok(isNew);
    }
    
    [HttpGet("regions")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetRegions(CancellationToken ct)
    {
        var regions = await _mediator.Send(new GetRegionsRequest(), ct);
        return Ok(regions);
    }
    

}