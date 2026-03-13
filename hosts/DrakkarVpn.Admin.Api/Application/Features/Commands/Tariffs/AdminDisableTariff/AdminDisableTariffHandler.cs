using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Tariffs.AdminDisableTariff;

public sealed class AdminDisableTariffHandler
    : IRequestHandler<AdminDisableTariffCommand, Unit>
{
    private readonly ITariffAdminService _tariffs;

    public AdminDisableTariffHandler(ITariffAdminService tariffs) => _tariffs = tariffs;

    public async Task<Unit> Handle(AdminDisableTariffCommand c, CancellationToken ct)
    {
        await _tariffs.DisableAsync(c.TariffId, ct);
        return Unit.Value;
    }
}