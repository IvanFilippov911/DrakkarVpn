using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Commands.Subscriptions.AdminBulkGrantSubscriptions;

public sealed class AdminBulkGrantSubscriptionsCommandValidator
    : AbstractValidator<AdminBulkGrantSubscriptionsCommand>
{
    public AdminBulkGrantSubscriptionsCommandValidator()
    {
        RuleFor(x => x.UserIds)
            .NotNull()
            .Must(x => x.Count > 0);

        RuleForEach(x => x.UserIds)
            .NotEmpty();

        RuleFor(x => x.TariffId)
            .NotEmpty();

        RuleFor(x => x.DeviceCount)
            .GreaterThan(0)
            .When(x => x.DeviceCount.HasValue);
    }
}