using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Overview;
using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;

public static class AdminOverviewApiMapping
{
    public static AdminOverviewApiResponse ToApiResponse(this AdminOverviewDto dto)
    {
        return new AdminOverviewApiResponse(
            TotalServers:        dto.TotalServers,
            ServersOnline:      dto.ServersOnline,
            TotalActivePeers:   dto.TotalActivePeers,
            PeersOnline:        dto.PeersOnline,
            AvgVpnSpeedMbps:    dto.AvgVpnSpeedMbps,
            AvgInfraLatencyMs:  dto.AvgInfraLatencyMs,
            TrafficTodayBytes:  dto.TrafficTodayBytes,
            TrafficLast24hBytes: dto.TrafficLast24hBytes,
            TotalUsers:         dto.TotalUsers,
            ActiveSubscriptions: dto.ActiveSubscriptions,
            ExpiringSoonDays3:   dto.ExpiringSoonDays3,
            OnlineUsersNow:     dto.OnlineUsersNow);
    }
}
