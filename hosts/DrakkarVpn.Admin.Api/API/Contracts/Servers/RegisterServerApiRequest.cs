namespace DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Servers;

/// <summary>
/// API request contract for server registration.
/// Mirrors RegisterServerRequest command shape.
/// </summary>
public sealed record RegisterServerApiRequest(
    string Name,
    string Region,
    string PublicHost,
    string AgentBaseUrl,
    string AgentTokenEncrypted,
    int?   MaxPeers);

