using DrakkarVpn.Core.Api.Modules.Admin.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using MediatR;


namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminOverview;

public sealed class GetAdminOverviewHandler
    : IRequestHandler<GetAdminOverviewQuery, AdminOverviewDto>
{
    private readonly IAdminOverviewReadStore _store;

    public GetAdminOverviewHandler(IAdminOverviewReadStore store)
    {
        _store = store;
    }

    public async Task<AdminOverviewDto> Handle(GetAdminOverviewQuery q, CancellationToken ct)
    {
        var servers = await _store.GetServersOverviewAsync(ct);
        var users   = await _store.GetUsersOverviewAsync(ct);
        var traffic = await _store.GetTrafficOverviewAsync(ct);

        return new AdminOverviewDto(
            TotalServers:        servers.TotalServers,
            ServersOnline:       servers.ServersOnline,
            TotalActivePeers:    servers.TotalActivePeers,
            PeersOnline:         servers.PeersOnline,
            AvgVpnSpeedMbps:     traffic.AvgSpeedMbps,
            AvgInfraLatencyMs:   traffic.AvgInfraLatencyMs,
            TrafficTodayBytes:   traffic.TrafficTodayBytes,
            TrafficLast24hBytes: traffic.TrafficLast24hBytes,
            TotalUsers:          users.TotalUsers,
            ActiveSubscriptions: users.ActiveSubscriptions,
            ExpiringSoonDays3:   users.ExpiringSoonDays3,
            OnlineUsersNow:      users.OnlineUsersNow
        );
    }
}