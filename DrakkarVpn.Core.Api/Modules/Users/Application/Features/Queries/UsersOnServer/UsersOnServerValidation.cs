using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Users.Application.Features.Queries.GetListServerUsers;

public sealed class UsersOnServerValidation: AbstractValidator<UsersOnServerRequest>
{
    public UsersOnServerValidation()
    {
        RuleFor(x => x.ServerId).NotEmpty();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.SortBy).IsInEnum();
        When(x => x.Status.HasValue, () => RuleFor(x => x.Status).IsInEnum());
        When(x => x.SubscriptionStatus.HasValue, () => RuleFor(x => x.SubscriptionStatus).IsInEnum());
    }
}