using DrakkarVpn.Users.Application.DTOs;

namespace DrakkarVpn.Users.Application.Abstractions.Services;

public interface IConnectDeviceService
{
    Task<ConnectDeviceResultDto> ConnectAsync(ConnectDeviceInput input, CancellationToken ct);
}