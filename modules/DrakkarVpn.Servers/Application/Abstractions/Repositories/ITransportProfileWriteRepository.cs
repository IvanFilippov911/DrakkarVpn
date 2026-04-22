using DrakkarVpn.Servers.Domain.Aggregates;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface ITransportProfileWriteRepository
{
    Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludeId,
        CancellationToken ct);

    Task AddAsync(TransportProfile profile, CancellationToken ct);
    Task<TransportProfile?> GetAsync(Guid profileId, CancellationToken ct);
    Task<bool> IsUsedInAnyActivationAsync(Guid profileId, CancellationToken ct);
    Task DeleteAsync(TransportProfile profile, CancellationToken ct);
}
