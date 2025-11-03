using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Abstractions;

public interface IXrayPeerClient
{
    Task<RegisterPeerResponseDto> RegisterPeerAsync(CancellationToken ct);
    Task<bool> RevokePeerAsync(Guid peerUuid, CancellationToken ct);
    Task<IReadOnlyList<PeersResultDto>> GetListPeersAsync(CancellationToken ct);
}