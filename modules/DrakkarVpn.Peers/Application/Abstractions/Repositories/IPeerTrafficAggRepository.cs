using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeerTrafficAggRepository
{
    Task UpsertBatchAsync(
        IReadOnlyCollection<PeerTrafficAgg> items,
        CancellationToken ct);
}