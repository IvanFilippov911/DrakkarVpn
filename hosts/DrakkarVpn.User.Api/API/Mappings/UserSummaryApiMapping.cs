using DrakkarVpn.Core.Api.Modules.Orchestrator.API.Contracts.Response;
using DrakkarVpn.Core.Api.Modules.Orchestrator.Application.DTOs;

namespace DrakkarVpn.Core.Api.Modules.Orchestrator.API.Mappings;

public static class UserSummaryApiMapping
{
    public static UserSummaryResponse ToApiResponse(this UserSummaryDto dto) =>
        new(
            SubscriptionEndAtUtc: dto.SubscriptionEndAtUtc,
            ConnectedDevices: dto.ConnectedDevices,
            MaxDevices: dto.MaxDevices
        );
}

