using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Features.Commands.CreateTariff;

public sealed class CreateTariffHandler : IRequestHandler<CreateTariffRequest, Guid>
{
    private readonly ITariffRepository _repo;

    public CreateTariffHandler(ITariffRepository repo) => _repo = repo;

    public async Task<Guid> Handle(CreateTariffRequest request, CancellationToken ct)
    {
        var tariff = Tariff.CreateNew(request.Name, request.Duration, request.Price, request.DefaultMaxDevices);
        await _repo.AddAsync(tariff, ct);
        return tariff.Id.Value;
    }
}