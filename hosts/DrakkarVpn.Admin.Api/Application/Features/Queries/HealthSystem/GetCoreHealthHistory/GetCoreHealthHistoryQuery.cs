using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.HealthSystem.GetCoreHealthHistory;

public sealed record GetCoreHealthHistoryQuery(int? Limit)
    : IRequest<IReadOnlyList<CoreHealthHistoryPointDto>>;
