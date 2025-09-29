using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Commands.EnableTariff;

public sealed class EnableTariffHandler : IRequestHandler<EnableTariffRequest, Unit>
{
    private readonly ITariffRepository _repo;

    public EnableTariffHandler(ITariffRepository repo) => _repo = repo;

    public async Task<Unit> Handle(EnableTariffRequest request, CancellationToken ct)
    {
        var tariff = await _repo.GetByIdAsync(new(request.TariffId), ct);

        if (tariff is null)
            throw new InvalidOperationException($"Tariff {request.TariffId} not found");

        tariff.Enable();
        
        return Unit.Value;
    }
}