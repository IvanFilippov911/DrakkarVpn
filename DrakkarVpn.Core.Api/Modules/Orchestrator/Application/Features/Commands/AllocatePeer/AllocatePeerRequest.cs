using DrakkarVpn.Core.Api.Modules.Peers.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Commands.AllocatePeer;

public sealed record AllocatePeerRequest(
    long TelegramId,
    string? Region,
    string DeviceId,           
    string? DeviceName,       
    string? Platform         
) : IRequest<PeerRegisterResponseDto>;