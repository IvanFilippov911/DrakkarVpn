using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;

namespace DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportActivations;

public interface IServerTransportManagementService
{
    Task AttachProfilesAsync(AttachServerTransportProfilesInput input, CancellationToken ct);

    Task UpdateAsync(UpdateServerTransportActivationInput input, CancellationToken ct);

    Task DetachAsync(DetachServerTransportActivationInput input, CancellationToken ct);
}
