using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;

namespace DrakkarVpn.Servers.Application.Abstractions.Services;

public interface IServerTransportActivationManagementService
{
    Task AttachProfilesAsync(AttachServerTransportProfilesInput input, CancellationToken ct);
    Task<IReadOnlyList<ServerTransportActivationListItemDto>> GetByServerAsync(Guid serverId, CancellationToken ct);
    Task UpdateAsync(UpdateServerTransportActivationInput input, CancellationToken ct);
    Task ActivateAsync(ActivateServerTransportActivationInput input, CancellationToken ct);
    Task DetachAsync(DetachServerTransportActivationInput input, CancellationToken ct);
}
