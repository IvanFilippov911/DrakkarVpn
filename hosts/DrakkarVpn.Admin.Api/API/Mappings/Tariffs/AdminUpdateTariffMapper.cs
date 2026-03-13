using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Tariffs.AdminUpdateTariff;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;

public static class AdminUpdateTariffMapper
{
    public static TariffUpdateDto ToDto(this AdminUpdateTariffCommand c)
        => new(
            Name: c.Name,
            Duration: c.Duration,
            Price: c.Price,
            DefaultMaxDevices: c.DefaultMaxDevices
        );
}