using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;
using DrakkarVpn.Users.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.API.Mappings;

public static class ConnectDeviceApiMapping
{
    public static ConnectDeviceResponse ToApiResponse(this ConnectDeviceResultDto dto) =>
        new(AccessToken: dto.AccessToken, DeviceId: dto.DeviceId);
}
