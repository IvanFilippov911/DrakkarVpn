using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.API.Mappings;

public static class RegisterUserApiMapping
{
    public static RegisterUserResponse ToApiResponse(this RegisterUserResultDto dto) =>
        new(IsNewUser: dto.IsNewUser);
}
