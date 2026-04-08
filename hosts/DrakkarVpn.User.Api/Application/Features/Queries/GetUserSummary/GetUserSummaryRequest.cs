using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetUserSummary;

public sealed record GetUserSummaryRequest(
    long TelegramId
) : IRequest<UserSummaryDto>;

