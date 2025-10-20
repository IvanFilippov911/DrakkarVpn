namespace DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;

public sealed record ConnectDeviceResponse(
    string AccessToken,
    string DeviceId
);