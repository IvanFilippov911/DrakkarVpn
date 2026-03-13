using DrakkarVpn.Servers.Application.Abstractions;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Servers.RevokeAllServerPeers;

public sealed record RevokeAllServerPeersCommand(Guid ServerId)
    : IRequest<int>, IServersCommand<int>;