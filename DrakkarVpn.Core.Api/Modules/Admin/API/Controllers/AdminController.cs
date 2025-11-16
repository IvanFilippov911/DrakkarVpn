using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;
using DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminOverview;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminServers;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminServerUsers;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeerHistory;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetServerPeersOnlineHistory;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.DeleteServer;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.RegisterServer;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerById;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerHistoryLast;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.BanUser;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.MarkUserInternal;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Commands.UnbanUser;
using DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.UserDevicesOnServer;
using DrakkarVpn.Shared;
using DrakkarVpn.Shared.Subscriptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Admin.API;

[ApiController]
[Route("api/admin")]
public sealed class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("overview")]
    public async Task<ActionResult<AdminOverviewDto>> GetOverview(CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetAdminOverviewQuery(), ct);
        return Ok(dto);
    }

    
    [HttpGet("servers")]
    public async Task<ActionResult<PagedResponseDto<AdminServerCardDto>>> GetServers(
        [FromQuery] GetAdminServersQuery query,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    
    [HttpGet("servers/{serverId:guid}")]
    public async Task<ActionResult<GetServersDetailDto>> GetServer(Guid serverId, CancellationToken ct)
    {
        var dto = await _mediator.Send(new GetServerByIdRequest(serverId), ct);
        if (dto is null) return NotFound();
        return Ok(dto);
    }
    
    
    [HttpGet("servers/{serverId:guid}/history")]
    public async Task<ActionResult<IReadOnlyList<ServerMetricsHistoryDto>>> GetServerHistory(
        Guid serverId,
        [FromQuery] int minutes = 24 * 60,
        CancellationToken ct = default)
    {
        var dto = await _mediator.Send(new GetServerHistoryLastRequest(serverId, minutes), ct);
        return Ok(dto);
    }
    
    [HttpGet("servers/{serverId:guid}/users")]
    public async Task<ActionResult<PagedResponseDto<AdminUserCardDto>>> GetServerUsers(
        Guid serverId,
        [FromQuery] GetAdminServerUsersApiQuery api,
        CancellationToken ct = default)
    {
        var query = new GetAdminServerUsersQuery(
            ServerId:           serverId,
            Page:               api.Page,
            PageSize:           api.PageSize,
            Search:             api.Search,
            Status:             api.Status,
            SubscriptionStatus: api.SubscriptionStatus,
            SortBy:             api.SortBy
        );

        var result = await _mediator.Send(query, ct);
        return Ok(result);
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
    
    [HttpGet("servers/{serverId:guid}/peers-online-history")]
    public async Task<ActionResult<IReadOnlyList<ServerOnlinePointDto>>> GetPeersOnlineHistory(
        Guid serverId,
        [FromQuery] int minutes = 24 * 60,
        CancellationToken ct = default)
    {
        var dto = await _mediator.Send(
            new GetServerPeersOnlineHistoryQuery(serverId, minutes),
            ct);

        return Ok(dto);
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
    
    [HttpPost("servers")]
    public async Task<ActionResult<Guid>> RegisterServer(
        [FromBody] RegisterServerRequest cmd,
        CancellationToken ct = default)
    {
        var id = await _mediator.Send(cmd, ct);
        return Created($"/api/admin/servers/{id}", new { id });
    }
    
    [HttpDelete("servers/{serverId:guid}")]
    public async Task<IActionResult> DeleteServer(Guid serverId, CancellationToken ct = default)
    {
        var ok = await _mediator.Send(new DeleteServerRequest(serverId), ct);
        return ok ? NoContent() : Conflict();
    }
    
    [HttpPost("users/{userId:guid}/ban")]
    public async Task<IActionResult> BanUser(
        Guid userId,
        [FromBody] BanUserRequest body,
        CancellationToken ct = default)
    {
        var ok = await _mediator.Send(
            new BanUserCommand(userId, body.Reason),
            ct);

        return ok ? NoContent() : NotFound();
    }

    [HttpPost("users/{userId:guid}/unban")]
    public async Task<IActionResult> UnbanUser(
        Guid userId,
        CancellationToken ct = default)
    {
        var ok = await _mediator.Send(new UnbanUserCommand(userId), ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("users/{userId:guid}/internal")]
    public async Task<IActionResult> MarkUserInternal(
        Guid userId,
        [FromBody] MarkUserInternalRequest body,
        CancellationToken ct = default)
    {
        var ok = await _mediator.Send(
            new MarkUserInternalCommand(userId, body.IsInternal),
            ct);

        return ok ? NoContent() : NotFound();
    }
    
    [HttpPost("users/{userId:guid}/subscriptions/grant")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SubscriptionDto>> GrantSubscription(
        [FromRoute] Guid userId,
        [FromBody] AdminGrantSubscriptionApiRequest body,
        CancellationToken ct = default)
    {
        var cmd = body.ToCommand(userId);
        var sub = await _mediator.Send(cmd, ct);
        return Ok(sub);
    }
}