using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;

namespace DrakkarVpn.Users.Application.Abstractions;

public interface IDevicesRevocationService
{
    Task<bool> RevokeDeviceAsync(
        string deviceId,
        DateTime markerUtc,
        CancellationToken ct);

    Task<BulkDevicesRevokeResult> RevokeUsersDevicesBulkAsync(
        IReadOnlyCollection<Guid> userIds,
        DateTime markerUtc,
        CancellationToken ct);
}