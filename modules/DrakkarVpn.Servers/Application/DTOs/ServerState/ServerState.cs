using DrakkarVpn.Core.Api.Modules.Servers.Domain;

namespace DrakkarVpn.Servers.Application.DTOs.ServerState;

public sealed record ServerState(
    Guid ServerId,
    ServerStatus Status,
    int ConsecutiveFailures,
    int? MaxPeers
);