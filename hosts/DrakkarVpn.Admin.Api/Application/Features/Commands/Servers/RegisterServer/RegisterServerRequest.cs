using DrakkarVpn.Servers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Servers.RegisterServer;

public sealed record RegisterServerRequest(
    string Name,
    string Region,
    string PublicHost,
    string AgentBaseUrl,
    string AgentTokenEncrypted,
    int?   MaxPeers
) : IRequest<Guid>, IServersCommand<Guid>;