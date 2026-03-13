using DrakkarVpn.Admin.Api.Application.Features.Commands.Tariffs.AdminCreateTariff;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;

public static class AdminCreateTariffMapper
{
    public static TariffCreateDto ToDto(this AdminCreateTariffCommand c)
        => new(
            Name: c.Name,
            Duration: c.Duration,
            Price: c.Price,
            DefaultMaxDevices: c.DefaultMaxDevices
        );
}