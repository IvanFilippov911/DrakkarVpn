namespace DrakkarVpn.Users.Application.DTOs.Devices;

public interface IDeviceSessionService
{
    Task<string> GetOrCreateDeviceIdAsync(
        Guid userId,
        ConnectDeviceInput input,
        DateTime nowUtc,
        CancellationToken ct);
}