using System.Net.Http.Json;
using System.Text.Json;
using DrakkarVpn.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Repositories;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles.AgentApply;

namespace DrakkarVpn.Servers.Infrastructure;

public sealed class ServerTransportAgentClient : IAgentTransportApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;

    public ServerTransportAgentClient(HttpClient http) => _http = http;

    public async Task<AgentTransportApplyCallResult> ApplyServerTransportAsync(
        string agentBaseUrl,
        AgentApplyServerTransportRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(agentBaseUrl))
            throw new ArgumentException("agentBaseUrl is required", nameof(agentBaseUrl));
        ArgumentNullException.ThrowIfNull(request);

        var baseUrl = agentBaseUrl.TrimEnd('/');
        var url = $"{baseUrl}/server-transport/apply";

        HttpResponseMessage response;
        try
        {
            response = await _http.PostAsJsonAsync(url, request, JsonOptions, ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException ex)
        {
            return AgentTransportApplyCallResult.TransportFailure(
                "agent_timeout",
                ex.Message);
        }
        catch (HttpRequestException ex)
        {
            return AgentTransportApplyCallResult.TransportFailure(
                "agent_http_request_failed",
                ex.Message);
        }

        var json = await response.Content.ReadAsStringAsync(ct);
        AgentApplyServerTransportWireResponse? wire = null;

        if (!string.IsNullOrWhiteSpace(json))
        {
            try
            {
                wire = JsonSerializer.Deserialize<AgentApplyServerTransportWireResponse>(
                    json,
                    JsonOptions);
            }
            catch (JsonException ex)
            {
                return AgentTransportApplyCallResult.TransportFailure(
                    "agent_response_malformed",
                    ex.Message);
            }
        }

        if (wire is not null)
            return AgentTransportApplyCallResult.FromWire(wire);

        return AgentTransportApplyCallResult.TransportFailure(
            code: $"HTTP_{(int)response.StatusCode}",
            message: $"Agent returned HTTP {(int)response.StatusCode} without a valid apply payload.");
    }
}
