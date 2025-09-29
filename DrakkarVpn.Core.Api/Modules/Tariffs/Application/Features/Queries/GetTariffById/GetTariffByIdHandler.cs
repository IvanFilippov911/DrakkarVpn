using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Mappers;
using DrakkarVpn.Shared.Tariffs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Queries.GetTariffById;

public sealed class GetTariffByIdHandler 
    : IRequestHandler<GetTariffByIdRequest, TariffDto?>
{
    private readonly ITariffRepository _repo;

    public GetTariffByIdHandler(ITariffRepository repo) => _repo = repo;

    public async Task<TariffDto?> Handle(GetTariffByIdRequest request, CancellationToken ct)
    {
        var tariff = await _repo.GetByIdAsync(new(request.TariffId), ct);

        if (tariff is null)
            return null;

        return tariff.ToDto();
    }
}