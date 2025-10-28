using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.CountActiveDevicesBySubscription;

public sealed class CountActiveDevicesBySubscriptionValidator
    : AbstractValidator<CountActiveDevicesBySubscriptionRequest>
{
    public CountActiveDevicesBySubscriptionValidator()
    {
        RuleFor(x => x.SubscriptionId).NotEmpty();
    }
}