using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

namespace DrakkarVpn.Core.Api.Modules.Servers.Infrastructure;

public sealed class ServerTransportAgentClient : IAgentTransportApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;

    public ServerTransportAgentClient(HttpClient http) => _http = http;

    public async Task ApplyServerTransportAsync(
        string agentBaseUrl,
        AgentApplyServerTransportRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(agentBaseUrl))
            throw new ArgumentException("agentBaseUrl is required", nameof(agentBaseUrl));
        ArgumentNullException.ThrowIfNull(request);

        var baseUrl = agentBaseUrl.TrimEnd('/');
        var url = $"{baseUrl}/server-transport/apply";

        var resp = await _http.PostAsJsonAsync(url, request, JsonOptions, ct);

        if (resp.IsSuccessStatusCode)
            return;

        AgentError? err = null;
        try
        {
            err = await resp.Content.ReadFromJsonAsync<AgentError>(JsonOptions, cancellationToken: ct);
        }
        catch
        {
            
        }

        var code = err?.Code ?? $"HTTP_{(int)resp.StatusCode}";
        var msg = err?.Message ?? $"Agent refused: {resp.StatusCode}";
        throw new ServerTransportAgentCallFailedException(code, msg);
    }

    private sealed class AgentError
    {
        [JsonPropertyName("code")]
        public string Code { get; init; } = default!;

        [JsonPropertyName("message")]
        public string Message { get; init; } = default!;
    }
}

public sealed class ServerTransportAgentCallFailedException : Exception
{
    public string Code { get; }

    public ServerTransportAgentCallFailedException(string code, string message) : base(message) =>
        Code = code;
}
