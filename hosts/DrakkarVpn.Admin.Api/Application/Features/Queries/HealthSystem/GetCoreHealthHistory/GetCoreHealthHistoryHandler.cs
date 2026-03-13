using DrakkarVpn.Core.Api.Modules.Admin.Application.DTOs;
using DrakkarVpn.Observability.Application.Abstracts.Services;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.HealthSystem.GetCoreHealthHistory;

public sealed class GetCoreHealthHistoryHandler
    : IRequestHandler<GetCoreHealthHistoryQuery, IReadOnlyList<CoreHealthHistoryPointDto>>
{
    private readonly ICoreHealthQueryService _coreHealth;

    public GetCoreHealthHistoryHandler(ICoreHealthQueryService coreHealth)
        => _coreHealth = coreHealth;

    public Task<IReadOnlyList<CoreHealthHistoryPointDto>> Handle(
        GetCoreHealthHistoryQuery q,
        CancellationToken ct)
        => _coreHealth.GetHistoryAsync(q.Limit, ct);
}