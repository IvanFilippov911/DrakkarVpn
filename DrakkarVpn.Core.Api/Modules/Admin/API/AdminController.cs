using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminOverview;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminServers;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminServerUsers;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeerHistory;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerById;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServerHistoryLast;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Shared;
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
}