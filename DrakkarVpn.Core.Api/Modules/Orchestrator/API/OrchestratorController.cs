using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.PurchaseSubscription;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetVpnConfig;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeerByUuid;
using DrakkarVpn.Core.Api.Modules.Subscriptions.Domain.ValueObjects;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Queries.GetActiveTariffs;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.Register;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetListServerUsers;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.UserDevicesOnServer;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Core.Api.Modules.Users.Infrastructure;
using DrakkarVpn.Shared;
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
        CancellationToken ct)
    {
        var req = new GetVpnConfigRequest(
            TelegramId: User.GetTelegramId(),
            DeviceId:   User.GetDeviceId(),
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
    
    [HttpGet("{serverId:guid}/users")]
    [ProducesResponseType(typeof(PagedResponseDto<UserCardDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<UserCardDto>>> GetUsersOnServer(
        [FromRoute] Guid serverId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? q = null,
        [FromQuery] UserStatus? status = null,
        [FromQuery] SubscriptionStatus? subStatus = null,
        [FromQuery] UsersSortBy sort = UsersSortBy.EndAtDesc,
        CancellationToken ct = default)
    {
        var request = new UsersOnServerRequest(
            ServerId: serverId,
            Page: page,
            PageSize: pageSize,
            Search: q,
            Status: status,
            SubscriptionStatus: subStatus,
            SortBy: sort
        );

        var result = await _mediator.Send(request, ct);
        return Ok(result);
    }
    
    
    [HttpGet("{serverId:guid}/{userId:guid}/devices")]
    [ProducesResponseType(typeof(IReadOnlyList<DeviceListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DeviceListItemDto>>> GetUserDevicesOnServer(
        [FromRoute] Guid serverId,
        [FromRoute] Guid userId,
        CancellationToken ct = default)
    {
        var query = new UserDevicesOnServerQuery(ServerId: serverId, UserId: userId);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

}