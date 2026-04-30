using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.EF;
using DrakkarVpn.Servers.Domain.Entities;
using DrakkarVpn.Servers.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure.Repositories;

public sealed class ServerTransportActivationWriteRepository : IServerTransportActivationWriteRepository
{
    private readonly ServerDbContext _db;

    public ServerTransportActivationWriteRepository(ServerDbContext db)
        => _db = db;

    public Task AddRangeAsync(IReadOnlyCollection<ServerTransportProfileActivation> activations, CancellationToken ct)
    {
        _db.Set<ServerTransportProfileActivation>().AddRange(activations);
        return Task.CompletedTask;
    }

    public Task<ServerTransportProfileActivation?> GetByServerAndActivationIdAsync(Guid serverId, Guid activationId, CancellationToken ct)
        => _db.Set<ServerTransportProfileActivation>()
            .FirstOrDefaultAsync(x => x.ServerId == serverId && x.Id == activationId, ct);

    public Task<ServerTransportProfileActivation?> GetActiveByServerIdAsync(Guid serverId, CancellationToken ct)
        => _db.Set<ServerTransportProfileActivation>()
            .FirstOrDefaultAsync(
                x => x.ServerId == serverId && x.Status == TransportActivationStatus.Active,
                ct);

    public Task DeleteAsync(ServerTransportProfileActivation profileActivation, CancellationToken ct)
    {
        _db.Set<ServerTransportProfileActivation>().Remove(profileActivation);
        return Task.CompletedTask;
    }
}
