using DrakkarVpn.Servers.Domain.Entities;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServerTransportActivationWriteRepository
{
    Task AddRangeAsync(IReadOnlyCollection<ServerTransportActivation> activations, CancellationToken ct);
    Task<ServerTransportActivation?> GetByServerAndActivationIdAsync(Guid serverId, Guid activationId, CancellationToken ct);
    Task<ServerTransportActivation?> GetActiveByServerIdAsync(Guid serverId, CancellationToken ct);
    Task DeleteAsync(ServerTransportActivation activation, CancellationToken ct);
}
