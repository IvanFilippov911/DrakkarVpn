namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Servers;

/// <summary>
/// API response contract for a single point in server peers-online history.
/// Mirrors ServerOnlinePointDto from servers application layer.
/// </summary>
public sealed record ServerOnlinePointApiResponse(
    DateTime PeriodStartUtc,
    int      OnlinePeers);

