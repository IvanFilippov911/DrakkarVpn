using DrakkarVpn.Core.Api.Modules.Admin.API.Contracts;
using DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.AdminGrantSubscription;

namespace DrakkarVpn.Core.Api.Modules.Admin.API.Mappings;

public static class AdminGrantSubscriptionMappings
{
    public static AdminGrantSubscriptionCommand ToCommand(
        this AdminGrantSubscriptionApiRequest body,
        Guid userId)
    {
        return new AdminGrantSubscriptionCommand(
            UserId:           userId,
            TariffId:         body.TariffId,
            MarkUserInternal: body.MarkUserInternal,
            DeviceCount:      body.DeviceCount
        );
    }
}