using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs.ProvisionPeers;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeersDomainCreateService
{
    Task<IReadOnlyList<PeerCreationResult>> CreateBatchAsync(
        IReadOnlyCollection<PeerBatchCreateDto> dtos,
        DateTime nowUtc,
        CancellationToken ct);
}