namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

public sealed record AgentHealthDto(
    bool Reachable,
    int PeersActive
);
