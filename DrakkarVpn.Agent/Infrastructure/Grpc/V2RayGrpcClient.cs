using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Agent.Application.DTOs;
using DrakkarVpn.Agent.Infrastructure.Grpc.Protos;
using Grpc.Net.Client;

namespace DrakkarVpn.Agent.Infrastructure.Grpc;

public sealed class V2RayGrpcClient : IV2RayGrpcClient, IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly V2RayTune.V2RayTuneClient _client;

    public V2RayGrpcClient(GrpcChannel channel)
    {
        _channel = channel;
        _client = new V2RayTune.V2RayTuneClient(channel);
    }

    public async Task<RegisterPeerResponseDto> RegisterPeerAsync(Guid userId, CancellationToken ct)
    {
        var req = new RegisterPeerRequest { UserId = userId.ToString() };
        var resp = await _client.RegisterPeerAsync(req, cancellationToken: ct);

        if (string.IsNullOrWhiteSpace(resp.PeerUuid) || string.IsNullOrWhiteSpace(resp.ConfigRaw))
            throw new InvalidOperationException("V2RayTune returned invalid payload");

        return new RegisterPeerResponseDto(Guid.Parse(resp.PeerUuid), resp.ConfigRaw);
    }

    public async Task<bool> RevokePeerAsync(Guid peerUuid, CancellationToken ct)
    {
        var req = new Protos.RevokePeerRequest { PeerUuid = peerUuid.ToString() };
        var resp = await _client.RevokePeerAsync(req, cancellationToken: ct);
        return resp.Success;
    }

    public void Dispose() => _channel.Dispose();

}