using DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Tariffs.AdminUpdateTariff;

public sealed class AdminUpdateTariffHandler
    : IRequestHandler<AdminUpdateTariffCommand, Unit>
{
    private readonly ITariffAdminService _tariffs;

    public AdminUpdateTariffHandler(ITariffAdminService tariffs) => _tariffs = tariffs;

    public async Task<Unit> Handle(AdminUpdateTariffCommand c, CancellationToken ct)
    {
        await _tariffs.UpdateAsync(c.TariffId, c.ToDto(), ct);
        return Unit.Value;
    }
}