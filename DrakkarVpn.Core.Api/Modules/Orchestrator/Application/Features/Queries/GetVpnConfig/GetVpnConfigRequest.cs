using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetVpnConfig;

public sealed record GetVpnConfigRequest(
    long TelegramId,
    string DeviceId,          
    string? DeviceName,
    string? Platform,
    string? Region            
) : IRequest<GetVpnConfigResponse?>;