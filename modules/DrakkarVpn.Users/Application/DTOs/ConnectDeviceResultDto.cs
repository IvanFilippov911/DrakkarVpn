namespace DrakkarVpn.Users.Application.DTOs;

public sealed record ConnectDeviceResultDto(
    string AccessToken,
    string DeviceId
);