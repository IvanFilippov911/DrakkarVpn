using DrakkarVpn.Core.Api.Infrastructure.EF;
using DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Users.Infrastructure.Repositories;

public sealed class DeviceRepository : IDeviceRepository
{
    private readonly AppDbContext _db;
    public DeviceRepository(AppDbContext db) => _db = db;

    public Task<Device?> GetByIdAsync(string deviceId, CancellationToken ct) =>
        _db.Set<Device>().FirstOrDefaultAsync(d => d.DeviceId == deviceId, ct);

    public Task<bool> OwnsAsync(Guid subsId, string deviceId, CancellationToken ct) =>
        _db.Set<Device>().AnyAsync(d => d.DeviceId == deviceId && d.SubscriptionId == subsId && d.Status == DeviceStatus.Active, ct);

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
    
    public Task<int> CountActiveBySubscriptionAsync(Guid subscriptionId, CancellationToken ct) =>
        _db.Devices.AsNoTracking()
            .CountAsync(d => d.SubscriptionId == subscriptionId && d.Status == DeviceStatus.Active, ct);
    
    public async Task<IReadOnlyList<Device>> ListBySubscriptionAsync(
        Guid subscriptionId,
        CancellationToken ct)
    {
        var list = await _db.Devices
            .AsNoTracking()
            .Where(d => d.SubscriptionId == subscriptionId)
            .OrderByDescending(d => d.CreatedAt)
            .ThenBy(d => d.DeviceId)
            .ToListAsync(ct);

        return list; 
    }
    
    
    
    
}
