using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using DrakkarVpn.Core.Api.Modules.Peers.Application.Abstractions;

public sealed class PeersAgentClient : IPeersAgentClient
{
    private readonly HttpClient _http;
    public PeersAgentClient(HttpClient http) => _http = http;

    public async Task RegisterPeerAsync(string agentBaseUrl, Guid peerUuid, string configRaw, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(agentBaseUrl)) throw new ArgumentException("agentBaseUrl is required", nameof(agentBaseUrl));
        if (peerUuid == Guid.Empty) throw new ArgumentException("peerUuid is required", nameof(peerUuid));
        if (string.IsNullOrWhiteSpace(configRaw)) throw new ArgumentException("configRaw is required", nameof(configRaw));

        var baseUrl = agentBaseUrl.TrimEnd('/');
        var url = $"{baseUrl}/peers";

        var body = new AgentRegisterPeerRequest { PeerUuid = peerUuid, ConfigRaw = configRaw };

        var resp = await _http.PostAsJsonAsync(url, body, ct);

        if (resp.IsSuccessStatusCode)
            return;
        
        if (resp.StatusCode == HttpStatusCode.Conflict)
            return;

        AgentError? err = null;
        try { err = await resp.Content.ReadFromJsonAsync<AgentError>(cancellationToken: ct); } catch { /* ignore */ }

        var code = err?.Code ?? $"HTTP_{(int)resp.StatusCode}";
        var msg  = err?.Message ?? $"Agent refused: {resp.StatusCode}";
        throw new PeersAgentCallFailedException(code, msg);
    }

    public async Task<bool> RevokePeerAsync(string agentBaseUrl, Guid peerUuid, CancellationToken ct)
    {
        var baseUrl = agentBaseUrl.TrimEnd('/');
        var url = $"{baseUrl}/peers/{peerUuid}";
        var resp = await _http.DeleteAsync(url, ct);
        if (resp.IsSuccessStatusCode)
            return true;

        if (resp.StatusCode == HttpStatusCode.NotFound)
            return true;

        return false;
    }

    private sealed class AgentRegisterPeerRequest
    {
        [JsonPropertyName("peerUuid")]  public Guid PeerUuid { get; init; }
        [JsonPropertyName("configRaw")] public string ConfigRaw { get; init; } = default!;
    }

    private sealed class AgentError
    {
        [JsonPropertyName("code")]    public string Code { get; init; } = default!;
        [JsonPropertyName("message")] public string Message { get; init; } = default!;
    }
}

public sealed class PeersAgentCallFailedException : Exception
{
    public string Code { get; }
    public PeersAgentCallFailedException(string code, string message) : base(message) => Code = code;
}