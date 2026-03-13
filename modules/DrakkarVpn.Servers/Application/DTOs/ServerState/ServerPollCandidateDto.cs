namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers.ServerState;

public sealed record ServerPollCandidateDto(
    Guid ServerId,
    uint Xmin,
    int LastKnownPeersActive,
    long LastRxTotal,
    long LastTxTotal
);