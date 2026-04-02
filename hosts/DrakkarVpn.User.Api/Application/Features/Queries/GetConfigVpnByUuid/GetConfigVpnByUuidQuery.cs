using MediatR;

namespace DrakkarVpn.Core.Api.Application.Features.Queries.GetPeerByUuid;

public sealed record GetConfigVpnByUuidQuery(Guid PeerUuid) : IRequest<string?>;