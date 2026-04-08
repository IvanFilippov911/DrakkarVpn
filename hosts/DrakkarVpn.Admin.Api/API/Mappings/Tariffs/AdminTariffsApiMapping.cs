using DrakkarVpn.Admin.Api.Application.Features.Commands.Tariffs.AdminCreateTariff;
using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts.Tariffs;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Tariffs.AdminUpdateTariff;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.Tariffs.AdminGetActiveTariffs;
using DrakkarVpn.Shared.Tariffs;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;

public static class AdminTariffsApiMapping
{
    public static TariffApiResponse ToApiResponse(this TariffDto dto)
    {
        return new TariffApiResponse(
            Id:               dto.Id,
            Name:             dto.Name,
            Price:            dto.Price,
            Duration:         dto.Duration,
            Status:           dto.Status.ToString(),
            CreatedAt:        dto.CreatedAt,
            DefaultMaxDevices: dto.DefaultMaxDevices,
            Kind:             dto.Kind.ToString());
    }

    public static IReadOnlyList<TariffApiResponse> ToApiResponse(
        this IReadOnlyList<TariffDto> items)
    {
        return items.Select(ToApiResponse).ToList();
    }

    public static AdminCreateTariffCommand ToCommand(this CreateTariffApiRequest body)
    {
        return new AdminCreateTariffCommand(
            Name:              body.Name,
            Duration:          TimeSpan.FromDays(body.DurationDays),
            Price:             body.Price,
            DefaultMaxDevices: body.DefaultMaxDevices,
            Kind:              body.Kind);
    }

    public static AdminUpdateTariffCommand ToCommand(this UpdateTariffApiRequest body, Guid id)
    {
        return new AdminUpdateTariffCommand(
            TariffId:          id,
            Name:              body.Name,
            Duration:          TimeSpan.FromDays(body.DurationDays),
            Price:             body.Price,
            DefaultMaxDevices: body.DefaultMaxDevices);
    }
}

