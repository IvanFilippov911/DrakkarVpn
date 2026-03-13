using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Subscriptions.Application.Features.Commands.ExpireSubscription;

public sealed class ExpireSubscriptionsBulkCommandValidator
    : AbstractValidator<ExpireSubscriptionsBulkCommand>
{
    public ExpireSubscriptionsBulkCommandValidator()
    {
        RuleFor(x => x.SubscriptionIds)
            .NotNull()
            .NotEmpty();

        RuleForEach(x => x.SubscriptionIds)
            .NotEmpty()              
            .Must(id => id != Guid.Empty);
        
        RuleFor(x => x.SubscriptionIds.Count)
            .LessThanOrEqualTo(500);
    }
}