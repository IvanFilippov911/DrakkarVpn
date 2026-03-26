using MediatR;

namespace DrakkarVpn.Core.Api.Application.Features.Queries.GetPeerByUuid;

public sealed record GetPeerByUuidQuery(Guid PeerUuid) : IRequest<string?>;