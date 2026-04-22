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
    string AgentBaseUrl,
    string AgentTokenEncrypted,
    int? MaxPeers,
    IReadOnlyList<RegisterServerTransportProfileApiItem>? TransportProfiles,
    Guid? ActivateProfileId);

public sealed record RegisterServerTransportProfileApiItem(
    Guid TransportProfileId,
    string RealityPublicKey,
    int LocalPriority);

