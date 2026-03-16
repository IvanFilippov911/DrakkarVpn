using DrakkarVpn.AdminAuth.Application.Authorization;
using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;
using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Servers;
using DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Servers.DeleteServer;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Servers.RegisterServer;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Servers.RevokeAllServerPeers;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminServers;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Peers.GetServerPeers;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Servers.GetServerById;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Servers.GetServerHistoryLast;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Servers.GetServerPeersOnlineHistory;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain.enums;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Shared;
using DrakkarVpn.Shared.Servers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DrakkarVpn.Core.Api.Modules.Admin.API;

[ApiController]
[Authorize]
[Route("api/admin")]
public sealed class AdminServersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminServersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Policy = AdminPolicies.ServersRead)]
    [HttpGet("servers")]
    [ProducesResponseType(typeof(PagedResponseDto<AdminServerCardApiResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<AdminServerCardApiResponse>>> GetServers(
        [FromQuery] GetAdminServersQuery query,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(query, ct);
        return Ok(result.ToApiResponse());
    }

    [Authorize(Policy = AdminPolicies.ServersRead)]
    [HttpGet("servers/{serverId:guid}")]
    [ProducesResponseType(typeof(ServerDetailsApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ServerDetailsApiResponse>> GetServer(
        Guid serverId,
        CancellationToken ct = default)
    {
        var dto = await _mediator.Send(new GetServerByIdRequest(serverId), ct);
        if (dto is null) return NotFound();
        return Ok(dto.ToApiResponse());
    }

    [Authorize(Policy = AdminPolicies.ObservabilityRead)]
    [HttpGet("servers/{serverId:guid}/history")]
    [ProducesResponseType(typeof(IReadOnlyList<ServerMetricsHistoryApiResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ServerMetricsHistoryApiResponse>>> GetServerHistory(
        Guid serverId,
        [FromQuery] int minutes = 24 * 60,
        CancellationToken ct = default)
    {
        var dto = await _mediator.Send(
            new GetServerHistoryLastRequest(serverId, minutes),
            ct);

        return Ok(dto.ToApiResponse());
    }
    

    [Authorize(Policy = AdminPolicies.ObservabilityRead)]
    [HttpGet("servers/{serverId:guid}/peers-online-history")]
    [ProducesResponseType(typeof(IReadOnlyList<ServerOnlinePointApiResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ServerOnlinePointApiResponse>>> GetPeersOnlineHistory(
        Guid serverId,
        [FromQuery] int minutes = 24 * 60,
        CancellationToken ct = default)
    {
        var dto = await _mediator.Send(
            new GetServerPeersOnlineHistoryQuery(serverId, minutes),
            ct);

        return Ok(dto.ToApiResponse());
    }

    [Authorize(Policy = AdminPolicies.ServersManage)]
    [HttpPost("servers")]
    [ProducesResponseType(typeof(RegisterServerApiResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<RegisterServerApiResponse>> RegisterServer(
        [FromBody] RegisterServerApiRequest body,
        CancellationToken ct = default)
    {
        var cmd = body.ToCommand();
        var id  = await _mediator.Send(cmd, ct);
        return Created($"/api/admin/servers/{id}", new RegisterServerApiResponse(id));
    }

    [Authorize(Policy = AdminPolicies.ServersManage)]
    [HttpDelete("servers/{serverId:guid}")]
    public async Task<IActionResult> DeleteServer(
        Guid serverId,
        CancellationToken ct = default)
    {
        var ok = await _mediator.Send(new DeleteServerRequest(serverId), ct);
        return ok ? NoContent() : Conflict();
    }

    [Authorize(Policy = AdminPolicies.PeersManage)]
    [HttpPost("servers/{serverId:guid}/peers/revoke")]
    [ProducesResponseType(typeof(AdminRevokeServerPeersApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AdminRevokeServerPeersApiResponse>> RevokeAllServerPeers(
        [FromRoute] Guid serverId,
        CancellationToken ct = default)
    {
        var revoked = await _mediator.Send(
            new RevokeAllServerPeersCommand(serverId),
            ct);

        return Ok(new AdminRevokeServerPeersApiResponse(revoked));
    }
    
    [Authorize(Policy = AdminPolicies.ServersRead)]
    [HttpGet("{serverId:guid}/peers")]
    [ProducesResponseType(typeof(PagedResponseDto<ServerPeerApiResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<ServerPeerApiResponse>>> GetServerPeers(
        Guid serverId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] bool? onlyOnline = null,
        [FromQuery] Guid? peerId = null,
        [FromQuery] ServerPeerSortBy sortBy = ServerPeerSortBy.LastActivity,
        [FromQuery] SortDirection peerSortDirection = SortDirection.Desc,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetServerPeersQuery(
                ServerId:      serverId,
                Page:          page,
                PageSize:      pageSize,
                OnlyOnline:    onlyOnline,
                PeerId:        peerId,
                SortBy:        sortBy,
                PeerSortDirection: peerSortDirection),
            ct);

        return Ok(result.ToApiResponse());
    }
}