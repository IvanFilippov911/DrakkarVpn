using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using DrakkarVpn.Servers.Domain.Aggregates;
using DrakkarVpn.Servers.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;

public sealed class TransportProfileWriteRepository : ITransportProfileWriteRepository
{
    private readonly ServerDbContext _db;

    public TransportProfileWriteRepository(ServerDbContext db)
        => _db = db;

    public Task<bool> ExistsByNameAsync(string name, Guid? excludeId, CancellationToken ct)
    {
        var normalizedName = name.Trim();

        return _db.ServerTransportProfiles.AnyAsync(
            x => x.Name == normalizedName &&
                 (!excludeId.HasValue || x.Id != excludeId.Value),
            ct);
    }

    public Task AddAsync(TransportProfile profile, CancellationToken ct)
    {
        _db.ServerTransportProfiles.Add(profile);
        return Task.CompletedTask;
    }

    public Task<TransportProfile?> GetAsync(Guid profileId, CancellationToken ct)
        => _db.ServerTransportProfiles.FirstOrDefaultAsync(x => x.Id == profileId, ct);

    public Task<bool> IsUsedInAnyActivationAsync(Guid profileId, CancellationToken ct)
        => _db.Set<ServerTransportProfileActivation>().AnyAsync(x => x.TransportProfileId == profileId, ct);

    public Task DeleteAsync(TransportProfile profile, CancellationToken ct)
    {
        _db.ServerTransportProfiles.Remove(profile);
        return Task.CompletedTask;
    }
}
