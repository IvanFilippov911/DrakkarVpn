using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetVpnConfig;

public sealed record GetVpnConfigRequest(
    long TelegramId,
    string DeviceId,          
    string? Region            
) : IRequest<GetVpnConfigResponse?>;