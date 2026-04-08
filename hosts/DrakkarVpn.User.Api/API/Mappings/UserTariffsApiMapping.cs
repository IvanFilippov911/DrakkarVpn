using DrakkarVpn.Core.Api.Modules.Orchestrator.API.Contracts.Tariffs;
using DrakkarVpn.Shared.Tariffs;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.API.Mappings;

public static class UserTariffsApiMapping
{
    public static UserTariffResponse ToUserApiResponse(this TariffDto dto) =>
        new(
            Id: dto.Id,
            Name: dto.Name,
            Price: dto.Price,
            Duration: dto.Duration,
            DefaultMaxDevices: dto.DefaultMaxDevices);

    public static IReadOnlyList<UserTariffResponse> ToUserApiResponse(this IReadOnlyList<TariffDto> items) =>
        items.Select(ToUserApiResponse).ToList();
}
