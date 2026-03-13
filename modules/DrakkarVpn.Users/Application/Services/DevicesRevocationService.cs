using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Users.Application.Abstractions;

namespace DrakkarVpn.Users.Application.Features.Services;

public sealed class DevicesRevocationService : IDevicesRevocationService
{
    private readonly IDeviceRepository _devices;

    public DevicesRevocationService(IDeviceRepository devices)
        => _devices = devices;

    public async Task<bool> RevokeDeviceAsync(
        string deviceId,
        DateTime markerUtc,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("DeviceId is required", nameof(deviceId));

        var updated = await _devices.RevokeByIdAsync(deviceId, markerUtc, ct);
        if (!updated)
            return false;

        return await _devices.IsRevokedByMarkerAsync(deviceId, markerUtc, ct);
    }

    
    public async Task<BulkDevicesRevokeResult> RevokeUsersDevicesBulkAsync(
        IReadOnlyCollection<Guid> userIds,
        DateTime markerUtc,
        CancellationToken ct)
    {
        var ids = userIds
            .Where(x => x != Guid.Empty)
            .Distinct()
            .ToArray();

        if (ids.Length == 0)
            return new BulkDevicesRevokeResult([], [], []);

        var activeRows = await _devices.GetActiveForRevokeByUsersAsync(ids, ct);
        
        if (activeRows.Count == 0)
        {
            return new BulkDevicesRevokeResult(
                SucceededUserIds: ids.ToList(),
                FailedUserIds: [],
                FailureDetails: []);
        }

        await _devices.RevokeManyByUsersAsync(ids, markerUtc, ct);

        var revokedRows = await _devices.GetRevokedByMarkerAsync(ids, markerUtc, ct);
        var revokedSet = revokedRows
            .Select(r => (r.UserId, r.DeviceId))
            .ToHashSet();

        var failures = new List<BulkDeviceRevokeFailure>(capacity: activeRows.Count);

        foreach (var row in activeRows)
        {
            if (!revokedSet.Contains((row.UserId, row.DeviceId)))
            {
                failures.Add(new BulkDeviceRevokeFailure(
                    UserId: row.UserId,
                    DeviceId: row.DeviceId,
                    Reason: "DeviceRevokeMarkerMismatch"
                ));
            }
        }

        if (failures.Count == 0)
        {
            return new BulkDevicesRevokeResult(
                SucceededUserIds: ids.ToList(),
                FailedUserIds: [],
                FailureDetails: []);
        }

        var failedUsers = failures.Select(f => f.UserId).Distinct().ToHashSet();
        var activeUsers = activeRows.Select(r => r.UserId).Distinct().ToHashSet();

        var succeededUsers = ids
            .Where(u => !activeUsers.Contains(u) || !failedUsers.Contains(u))
            .Distinct()
            .ToList();

        return new BulkDevicesRevokeResult(
            SucceededUserIds: succeededUsers,
            FailedUserIds: failedUsers.ToList(),
            FailureDetails: failures
        );
    }
}