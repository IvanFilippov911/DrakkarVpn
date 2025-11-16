using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Peers.Application.Features.Queries.CountPeersOnServer;

public sealed record CountPeersOnServerQuery(Guid ServerId) : IRequest<int>;