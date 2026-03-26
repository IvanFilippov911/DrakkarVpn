namespace DrakkarVpn.Admin.Api.API.Contracts.Servers;

/// <summary>
/// API request contract for server registration.
/// Mirrors RegisterServerRequest command shape.
/// </summary>
public sealed record RegisterServerApiRequest(
    string Name,
    string Region,
    string PublicHost,
    int PublicPort,
    string RealityPublicKey,
    string RealityShortId,
    string RealitySni,
    string AgentBaseUrl,
    string AgentTokenEncrypted,
    int? MaxPeers);

