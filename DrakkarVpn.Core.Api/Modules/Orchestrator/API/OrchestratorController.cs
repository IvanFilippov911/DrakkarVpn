using System.Security.Claims;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.PurchaseSubscription;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetUserPeers;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetVpnConfig;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeerByUuid;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Queries.GetActiveTariffs;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.Register;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure;
using DrakkarVpn.Shared.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.API;

[ApiController]
[Route("api/orchestrator")]
public sealed class OrchestratorController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrchestratorController(IMediator mediator) => _mediator = mediator;
    
    [HttpPost("purchase")]
    public async Task<ActionResult<Guid>> Purchase([FromBody] PurchaseSubscriptionRequest body, CancellationToken ct)
    {
        var subId = await _mediator.Send(body, ct);
        return Ok(subId);
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
    
    [HttpGet("tariffs")]
    public async Task<ActionResult<Guid>> GetActiveTariffs(CancellationToken ct)
    {
        var tattifs = await _mediator.Send(new GetActiveTariffsRequest(), ct);
        return Ok(tattifs);
    }
    
    [Authorize]
    [HttpGet("config")]
    public async Task<IActionResult> GetConfig(
        [FromQuery] string? region,
        [FromQuery] string? deviceName,
        [FromQuery] string? platform,
        CancellationToken ct)
    {
        var req = new GetVpnConfigRequest(
            TelegramId: User.GetTelegramId(),
            DeviceId:   User.GetDeviceId(),
            DeviceName: deviceName ?? User.GetDeviceName(),
            Platform:   platform   ?? User.GetPlatform(),
            Region:     region
        );

        var res = await _mediator.Send(req, ct);
        return res is null ? NotFound("Активная подписка или пир не найдены") : Ok(res);
    }
    
    [HttpGet("s/{peerUuid:guid}")]
    public async Task<IActionResult> GetPeerConfig(Guid peerUuid, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPeerByUuidRequest(peerUuid), ct);
        if (result is null)
            return NotFound("Пир не найден");
        
        return Content(result.ConfigRaw, "text/plain");
    }

}