using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.API.Mappings;

public static class StatusVpnConfigApiMapping
{
    public static StatusVpnConfigResponse ToApiResponse(this StatusVpnConfigDto dto) =>
        new(
            StatusDto: dto.Status,
            ConfigRaw: dto.ConfigRaw,
            HappLink: dto.HappLink,
            JobId: dto.JobId,
            PollUrl: dto.PollUrl);
}
