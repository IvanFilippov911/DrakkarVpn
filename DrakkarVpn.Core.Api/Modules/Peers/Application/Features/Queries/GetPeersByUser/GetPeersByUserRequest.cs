using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersByUser;

public sealed record GetPeersByUserRequest(Guid UserId) : IRequest<IReadOnlyList<PeerResponseDto>>;
