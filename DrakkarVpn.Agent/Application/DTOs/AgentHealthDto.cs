namespace DrakkarVpn.Agent.Application.DTOs;

public sealed record AgentHealthDto(
    bool Reachable,
    int PeersActive
);