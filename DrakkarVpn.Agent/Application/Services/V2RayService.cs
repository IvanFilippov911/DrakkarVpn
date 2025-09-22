using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.DTOs;
using DrakkarVpn.Agent.Infrastructure.Grpc;

namespace DrakkarVpn.Agent.Application.Services;

public sealed class V2RayService : IV2RayService
{
    private readonly IV2RayClient _client;

    public V2RayService(IV2RayClient client) => _client = client;

    public Task<RegisterPeerResponseDto> RegisterPeerAsync(CancellationToken ct) =>
        _client.RegisterPeerAsync(ct);

    public Task<bool> RevokePeerAsync(Guid peerUuid, CancellationToken ct) =>
        _client.RevokePeerAsync(peerUuid, ct);
    
    public Task<IReadOnlyList<PeersResultDto>> GetListPeersAsync(CancellationToken ct) =>
        _client.GetListPeersAsync(ct);

}