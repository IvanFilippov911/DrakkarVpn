using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.RegisterServer;

public sealed record RegisterServerRequest(
    string Name,
    string Region,
    string PublicHost,
    string AgentBaseUrl,
    string AgentTokenEncrypted,
    int? MaxPeers
) : IRequest<Guid>;