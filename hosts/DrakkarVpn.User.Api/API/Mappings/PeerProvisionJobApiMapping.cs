using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.API.Mappings;

public static class PeerProvisionJobApiMapping
{
    public static PeerProvisionJobResponse ToApiResponse(this PeerProvisionJobStatusDto dto) =>
        new(
            Status: dto.Status,
            ErrorCode: dto.ErrorCode,
            ErrorMessage: dto.ErrorMessage);
}
