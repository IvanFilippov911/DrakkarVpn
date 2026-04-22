using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;

public interface IServerTransportActivationReadRepository
{
    Task<bool> ServerExistsAsync(Guid serverId, CancellationToken ct);
    Task<IReadOnlyList<TransportProfileAttachCandidateDto>> GetTransportProfilesForAttachAsync(
        IReadOnlyCollection<Guid> transportProfileIds,
        CancellationToken ct);

    Task<IReadOnlySet<Guid>> GetAttachedProfileIdsAsync(
        Guid serverId,
        IReadOnlyCollection<Guid> transportProfileIds,
        CancellationToken ct);

    Task<IReadOnlyList<ServerTransportActivationListItemDto>> GetByServerAsync(Guid serverId, CancellationToken ct);
}
