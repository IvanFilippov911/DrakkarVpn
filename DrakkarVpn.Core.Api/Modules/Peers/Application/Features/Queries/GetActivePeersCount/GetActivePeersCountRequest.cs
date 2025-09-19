using MediatR;
namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetActivePeersCount;

public sealed record GetActivePeersCountRequest(Guid ServerId) : IRequest<int>;