using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.UpdateServerHealth;

public sealed record UpdateServerHealthRequest(
    Guid ServerId,
    ServerStatus Status,
    bool Reachable,
    int PeersActive
) : IRequest<bool>;