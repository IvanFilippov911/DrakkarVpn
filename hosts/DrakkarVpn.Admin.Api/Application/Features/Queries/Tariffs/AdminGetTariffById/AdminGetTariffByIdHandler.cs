using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Shared.Tariffs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Tariffs.AdminGetTariffById;

public sealed class AdminGetTariffByIdHandler
    : IRequestHandler<AdminGetTariffByIdQuery, TariffDto?>
{
    private readonly ITariffQueryService _tariffs;

    public AdminGetTariffByIdHandler(ITariffQueryService tariffs) => _tariffs = tariffs;

    public Task<TariffDto?> Handle(AdminGetTariffByIdQuery q, CancellationToken ct)
        => _tariffs.GetByIdAsync(q.TariffId, ct);
}