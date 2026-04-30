using DrakkarVpn.Servers.Domain.Entities;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServerTransportActivationWriteRepository
{
    Task AddRangeAsync(IReadOnlyCollection<ServerTransportProfileActivation> activations, CancellationToken ct);
    Task<ServerTransportProfileActivation?> GetByServerAndActivationIdAsync(Guid serverId, Guid activationId, CancellationToken ct);
    Task<ServerTransportProfileActivation?> GetActiveByServerIdAsync(Guid serverId, CancellationToken ct);
    Task DeleteAsync(ServerTransportProfileActivation profileActivation, CancellationToken ct);
}
