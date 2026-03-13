using DrakkarVpn.Shared.Peers;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

public sealed record AllocatePeerDto(
    AllocatePeerStatus Status,
    Guid? JobId,
    string? ConfigRaw,
    Guid? AgentPeerUuid,
    string? HappLink
)
{
    public static AllocatePeerDto Ready(Guid agentPeerUuid, string configRaw, string happLink)
        => new(AllocatePeerStatus.Ready, null, configRaw, agentPeerUuid, happLink);

    public static AllocatePeerDto Accepted(Guid jobId)
        => new(AllocatePeerStatus.Accepted, jobId, null, null, null);
}