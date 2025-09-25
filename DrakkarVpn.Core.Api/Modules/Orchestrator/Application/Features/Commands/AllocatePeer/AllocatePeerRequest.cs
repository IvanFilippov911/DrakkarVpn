using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.AllocatePeer;

public sealed record AllocatePeerRequest(Guid UserId, string? Region) : IRequest<PeerRegisterResponseDto>;