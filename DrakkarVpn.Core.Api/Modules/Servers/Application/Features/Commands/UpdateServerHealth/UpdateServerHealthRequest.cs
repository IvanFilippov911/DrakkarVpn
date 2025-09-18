using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Servers.Application.Features.Commands.UpdateServerHealth;

public sealed record UpdateServerHealthRequest(Guid ServerId, bool Reachable, int PeersActive) : IRequest<bool>;