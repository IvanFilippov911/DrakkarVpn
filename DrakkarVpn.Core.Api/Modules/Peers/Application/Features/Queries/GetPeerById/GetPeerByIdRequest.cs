using MediatR;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeerById;

public sealed record GetPeerByIdRequest(Guid PeerId) : IRequest<PeerResponseDto>;