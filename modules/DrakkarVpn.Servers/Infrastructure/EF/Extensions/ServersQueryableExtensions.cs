using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF.ReadModels;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Entities;
using DrakkarVpn.Servers.Domain.Aggregates;

namespace DrakkarVpn.Servers.Infrastructure.EF.Extensions;

public static class ServersQueryableExtensions
{
    public static IQueryable<Server> FilterByRegion(this IQueryable<Server> q, string? region)
    {
        if (string.IsNullOrWhiteSpace(region)) return q;
        return q.Where(s => s.Region.Code == region.Trim());
    }

    public static IQueryable<Server> FilterByStatus(
        this IQueryable<Server> q,
        ServerStatus? status)
    {
        if (!status.HasValue)
            return q;

        return q.Where(s => s.Status == status.Value);
    }

    public static IQueryable<ServerWithRealtimeStatsRow> WithRealtimeStats(
        this IQueryable<Server> servers,
        IQueryable<ServerRealtimeStats> stats)
    {
        return
            from s in servers
            join st in stats on s.Id equals st.ServerId into gj
            from st in gj.DefaultIfEmpty()
            select new ServerWithRealtimeStatsRow
            {
                Server = s,
                Stats = st
            };
    }

    public static IQueryable<AdminServerIndexRowDto> ProjectToAdminIndex(
        this IQueryable<ServerWithRealtimeStatsRow> q)
    {
        return q.Select(x => new AdminServerIndexRowDto(
            x.Server.Id,
            x.Server.Name,
            x.Server.Region.Code,
            x.Server.Status.ToString(),
            x.Server.Health.Reachable,
            x.Server.Health.PeersActive,
            x.Server.MaxPeers,
            x.Server.Metrics.VpnSpeedMbps,
            x.Server.Metrics.InfraLatencyMs,
            x.Stats != null ? x.Stats.OnlinePeers : 0,
            x.Stats != null ? x.Stats.TrafficLast1hBytes : 0L,
            x.Stats != null ? x.Stats.TrafficLast24hBytes : 0L
        ));
    }
}