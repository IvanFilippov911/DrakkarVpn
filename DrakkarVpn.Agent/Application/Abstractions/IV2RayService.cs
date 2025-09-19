using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Abstractions;

public interface IV2RayService
{
    Task<RegisterPeerResponseDto> RegisterPeerAsync(Guid userId, CancellationToken ct);
    Task<bool> RevokePeerAsync(Guid peerUuid, CancellationToken ct);
}