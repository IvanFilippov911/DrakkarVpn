using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using DrakkarVpn.Users.Application.Abstractions.Repositories;
using DrakkarVpn.Users.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Users.Infrastructure.Repositories;

public sealed class DeviceRepository : IDeviceRepository
{
    private readonly UsersDbContext _db;
    public DeviceRepository(UsersDbContext db) => _db = db;

    public Task<Device?> GetByIdAsync(string deviceId, CancellationToken ct) =>
        _db.Set<Device>().FirstOrDefaultAsync(d => d.DeviceId == deviceId, ct);
    
    public Task<Device?> GetForUpdateAsync(string deviceId, CancellationToken ct) =>
        _db.Devices
            .FirstOrDefaultAsync(d => d.DeviceId == deviceId, ct);

    public Task<bool> OwnsAsync(Guid userId, string deviceId, CancellationToken ct) =>
        _db.Set<Device>().AnyAsync(d => d.DeviceId == deviceId && d.UserId == userId && d.Status == DeviceStatus.Active, ct);

    public async Task AddAsync(Device device, CancellationToken ct) =>
        await _db.Set<Device>().AddAsync(device, ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);

    public async Task UpdateAsync(string deviceId, string? name, string? platform, CancellationToken ct)
    {
        var nowUtc = DateTime.UtcNow;

        await _db.Set<Device>()
            .Where(d => d.DeviceId == deviceId && d.Status == DeviceStatus.Active)
            .ExecuteUpdateAsync(up => up
                    .SetProperty(d => d.LastSeen, nowUtc)
                    .SetProperty(d => d.Name,    d => name    != null ? name    : d.Name)
                    .SetProperty(d => d.Platform,d => platform!= null ? platform: d.Platform)
                , ct);
    }
    
    
    
    public Task<int> CountActiveByUserAsync(Guid userId, CancellationToken ct) =>
        _db.Devices.AsNoTracking()
            .CountAsync(d => d.UserId == userId && d.Status == DeviceStatus.Active, ct);
    
    public async Task<IReadOnlyList<Device>> ListByUserAsync(
        Guid userId,
        CancellationToken ct)
    {
        var list = await _db.Devices
            .AsNoTracking()
            .Where(d => d.UserId == userId && d.Status == DeviceStatus.Active)
            .OrderByDescending(d => d.CreatedAt)
            .ThenBy(d => d.DeviceId)
            .ToListAsync(ct);

        return list; 
    }
    
    public Task RevokeManyByUsersAsync(
        IReadOnlyCollection<Guid> userIds,
        DateTime markerUtc,
        CancellationToken ct)
    {
        var ids = userIds.Distinct().ToArray();

        return _db.Devices
            .Where(d => ids.Contains(d.UserId))
            .Where(d => d.Status == DeviceStatus.Active)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(d => d.Status, DeviceStatus.Revoked)
                    .SetProperty(d => d.StatusUpdatedAtUtc, markerUtc),
                ct);
    }

    public async Task<IReadOnlyList<DeviceRevokeRow>> GetRevokedByMarkerAsync(
        IReadOnlyCollection<Guid> userIds,
        DateTime markerUtc,
        CancellationToken ct)
    {
        var ids = userIds.Distinct().ToArray();

        return await _db.Devices
            .AsNoTracking()
            .Where(d => ids.Contains(d.UserId))
            .Where(d => d.StatusUpdatedAtUtc == markerUtc)
            .Where(d => d.Status == DeviceStatus.Revoked)
            .Select(d => new DeviceRevokeRow(d.UserId, d.DeviceId))
            .ToListAsync(ct);
    }
    
    public async Task<IReadOnlyList<DeviceRevokeRow>> GetActiveForRevokeByUsersAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken ct)
    {
        var ids = userIds.Distinct().ToArray();

        return await _db.Devices.AsNoTracking()
            .Where(d => ids.Contains(d.UserId))
            .Where(d => d.Status == DeviceStatus.Active)
            .Select(d => new DeviceRevokeRow(d.UserId, d.DeviceId))
            .ToListAsync(ct);
    }
    
    public async Task<bool> RevokeByIdAsync(string deviceId, DateTime markerUtc, CancellationToken ct)
    {
        var affected = await _db.Devices
            .Where(d => d.DeviceId == deviceId)
            .Where(d => d.Status == DeviceStatus.Active)
            .ExecuteUpdateAsync(setters => setters
                    .SetProperty(d => d.Status, DeviceStatus.Revoked)
                    .SetProperty(d => d.StatusUpdatedAtUtc, markerUtc),
                ct);

        return affected > 0;
    }

    public Task<bool> IsRevokedByMarkerAsync(string deviceId, DateTime markerUtc, CancellationToken ct)
        => _db.Devices.AsNoTracking()
            .AnyAsync(d =>
                d.DeviceId == deviceId &&
                d.Status == DeviceStatus.Revoked &&
                d.StatusUpdatedAtUtc == markerUtc, ct);
    
    
}
