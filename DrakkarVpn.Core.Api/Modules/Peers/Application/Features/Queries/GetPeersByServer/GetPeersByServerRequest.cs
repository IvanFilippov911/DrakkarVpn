using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersByServer;

public sealed record GetPeersByServerRequest(Guid ServerId) : IRequest<IReadOnlyList<PeerResponseDto>>;
