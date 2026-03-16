namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Servers;

/// <summary>
/// API response contract for a single peer in the server peers list.
/// Based on ServerPeerDto from peers application layer.
/// </summary>
public sealed record ServerPeerApiResponse(
    Guid      PeerId,
    Guid      UserId,
    string    Status,
    bool      IsOnline,
    DateTime? LastDataAtUtc,
    long?     TrafficLast1hBytes,
    long?     TrafficLast24hBytes,
    double?   SpeedMbps,
    double?   VpnLatencyMs,
    DateTime  CreatedAtUtc);

