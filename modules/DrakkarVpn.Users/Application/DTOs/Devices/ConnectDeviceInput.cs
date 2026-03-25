namespace DrakkarVpn.Users.Application.DTOs;

public sealed record ConnectDeviceInput(
    string InitData,
    string? DeviceName,
    string? Platform,
    string? ExistingDeviceId
);