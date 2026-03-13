using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Shared.Tariffs;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Tariffs.AdminGetActiveTariffs;

public sealed class AdminGetActiveTariffsHandler
    : IRequestHandler<AdminGetActiveTariffsQuery, IReadOnlyList<TariffDto>>
{
    private readonly ITariffQueryService _tariffs;

    public AdminGetActiveTariffsHandler(ITariffQueryService tariffs) => _tariffs = tariffs;

    public Task<IReadOnlyList<TariffDto>> Handle(AdminGetActiveTariffsQuery _, CancellationToken ct)
        => _tariffs.GetActiveAsync(ct);
}