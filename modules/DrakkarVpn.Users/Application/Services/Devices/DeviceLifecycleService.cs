using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Users.Application.Abstractions.Repositories;
using DrakkarVpn.Users.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace DrakkarVpn.Users.Application.Services.Devices;

public sealed class DeviceLifecycleService : IDeviceLifecycleService
{
    private readonly IDeviceRepository _devices;
    private readonly ILogger<DeviceLifecycleService> _log;

    public DeviceLifecycleService(
        IDeviceRepository devices,
        ILogger<DeviceLifecycleService> log)
    {
        _devices = devices;
        _log = log;
    }

    public async Task ActivateAsync(string deviceId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("deviceId is required", nameof(deviceId));

        var device = await _devices.GetByIdAsync(deviceId, ct)
                     ?? throw new InvalidOperationException($"Device '{deviceId}' not found.");

        if (device.Status == DeviceStatus.Active)
        {
            _log.LogDebug("Device {DeviceId} is already active", deviceId);
            return;
        }

        if (device.Status == DeviceStatus.Revoked)
            throw new InvalidOperationException($"Device '{deviceId}' is revoked and cannot be activated.");

        device.Activate(DateTime.UtcNow);

        _log.LogInformation("Device {DeviceId} activated", deviceId);
    }
}