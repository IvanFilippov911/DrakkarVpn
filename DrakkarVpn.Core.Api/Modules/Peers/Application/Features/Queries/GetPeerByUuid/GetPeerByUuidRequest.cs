using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeerByUuid;

public sealed record GetPeerByUuidRequest(Guid PeerUuid) : IRequest<GetPeerByUuidResponse?>;