using DrakkarVpn.Agent.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Agent.Application.Peers.Queries.Peers;

public sealed record PeersQuery() 
    : IRequest<IReadOnlyList<PeersResultDto>>;