using DrakkarVpn.Core.Api.Modules.Servers.Domain;
using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Admin.Application.Features.Queries.GetAdminServers;

public sealed class GetAdminServersQueryValidator : AbstractValidator<GetAdminServersQuery>
{
    public GetAdminServersQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page must be >= 1");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 200)
            .WithMessage("PageSize must be between 1 and 200");

        RuleFor(x => x.Region)
            .MaximumLength(16)
            .When(x => !string.IsNullOrWhiteSpace(x.Region));

        RuleFor(x => x.Status)
            .Must(BeValidStatus)
            .When(x => !string.IsNullOrWhiteSpace(x.Status))
            .WithMessage("Unknown server status");
    }

    private static bool BeValidStatus(string status) =>
        Enum.TryParse<ServerStatus>(status, true, out _);
}