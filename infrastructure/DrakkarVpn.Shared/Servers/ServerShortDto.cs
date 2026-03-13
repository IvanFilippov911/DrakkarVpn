using DrakkarVpn.Core.Api.Modules.Servers.Domain;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Queries.GetServers;

public sealed record ServerShortDto(
    Guid ServerId,
    string Name,
    string Region,
    ServerStatus Status,
    bool Reachable
);