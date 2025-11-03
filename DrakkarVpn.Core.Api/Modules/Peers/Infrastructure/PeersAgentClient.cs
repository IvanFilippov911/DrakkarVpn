using System.Text.Json.Serialization;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Domain;
using DrakkarVpn.Core.Api.Modules.Servers.Domain;

namespace DrakkarVpn.Core.Api.Modules.Peers.Infrastructure;

public sealed class PeersAgentClient : IPeersAgentClient
{
    private readonly HttpClient _http;
    public PeersAgentClient(HttpClient http) => _http = http;

    public async Task<PeerAgentRegisterResponseDto> RegisterPeerAsync(Server server, CancellationToken ct)
    {
        var url = $"{server.AgentBaseUrl}peers";

        var response = await _http.PostAsync(url, null, ct);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Agent {server.Id} refused peer creation: {response.StatusCode}");

        var wire = await response.Content.ReadFromJsonAsync<AgentRegisterPeerResponse>(cancellationToken: ct);
        if (wire is null || wire.PeerUuid == Guid.Empty || string.IsNullOrWhiteSpace(wire.ConfigRaw))
            throw new InvalidOperationException($"Agent {server.Id} returned invalid payload");
        
        return new PeerAgentRegisterResponseDto(wire.PeerUuid, wire.ConfigRaw);
    }


    public async Task<bool> RevokePeerAsync(Server server, Guid? peerUuid, CancellationToken ct)
    {
        var url = $"{server.AgentBaseUrl}peers/{peerUuid}";
        var resp = await _http.DeleteAsync(url, ct);
        return resp.IsSuccessStatusCode;
    }
    
    private sealed class AgentRegisterPeerResponse
    {
        [JsonPropertyName("peerUuid")] public Guid PeerUuid { get; init; }
        [JsonPropertyName("configRaw")] public string ConfigRaw { get; init; } = default!;
    }
}
