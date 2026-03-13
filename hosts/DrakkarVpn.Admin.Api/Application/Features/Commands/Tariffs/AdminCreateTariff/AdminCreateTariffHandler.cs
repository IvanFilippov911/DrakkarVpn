using DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using MediatR;

namespace DrakkarVpn.Admin.Api.Application.Features.Commands.Tariffs.AdminCreateTariff;

public sealed class AdminCreateTariffHandler
    : IRequestHandler<AdminCreateTariffCommand, Guid>
{
    private readonly ITariffAdminService _tariffs;

    public AdminCreateTariffHandler(ITariffAdminService tariffs) => _tariffs = tariffs;

    public Task<Guid> Handle(AdminCreateTariffCommand c, CancellationToken ct)
        => _tariffs.CreateAsync(c.ToDto(), ct);
}