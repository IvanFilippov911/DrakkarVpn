using DrakkarVpn.Core.Api.Modules.Users.Domain;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Abstractions;

public interface IDeviceRepository
{
    Task<Device?> GetByIdAsync(string deviceId, CancellationToken ct);
    Task<bool> OwnsAsync(Guid userId, string deviceId, CancellationToken ct);
    Task AddAsync(Device device, CancellationToken ct);
    Task UpdateAsync(string deviceId, string? name, string? platform, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}