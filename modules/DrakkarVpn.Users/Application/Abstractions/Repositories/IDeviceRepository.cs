using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Domain;

namespace DrakkarVpn.Users.Application.Abstractions.Repositories;

public interface IDeviceRepository
{
    Task<Device?> GetByIdAsync(string deviceId, CancellationToken ct);
    Task<bool> OwnsAsync(Guid userId, string deviceId, CancellationToken ct);
    Task AddAsync(Device device, CancellationToken ct);
    Task UpdateAsync(string deviceId, string? name, string? platform, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);

    Task<int> CountActiveByUserAsync(Guid userId, CancellationToken ct);
    
    Task RevokeManyByUsersAsync(
        IReadOnlyCollection<Guid> userIds,
        DateTime markerUtc,
        CancellationToken ct);

    Task<IReadOnlyList<DeviceRevokeRow>> GetRevokedByMarkerAsync(
        IReadOnlyCollection<Guid> userIds,
        DateTime markerUtc,
        CancellationToken ct);

    Task<IReadOnlyList<DeviceRevokeRow>> GetActiveForRevokeByUsersAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken ct);
    
    Task<bool> RevokeByIdAsync(string deviceId, DateTime markerUtc, CancellationToken ct);
    Task<bool> IsRevokedByMarkerAsync(string deviceId, DateTime markerUtc, CancellationToken ct);
}