using DrakkarVpn.Core.Api.Modules.Orchestrator.API.Contracts.Response;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.API.Mappings;

public static class HomeContextApiMapping
{
    public static HomeContextResponse ToApiResponse(this HomeContextDto dto) =>
        new(
            State: dto.State,
            PendingProvisionJobId: dto.PendingProvisionJobId,
            PollUrl: dto.PollUrl);
}
