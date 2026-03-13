using DrakkarVpn.Users.Application.DTOs;

namespace DrakkarVpn.Users.Application.Abstractions;

public interface IUserDevicesReadStore
{
    Task<IReadOnlyList<UserDeviceShortDto>> GetDevicesAsync(
        Guid userId,
        CancellationToken ct);
}