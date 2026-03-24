using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetHomeContext;

public sealed record GetHomeContextRequest(
    long TelegramId,
    string DeviceId
) : IRequest<HomeContextDto>;

