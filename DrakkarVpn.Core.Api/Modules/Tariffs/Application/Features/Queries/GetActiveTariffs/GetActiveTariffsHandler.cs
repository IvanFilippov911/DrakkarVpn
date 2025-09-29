using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Mappers;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using DrakkarVpn.Shared.Tariffs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Queries.GetActiveTariffs;

public sealed class GetActiveTariffsHandler 
    : IRequestHandler<GetActiveTariffsRequest, IReadOnlyList<TariffDto>>
{
    private readonly ITariffRepository _repo;

    public GetActiveTariffsHandler(ITariffRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<TariffDto>> Handle(GetActiveTariffsRequest request, CancellationToken ct)
    {
        var tariffs = await _repo.GetAllActiveAsync(ct);

        return tariffs.Select(t => t.ToDto()).ToList();
    }
}