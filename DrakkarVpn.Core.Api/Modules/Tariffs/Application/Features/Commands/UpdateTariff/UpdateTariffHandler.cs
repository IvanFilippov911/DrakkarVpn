using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Commands.UpdateTariff;

public sealed class UpdateTariffHandler : IRequestHandler<UpdateTariffRequest, Unit>
{
    private readonly ITariffRepository _repo;

    public UpdateTariffHandler(ITariffRepository repo) => _repo = repo;

    public async Task<Unit> Handle(UpdateTariffRequest request, CancellationToken ct)
    {
        var tariff = await _repo.GetByIdAsync(new(request.TariffId), ct);

        if (tariff is null)
            throw new InvalidOperationException($"Tariff {request.TariffId} not found");

        tariff.Update(request.Name, request.Price, request.Duration, request.DefaultMaxDevices);

        return Unit.Value;
    }
}