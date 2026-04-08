using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using DrakkarVpn.Shared.Tariffs;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Mappers;

public static class TariffMapper
{
    public static TariffDto ToDto(this Tariff tariff) =>
        new(
            tariff.Id.Value,
            tariff.Name,
            tariff.Price,
            tariff.Duration,
            tariff.Status,
            tariff.CreatedAt,
            tariff.DefaultMaxDevices,
            tariff.Kind
        );
}