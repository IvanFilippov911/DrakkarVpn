namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF.ReadModels;

public sealed record ServerRealtimeStatsUpsertRow(
    Guid ServerId,
    int OnlinePeers,
    long TrafficLast1hBytes,
    long TrafficLast24hBytes);