using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Users.Application.Abstractions;
using DrakkarVpn.Users.Application.Abstractions.Repositories;
using DrakkarVpn.Users.Application.DTOs;
using DrakkarVpn.Users.Application.DTOs.Devices;

namespace DrakkarVpn.Users.Application.Services;

public sealed class DeviceSessionService : IDeviceSessionService
{
    private readonly IDeviceRepository _devices;
    private readonly IDeviceIdGenerator _deviceIds;

    public DeviceSessionService(
        IDeviceRepository devices,
        IDeviceIdGenerator deviceIds)
    {
        _devices = devices;
        _deviceIds = deviceIds;
    }

    public async Task<string> GetOrCreateDeviceIdAsync(
        Guid userId,
        ConnectDeviceInput input,
        DateTime nowUtc,
        CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(input.ExistingDeviceId))
        {
            //var owns = await _devices.OwnsAsync(userId, input.ExistingDeviceId!, ct);
            //if (!owns)
            //  throw new UnauthorizedAccessException("device does not belong to this user or is revoked");
            
            var existingDevice = await _devices.GetByIdAsync(input.ExistingDeviceId!, ct);

            if (existingDevice is not null)
            {
                await _devices.UpdateAsync(input.ExistingDeviceId!, input.DeviceName, input.Platform, ct);
                return input.ExistingDeviceId!;
            }

            var recreatedDevice = Device.Create(
                deviceId: input.ExistingDeviceId!,
                userId: userId,
                name: input.DeviceName,
                platform: input.Platform,
                nowUtc: nowUtc);

            await _devices.AddAsync(recreatedDevice, ct);
            return input.ExistingDeviceId!;
        }

        var deviceId = _deviceIds.Generate();

        var device = Device.Create(
            deviceId: deviceId,
            userId: userId,
            name: input.DeviceName,
            platform: input.Platform,
            nowUtc: nowUtc);

        await _devices.AddAsync(device, ct);
        return deviceId;
    }
}