using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.API.Mappings;

public static class PeerProvisionJobApiMapping
{
    public static PeerProvisionJobResponse ToApiResponse(this PeerProvisionJobDto dto) =>
        new(
            JobId: dto.JobId,
            Status: dto.Status,
            Attempt: dto.Attempt,
            MaxAttempts: dto.MaxAttempts,
            NextAttemptAtUtc: dto.NextAttemptAtUtc,
            PeerId: dto.PeerId,
            AgentPeerUuid: dto.AgentPeerUuid,
            ConfigRaw: dto.ConfigRaw,
            HappLink: dto.HappLink,
            ErrorCode: dto.ErrorCode,
            ErrorMessage: dto.ErrorMessage);
}
