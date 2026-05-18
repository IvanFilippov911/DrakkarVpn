using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;

namespace DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportActivations;

public interface IServerTransportActivationQueryService
{
    Task<IReadOnlyList<ServerTransportActivationListItemDto>> GetByServerAsync(
        Guid serverId,
        CancellationToken ct);
}
