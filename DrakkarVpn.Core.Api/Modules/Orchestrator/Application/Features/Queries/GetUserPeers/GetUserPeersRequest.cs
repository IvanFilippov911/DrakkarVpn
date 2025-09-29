using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetUserPeers;

public sealed record GetUserPeersRequest(long TelegramId) 
    : IRequest<IReadOnlyList<GetTgPeersDto>>;