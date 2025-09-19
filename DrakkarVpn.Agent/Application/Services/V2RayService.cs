using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Infrastructure.Grpc.Protos;
using DrakkarVpn.Agent.Application.DTOs;

namespace DrakkarVpn.Agent.Application.Services;

public sealed class V2RayService : IV2RayService
{
    private readonly IV2RayGrpcClient _client;

    public V2RayService(IV2RayGrpcClient client) => _client = client;

    public Task<RegisterPeerResponseDto> RegisterPeerAsync(Guid userId, CancellationToken ct) =>
        _client.RegisterPeerAsync(userId, ct);

    public Task<bool> RevokePeerAsync(Guid peerUuid, CancellationToken ct) =>
        _client.RevokePeerAsync(peerUuid, ct);
}