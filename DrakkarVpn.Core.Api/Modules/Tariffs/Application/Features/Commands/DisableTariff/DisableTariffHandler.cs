using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Commands.DisableTariff;

public sealed class DisableTariffHandler : IRequestHandler<DisableTariffRequest, Unit>
{
    private readonly ITariffRepository _repo;

    public DisableTariffHandler(ITariffRepository repo) => _repo = repo;

    public async Task<Unit> Handle(DisableTariffRequest request, CancellationToken ct)
    {
        var tariff = await _repo.GetByIdAsync(new(request.TariffId), ct);

        if (tariff is null)
            throw new InvalidOperationException($"Tariff {request.TariffId} not found");

        tariff.Disable();
        
        return Unit.Value;
    }
}