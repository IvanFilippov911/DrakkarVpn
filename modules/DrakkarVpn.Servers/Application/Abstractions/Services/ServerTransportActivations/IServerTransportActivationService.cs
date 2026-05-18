using DrakkarVpn.Servers.Application.DTOs.ServerTransportActivations;

namespace DrakkarVpn.Servers.Application.Abstractions.Services.ServerTransportActivations;

public interface IServerTransportActivationService
{
    Task<long> ActivateAsync(ActivateServerTransportActivationInput input, CancellationToken ct);
}
