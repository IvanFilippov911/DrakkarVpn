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

    public Task AddRangeAsync(IReadOnlyCollection<ServerTransportActivation> activations, CancellationToken ct)
    {
        _db.Set<ServerTransportActivation>().AddRange(activations);
        return Task.CompletedTask;
    }

    public Task<ServerTransportActivation?> GetByServerAndActivationIdAsync(Guid serverId, Guid activationId, CancellationToken ct)
        => _db.Set<ServerTransportActivation>()
            .FirstOrDefaultAsync(x => x.ServerId == serverId && x.Id == activationId, ct);

    public Task<ServerTransportActivation?> GetActiveByServerIdAsync(Guid serverId, CancellationToken ct)
        => _db.Set<ServerTransportActivation>()
            .FirstOrDefaultAsync(
                x => x.ServerId == serverId && x.Status == TransportActivationStatus.Active,
                ct);

    public Task DeleteAsync(ServerTransportActivation activation, CancellationToken ct)
    {
        _db.Set<ServerTransportActivation>().Remove(activation);
        return Task.CompletedTask;
    }
}
