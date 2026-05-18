using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Servers.Application.Abstractions.Services.Queries;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminServers;

public sealed class GetAdminServersHandler 
    : IRequestHandler<GetAdminServersQuery, PagedResponseDto<AdminServerCardDto>>
{
    private readonly IServersQueryService _servers;

    public GetAdminServersHandler(IServersQueryService servers)
    {
        _servers = servers;
    }

    public async Task<PagedResponseDto<AdminServerCardDto>> Handle(
        GetAdminServersQuery q,
        CancellationToken ct)
    {
        var page     = q.Page     <= 0 ? 1  : q.Page;
        var pageSize = q.PageSize <= 0 ? 50 : q.PageSize;
        pageSize     = Math.Clamp(pageSize, 1, 200);

        var pages = await _servers.GetPagedAsync(
            region: q.Region,
            status: q.Status,
            page: page,
            pageSize: pageSize,
            ct: ct);

        if (pages.Total == 0 || pages.PageSize == 0)
            return PagedResponseDto<AdminServerCardDto>.Empty(page, pageSize);

        var items = pages.Items.Select(item => new AdminServerCardDto(
            Id:                  item.Id,
            Name:                item.Name,
            Region:              item.Region,
            Status:              item.Status,
            Reachable:           item.Reachable,
            OnlinePeers:         item.OnlinePeers,
            PeersActive:         item.PeersActive,
            MaxPeers:            item.MaxPeers,
            VpnSpeedMbps:        item.VpnSpeedMbps,
            InfraLatencyMs:      item.InfraLatencyMs,
            TrafficLast1hBytes:  item.TrafficLast1hBytes,
            TrafficLast24hBytes: item.TrafficLast24hBytes
        )).ToList();

        return PagedResponseDto<AdminServerCardDto>.From(
            items,
            page,
            pageSize,
            pages.Total);
    }
}