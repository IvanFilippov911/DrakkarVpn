using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Tariffs.AdminEnableTariff;

public sealed class AdminEnableTariffHandler
    : IRequestHandler<AdminEnableTariffCommand, Unit>
{
    private readonly ITariffAdminService _tariffs;

    public AdminEnableTariffHandler(ITariffAdminService tariffs) => _tariffs = tariffs;

    public async Task<Unit> Handle(AdminEnableTariffCommand c, CancellationToken ct)
    {
        await _tariffs.EnableAsync(c.TariffId, ct);
        return Unit.Value;
    }
}