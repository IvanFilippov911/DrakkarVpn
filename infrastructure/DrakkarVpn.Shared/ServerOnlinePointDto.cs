namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

public sealed record ServerOnlinePointDto(
    DateTime PeriodStartUtc,
    int OnlinePeers
);