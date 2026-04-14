using DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers.ServerState;

namespace DrakkarVpn.Servers.Application.DTOs.ServerState;

public sealed record ServerPollResultDto(
    Guid ServerId,
    uint Xmin,
    bool Reachable,
    int PeersActive,
    long RxTotal,
    long TxTotal,
    double InfraLatencyMs,
    double VpnSpeedMbps,
    int HttpLatencyMs,
    bool Success,
    string? ErrorCode
)
{
    public static ServerPollResultDto Failure(
        ServerPollCandidateDto c,
        int httpLatencyMs = 0,
        string? errorCode = null)
        => new(
            ServerId:       c.ServerId,
            Xmin:           c.Xmin,
            Reachable:      false,
            PeersActive:    c.LastKnownPeersActive,
            RxTotal:        c.LastRxTotal,
            TxTotal:        c.LastTxTotal,
            InfraLatencyMs: 0,
            VpnSpeedMbps:   0,
            HttpLatencyMs:  httpLatencyMs,
            Success:        false,
            ErrorCode:      errorCode
        );
}