using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.GetPeersForRevoke;

public sealed record GetPeersForRevokeQuery(Guid SubscriptionId)
    : IRequest<IReadOnlyList<PeerForRevokeDto>>;

    
