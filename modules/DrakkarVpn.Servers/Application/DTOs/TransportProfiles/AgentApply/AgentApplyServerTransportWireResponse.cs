using System.Text.Json.Serialization;

namespace DrakkarVpn.Servers.Application.DTOs.TransportProfiles;

public sealed record AgentApplyServerTransportWireResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("outcome")]
    public string? Outcome { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("phase")]
    public string Phase { get; init; } = string.Empty;

    [JsonPropertyName("payloadHash")]
    public string? PayloadHash { get; init; }

    [JsonPropertyName("rollbackAttempted")]
    public bool RollbackAttempted { get; init; }

    [JsonPropertyName("rollbackSucceeded")]
    public bool RollbackSucceeded { get; init; }
}
