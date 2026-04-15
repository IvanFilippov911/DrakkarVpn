using DrakkarVpn.Servers.Application.Abstractions;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.Servers.RegisterServer;

public sealed record RegisterServerRequest(
    string Name,
    string Region,
    string PublicHost,
    int PublicPort,
    string AgentBaseUrl,
    string AgentTokenEncrypted,
    int? MaxPeers
) : IServersCommand<Guid>;