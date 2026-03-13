namespace DrakkarVpn.Core.Api.Modules.Peers.API.Contracts.Response;

public sealed record ConnectDeviceResponse(
    string AccessToken,
    string DeviceId
);