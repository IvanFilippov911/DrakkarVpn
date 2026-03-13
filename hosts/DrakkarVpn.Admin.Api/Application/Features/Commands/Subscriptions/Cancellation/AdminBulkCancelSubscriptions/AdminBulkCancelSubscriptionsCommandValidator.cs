using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Subscriptions.AdminBulkCancelSubscriptions;

public sealed class AdminBulkCancelSubscriptionsCommandValidator
    : AbstractValidator<AdminBulkCancelSubscriptionsCommand>
{
    public AdminBulkCancelSubscriptionsCommandValidator()
    {
        RuleFor(x => x.SubscriptionIds)
            .NotNull()
            .NotEmpty();

        RuleForEach(x => x.SubscriptionIds)
            .NotEmpty();

        RuleFor(x => x.SubscriptionIds.Count)
            .LessThanOrEqualTo(500);
    }
}