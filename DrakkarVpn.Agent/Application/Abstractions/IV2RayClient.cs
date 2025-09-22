using DrakkarVpn.Agent.Application.DTOs;
using DrakkarVpn.Agent.Infrastructure.Grpc;

namespace DrakkarVpn.Agent.Application.Abstractions;

public interface IV2RayClient
{
    Task<RegisterPeerResponseDto> RegisterPeerAsync(CancellationToken ct);
    Task<bool> RevokePeerAsync(Guid peerUuid, CancellationToken ct);
    Task<IReadOnlyList<PeersResultDto>> GetListPeersAsync(CancellationToken ct);
}