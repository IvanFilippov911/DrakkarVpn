using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServersTrafficSummary;
using DrakkarVpn.Shared;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminServers;

public sealed class GetAdminServersHandler 
    : IRequestHandler<GetAdminServersQuery, PagedResponseDto<AdminServerCardDto>>
{
    private readonly IMediator _mediator;

    public GetAdminServersHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<PagedResponseDto<AdminServerCardDto>> Handle(
        GetAdminServersQuery q,
        CancellationToken ct)
    {
        var servers = await _mediator.Send(
            new GetServersRequest(q.Region, q.Status),
            ct);

        if (servers.Count == 0)
            return PagedResponseDto<AdminServerCardDto>.Empty(q.Page, q.PageSize);

        var page     = q.Page     <= 0 ? 1  : q.Page;
        var pageSize = q.PageSize <= 0 ? 50 : q.PageSize;
        pageSize     = Math.Clamp(pageSize, 1, 200);

        var total = servers.Count;
        var pageItems = servers
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var nowUtc  = DateTime.UtcNow;
        var from1h  = nowUtc.AddHours(-1);
        var from24h = nowUtc.AddHours(-24);
        var ids     = pageItems.Select(s => s.Id).ToArray();
        
        var traffic1hDict = await _mediator.Send(
            new GetServersTrafficSummaryRequest(ids, from1h),
            ct);

        var traffic24hDict = await _mediator.Send(
            new GetServersTrafficSummaryRequest(ids, from24h),
            ct);

        var items = pageItems.Select(s =>
        {
            traffic1hDict.TryGetValue(s.Id, out var t1);
            traffic24hDict.TryGetValue(s.Id, out var t24);

            return new AdminServerCardDto(
                Id:                 s.Id,
                Name:               s.Name,
                Region:             s.Region,
                Status:             s.Status,
                Reachable:          s.Reachable,
                PeersActive:        s.PeersActive,
                MaxPeers:           s.MaxPeers,
                VpnSpeedMbps:       s.VpnSpeedMbps,
                InfraLatencyMs:     s.InfraLatencyMs,
                TrafficLast1hBytes:  t1,
                TrafficLast24hBytes: t24
            );
        }).ToList();

        return PagedResponseDto<AdminServerCardDto>.From(items, page, pageSize, total);
    }
}