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

    public async Task<AgentApplyServerTransportWireResponse> ApplyServerTransportAsync(
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

        var json = await resp.Content.ReadAsStringAsync(ct);
        AgentApplyServerTransportWireResponse? wire = null;
        if (!string.IsNullOrWhiteSpace(json))
        {
            try
            {
                wire = JsonSerializer.Deserialize<AgentApplyServerTransportWireResponse>(
                    json,
                    JsonOptions);
            }
            catch (JsonException)
            {
                // leave wire null → throw below with generic mapping
            }
        }

        if (wire?.Success == true)
            return wire;

        var code = wire?.Code ?? $"HTTP_{(int)resp.StatusCode}";
        var msg = wire?.Message ?? "Agent refused, malformed agent response, or apply did not succeed.";
        throw new ServerTransportAgentCallFailedException(
            code,
            msg,
            phase: wire?.Phase ?? string.Empty,
            payloadHash: wire?.PayloadHash,
            rollbackAttempted: wire?.RollbackAttempted,
            rollbackSucceeded: wire?.RollbackSucceeded);
    }
}

public sealed class ServerTransportAgentCallFailedException : Exception
{
    public string Code { get; }

    public string Phase { get; }

    public string? PayloadHash { get; }

    public bool? RollbackAttempted { get; }

    public bool? RollbackSucceeded { get; }

    public ServerTransportAgentCallFailedException(
        string code,
        string message,
        string? phase = null,
        string? payloadHash = null,
        bool? rollbackAttempted = null,
        bool? rollbackSucceeded = null) : base(message)
    {
        Code = code;
        Phase = phase ?? string.Empty;
        PayloadHash = payloadHash;
        RollbackAttempted = rollbackAttempted;
        RollbackSucceeded = rollbackSucceeded;
    }
}
