using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF.ReadModels;

namespace DrakkarVpn.Servers.Application.Mappers;

public static class ServerDtoMappers
{
    public static GetServerDto ToGetServerDto(this AdminServerIndexRowDto r)
        => new(
            Id: r.Id,
            Name: r.Name,
            Region: r.Region,
            Status: r.Status,
            Reachable: r.Reachable,
            PeersActive: r.PeersActive,
            MaxPeers: r.MaxPeers,
            VpnSpeedMbps: r.VpnSpeedMbps,
            InfraLatencyMs: r.InfraLatencyMs,
            OnlinePeers:  r.OnlinePeers,
            TrafficLast1hBytes: r.TrafficLast1hBytes,
            TrafficLast24hBytes: r.TrafficLast24hBytes
        );
}