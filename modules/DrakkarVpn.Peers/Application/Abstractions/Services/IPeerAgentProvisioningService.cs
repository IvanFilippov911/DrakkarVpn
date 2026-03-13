using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs.ProvisionPeers;
using DrakkarVpn.Core.Api.Modules.Peers.Infrastructure.Entities;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public interface IPeerAgentProvisioningService
{
    Task<IReadOnlyList<AgentProvisionAttemptResult>> ProvisionOnAgentsAsync(
        IReadOnlyList<PeerProvisionJob> jobs,
        int maxParallel,
        DateTime nowUtc,
        CancellationToken ct);
}