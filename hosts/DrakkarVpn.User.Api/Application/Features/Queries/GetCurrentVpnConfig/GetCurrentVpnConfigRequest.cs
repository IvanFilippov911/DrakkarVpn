using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetCurrentVpnConfig;

public sealed record GetCurrentVpnConfigRequest(
    long TelegramId,
    string DeviceId
) : IRequest<StatusVpnConfigDto>;

