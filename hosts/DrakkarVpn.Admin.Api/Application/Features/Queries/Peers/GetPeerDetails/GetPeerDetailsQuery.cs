using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Peers.GetPeerDetails;

public sealed record GetPeerDetailsQuery(Guid PeerId) : IRequest<PeerDetailsDto?>;