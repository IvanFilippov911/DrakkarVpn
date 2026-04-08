using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Shared.Tariffs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.Application.Features.Queries.GetActiveTariffs;

public sealed class GetActiveTariffsHandler
    : IRequestHandler<GetActiveTariffsQuery, IReadOnlyList<TariffDto>>
{
    private readonly ITariffQueryService _tariffs;

    public GetActiveTariffsHandler(ITariffQueryService tariffs) => _tariffs = tariffs;

    public Task<IReadOnlyList<TariffDto>> Handle(GetActiveTariffsQuery _, CancellationToken ct) =>
        _tariffs.GetActiveNonTrialAsync(ct);
}
