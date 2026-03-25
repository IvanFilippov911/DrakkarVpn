namespace DrakkarVpn.Users.Application.Abstractions.Services;

public interface IDeviceLifecycleService
{
    Task ActivateAsync(string deviceId, CancellationToken ct);
}